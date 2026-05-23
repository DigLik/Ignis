using System.Numerics;

using Ignis.Core.Input;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Input;

public sealed partial class Mouse : IDisposable
{
    private const int MaxButtons = 8;
    private readonly Window _window;

    private readonly byte[] _previousStates = new byte[MaxButtons];
    private readonly byte[] _currentStates = new byte[MaxButtons];

    public float MouseWheelDeltaY { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public Mouse(Window window)
    {
        _window = window;
        InitPlatformMouse();
    }
    private partial void InitPlatformMouse();

    public void Update()
    {
        Array.Copy(_currentStates, _previousStates, MaxButtons);
        MouseWheelDeltaY = 0;
        UpdatePlatform();
    }
    private partial void UpdatePlatform();

    public bool IsButtonDown(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _currentStates[code] == 1;
    }
    public bool IsButtonUp(MouseButton button) => !IsButtonDown(button);

    public bool IsButtonPressed(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _currentStates[code] == 1 && _previousStates[code] == 0;
    }

    public bool IsButtonReleased(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _currentStates[code] == 0 && _previousStates[code] == 1;
    }

    public Vector2 GetWorldPosition(Vector2 position, Vector2 boundingBox)
    {
        float w = _window.Size.X;
        float h = _window.Size.Y;
        if (w <= 0 || h <= 0) return position;
        return position + MousePosition * (boundingBox * 0.5f);
    }

    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }
    private partial void DisposePlatform();
}