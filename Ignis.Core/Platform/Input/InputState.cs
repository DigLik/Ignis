namespace Ignis.Core.Platform.Input;

/// <summary>Состояние кнопки ввода (клавиша клавиатуры или кнопка мыши).</summary>
public enum InputState
{
    /// <summary>Кнопка не нажата.</summary>
    Up,
    /// <summary>Кнопка только что нажата (в этом кадре).</summary>
    Pressed,
    /// <summary>Кнопка удерживается (нажата более одного кадра).</summary>
    Held,
    /// <summary>Кнопка только что отпущена (в этом кадре).</summary>
    Released
}
