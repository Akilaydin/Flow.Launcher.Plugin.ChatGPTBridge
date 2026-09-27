# Product

ChatGPT Bridge is a Flow Launcher plugin for opening prompts in ChatGPT Web.

## Commands

The plugin has two independently configurable action keywords. Their defaults are:

- `gpt <prompt>` — open a normal ChatGPT chat.
- `gptt <prompt>` — open a temporary ChatGPT chat.

Empty commands are valid: `gpt` opens `https://chatgpt.com/` and `gptt` opens `https://chatgpt.com/?temporary-chat=true`.

The settings panel exposes `Normal chat keyword` and `Temporary chat keyword`. Changes take effect immediately without restarting Flow Launcher.

## Browser selection

The settings panel contains one browser selector:

1. `System default`.
2. Automatically detected Windows browsers.
3. `Custom executable...`, which opens a Windows `.exe` file picker.

The default is `System default`. The same browser setting is used for normal and temporary chats.

When `System default` is selected, Flow Launcher's URL-opening API is used. For a configured browser executable, the plugin starts that executable directly with the ChatGPT URL.

If the saved executable no longer exists, the plugin shows `Browser not found` with the subtitle `The configured browser is no longer available.` and a `Choose browser...` button. The button opens the same browser selector used by the settings panel. After the user confirms a replacement, the plugin saves it and retries the original ChatGPT action.
