namespace Ignis.Core.Input;

/// <summary>Кнопка мыши, идентифицируемая числовым кодом.</summary>
/// <param name="code">Числовой код кнопки мыши.</param>
public readonly struct MouseButton(int code) : IEquatable<MouseButton>
{
    /// <summary>Числовой код кнопки.</summary>
    public int Code { get; } = code;

    /// <summary>Левая кнопка мыши.</summary>
    public static readonly MouseButton Left = new(0);
    /// <summary>Правая кнопка мыши.</summary>
    public static readonly MouseButton Right = new(1);
    /// <summary>Средняя кнопка мыши (колёсико).</summary>
    public static readonly MouseButton Middle = new(2);
    /// <summary>Первая дополнительная кнопка мыши.</summary>
    public static readonly MouseButton XButton1 = new(3);
    /// <summary>Вторая дополнительная кнопка мыши.</summary>
    public static readonly MouseButton XButton2 = new(4);

    /// <summary>Проверяет равенство двух кнопок мыши.</summary>
    /// <param name="left">Первая кнопка мыши.</param>
    /// <param name="right">Вторая кнопка мыши.</param>
    /// <returns>True, если кнопки равны, иначе false.</returns>
    public static bool operator ==(MouseButton left, MouseButton right) => left.Code == right.Code;

    /// <summary>Проверяет неравенство двух кнопок мыши.</summary>
    /// <param name="left">Первая кнопка мыши.</param>
    /// <param name="right">Вторая кнопка мыши.</param>
    /// <returns>True, если кнопки не равны, иначе false.</returns>
    public static bool operator !=(MouseButton left, MouseButton right) => left.Code != right.Code;

    /// <summary>Проверяет равенство текущей кнопки мыши с другой кнопкой.</summary>
    /// <param name="other">Другая кнопка мыши.</param>
    /// <returns>True, если кнопки равны, иначе false.</returns>
    public bool Equals(MouseButton other) => Code == other.Code;

    /// <summary>Проверяет равенство текущей кнопки мыши с объектом.</summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>True, если объект является кнопкой мыши MouseButton и равен текущей кнопке, иначе false.</returns>
    public override bool Equals(object? obj) => obj is MouseButton button && Equals(button);

    /// <summary>Возвращает хэш-код текущей кнопки мыши.</summary>
    /// <returns>Хэш-код кнопки мыши.</returns>
    public override int GetHashCode() => Code;

    /// <summary>Возвращает строковое представление кнопки мыши.</summary>
    /// <returns>Строковое представление кнопки мыши.</returns>
    public override string ToString()
        => Code switch
        {
            0 => $"{nameof(MouseButton)}:{nameof(Left)}",
            1 => $"{nameof(MouseButton)}:{nameof(Right)}",
            2 => $"{nameof(MouseButton)}:{nameof(Middle)}",
            _ => $"{nameof(MouseButton)}:{Code}",
        };

    /// <summary>Явное преобразование кнопки мыши в её целочисленный код.</summary>
    /// <param name="button">Кнопка мыши.</param>
    public static explicit operator int(MouseButton button) => button.Code;

    /// <summary>Явное преобразование целочисленного кода в кнопку мыши.</summary>
    /// <param name="code">Целочисленный код кнопки.</param>
    public static explicit operator MouseButton(int code) => new(code);

    /// <summary>Преобразует кнопку мыши в её целочисленный код.</summary>
    /// <returns>Целочисленный код кнопки мыши.</returns>
    public int ToInt32() => Code;

    /// <summary>Создает кнопку мыши из её целочисленного кода.</summary>
    /// <param name="code">Целочисленный код кнопки мыши.</param>
    /// <returns>Экземпляр кнопки мыши.</returns>
    public static MouseButton FromInt32(int code) => new(code);
}