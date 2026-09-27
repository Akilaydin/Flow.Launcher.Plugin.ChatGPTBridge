using System.Windows.Controls;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.ChatGPTBridge.Services;
using Flow.Launcher.Plugin.ChatGPTBridge.Views;

namespace Flow.Launcher.Plugin.ChatGPTBridge;

public sealed class Main : IPlugin, ISettingProvider
{
    private const string IconPath = "Images/icon.png";

    private PluginInitContext? _context;
    private Settings? _settings;
    private ActionKeywordService? _actionKeywordService;
    private BrowserDiscoveryService? _browserDiscovery;
    private BrowserLauncher? _browserLauncher;

    public void Init(PluginInitContext context)
    {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings>();
        _actionKeywordService = new ActionKeywordService(
            context.API,
            context.CurrentPluginMetadata.ID,
            _settings);
        _browserDiscovery = new BrowserDiscoveryService();
        _browserLauncher = new BrowserLauncher(context.API, _settings, _browserDiscovery);
    }

    public List<Result> Query(Query query)
    {
        if (_settings is null || _browserLauncher is null)
        {
            return [];
        }

        var temporary = string.Equals(
            query.ActionKeyword,
            _settings.TemporaryActionKeyword,
            StringComparison.OrdinalIgnoreCase);
        var prompt = query.Search?.Trim() ?? string.Empty;
        var url = ChatGptUrlBuilder.Build(prompt, temporary);
        var browserName = _browserLauncher.GetConfiguredBrowserName();

        return
        [
            new Result
            {
                Title = GetTitle(prompt, temporary),
                SubTitle = temporary
                    ? $"Open temporary chat in ChatGPT · {browserName}"
                    : $"Open in ChatGPT · {browserName}",
                IcoPath = IconPath,
                Action = _ =>
                {
                    OpenUrl(url);
                    return true;
                }
            }
        ];
    }

    public Control CreateSettingPanel()
    {
        if (_context is null || _settings is null || _actionKeywordService is null || _browserDiscovery is null)
        {
            return new UserControl();
        }

        return new SettingsControl(
            _settings,
            _actionKeywordService,
            _browserDiscovery,
            () => _context.API.SaveSettingJsonStorage<Settings>(),
            error => _context.API.ShowMsgError("Invalid action keyword", error));
    }

    private static string GetTitle(string prompt, bool temporary)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            return prompt;
        }

        return temporary ? "Open temporary ChatGPT" : "Open ChatGPT";
    }

    private void OpenUrl(string url)
    {
        if (_context is null || _settings is null || _browserDiscovery is null || _browserLauncher is null)
        {
            return;
        }

        if (_browserLauncher.Open(url) != BrowserOpenResult.BrowserNotFound)
        {
            return;
        }

        _context.API.ShowMsgErrorWithButton(
            "Browser not found",
            "Choose browser...",
            () => ChooseBrowserAndRetry(url),
            "The configured browser is no longer available.");
    }

    private void ChooseBrowserAndRetry(string url)
    {
        if (_context is null || _settings is null || _browserDiscovery is null || _browserLauncher is null)
        {
            return;
        }

        var dialog = new BrowserSelectionDialog(_browserDiscovery, _settings.BrowserExecutablePath);
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        _settings.BrowserExecutablePath = dialog.SelectedExecutablePath;
        _context.API.SaveSettingJsonStorage<Settings>();

        if (_browserLauncher.Open(url) == BrowserOpenResult.BrowserNotFound)
        {
            _context.API.ShowMsgError(
                "Browser not found",
                "The selected browser is no longer available.");
        }
    }
}
