using System.Windows;
using Flow.Launcher.Plugin.ChatGPTBridge.Services;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Views;

public partial class BrowserSelectionDialog : Window
{
    public BrowserSelectionDialog(
        BrowserDiscoveryService browserDiscovery,
        string? selectedExecutablePath)
    {
        InitializeComponent();
        BrowserSelection.Initialize(browserDiscovery, selectedExecutablePath);
    }

    public string? SelectedExecutablePath => BrowserSelection.SelectedExecutablePath;

    private void UseBrowserButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}

