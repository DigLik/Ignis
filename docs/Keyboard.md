# Документация компонента: Keyboard

Класс `Ignis.Platform.Input.Keyboard` отвечает за обработку и отслеживание ввода с клавиатуры. Он работает в связке с классом `Window` и собирает события нажатия и отпускания клавиш.

---

## Конструктор

```csharp
public Keyboard(Window window)
```
Создает экземпляр класса `Keyboard` для указанного окна. Автоматически регистрирует клавиатуру в системе ввода окна и подписывается на его системные события ввода.

---

## События

*   **`KeyPressed`** (`event Action<Key>`): Вызывается однократно в кадре, когда клавиша переходит из отпущенного состояния в нажатое.
*   **`KeyDown`** (`event Action<Key>`): Вызывается каждый кадр, пока клавиша остается зажатой.
*   **`KeyReleased`** (`event Action<Key>`): Вызывается однократно в кадре, когда клавиша переходит из нажатого состояния в отпущенное.

---

## Методы опроса состояния (Polling)

Вы можете опрашивать состояние клавиш в любой момент времени (например, в цикле обновления игры):

*   **`IsKeyDown(Key key)`** (`bool`): Возвращает `true`, если указанная клавиша зажата в текущем кадре.
*   **`IsKeyUp(Key key)`** (`bool`): Возвращает `true`, если указанная клавиша отпущена в текущем кадре.
*   **`IsKeyPressed(Key key)`** (`bool`): Возвращает `true` только в том кадре, когда клавиша была нажата (переход из Up в Down). Полезно для действий, которые должны срабатывать один раз при нажатии (например, открытие меню или прыжок).
*   **`IsKeyReleased(Key key)`** (`bool`): Возвращает `true` только в том кадре, когда клавиша была отпущена (переход из Down в Up).

---

## Структура Key

Все клавиши представлены readonly структурой `Ignis.Core.Input.Key`, содержащей виртуальный код клавиши. В ней предопределены статические свойства для большинства клавиш:
*   Буквы: `Key.A` - `Key.Z`
*   Цифры: `Key.D0` - `Key.D9`
*   Функциональные клавиши: `Key.F1` - `Key.F24`
*   Стрелки: `Key.Left`, `Key.Right`, `Key.Up`, `Key.Down`
*   Системные клавиши: `Key.Escape`, `Key.Space`, `Key.Enter`, `Key.Shift`, `Key.Control`, `Key.Alt`
*   Кнопки Numpad: `Key.Numpad0` - `Key.Numpad9`

---

## Пример использования

```csharp
using System;
using Ignis.Core.Input;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

class Program
{
    static void Main()
    {
        using var window = new Window { Title = "Пример Keyboard" };
        using var keyboard = new Keyboard(window);

        // Обработка через события
        keyboard.KeyPressed += (key) =>
        {
            Console.WriteLine($"Событие: Нажата клавиша {key}");
            if (key == Key.Escape)
            {
                window.Close();
            }
        };

        keyboard.KeyReleased += (key) =>
        {
            Console.WriteLine($"Событие: Отпущена клавиша {key}");
        };

        // Подписка на обновление кадра окна для опроса состояния (Polling)
        window.FrameTick += () =>
        {
            if (keyboard.IsKeyDown(Key.W))
            {
                Console.WriteLine("Игрок двигается вперед (клавиша W зажата).");
            }
            if (keyboard.IsKeyPressed(Key.Space))
            {
                Console.WriteLine("Игрок совершил прыжок (Space нажат в этом кадре).");
            }
        };

        window.WaitClose();
    }
}
```
