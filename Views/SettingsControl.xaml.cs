using System.Windows.Controls;
using System.Windows.Input;
using Flow.Launcher.Plugin.ChatGPTBridge.Services;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Views;

public partial class SettingsControl : UserControl
{
    private readonly Settings _settings;
    private readonly ActionKeywordService _actionKeywordService;
    private readonly Action _saveSettings;
    private readonly Action<string> _showKeywordError;

    public SettingsControl(
        Settings settings,
        ActionKeywordService actionKeywordService,
        BrowserDiscoveryService browserDiscovery,
        Action saveSettings,
        Action<string> showKeywordError)
    {
        _settings = settings;
        _actionKeywordService = actionKeywordService;
        _saveSettings = saveSettings;
        _showKeywordError = showKeywordError;

        InitializeComponent();

        NormalKeywordTextBox.Text = settings.NormalActionKeyword;
        TemporaryKeywordTextBox.Text = settings.TemporaryActionKeyword;
        BrowserSelection.Initialize(browserDiscovery, settings.BrowserExecutablePath);
        BrowserSelection.SelectionChanged += BrowserSelection_OnSelectionChanged;
    }

    private void BrowserSelection_OnSelectionChanged(object? sender, EventArgs e)
    {
        _settings.BrowserExecutablePath = BrowserSelection.SelectedExecutablePath;
        _saveSettings();
    }

    private void NormalKeywordTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        CommitNormalKeyword();
    }

    private void NormalKeywordTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        CommitNormalKeyword();
        e.Handled = true;
    }

    private void TemporaryKeywordTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        CommitTemporaryKeyword();
    }

    private void TemporaryKeywordTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        CommitTemporaryKeyword();
        e.Handled = true;
    }

    private void CommitNormalKeyword()
    {
        if (_actionKeywordService.TryUpdateNormal(NormalKeywordTextBox.Text, out var error))
        {
            NormalKeywordTextBox.Text = _settings.NormalActionKeyword;
            return;
        }

        NormalKeywordTextBox.Text = _settings.NormalActionKeyword;
        _showKeywordError(error);
    }

    private void CommitTemporaryKeyword()
    {
        if (_actionKeywordService.TryUpdateTemporary(TemporaryKeywordTextBox.Text, out var error))
        {
            TemporaryKeywordTextBox.Text = _settings.TemporaryActionKeyword;
            return;
        }

        TemporaryKeywordTextBox.Text = _settings.TemporaryActionKeyword;
        _showKeywordError(error);
    }
}
