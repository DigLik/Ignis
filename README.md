# Ignis

**Ignis** — это легковесный, высокопроизводительный фреймворк для создания 2D-приложений и игр на C# / .NET 10. Фреймворк предоставляет удобные абстракции для работы с системными окнами, обработки ввода и быстрого рендеринга 2D-графики на базе Vulkan API.

В данный момент фреймворк поддерживает работу на платформе **Windows (Win32)**.

---

## Ключевые возможности

*   **Нативные Окна**: Управление окнами ОС, поддержка полноэкранного режима, кастомизация курсора (свободный, захваченный, заблокированный).
*   **Современный Ввод (Raw Input)**: Высокоточный и быстрый опрос состояний клавиатуры и мыши через системный Raw Input без задержек.
*   **Пакетный Рендеринг на Vulkan**: 2D-рендерер, автоматически объединяющий фигуры (круги, эллипсы, прямоугольники, линии, многоугольники) и текст в пакеты для минимизации вызовов отрисовки (draw calls) и достижения максимального FPS.
*   **Встроенный Шрифт**: Быстрый рендеринг текста с автоматическим измерением размеров строк в мировых координатах.

---

## Подробная документация по компонентам

Для детального изучения каждого модуля Ignis перейдите по соответствующим ссылкам:

1.  **[Управление окнами (Window.md)](docs/Window.md)** — жизненный цикл окна, полноэкранный режим, размеры и режимы курсора.
2.  **[Устройство клавиатуры (Keyboard.md)](docs/Keyboard.md)** — события клавиш, опрос состояний и перечисление кнопок.
3.  **[Устройство мыши (Mouse.md)](docs/Mouse.md)** — позиция курсора, события кликов, прокрутка колесика и трансформация координат в игровой мир.
4.  **[Система рендеринга (Renderer.md)](docs/Renderer.md)** — VSync, ограничение FPS, камера, фазы кадра, рисование плоских геометрических фигур и текста.

---

## Быстрый старт

Простейший пример игрового приложения с использованием Ignis:

```csharp
using System.Numerics;
using Ignis.Core.Graphics;
using Ignis.Core.Numerics;
using Ignis.Platform.Graphics;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

// 1. Создаем окно приложения
using var window = new Window 
{ 
    Title = "Ignis Quick Start", 
    Size = new Vector2Int(1280, 720) 
};

// 2. Инициализируем устройства ввода
using var keyboard = new Keyboard(window);
using var mouse = new Mouse(window);

// 3. Создаем рендерер с вертикальной синхронизацией
using var renderer = new Renderer(window) { VSync = VSyncMode.On };

// Позиция нашего игрока
Vector2 playerPosition = Vector2.Zero;

// 4. Подписываемся на фазу рендеринга кадра
renderer.RenderRequested += () =>
{
    // Очищаем кадр
    renderer.ClearBackground(new Color(0.15f, 0.15f, 0.15f));

    // Настраиваем камеру (область видимости 20x11.25 игровых единиц)
    renderer.UpdateCamera(Vector2.Zero, new Vector2(20f, 11.25f));

    // Обрабатываем перемещение игрока на основе дельты времени кадра
    float speed = 5f;
    if (keyboard.IsKeyDown(Key.W) || keyboard.IsKeyDown(Key.Up)) playerPosition.Y += speed * renderer.DeltaTime;
    if (keyboard.IsKeyDown(Key.S) || keyboard.IsKeyDown(Key.Down)) playerPosition.Y -= speed * renderer.DeltaTime;
    if (keyboard.IsKeyDown(Key.A) || keyboard.IsKeyDown(Key.Left)) playerPosition.X -= speed * renderer.DeltaTime;
    if (keyboard.IsKeyDown(Key.D) || keyboard.IsKeyDown(Key.Right)) playerPosition.X += speed * renderer.DeltaTime;

    // Закрываем окно по кнопке Escape
    if (keyboard.IsKeyPressed(Key.Escape)) window.Close();

    // Рисуем сетку
    renderer.DrawLine(new Vector2(-10, 0), new Vector2(10, 0), 0.02f, Color.Red);
    renderer.DrawLine(new Vector2(0, -6), new Vector2(0, 6), 0.02f, Color.Green);

    // Рисуем игрока (желтый круг)
    renderer.DrawCircle(playerPosition, 0.5f, Color.Yellow);

    // Рисуем координаты под игроком
    string text = $"Pos: {playerPosition.X:F2}, {playerPosition.Y:F2}";
    float textWidth = Renderer.MeasureText(text, 0.4f);
    renderer.DrawText(text, new Vector2(playerPosition.X - textWidth / 2f, playerPosition.Y - 0.7f), 0.4f, Color.White);
};

// 5. Запускаем цикл обработки сообщений окна
window.WaitClose();
```

---

## Сборка и создание NuGet-пакета

Для сборки проекта и упаковки его в NuGet-пакет воспользуйтесь стандартными инструментами .NET SDK.

### Шаг 1: Компиляция шейдеров и сборка библиотеки
Сборка всего решения в конфигурации `Release`:
```bash
dotnet build Ignis.slnx -c Release
```

### Шаг 2: Упаковка библиотеки
Создание NuGet-пакета `.nupkg` на базе проекта `Ignis.Platform`:
```bash
dotnet pack Ignis.Platform/Ignis.Platform.csproj -c Release
```
Сгенерированный пакет будет находиться по пути `Ignis.Platform/bin/Release/DigLik.Ignis.<Version>.nupkg` и будет содержать:
- Сборки библиотеки (`Ignis.Platform.dll`, `Ignis.Core.dll`, а также биндинги `Vulkan.dll` и `Windows.dll`).
- Скомпилированные файлы XML-документации API (`Ignis.Platform.xml` и `Ignis.Core.xml`).
- Описание и файл `README.md`.

---

## Лицензия

Проект распространяется под лицензией **Mozilla Public License 2.0 (MPL-2.0)**. Подробнее см. в файле [LICENSE.txt](LICENSE.txt).