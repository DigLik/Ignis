# Документация компонента: Renderer

Класс `Ignis.Platform.Graphics.Renderer` является ядром графической подсистемы Ignis. Он инициализирует графический контекст Vulkan для окна, берет на себя управление очередью кадра и предоставляет простой интерфейс для пакетного рисования (batch rendering) плоских геометрических фигур и текста.

---

## Конструктор и инициализация

```csharp
public Renderer(Window window)
```
Инициализирует графические ресурсы Vulkan (логическое и физическое устройства, очередь команд, аллокатор памяти, swapchain и конвейер рендеринга) для указанного окна.

---

## Основные свойства и настройки

*   **`VSync`** (`VSyncMode`): Режим вертикальной синхронизации. Изменение свойства на лету автоматически пересоздает swapchain. Поддерживаемые режимы:
    *   `VSyncMode.Off` — Immediate (выключено, с возможными разрывами экрана, максимальный FPS).
    *   `VSyncMode.On` — FIFO (включено, синхронизация с частотой экрана, без разрывов).
    *   `VSyncMode.Relaxed` — FIFO Relaxed (включено, но выводится немедленно при задержках кадра).
    *   `VSyncMode.Mailbox` — Mailbox (включено с тройной буферизацией, минимальная задержка ввода без разрывов).
*   **`MaxFPS`** (`int`): Ограничение максимальной частоты кадров. Если установлено в 0 или меньше, ограничение отключено.
*   **`DeltaTime`** (`float`): Время, затраченное на обработку и отрисовку предыдущего кадра, в секундах. Используется для плавной анимации и физики, независимой от FPS.

---

## События цикла отрисовки

Цикл обновления кадра состоит из трех фаз, для каждой из которых предусмотрено событие:
*   **`EarlyRenderRequested`**: Ранняя фаза отрисовки (например, подготовка данных или фоновая логика).
*   **`RenderRequested`**: Основная фаза отрисовки игровых объектов.
*   **`LateRenderRequested`**: Поздняя фаза отрисовки (например, UI, накладываемый поверх игрового мира).

---

## Ортографическая камера

Рендерер использует ортографическую проекцию для рисования в мировых координатах. Камера настраивается методом:

```csharp
public void UpdateCamera(Vector2 position, Vector2 size)
```
*   `position` — координаты центра видимой области камеры в игровом мире.
*   `size` — ширина и высота видимой области камеры в игровом мире (например, `100x100` единиц).

---

## Методы рисования фигур (2D-примитивов)

Все геометрические фигуры отрисовываются пакетами для максимальной производительности:

*   **`ClearBackground(Color color)`**: Очищает текущий кадр сплошным цветом.
*   **`DrawLine(Vector2 p1, Vector2 p2, float thickness, Color color)`**: Рисует линию между двумя точками с заданной толщиной.
*   **`DrawCircle(Vector2 position, float radius, Color color)`**: Рисует заполненный круг.
*   **`DrawEllipse(Vector2 position, Vector2 size, float angle, Color color)`**: Рисует заполненный эллипс, повернутый на заданный угол `angle` (в радианах).
*   **`DrawRectangle(Vector2 position, Vector2 size, float angle, Color color)`**: Рисует заполненный повернутый прямоугольник. `position` определяет центр прямоугольника.
*   **`DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color)`**: Рисует заполненный треугольник по трем точкам вершин.
*   **`DrawPolygon(ReadOnlySpan<Vector2> vertices, Color color)`**: Рисует произвольный выпуклый заполненный многоугольник.

---

## Отрисовка и измерение текста

Рендерер включает встроенный растровый шрифт для быстрого вывода отладочного или игрового текста:

*   **`DrawText(string text, Vector2 position, float size, Color color)`**: Отрисовывает текст на экране. `position` указывает на левый верхний угол первой буквы. `size` задает высоту текста в игровых единицах.
*   **`MeasureText(string text, float size)`** (`static float`): Статический метод, позволяющий измерить ширину строки текста в мировых единицах при заданном размере шрифта `size`.

---

## Пример использования

```csharp
using System;
using System.Numerics;
using Ignis.Core.Graphics;
using Ignis.Core.Numerics;
using Ignis.Platform.Graphics;
using Ignis.Platform.Windowing;

class Program
{
    static void Main()
    {
        using var window = new Window { Title = "Пример рендеринга", Size = new Vector2Int(1024, 768) };
        using var renderer = new Renderer(window) { VSync = VSyncMode.Mailbox };

        float angle = 0f;

        // Подписываемся на событие основного рендеринга
        renderer.RenderRequested += () =>
        {
            // Очистка фона в темно-серый цвет
            renderer.ClearBackground(new Color(0.1f, 0.1f, 0.1f));

            // Обновление камеры (центр в 0, 0, размер области 20x20 единиц)
            renderer.UpdateCamera(Vector2.Zero, new Vector2(20f, 20f));

            // Рисуем сетку координат
            renderer.DrawLine(new Vector2(-10, 0), new Vector2(10, 0), 0.05f, Color.Red);
            renderer.DrawLine(new Vector2(0, -10), new Vector2(0, 10), 0.05f, Color.Green);

            // Рисуем вращающийся квадрат
            angle += renderer.DeltaTime;
            renderer.DrawRectangle(new Vector2(2, 2), new Vector2(3, 3), angle, Color.Yellow);

            // Рисуем круги и треугольники
            renderer.DrawCircle(new Vector2(-3, -3), 1.5f, Color.Cyan);
            renderer.DrawTriangle(new Vector2(0, 3), new Vector2(-2, 1), new Vector2(2, 1), Color.Magenta);

            // Рисуем текст
            string text = "Привет, Ignis!";
            float textWidth = Renderer.MeasureText(text, 1f);
            // Выравниваем текст по центру
            renderer.DrawText(text, new Vector2(-textWidth / 2f, -6f), 1f, Color.White);
        };

        // Запуск окна
        window.WaitClose();
    }
}
```
