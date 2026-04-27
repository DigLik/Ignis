using System.Drawing;

namespace Ignis.Core.Platform.Window.Abstraction;

/// <summary>
/// Контракт окна приложения. Управляет состоянием окна, его размерами, позицией и курсором.
/// </summary>
public interface IWindow : IDisposable
{
    /// <summary>Нативный хэндл окна.</summary>
    nint WindowHandle { get; }

    /// <summary>Заголовок окна.</summary>
    string Title { get; set; }

    /// <summary>Размер клиентской области (без рамки).</summary>
    Size ClientSize { get; set; }

    /// <summary>Полный размер окна (включая рамку и заголовок).</summary>
    Size WindowSize { get; set; }

    /// <summary>Позиция верхнего левого угла окна на экране.</summary>
    Point Position { get; set; }

    /// <summary>Видимо ли окно.</summary>
    bool IsVisible { get; set; }

    /// <summary>Должно ли окно закрыться (установлено пользователем или системой).</summary>
    bool ShouldClose { get; }

    /// <summary>Видим ли курсор мыши.</summary>
    bool CursorVisible { get; set; }

    /// <summary>Режим курсора (свободный, захваченный, заблокированный).</summary>
    CursorMode CursorMode { get; set; }

    /// <summary>Полноэкранный режим.</summary>
    bool IsFullscreen { get; set; }

    /// <summary>Вызывается при закрытии окна.</summary>
    event EventHandler Closed;

    /// <summary>Вызывается при изменении размера окна.</summary>
    event EventHandler<SizeChangedEventArgs> SizeChanged;

    /// <summary>Обрабатывает оконные события. Вызывается каждый кадр.</summary>
    void Update();

    /// <summary>Запрашивает закрытие окна.</summary>
    void Close();
}