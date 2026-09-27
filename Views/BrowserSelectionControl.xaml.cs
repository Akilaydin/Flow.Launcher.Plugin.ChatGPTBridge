using System.IO;
using System.Windows.Controls;
using Flow.Launcher.Plugin.ChatGPTBridge.Services;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Views;

public partial class BrowserSelectionControl : UserControl
{
    private BrowserDiscoveryService? _browserDiscovery;
    private bool _isUpdating;

    public BrowserSelectionControl()
    {
        InitializeComponent();
    }

    public string? SelectedExecutablePath { get; private set; }

    public event EventHandler? SelectionChanged;

    public void Initialize(BrowserDiscoveryService browserDiscovery, string? selectedExecutablePath)
    {
        _browserDiscovery = browserDiscovery;
        SelectedExecutablePath = selectedExecutablePath;
        RefreshBrowserOptions();
    }

    private void BrowserComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdating || BrowserComboBox.SelectedItem is not BrowserOption option)
        {
            return;
        }

        if (option.Kind == BrowserOptionKind.CustomPicker)
        {
            SelectCustomExecutable();
            return;
        }

        SelectedExecutablePath = option.ExecutablePath;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SelectCustomExecutable()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select browser executable",
            Filter = "Applications (*.exe)|*.exe",
            CheckFileExists = true,
            Multiselect = false
        };

        if (!string.IsNullOrWhiteSpace(SelectedExecutablePath))
        {
            var currentDirectory = Path.GetDirectoryName(SelectedExecutablePath);
            if (!string.IsNullOrWhiteSpace(currentDirectory) && Directory.Exists(currentDirectory))
            {
                dialog.InitialDirectory = currentDirectory;
            }
        }

        if (dialog.ShowDialog() == true)
        {
            SelectedExecutablePath = dialog.FileName;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        RefreshBrowserOptions();
    }

    private void RefreshBrowserOptions()
    {
        if (_browserDiscovery is null)
        {
            return;
        }

        _isUpdating = true;

        try
        {
            var discovered = _browserDiscovery.GetInstalledBrowsers();
            var options = new List<BrowserOption>
            {
                new("System default", null, BrowserOptionKind.SystemDefault)
            };

            options.AddRange(discovered.Select(browser =>
                new BrowserOption(browser.Name, browser.ExecutablePath, BrowserOptionKind.Browser)));

            if (!string.IsNullOrWhiteSpace(SelectedExecutablePath)
                && discovered.All(browser =>
                    !BrowserDiscoveryService.PathsEqual(browser.ExecutablePath, SelectedExecutablePath)))
            {
                var suffix = File.Exists(SelectedExecutablePath) ? "Custom" : "Missing";
                options.Add(new BrowserOption(
                    $"{_browserDiscovery.GetDisplayName(SelectedExecutablePath)} ({suffix})",
                    SelectedExecutablePath,
                    BrowserOptionKind.Browser));
            }

            options.Add(new BrowserOption("Custom executable...", null, BrowserOptionKind.CustomPicker));

            BrowserComboBox.ItemsSource = options;
            BrowserComboBox.SelectedItem = string.IsNullOrWhiteSpace(SelectedExecutablePath)
                ? options[0]
                : options.FirstOrDefault(option =>
                    option.ExecutablePath is not null
                    && BrowserDiscoveryService.PathsEqual(option.ExecutablePath, SelectedExecutablePath))
                  ?? options[0];
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private sealed record BrowserOption(
        string DisplayName,
        string? ExecutablePath,
        BrowserOptionKind Kind);

    private enum BrowserOptionKind
    {
        SystemDefault,
        Browser,
        CustomPicker
    }
}
