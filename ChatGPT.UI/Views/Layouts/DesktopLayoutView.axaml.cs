using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace ChatGPT.Views.Layouts;

public partial class DesktopLayoutView : UserControl
{
    private bool _draggingWindow;

    public DesktopLayoutView()
    {
        InitializeComponent();

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
