using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
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
        
        Clippy.PointerPressed += (_, args) =>
        {
            MoveDrag(args);
        };
    }

    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
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

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            MoveDrag(e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_draggingWindow)
        {
            EndDrag(e);
        }
    }
}
