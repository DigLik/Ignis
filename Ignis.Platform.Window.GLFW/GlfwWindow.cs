using System.Drawing;

using Ignis.Core.Platform.Window;
using Ignis.Core.Platform.Window.Abstraction;

using Silk.NET.GLFW;

namespace Ignis.Platform.Window.GLFW;

public sealed unsafe class GlfwWindow : IWindow
{
    private readonly Glfw _glfw;
    private readonly WindowHandle* _window;

    private readonly GlfwCallbacks.WindowSizeCallback _sizeCallback;
    private readonly GlfwCallbacks.WindowCloseCallback _closeCallback;
    private bool _isDisposed;

    private Point _windowedPos;
    private Size _windowedSize;

    public nint WindowHandle => (nint)_window;

    public string Title
    {
        get;
        set
        {
            field = value;
            _glfw.SetWindowTitle(_window, value);
        }
    } = string.Empty;

    public Size ClientSize
    {
        get
        {
            _glfw.GetWindowSize(_window, out int width, out int height);
            return new Size(width, height);
        }
        set => _glfw.SetWindowSize(_window, value.Width, value.Height);
    }

    public Size WindowSize
    {
        get
        {
            _glfw.GetWindowFrameSize(_window, out int left, out int top, out int right, out int bottom);
            var client = ClientSize;
            return new Size(client.Width + left + right, client.Height + top + bottom);
        }
        set
        {
            _glfw.GetWindowFrameSize(_window, out int left, out int top, out int right, out int bottom);
            _glfw.SetWindowSize(_window, value.Width - left - right, value.Height - top - bottom);
        }
    }

    public Point Position
    {
        get
        {
            _glfw.GetWindowPos(_window, out int x, out int y);
            return new Point(x, y);
        }
        set => _glfw.SetWindowPos(_window, value.X, value.Y);
    }

    public bool IsVisible
    {
        get => _glfw.GetWindowAttrib(_window, WindowAttributeGetter.Visible);
        set
        {
            if (value) _glfw.ShowWindow(_window);
            else _glfw.HideWindow(_window);
        }
    }

    public bool ShouldClose => _glfw.WindowShouldClose(_window);

    public bool CursorVisible
    {
        get => _glfw.GetInputMode(_window, CursorStateAttribute.Cursor) != (int)CursorModeValue.CursorHidden;
        set
        {
            var mode = value ? CursorModeValue.CursorNormal : CursorModeValue.CursorHidden;
            _glfw.SetInputMode(_window, CursorStateAttribute.Cursor, mode);
        }
    }

    public CursorMode CursorMode
    {
        get
        {
            var mode = _glfw.GetInputMode(_window, CursorStateAttribute.Cursor);
            return mode switch
            {
                (int)CursorModeValue.CursorDisabled => CursorMode.CursorLocked,
                (int)CursorModeValue.CursorHidden => CursorMode.CursorGrabbed,
                _ => CursorMode.CursorFree
            };
        }
        set
        {
            var mode = value switch
            {
                CursorMode.CursorLocked => CursorModeValue.CursorDisabled,
                CursorMode.CursorGrabbed => CursorModeValue.CursorHidden,
                _ => CursorModeValue.CursorNormal
            };
            _glfw.SetInputMode(_window, CursorStateAttribute.Cursor, mode);
        }
    }

    public bool IsFullscreen
    {
        get;
        set
        {
            if (field == value) return;

            Silk.NET.GLFW.Monitor* monitor = _glfw.GetPrimaryMonitor();
            VideoMode* mode = _glfw.GetVideoMode(monitor);

            if (value)
            {
                _glfw.GetWindowPos(_window, out int x, out int y);
                _glfw.GetWindowSize(_window, out int w, out int h);
                _windowedPos = new Point(x, y);
                _windowedSize = new Size(w, h);
                _glfw.SetWindowMonitor(_window, monitor, 0, 0, mode->Width, mode->Height, mode->RefreshRate);
            }
            else
            {
                _glfw.SetWindowMonitor(_window, null,
                    _windowedPos.X, _windowedPos.Y,
                    _windowedSize.Width, _windowedSize.Height, 0);
            }

            field = value;
        }
    }

    public event EventHandler? Closed;
    public event EventHandler<SizeChangedEventArgs>? SizeChanged;

    public GlfwWindow(Ignis.Context.GLFW.GlfwContext context)
    {
        _glfw = context.Api;
        _glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        _window = _glfw.CreateWindow(800, 600, Title, null, null);

        if (_window == null)
            throw new Exception("Не удалось создать окно GLFW.");

        _sizeCallback = OnSizeChanged;
        _closeCallback = OnClosed;

        _glfw.SetWindowSizeCallback(_window, _sizeCallback);
        _glfw.SetWindowCloseCallback(_window, _closeCallback);
    }

    private void OnSizeChanged(WindowHandle* window, int width, int height)
        => SizeChanged?.Invoke(this, new SizeChangedEventArgs(new Size(width, height)));

    private void OnClosed(WindowHandle* window) => Closed?.Invoke(this, EventArgs.Empty);

    public void Update() => _glfw.PollEvents();

    public void Close() => _glfw.SetWindowShouldClose(_window, true);

    ~GlfwWindow() => Dispose(false);

    private void Dispose(bool _)
    {
        if (_isDisposed) return;

        if (_window != null)
            _glfw.DestroyWindow(_window);

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}