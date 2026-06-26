using Ignis.Core.Numerics;
using Ignis.Core.Window;
using Ignis.Platform.Input;

namespace Ignis.Platform.Windowing;

/// <summary>Представляет окно операционной системы.</summary>
public sealed partial class Window : IDisposable
{
    internal event Action<nint>? RawInputReceived;
    internal event Action? FrameTick;
    internal readonly List<IInputDevice> InputDevices = [];

    /// <summary>Системный дескриптор окна (HWND в Windows).</summary>
    public nint Handle { get; private set; }

    /// <summary>Заголовок окна.</summary>
    public string Title
    {
        get;
        set
        {
            field = value;
            SetTitlePlatform(value);
        }
    } = string.Empty;

    private partial void SetTitlePlatform(string title);

    /// <summary>Размер клиентской области окна в пикселях.</summary>
    public Vector2Int Size
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            SetSizePlatform(value);
        }
    }

    private partial void SetSizePlatform(Vector2Int size);

    /// <summary>Положение левого верхнего угла клиентской области окна на экране.</summary>
    public Vector2Int Position { get; set; }

    /// <summary>Показывает или скрывает окно.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Флаг, указывающий, должно ли окно закрыться.</summary>
    public bool ShouldClose { get; internal set; }

    /// <summary>Управляет видимостью курсора мыши в пределах окна.</summary>
    public bool CursorVisible { get; set; }

    /// <summary>Режим отображения и захвата курсора мыши.</summary>
    public CursorMode CursorMode { get; set; }

    /// <summary>Указывает, находится ли окно в полноэкранном режиме.</summary>
    public bool IsFullscreen { get; set; }

    /// <summary>Событие, возникающее при закрытии окна.</summary>
    public event EventHandler? Closed;

    /// <summary>Событие, возникающее при изменении размера окна.</summary>
    public event EventHandler<SizeChangedEventArgs>? SizeChanged;

    /// <summary>Указывает, находится ли окно в процессе перемещения или изменения размера пользователем.</summary>
    public bool InSizeMove { get; internal set; }

    internal event EventHandler? ResizeEnded;

    /// <summary>Создает и инициализирует новый экземпляр класса <see cref="Window"/>.</summary>
    public Window() => InitPlatformWindow();

    private partial void InitPlatformWindow();

    /// <summary>Запускает цикл обработки сообщений окна и блокирует вызывающий поток, пока окно не закроется.</summary>
    public void WaitClose() => WaitClosePlatform();

    private partial void WaitClosePlatform();

    internal void RegisterInputDevice(IInputDevice device)
    {
        InputDevices.Add(device);
    }

    /// <summary>Инициирует закрытие окна.</summary>
    public void Close()
    {
        ShouldClose = true;
        ClosePlatform();
    }

    private partial void ClosePlatform();

    /// <summary>Освобождает ресурсы, используемые окном.</summary>
    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }

    private partial void DisposePlatform();
}