using System.Numerics;

using Ignis.Core.Platform.Input;
using Ignis.Core.Platform.Input.Abstraction;
using Ignis.Core.Platform.Window.Abstraction;

using Silk.NET.GLFW;

namespace Ignis.Platform.Input.GLFW;

public sealed unsafe class GlfwMouse : IMouse
{
    private const int MaxButtons = 8;

    private readonly Glfw _glfw;
    private readonly WindowHandle* _window;
    private readonly IWindow _appWindow;

    private Vector2 _mousePosition;
    private float _currentWheelDelta;

    private readonly byte[] _previousStates = new byte[MaxButtons];
    private readonly byte[] _currentStates = new byte[MaxButtons];

    private readonly GlfwCallbacks.ScrollCallback _scrollCallback;
    private readonly GlfwCallbacks.MouseButtonCallback _mouseButtonCallback;
    private readonly GlfwCallbacks.CursorPosCallback _cursorPosCallback;

    public float MouseWheelDeltaY { get; private set; }
    public Vector2 MousePosition => _mousePosition;

    public GlfwMouse(Ignis.Context.GLFW.GlfwContext context, IWindow window)
    {
        _glfw = context.Api;
        _window = (WindowHandle*)window.WindowHandle;
        _appWindow = window;

        _scrollCallback = OnScroll;
        _mouseButtonCallback = OnMouseButton;
        _cursorPosCallback = OnCursorPos;

        _glfw.SetScrollCallback(_window, _scrollCallback);
        _glfw.SetMouseButtonCallback(_window, _mouseButtonCallback);
        _glfw.SetCursorPosCallback(_window, _cursorPosCallback);
    }

    private void OnScroll(WindowHandle* window, double offsetX, double offsetY)
        => _currentWheelDelta += (float)offsetY;

    private void OnMouseButton(WindowHandle* window, Silk.NET.GLFW.MouseButton button, InputAction action, KeyModifiers mods)
    {
        int code = (int)button;
        if (code is >= 0 and < MaxButtons)
            _currentStates[code] = (byte)(action != InputAction.Release ? InputAction.Press : InputAction.Release);
    }

    private void OnCursorPos(WindowHandle* window, double x, double y)
    {
        float w = _appWindow.ClientSize.Width;
        float h = _appWindow.ClientSize.Height;

        if (w > 0 && h > 0)
        {
            float nx = (float)(x / w) * 2f - 1f;
            float ny = -((float)(y / h) * 2f - 1f);

            _mousePosition = new Vector2(nx, ny);
        }
    }

    public void Update()
    {
        Array.Copy(_currentStates, _previousStates, MaxButtons);
        MouseWheelDeltaY = _currentWheelDelta;
        _currentWheelDelta = 0;
    }

    public InputState GetMouseButtonState(Core.Platform.Input.MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return InputState.Up;

        bool isDown = _currentStates[code] == (int)InputAction.Press;
        bool wasDown = _previousStates[code] == (int)InputAction.Press;

        if (isDown && wasDown) return InputState.Held;
        if (isDown && !wasDown) return InputState.Pressed;
        if (!isDown && wasDown) return InputState.Released;

        return InputState.Up;
    }

    public Vector2 GetWorldPosition(Vector2 position, Vector2 boundingBox)
    {
        float w = _appWindow.ClientSize.Width;
        float h = _appWindow.ClientSize.Height;

        if (w <= 0 || h <= 0) return position;

        return position + _mousePosition * (boundingBox * 0.5f);
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed) return;

        _glfw.SetScrollCallback(_window, null);
        _glfw.SetMouseButtonCallback(_window, null);
        _glfw.SetCursorPosCallback(_window, null);
        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}