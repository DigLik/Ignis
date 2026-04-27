using Ignis.Core.Platform.Graphics.Abstraction;
using Ignis.Core.Platform.Input.Abstraction;
using Ignis.Core.Platform.Window.Abstraction;

namespace Ignis.Core;

/// <summary>
/// Контейнер платформенных сервисов движка.
/// Владеет всеми зарегистрированными сервисами и освобождает их при Dispose
/// в порядке: ввод → рендерер → окно.
/// </summary>
public sealed class IgnisEngine : IDisposable
{
    /// <summary>Окно приложения.</summary>
    public required IWindow Window { get; init; }

    /// <summary>Клавиатура.</summary>
    public required IKeyboard Keyboard { get; init; }

    /// <summary>Мышь.</summary>
    public required IMouse Mouse { get; init; }

    /// <summary>Рендерер.</summary>
    public required IRenderer Renderer { get; init; }

    public void Dispose()
    {
        Mouse.Dispose();
        Keyboard.Dispose();
        Renderer.Dispose();
        Window.Dispose();
    }
}
