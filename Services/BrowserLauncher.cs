using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.ChatGPTBridge;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Services;

public sealed class BrowserLauncher(
    IPublicAPI api,
    Settings settings,
    BrowserDiscoveryService browserDiscovery)
{
    public string GetConfiguredBrowserName()
    {
        return string.IsNullOrWhiteSpace(settings.BrowserExecutablePath)
            ? "Default browser"
            : browserDiscovery.GetDisplayName(settings.BrowserExecutablePath);
    }

    public BrowserOpenResult Open(string url)
    {
        var browserPath = settings.BrowserExecutablePath;

        if (string.IsNullOrWhiteSpace(browserPath))
        {
            api.OpenUrl(url);
            return BrowserOpenResult.Opened;
        }

        if (!File.Exists(browserPath))
        {
            return BrowserOpenResult.BrowserNotFound;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = browserPath,
                Arguments = $"\"{url}\"",
                UseShellExecute = true
            });

            return BrowserOpenResult.Opened;
        }
        catch (Exception ex)
        {
            api.ShowMsgError("Could not open browser", ex.Message);
            return BrowserOpenResult.Failed;
        }
    }
}

public enum BrowserOpenResult
{
    Opened,
    BrowserNotFound,
    Failed
}
