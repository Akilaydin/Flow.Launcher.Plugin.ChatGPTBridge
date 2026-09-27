namespace Flow.Launcher.Plugin.ChatGPTBridge.Services;

public static class ChatGptUrlBuilder
{
    private const string BaseUrl = "https://chatgpt.com/";

    public static string Build(string prompt, bool temporary)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return temporary
                ? $"{BaseUrl}?temporary-chat=true"
                : BaseUrl;
        }

        var encodedPrompt = Uri.EscapeDataString(prompt);

        return temporary
            ? $"{BaseUrl}?prompt={encodedPrompt}&temporary-chat=true"
            : $"{BaseUrl}?prompt={encodedPrompt}";
    }
}

