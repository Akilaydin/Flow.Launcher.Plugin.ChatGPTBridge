using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.ChatGPTBridge.Services;

public sealed class ActionKeywordService(
    IPublicAPI api,
    string pluginId,
    Settings settings)
{
    public bool TryUpdateNormal(string value, out string error)
    {
        return TryUpdate(
            value,
            settings.NormalActionKeyword,
            settings.TemporaryActionKeyword,
            keyword => settings.NormalActionKeyword = keyword,
            out error);
    }

    public bool TryUpdateTemporary(string value, out string error)
    {
        return TryUpdate(
            value,
            settings.TemporaryActionKeyword,
            settings.NormalActionKeyword,
            keyword => settings.TemporaryActionKeyword = keyword,
            out error);
    }

    private bool TryUpdate(
        string value,
        string currentKeyword,
        string otherKeyword,
        Action<string> updateSetting,
        out string error)
    {
        if (!TryNormalize(value, otherKeyword, out var newKeyword, out error))
        {
            return false;
        }

        if (string.Equals(currentKeyword, newKeyword, StringComparison.Ordinal))
        {
            return true;
        }

        try
        {
            api.RemoveActionKeyword(pluginId, currentKeyword);
            api.AddActionKeyword(pluginId, newKeyword);

            updateSetting(newKeyword);
            api.SaveSettingJsonStorage<Settings>();
            api.SavePluginSettings();

            return true;
        }
        catch (Exception ex)
        {
            updateSetting(currentKeyword);

            try
            {
                api.RemoveActionKeyword(pluginId, newKeyword);
                api.AddActionKeyword(pluginId, currentKeyword);
                api.SaveSettingJsonStorage<Settings>();
                api.SavePluginSettings();
            }
            catch
            {
                // Best-effort rollback. Flow will restore persisted plugin settings after restart.
            }

            error = $"Could not update the action keyword: {ex.Message}";
            return false;
        }
    }

    private static bool TryNormalize(
        string value,
        string otherKeyword,
        out string keyword,
        out string error)
    {
        keyword = value.Trim();

        if (string.IsNullOrEmpty(keyword))
        {
            error = "Action keyword cannot be empty.";
            return false;
        }

        if (keyword == Query.GlobalPluginWildcardSign)
        {
            error = "The global '*' keyword cannot be used because ChatGPT Bridge needs to distinguish normal and temporary chat modes.";
            return false;
        }

        if (keyword.Any(char.IsWhiteSpace))
        {
            error = "Action keyword cannot contain whitespace.";
            return false;
        }

        if (string.Equals(keyword, otherKeyword, StringComparison.OrdinalIgnoreCase))
        {
            error = "Normal and temporary chat must use different action keywords.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
