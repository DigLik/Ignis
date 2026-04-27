using System.Numerics;

using Ignis.Core.Graphics;

namespace Ignis.Core.Platform.Graphics.Abstraction;

/// <summary>
/// Контракт рендерера. Определяет примитивы рисования, управление кадром и камерой.
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>Начинает новый кадр. Возвращает false если кадр невозможен (например, окно свёрнуто).</summary>
    bool BeginFrame();

    /// <summary>Очищает фон указанным цветом.</summary>
    /// <param name="clearColor">Цвет заливки фона.</param>
    bool ClearBackground(Color clearColor);

    /// <summary>Завершает кадр и отправляет его на отображение.</summary>
    void EndFrame();

    /// <summary>Рисует закрашенный круг.</summary>
    void DrawCircle(Vector2 position, float radius, Color color);

    /// <summary>Рисует закрашенный эллипс с вращением.</summary>
    void DrawEllipse(Vector2 position, Vector2 size, float angle, Color color);

    /// <summary>Рисует закрашенный прямоугольник с вращением.</summary>
    void DrawRectangle(Vector2 position, Vector2 size, float angle, Color color);

    /// <summary>Рисует линию заданной толщины.</summary>
    void DrawLine(Vector2 firstPoint, Vector2 secondPoint, float thickness, Color color);

    /// <summary>Рисует закрашенный треугольник по трём вершинам.</summary>
    void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color);

    /// <summary>Рисует закрашенный полигон по набору вершин.</summary>
    void DrawPolygon(ReadOnlySpan<Vector2> points, Color color);

    /// <summary>Рисует текст.</summary>
    void DrawText(string text, Vector2 position, float size, Color color);

    /// <summary>Обновляет позицию и область видимости камеры.</summary>
    /// <param name="position">Центр камеры в мировых координатах.</param>
    /// <param name="boundingBox">Размер видимой области.</param>
    void UpdateCamera(Vector2 position, Vector2 boundingBox);

    /// <summary>Обрабатывает изменение размера поверхности рендеринга.</summary>
    /// <param name="newSize">Новый размер клиентской области.</param>
    void Resize(System.Drawing.Size newSize);
}