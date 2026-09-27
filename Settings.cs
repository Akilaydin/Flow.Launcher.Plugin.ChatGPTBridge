namespace Flow.Launcher.Plugin.ChatGPTBridge;

public sealed class Settings
{
    public string NormalActionKeyword { get; set; } = "gpt";

    public string TemporaryActionKeyword { get; set; } = "gptt";

    public string? BrowserExecutablePath { get; set; }
}

