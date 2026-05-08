using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Ignis.Bindings.Windows;

namespace Ignis.Platform.Windowing;

#pragma warning disable CA1724
public sealed unsafe partial class Window
#pragma warning restore CA1724
{
    private static bool _classRegistered;
    private static readonly Dictionary<nint, Window> _windows = [];

    private partial void InitPlatformWindow()
    {
        nint hInstance = Kernel32.GetModuleHandleW(null);

        const string classNameStr = "IgnisWindowClass";

        fixed (char* className = classNameStr)
        {
            if (!_classRegistered)
            {
                var wc = new User32.WNDCLASSEXW
                {
                    cbSize =
                        (uint)sizeof(User32.WNDCLASSEXW),
                    style = User32.CS_HREDRAW |
                        User32.CS_VREDRAW,
                    lpfnWndProc = &WndProc,
                    hInstance = hInstance,
                    hCursor = User32.LoadCursorW(
                        0, User32.IDC_ARROW),
                    lpszClassName = className
                };
                User32.RegisterClassExW(in wc);
                _classRegistered = true;
            }

            Handle = User32.CreateWindowExW(
                0, className, Title,
                User32.WS_OVERLAPPEDWINDOW,
                User32.CW_USEDEFAULT, User32.CW_USEDEFAULT,
                800, 600, 0, 0, hInstance, 0);
        }

        if (Handle == 0)
        {
            throw new InvalidOperationException(
                "Не удалось создать окно Win32.");
        }

        _windows[Handle] = this;

        User32.GetClientRect(Handle, out var rect);
        Size = new Size(
            rect.right - rect.left,
            rect.bottom - rect.top);

        User32.ShowWindow(Handle, User32.SW_SHOW);
    }

    private partial void SetTitlePlatform(string title)
    {
        if (Handle != 0)
            User32.SetWindowTextW(Handle, title);
    }

    private partial void UpdatePlatform()
    {
        while (User32.PeekMessageW(
            out var msg, 0, 0, 0, User32.PM_REMOVE))
        {
            if (msg.message == User32.WM_QUIT)
            {
                ShouldClose = true;
                break;
            }
            User32.TranslateMessage(in msg);
            User32.DispatchMessageW(in msg);
        }
    }

    private partial void ClosePlatform()
    {
        if (Handle != 0)
            User32.DestroyWindow(Handle);
    }

    private partial void DisposePlatform()
    {
        if (Handle != 0)
        {
            _windows.Remove(Handle);
            User32.DestroyWindow(Handle);
            Handle = 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint WndProc(
        nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (_windows.TryGetValue(hWnd, out var window))
        {
            switch (msg)
            {
                case User32.WM_INPUT:
                    window.RawInputReceived?.Invoke(lParam);
                    break;

                case User32.WM_SIZE:
                    int width = (int)(lParam & 0xFFFF);
                    int height = (int)((lParam >> 16) & 0xFFFF);
                    window.Size = new Size(width, height);

                    window.SizeChanged?.Invoke(
                        window,
                        new Core.Window.SizeChangedEventArgs(
                            window.Size));
                    return 0;

                case User32.WM_CLOSE:
                    window.ShouldClose = true;
                    window.Closed?.Invoke(
                        window, EventArgs.Empty);
                    return 0;

                case User32.WM_DESTROY:
                    User32.PostQuitMessage(0);
                    return 0;
            }
        }
        return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
    }
}