# Документация компонента: Mouse

Класс `Ignis.Platform.Input.Mouse` отвечает за обработку событий координатного устройства ввода (мыши). Он собирает данные о перемещении курсора, кликах, удержании и отпускании кнопок, а также о прокрутке колесика.

---

## Конструктор

```csharp
public Mouse(Window window)
```
Создает экземпляр класса `Mouse` для указанного окна и автоматически привязывает его к циклу обработки сообщений ввода этого окна.

---

## Основные свойства

*   **`MousePosition`** (`Vector2`): Текущие координаты курсора мыши в пикселях относительно левого верхнего угла клиентской области окна.
*   **`MouseWheelDeltaY`** (`float`): Значение прокрутки колесика мыши по вертикали в текущем кадре. Если колесико не прокручивалось, равно `0`.
*   **`MotionPollRate`** (`float`): Частота опроса перемещения курсора мыши (по умолчанию `1000f` Гц).

---

## События

*   **`Moved`** (`event Action<Vector2>`): Вызывается при любом перемещении курсора мыши, возвращает новые координаты.
*   **`ButtonPressed`** (`event Action<MouseButton>`): Вызывается один раз в кадре при нажатии кнопки мыши.
*   **`ButtonDown`** (`event Action<MouseButton>`): Вызывается каждый кадр, пока кнопка мыши удерживается.
*   **`ButtonReleased`** (`event Action<MouseButton>`): Вызывается один раз в кадре при отпускании кнопки мыши.
*   **`WheelScrolled`** (`event Action<float>`): Вызывается при прокрутке колесика мыши, передает величину прокрутки.

---

## Методы опроса состояния (Polling)

*   **`IsButtonDown(MouseButton button)`** (`bool`): Возвращает `true`, если указанная кнопка мыши зажата в текущем кадре.
*   **`IsButtonUp(MouseButton button)`** (`bool`): Возвращает `true`, если указанная кнопка мыши отпущена в текущем кадре.
*   **`IsButtonPressed(MouseButton button)`** (`bool`): Возвращает `true` только в том кадре, когда кнопка мыши перешла в нажатое состояние (клик).
*   **`IsButtonReleased(MouseButton button)`** (`bool`): Возвращает `true` только в том кадре, когда кнопка мыши перешла в отпущенное состояние.

### Предопределенные кнопки `MouseButton`
*   `MouseButton.Left` — левая кнопка мыши.
*   `MouseButton.Right` — правая кнопка мыши.
*   `MouseButton.Middle` — средняя кнопка мыши (колёсико).
*   `MouseButton.XButton1` — первая дополнительная боковая кнопка.
*   `MouseButton.XButton2` — вторая дополнительная боковая кнопка.

---

## Преобразование в мировые координаты

Для 2D игр часто необходимо переводить позицию курсора из экранных координат (пикселей) в координаты игрового мира. Метод `GetWorldPosition` выполняет это преобразование:

```csharp
public Vector2 GetWorldPosition(Vector2 cameraPosition, Vector2 cameraSize)
```
*   **`cameraPosition`**: Позиция центра камеры в игровом мире.
*   **`cameraSize`**: Размеры видимой области камеры в игровых единицах.
*   **Возвращает**: Координаты курсора мыши в пространстве игрового мира.

---

## Пример использования

```csharp
using System;
using System.Numerics;
using Ignis.Core.Input;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

class Program
{
    static void Main()
    {
        using var window = new Window { Title = "Пример Mouse" };
        using var mouse = new Mouse(window);

        // Обработка через события
        mouse.ButtonPressed += (button) =>
        {
            Console.WriteLine($"Кликнута кнопка: {button}");
        };

        mouse.WheelScrolled += (delta) =>
        {
            Console.WriteLine($"Колесико прокручено на: {delta}");
        };

        // Опрос состояния (Polling) в цикле обновления кадра
        window.FrameTick += () =>
        {
            Vector2 pos = mouse.MousePosition;
            
            if (mouse.IsButtonDown(MouseButton.Left))
            {
                Console.WriteLine($"Левая кнопка мыши зажата. Курсор на: {pos.X}, {pos.Y}");
            }

            // Получение позиции курсора в игровом мире
            Vector2 worldPos = mouse.GetWorldPosition(new Vector2(0, 0), new Vector2(100, 100));
            // worldPos теперь содержит позицию курсора с учетом ортографической камеры
        };

        window.WaitClose();
    }
}
```
