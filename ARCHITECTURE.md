# Architecture

## Current technical decisions

- One C# project: `Flow.Launcher.Plugin.ChatGPTBridge`.
- Target framework: `net9.0-windows10.0.19041.0`, required by `Flow.Launcher.Plugin` 5.3.2.
- Flow Launcher minimum app version: 2.1.4.
- ChatGPT integration is URL-based: the plugin constructs `chatgpt.com` URLs and opens them in a browser.
- `gpt` and `gptt` are the default `ActionKeywords`; users can configure the normal and temporary keywords independently.
- Flow's generic multi-keyword editor stays hidden because it treats multiple keywords as an unordered set and cannot preserve the normal/temporary semantic mapping.
- Settings store the semantic action-keyword mapping and selected browser executable path. A null browser path means system default.

## System structure

### `Main`

Owns Flow Launcher integration: initialization, settings loading, query handling, result construction and settings-panel creation.

### `ChatGptUrlBuilder`

Pure URL construction for normal and temporary ChatGPT chats.

### `ActionKeywordService`

Validates semantic action-keyword changes and applies them at runtime through Flow's public `AddActionKeyword` / `RemoveActionKeyword` APIs. It persists both the plugin's semantic mapping and Flow's plugin keyword registration, so changes take effect without restart.

### `BrowserDiscoveryService`

Reads registered Windows browsers from `Software\Clients\StartMenuInternet` in HKCU/HKLM and both registry views. Discovery is best-effort and cached for the lifetime of the plugin instance. Only registrations whose executable currently exists are returned. Duplicate executable paths are collapsed case-insensitively.

### `BrowserLauncher`

Uses `IPublicAPI.OpenUrl` for the system default browser. For an explicit browser path, starts the executable directly with the URL as its sole argument. It reports `BrowserNotFound` when the saved executable is missing and shows a Flow error when process startup fails.

### `BrowserSelectionControl`

Reusable browser selector used by both the Flow settings panel and the missing-browser recovery dialog. Custom browser selection uses `Microsoft.Win32.OpenFileDialog` restricted to `.exe` files.

### `SettingsControl`

Flow settings view using Flow's standard settings spacing resources. Browser changes are persisted on selection. Action-keyword changes are validated and committed on Enter or when the field loses keyboard focus.

### `BrowserSelectionDialog`

Recovery dialog shown only after an explicitly configured browser disappears. It reuses `BrowserSelectionControl`; confirming saves the replacement and retries the original URL.

## Data flow

1. Flow loads `plugin.json` and initializes `Main`.
2. `Main` loads `Settings` through Flow's JSON settings storage.
3. The query's configured action keyword determines normal vs temporary mode and is converted to a ChatGPT URL.
4. The result subtitle resolves the configured browser display name.
5. Selecting the result delegates URL opening to `BrowserLauncher`.
6. If an explicitly configured browser is missing, `Main` shows the recovery notification, opens `BrowserSelectionDialog` on request, saves the replacement browser and retries the original URL.
