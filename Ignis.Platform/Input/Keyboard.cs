using Ignis.Core.Input;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Input;

/// <summary>Представляет устройство ввода клавиатуры.</summary>
public sealed partial class Keyboard : IInputDevice, IDisposable
{
    private const int MaxKeys = 350;
    private readonly Window _window;

    private readonly byte[] _rawCurrentStates = new byte[MaxKeys];
    private readonly byte[] _framePreviousStates = new byte[MaxKeys];
    private readonly byte[] _frameCurrentStates = new byte[MaxKeys];

    /// <summary>Событие, возникающее при однократном нажатии клавиши в текущем кадре.</summary>
    public event Action<Key>? KeyPressed;

    /// <summary>Событие, возникающее при удержании клавиши.</summary>
    public event Action<Key>? KeyDown;

    /// <summary>Событие, возникающее при отпускании клавиши.</summary>
    public event Action<Key>? KeyReleased;

    /// <summary>Создает новый экземпляр класса <see cref="Keyboard"/> для указанного окна.</summary>
    /// <param name="window">Окно, события ввода которого будут отслеживаться.</param>
    public Keyboard(Window window)
    {
        _window = window;
        _window.RegisterInputDevice(this);
        InitPlatformKeyboard();
    }

    private partial void InitPlatformKeyboard();

    void IInputDevice.SnapshotFrame()
    {
        Array.Copy(_frameCurrentStates, _framePreviousStates, MaxKeys);
        Array.Copy(_rawCurrentStates, _frameCurrentStates, MaxKeys);
    }

    /// <summary>Проверяет, зажата ли клавиша в текущем кадре.</summary>
    /// <param name="key">Клавиша для проверки.</param>
    /// <returns>True, если клавиша зажата, иначе false.</returns>
    public bool IsKeyDown(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _frameCurrentStates[code] == 1;
    }

    /// <summary>Проверяет, отпущена ли клавиша в текущем кадре.</summary>
    /// <param name="key">Клавиша для проверки.</param>
    /// <returns>True, если клавиша отпущена, иначе false.</returns>
    public bool IsKeyUp(Key key) => !IsKeyDown(key);

    /// <summary>Проверяет, была ли клавиша нажата именно в текущем кадре (переход из ненажатого состояния в нажатое).</summary>
    /// <param name="key">Клавиша для проверки.</param>
    /// <returns>True, если клавиша была нажата в этом кадре, иначе false.</returns>
    public bool IsKeyPressed(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _frameCurrentStates[code] == 1 && _framePreviousStates[code] == 0;
    }

    /// <summary>Проверяет, была ли клавиша отпущена именно в текущем кадре (переход из нажатого состояния в ненажатое).</summary>
    /// <param name="key">Клавиша для проверки.</param>
    /// <returns>True, если клавиша была отпущена в этом кадре, иначе false.</returns>
    public bool IsKeyReleased(Key key)
    {
        int code = key.Code;
        if (code is < 0 or >= MaxKeys) return false;
        return _frameCurrentStates[code] == 0 && _framePreviousStates[code] == 1;
    }

    /// <summary>Освобождает ресурсы, используемые клавиатурой.</summary>
    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }

    private partial void DisposePlatform();
}