using System.Numerics;

namespace Ignis.Core.Platform.Input.Abstraction;

/// <summary>
/// Контракт мыши. Предоставляет позицию курсора, состояние кнопок и колесо прокрутки.
/// </summary>
public interface IMouse : IDisposable
{
    /// <summary>Дельта колеса прокрутки по оси Y за последний кадр.</summary>
    float MouseWheelDeltaY { get; }

    /// <summary>Нормализованная позиция курсора в диапазоне [-1, 1] относительно окна.</summary>
    Vector2 MousePosition { get; }

    /// <summary>Обновляет внутреннее состояние мыши. Вызывается каждый кадр.</summary>
    void Update();

    /// <summary>Возвращает текущее состояние указанной кнопки мыши.</summary>
    /// <param name="button">Кнопка мыши для проверки.</param>
    InputState GetMouseButtonState(MouseButton button);

    /// <summary>Преобразует нормализованную позицию курсора в мировые координаты.</summary>
    /// <param name="position">Позиция камеры в мировых координатах.</param>
    /// <param name="boundingBox">Размер видимой области.</param>
    Vector2 GetWorldPosition(Vector2 position, Vector2 boundingBox);
}