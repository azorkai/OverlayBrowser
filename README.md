# OverlayBrowser Application

This project demonstrates a Windows Forms application that integrates the WebView2 control for browsing functionality, supports global hotkeys for minimizing and restoring the form, and includes settings management via a `settings.json` file. The application also features aesthetic design elements for enhanced user experience.

## Features

1. **WebView2 Integration**
   - The application embeds a WebView2 browser control for displaying web content.
   - Configurable start URL through the `settings.json` file.

2. **Global Hotkeys**
   - `Alt + M`: Minimize or restore the form.
   - The hotkeys are dynamically configurable through `settings.json`.

3. **Settings Management**
   - Settings such as opacity, start URL, and hotkey configuration are stored in `settings.json`.
   - If the file does not exist, it is automatically created with default values.

4. **Aesthetic Design**
   - Custom-designed top panel for form dragging.
   - Transparent form with adjustable opacity levels.
   - Always-on-top functionality for better visibility.

5. **Error Handling**
   - Handles initialization errors for WebView2 gracefully.
   - Displays user-friendly error messages for issues like WebView2 not being available.

## Requirements

- .NET Framework 4.7.2 or later
- WebView2 Runtime installed on the system
- Newtonsoft.Json library for handling JSON files

## Setup

1. Clone the repository.
2. Build the solution in Visual Studio.
3. Ensure that the WebView2 runtime is installed. If not, download and install it from [Microsoft Edge WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).
4. Run the application.

## Configuration

The `settings.json` file allows you to configure the application's behavior. By default, it contains:

```json
{
  "Opacity": 80,
  "StartUrl": "https://google.com",
  "HotkeyModifier": 1,
  "HotkeyKey": "M"
}
```

- `Opacity`: Adjust the transparency of the form (0-100).
- `StartUrl`: The initial URL loaded in WebView2.
- `HotkeyModifier`: Modifier key for the global hotkey (1 for `Alt`).
- `HotkeyKey`: Key for the global hotkey (e.g., `M`).

## Usage

1. **Running the Application**
   - Launch the application. The form starts in the center of the screen.
   - The WebView2 browser loads the start URL specified in `settings.json`.

2. **Hotkey Functionality**
   - Press `Alt + M` to toggle between minimizing and restoring the form.

3. **Form Interaction**
   - Drag the form by clicking and holding the top panel.
   - Exit the application using the `Escape` key.

## Troubleshooting

- If the WebView2 control fails to initialize, ensure the WebView2 runtime is installed on your system.
- Check the `settings.json` file for proper configuration.
- Run the application as an administrator if hotkeys do not function as expected.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
