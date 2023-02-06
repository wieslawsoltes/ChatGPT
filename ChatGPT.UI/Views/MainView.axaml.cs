using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using ChatGPT.UI.ViewModels;

namespace ChatGPT.UI.Views;

public partial class MainView : UserControl
{
    private bool _draggingWindow;

    public MainView()
    {
        InitializeComponent();

        DataContext = new MainViewModel(Exit);
        
        ClippyImage.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                MoveDrag(e);
            }
        };
        
        ClippyImage.PointerReleased += (_, e) =>
        {
            if (_draggingWindow)
            {
                EndDrag(e);
            }
        };
    }

    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }

    private void ThemeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { })
        {
            Application.Current.RequestedThemeVariant = 
                Application.Current.RequestedThemeVariant == ThemeVariant.Light 
                    ? ThemeVariant.Dark 
                    : ThemeVariant.Light;
        }
    }

    private void MoveDrag(PointerPressedEventArgs e)
    {
        _draggingWindow = true;

        (this.GetVisualRoot() as Window)?.BeginMoveDrag(e);
            
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            EndDrag(e);
        }
    }

    private void EndDrag(PointerEventArgs e)
    {
        _draggingWindow = false;
    }
}
