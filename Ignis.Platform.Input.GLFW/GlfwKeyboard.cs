using Ignis.Core.Platform.Input;
using Ignis.Core.Platform.Input.Abstraction;
using Ignis.Core.Platform.Window.Abstraction;

using Silk.NET.GLFW;

namespace Ignis.Platform.Input.GLFW;

public sealed unsafe class GlfwKeyboard : IKeyboard
{
    private const int MaxKeys = 350;

    private readonly Glfw _glfw;
    private readonly WindowHandle* _window;

    private readonly byte[] _previousStates = new byte[MaxKeys];
    private readonly byte[] _currentStates = new byte[MaxKeys];

    private readonly GlfwCallbacks.KeyCallback _keyCallback;

    public GlfwKeyboard(Ignis.Context.GLFW.GlfwContext context, IWindow window)
    {
        _glfw = context.Api;
        _window = (WindowHandle*)window.WindowHandle;

        _keyCallback = OnKey;
        _glfw.SetKeyCallback(_window, _keyCallback);
    }

    private void OnKey(WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods)
    {
        int code = (int)key;
        if (code is >= 0 and < MaxKeys)
            _currentStates[code] = (byte)(action != InputAction.Release ? InputAction.Press : InputAction.Release);
    }

    public void Update() => Array.Copy(_currentStates, _previousStates, MaxKeys);

    public InputState GetKeyState(Key key)
    {
        var glfwKey = MapToGlfwKey(key);
        int code = (int)glfwKey;

        if (code is < 0 or >= MaxKeys) return InputState.Up;

        bool isDown = _currentStates[code] == (int)InputAction.Press;
        bool wasDown = _previousStates[code] == (int)InputAction.Press;

        if (isDown && wasDown) return InputState.Held;
        if (isDown && !wasDown) return InputState.Pressed;
        if (!isDown && wasDown) return InputState.Released;

        return InputState.Up;
    }

    private static Keys MapToGlfwKey(Key key)
    {
        int code = key.Code;
        if (code is >= 65 and <= 90) return (Keys)code;
        if (code is >= 48 and <= 57) return (Keys)code;
        if (code is >= 112 and <= 135) return (Keys)(code - 112 + 290);
        if (code is >= 96 and <= 105) return (Keys)(code - 96 + 320);

        return code switch
        {
            8 => Keys.Backspace,
            9 => Keys.Tab,
            13 => Keys.Enter,
            16 => Keys.ShiftLeft,
            17 => Keys.ControlLeft,
            18 => Keys.AltLeft,
            19 => Keys.Pause,
            20 => Keys.CapsLock,
            27 => Keys.Escape,
            32 => Keys.Space,
            33 => Keys.PageUp,
            34 => Keys.PageDown,
            35 => Keys.End,
            36 => Keys.Home,
            37 => Keys.Left,
            38 => Keys.Up,
            39 => Keys.Right,
            40 => Keys.Down,
            44 => Keys.PrintScreen,
            45 => Keys.Insert,
            46 => Keys.Delete,
            91 => Keys.SuperLeft,
            92 => Keys.SuperRight,
            106 => Keys.KeypadMultiply,
            107 => Keys.KeypadAdd,
            109 => Keys.KeypadSubtract,
            110 => Keys.KeypadDecimal,
            111 => Keys.KeypadDivide,
            144 => Keys.NumLock,
            145 => Keys.ScrollLock,
            160 => Keys.ShiftLeft,
            161 => Keys.ShiftRight,
            162 => Keys.ControlLeft,
            163 => Keys.ControlRight,
            164 => Keys.AltLeft,
            165 => Keys.AltRight,
            186 => Keys.Semicolon,
            187 => Keys.Equal,
            188 => Keys.Comma,
            189 => Keys.Minus,
            190 => Keys.Period,
            191 => Keys.Slash,
            192 => Keys.GraveAccent,
            219 => Keys.LeftBracket,
            220 => Keys.BackSlash,
            221 => Keys.RightBracket,
            222 => Keys.Apostrophe,
            _ => Keys.Unknown
        };
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed) return;

        _glfw.SetKeyCallback(_window, null);
        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}