using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Services;

public sealed partial class BrowserDiscoveryService
{
    private const string BrowserClientsRegistryPath = @"Software\Clients\StartMenuInternet";
    private readonly Lazy<IReadOnlyList<BrowserInfo>> _installedBrowsers = new(DiscoverInstalledBrowsers);

    public IReadOnlyList<BrowserInfo> GetInstalledBrowsers()
    {
        return _installedBrowsers.Value;
    }

    private static IReadOnlyList<BrowserInfo> DiscoverInstalledBrowsers()
    {
        var browsers = new Dictionary<string, BrowserInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        {
            foreach (var view in GetRegistryViews())
            {
                AddBrowsersFromRegistry(hive, view, browsers);
            }
        }

        return browsers.Values
            .OrderBy(browser => browser.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    public string GetDisplayName(string executablePath)
    {
        var discoveredBrowser = GetInstalledBrowsers()
            .FirstOrDefault(browser => PathsEqual(browser.ExecutablePath, executablePath));

        if (discoveredBrowser is not null)
        {
            return discoveredBrowser.Name;
        }

        if (File.Exists(executablePath))
        {
            try
            {
                var productName = FileVersionInfo.GetVersionInfo(executablePath).ProductName;
                if (!string.IsNullOrWhiteSpace(productName))
                {
                    return productName.Trim();
                }
            }
            catch
            {
                // Fall back to the executable name below.
            }
        }

        var fileName = Path.GetFileNameWithoutExtension(executablePath);
        return string.IsNullOrWhiteSpace(fileName) ? "Configured browser" : fileName;
    }

    public static bool PathsEqual(string first, string second)
    {
        try
        {
            return string.Equals(
                Path.GetFullPath(first),
                Path.GetFullPath(second),
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static IEnumerable<RegistryView> GetRegistryViews()
    {
        yield return RegistryView.Registry64;
        yield return RegistryView.Registry32;
    }

    private static void AddBrowsersFromRegistry(
        RegistryHive hive,
        RegistryView view,
        IDictionary<string, BrowserInfo> browsers)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, view);
            using var clientsKey = baseKey.OpenSubKey(BrowserClientsRegistryPath);

            if (clientsKey is null)
            {
                return;
            }

            foreach (var subKeyName in clientsKey.GetSubKeyNames())
            {
                using var browserKey = clientsKey.OpenSubKey(subKeyName);
                using var commandKey = browserKey?.OpenSubKey(@"shell\open\command");

                var command = commandKey?.GetValue(null) as string;
                var executablePath = TryExtractExecutablePath(command);

                if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
                {
                    continue;
                }

                var existingExecutablePath = executablePath;

                var displayName = browserKey?.GetValue(null) as string;
                if (string.IsNullOrWhiteSpace(displayName) || displayName.StartsWith('@'))
                {
                    displayName = subKeyName;
                }

                browsers.TryAdd(
                    existingExecutablePath,
                    new BrowserInfo(displayName.Trim(), existingExecutablePath));
            }
        }
        catch (Exception) when (hive is RegistryHive.CurrentUser or RegistryHive.LocalMachine)
        {
            // A broken or inaccessible browser registration should not break plugin initialization.
        }
    }

    private static string? TryExtractExecutablePath(string? command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return null;
        }

        var match = BrowserExecutableRegex().Match(command);
        if (!match.Success)
        {
            return null;
        }

        var path = match.Groups["quoted"].Success
            ? match.Groups["quoted"].Value
            : match.Groups["plain"].Value;

        return Environment.ExpandEnvironmentVariables(path.Trim());
    }

    [GeneratedRegex("^\\s*(?:\"(?<quoted>[^\"]+\\.exe)\"|(?<plain>.+?\\.exe))(?:\\s|$)", RegexOptions.IgnoreCase)]
    private static partial Regex BrowserExecutableRegex();
}
