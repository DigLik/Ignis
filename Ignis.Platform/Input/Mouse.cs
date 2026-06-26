using System.Numerics;
using Ignis.Core.Input;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Input;

/// <summary>Представляет устройство ввода мыши.</summary>
public sealed partial class Mouse : IInputDevice, IDisposable
{
    private const int MaxButtons = 8;
    private readonly Window _window;

    private readonly byte[] _rawCurrentStates = new byte[MaxButtons];
    private readonly byte[] _framePreviousStates = new byte[MaxButtons];
    private readonly byte[] _frameCurrentStates = new byte[MaxButtons];

    private Vector2 _rawMousePosition;
    private float _rawWheelDeltaY;

    /// <summary>Изменение прокрутки колесика мыши по оси Y в текущем кадре.</summary>
    public float MouseWheelDeltaY { get; private set; }

    /// <summary>Текущее положение курсора мыши в клиентских координатах окна.</summary>
    public Vector2 MousePosition { get; private set; }

    /// <summary>Частота опроса перемещения мыши.</summary>
    public float MotionPollRate { get; set; } = 1000f;

    /// <summary>Событие, возникающее при перемещении мыши.</summary>
    public event Action<Vector2>? Moved;

    /// <summary>Событие, возникающее при однократном нажатии кнопки мыши в текущем кадре.</summary>
    public event Action<MouseButton>? ButtonPressed;

    /// <summary>Событие, возникающее при зажатии кнопки мыши.</summary>
    public event Action<MouseButton>? ButtonDown;

    /// <summary>Событие, возникающее при отпускании кнопки мыши.</summary>
    public event Action<MouseButton>? ButtonReleased;

    /// <summary>Событие, возникающее при прокрутке колесика мыши.</summary>
    public event Action<float>? WheelScrolled;

    /// <summary>Создает новый экземпляр класса <see cref="Mouse"/> для указанного окна.</summary>
    /// <param name="window">Окно, события ввода которого будут отслеживаться.</param>
    public Mouse(Window window)
    {
        _window = window;
        _window.RegisterInputDevice(this);
        InitPlatformMouse();
    }

    private partial void InitPlatformMouse();

    void IInputDevice.SnapshotFrame()
    {
        Array.Copy(_frameCurrentStates, _framePreviousStates, MaxButtons);
        Array.Copy(_rawCurrentStates, _frameCurrentStates, MaxButtons);
        MousePosition = _rawMousePosition;
        MouseWheelDeltaY = _rawWheelDeltaY;
        _rawWheelDeltaY = 0f;
    }

    /// <summary>Проверяет, зажата ли кнопка мыши в текущем кадре.</summary>
    /// <param name="button">Кнопка мыши для проверки.</param>
    /// <returns>True, если кнопка зажата, иначе false.</returns>
    public bool IsButtonDown(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _frameCurrentStates[code] == 1;
    }

    /// <summary>Проверяет, отпущена ли кнопка мыши в текущем кадре.</summary>
    /// <param name="button">Кнопка мыши для проверки.</param>
    /// <returns>True, если кнопка отпущена, иначе false.</returns>
    public bool IsButtonUp(MouseButton button) => !IsButtonDown(button);

    /// <summary>Проверяет, была ли кнопка мыши нажата именно в текущем кадре (переход из ненажатого состояния в нажатое).</summary>
    /// <param name="button">Кнопка мыши для проверки.</param>
    /// <returns>True, если кнопка была нажата в этом кадре, иначе false.</returns>
    public bool IsButtonPressed(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _frameCurrentStates[code] == 1 && _framePreviousStates[code] == 0;
    }

    /// <summary>Проверяет, была ли кнопка мыши отпущена именно в текущем кадре (переход из нажатого состояния в ненажатое).</summary>
    /// <param name="button">Кнопка мыши для проверки.</param>
    /// <returns>True, если кнопка была отпущена в этом кадре, иначе false.</returns>
    public bool IsButtonReleased(MouseButton button)
    {
        int code = button.Code;
        if (code is < 0 or >= MaxButtons) return false;
        return _frameCurrentStates[code] == 0 && _framePreviousStates[code] == 1;
    }

    /// <summary>Преобразует координаты мыши из оконных в мировое пространство.</summary>
    /// <param name="position">Исходная позиция в мировом пространстве.</param>
    /// <param name="boundingBox">Размеры видимой области мирового пространства.</param>
    /// <returns>Результирующая позиция мыши в мировом пространстве.</returns>
    public Vector2 GetWorldPosition(Vector2 position, Vector2 boundingBox)
    {
        float w = _window.Size.X;
        float h = _window.Size.Y;
        if (w <= 0 || h <= 0 || boundingBox.X <= 0 || boundingBox.Y <= 0) return position;

        float windowAspect = w / h;
        float targetAspect = boundingBox.X / boundingBox.Y;
        Vector2 correctedSize;

        if (windowAspect > targetAspect)
        {
            correctedSize = new Vector2(boundingBox.Y * windowAspect, boundingBox.Y);
        }
        else
        {
            correctedSize = new Vector2(boundingBox.X, boundingBox.X / windowAspect);
        }

        Vector2 cameraCenter = position + boundingBox * 0.5f;
        return cameraCenter + MousePosition * (correctedSize * 0.5f);
    }

    /// <summary>Освобождает ресурсы, используемые мышью.</summary>
    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }

    private partial void DisposePlatform();
}