using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(Key.A, KeyModifiers.Control | KeyModifiers.Shift),
            Command = new RelayCommand(ToggleAcrylicBlur)
        });
    }

    private void ToggleAcrylicBlur()
    {
        if (TransparencyLevelHint == WindowTransparencyLevel.AcrylicBlur)
        {
            SystemDecorations = SystemDecorations.None;
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            TransparencyLevelHint = WindowTransparencyLevel.Transparent;
            AcrylicBorder.IsVisible = false;
        }
        else
        {
            SystemDecorations = SystemDecorations.Full;
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
            TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
            AcrylicBorder.IsVisible = true;
        }
    }
}
