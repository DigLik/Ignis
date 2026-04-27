namespace Ignis.Core.Platform.Input;

/// <summary>Кнопка мыши, идентифицируемая числовым кодом.</summary>
public readonly struct MouseButton(int code) : IEquatable<MouseButton>
{
    /// <summary>Числовой код кнопки.</summary>
    public readonly int Code = code;

    public static readonly MouseButton Left = new(0);
    public static readonly MouseButton Right = new(1);
    public static readonly MouseButton Middle = new(2);
    public static readonly MouseButton XButton1 = new(3);
    public static readonly MouseButton XButton2 = new(4);

    public static bool operator ==(MouseButton left, MouseButton right) => left.Code == right.Code;
    public static bool operator !=(MouseButton left, MouseButton right) => left.Code != right.Code;

    public bool Equals(MouseButton other) => Code == other.Code;
    public override bool Equals(object? obj) => obj is MouseButton button && Equals(button);
    public override int GetHashCode() => Code;
    public override string ToString()
        => Code switch
        {
            0 => $"{nameof(MouseButton)}:{nameof(Left)}",
            1 => $"{nameof(MouseButton)}:{nameof(Right)}",
            2 => $"{nameof(MouseButton)}:{nameof(Middle)}",
            _ => $"{nameof(MouseButton)}:{Code}",
        };

    public static explicit operator int(MouseButton button) => button.Code;
    public static explicit operator MouseButton(int code) => new(code);
}