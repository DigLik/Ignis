using System.Runtime.InteropServices;

using Ignis.Bindings.Windows;
using Ignis.Core.Numerics;

namespace Ignis.Platform.Windowing;

#pragma warning disable CA1724
public sealed unsafe partial class Window
#pragma warning restore CA1724
{
    private static bool _classRegistered;
    private static readonly Dictionary<nint, Window> _windows = [];
    private static readonly Lock _classRegistrationLock = new();

    private partial void InitPlatformWindow()
    {
        nint hInstance = Kernel32.GetModuleHandleW(null);
        const string classNameStr = "IgnisWindowClass";

        fixed (char* className = classNameStr)
        {
            lock (_classRegistrationLock)
            {
                if (!_classRegistered)
                {
                    nint pfnWndProc = Marshal.GetFunctionPointerForDelegate(WndProcInstance);
                    var wc = new User32.WNDCLASSEXW
                    {
                        cbSize = (uint)sizeof(User32.WNDCLASSEXW),
                        style = User32.CS_HREDRAW | User32.CS_VREDRAW,
                        lpfnWndProc = (delegate* unmanaged<nint, uint, nint, nint, nint>)pfnWndProc,
                        hInstance = hInstance,
                        hCursor = User32.LoadCursorW(0, User32.IDC_ARROW),
                        lpszClassName = className
                    };
                    User32.RegisterClassExW(in wc);
                    _classRegistered = true;
                }
            }

            Handle = User32.CreateWindowExW(
                0, className, Title,
                User32.WS_OVERLAPPEDWINDOW,
                User32.CW_USEDEFAULT, User32.CW_USEDEFAULT,
                800, 600, 0, 0, hInstance, 0);
        }

        if (Handle == 0)
        {
            throw new InvalidOperationException("Не удалось создать окно Win32.");
        }

        _windows[Handle] = this;

        User32.GetClientRect(Handle, out var rect);
        Size = new Vector2Int(
            rect.right - rect.left,
            rect.bottom - rect.top
        );

        User32.ShowWindow(Handle, User32.SW_SHOW);
    }

    private partial void SetTitlePlatform(string title)
    {
        if (Handle != 0)
            User32.SetWindowTextW(Handle, title);
    }

    private partial void WaitClosePlatform()
    {
        _ = Winmm.TimeBeginPeriod(1);
        try
        {
            while (!ShouldClose)
            {
                if (User32.PeekMessageW(out User32.MSG msg, 0, 0, 0, User32.PM_REMOVE))
                {
                    User32.TranslateMessage(in msg);
                    User32.DispatchMessageW(in msg);
                }
                else
                {
                    FrameTick?.Invoke();
                }
            }
        }
        finally
        {
            _ = Winmm.TimeEndPeriod(1);
        }
    }

    private partial void ClosePlatform()
    {
        if (Handle != 0)
        {
            User32.PostMessageW(Handle, User32.WM_CLOSE, 0, 0);
        }
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

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    private static readonly WndProcDelegate WndProcInstance = WndProc;

    private static nint WndProc(
        nint hWnd, uint msg, nint wParam, nint lParam)
    {
        _windows.TryGetValue(hWnd, out var window);

        if (window != null)
        {
            switch (msg)
            {
                case User32.WM_INPUT:
                    window.RawInputReceived?.Invoke(lParam);
                    break;

                case User32.WM_SIZE:
                    int width = (int)(lParam & 0xFFFF);
                    int height = (int)((lParam >> 16) & 0xFFFF);
                    window.Size = new(width, height);

                    window.SizeChanged?.Invoke(
                        window,
                        new Core.Window.SizeChangedEventArgs(
                            window.Size));
                    return 0;

                case User32.WM_ENTERSIZEMOVE:
                    window.InSizeMove = true;
                    return 0;

                case User32.WM_EXITSIZEMOVE:
                    window.InSizeMove = false;
                    window.ResizeEnded?.Invoke(window, EventArgs.Empty);
                    return 0;

                case User32.WM_PAINT:
                    User32.ValidateRect(hWnd, 0);
                    return 0;

                case User32.WM_CLOSE:
                    window.ShouldClose = true;
                    window.Closed?.Invoke(
                        window, EventArgs.Empty);
                    User32.DestroyWindow(hWnd);
                    return 0;

                case User32.WM_DESTROY:
                    User32.PostQuitMessage(0);
                    return 0;
            }
        }
        return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    private partial void SetSizePlatform(Vector2Int size)
    {
        if (Handle == 0) return;

        User32.GetClientRect(Handle, out var rect);
        int currentClientWidth = rect.right - rect.left;
        int currentClientHeight = rect.bottom - rect.top;

        if (currentClientWidth == size.X && currentClientHeight == size.Y)
            return;

        uint style = (uint)User32.GetWindowLongW(Handle, User32.GWL_STYLE);
        uint exStyle = (uint)User32.GetWindowLongW(Handle, User32.GWL_EXSTYLE);
        int hasMenu = User32.GetMenu(Handle) != 0 ? 1 : 0;

        User32.RECT adjustRect = new()
        {
            left = 0,
            top = 0,
            right = size.X,
            bottom = size.Y
        };

        User32.AdjustWindowRectEx(&adjustRect, style, hasMenu != 0, exStyle);

        int width = adjustRect.right - adjustRect.left;
        int height = adjustRect.bottom - adjustRect.top;

        User32.SetWindowPos(Handle, 0, 0, 0, width, height, User32.SWP_NOMOVE | User32.SWP_NOZORDER | User32.SWP_NOACTIVATE);
    }
}