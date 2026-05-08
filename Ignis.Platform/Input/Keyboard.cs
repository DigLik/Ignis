using Ignis.Core.Input;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Input;

public sealed partial class Keyboard : IDisposable
{
    private const int MaxKeys = 350;
    private readonly Window _window;

    private readonly byte[] _previousStates = new byte[MaxKeys];
    private readonly byte[] _currentStates = new byte[MaxKeys];

    public Keyboard(Window window)
    {
        _window = window;
        InitPlatformKeyboard();
    }
    private partial void InitPlatformKeyboard();

    public void Update()
    {
        Array.Copy(_currentStates, _previousStates, MaxKeys);
        UpdatePlatform();
    }
    private partial void UpdatePlatform();

    public bool IsKeyDown(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _currentStates[code] == 1;
    }

    public bool IsKeyUp(Key key) => !IsKeyDown(key);

    public bool IsKeyPressed(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _currentStates[code] == 1 && _previousStates[code] == 0;
    }

    public bool IsKeyReleased(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _currentStates[code] == 0 && _previousStates[code] == 1;
    }

    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }
    private partial void DisposePlatform();
}