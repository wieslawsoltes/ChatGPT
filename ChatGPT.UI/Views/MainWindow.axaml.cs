using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace ChatGPT.UI.Views;

internal static class Win32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr WS_EX_TRANSPARENT = 0x00000020;

    public static int GWL_EXSTYLE = -20;
}

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (PlatformImpl?.Handle.Handle is { } windowHandle)
        {
            IntPtr extendedStyle = Win32.GetWindowLongPtr(windowHandle, Win32.GWL_EXSTYLE);
            Win32.SetWindowLongPtr(windowHandle, Win32.GWL_EXSTYLE, extendedStyle | Win32.WS_EX_TRANSPARENT);
        }
    }
}
