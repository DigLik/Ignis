using System.Numerics;

namespace Ignis.Core.Graphics;

/// <summary>Цвет в формате RGBA с компонентами float [0..1].</summary>
/// <param name="r">Компонент красного цвета [0..1].</param>
/// <param name="g">Компонент зелёного цвета [0..1].</param>
/// <param name="b">Компонент синего цвета [0..1].</param>
/// <param name="a">Альфа-компонент (прозрачность) [0..1].</param>
public readonly struct Color(float r, float g, float b, float a = 1f) : IEquatable<Color>
{
    /// <summary>Красный канал [0..1].</summary>
    public float R { get; } = r;

    /// <summary>Зелёный канал [0..1].</summary>
    public float G { get; } = g;

    /// <summary>Синий канал [0..1].</summary>
    public float B { get; } = b;

    /// <summary>Альфа-канал (прозрачность) [0..1]. 1 — полностью непрозрачный.</summary>
    public float A { get; } = a;

    #region Предопределённые цвета

    /// <summary>Белый цвет.</summary>
    public static readonly Color White = new(1, 1, 1);
    /// <summary>Чёрный цвет.</summary>
    public static readonly Color Black = new(0, 0, 0);
    /// <summary>Красный цвет.</summary>
    public static readonly Color Red = new(1, 0, 0);
    /// <summary>Зелёный цвет.</summary>
    public static readonly Color Green = new(0, 1, 0);
    /// <summary>Синий цвет.</summary>
    public static readonly Color Blue = new(0, 0, 1);
    /// <summary>Жёлтый цвет.</summary>
    public static readonly Color Yellow = new(1, 1, 0);
    /// <summary>Бирюзовый (циан) цвет.</summary>
    public static readonly Color Cyan = new(0, 1, 1);
    /// <summary>Пурпурный (маджента) цвет.</summary>
    public static readonly Color Magenta = new(1, 0, 1);
    /// <summary>Полностью прозрачный цвет.</summary>
    public static readonly Color Transparent = new(0, 0, 0, 0);

    #endregion

    /// <summary>Создаёт цвет из байтовых компонент [0..255].</summary>
    public static Color FromBytes(byte r, byte g, byte b, byte a = 255)
        => new(r / 255f, g / 255f, b / 255f, a / 255f);

    /// <summary>Создаёт цвет из RGBA hex-значения (0xRRGGBBAA).</summary>
    public static Color FromHex(uint hex)
        => new(
            ((hex >> 24) & 0xFF) / 255f,
            ((hex >> 16) & 0xFF) / 255f,
            ((hex >> 8) & 0xFF) / 255f,
            (hex & 0xFF) / 255f);

    /// <summary>Преобразует в <see cref="Vector4"/> (R, G, B, A).</summary>
    public Vector4 ToVector4() => new(R, G, B, A);

    /// <summary>Неявное преобразование в <see cref="Vector4"/>.</summary>
    public static implicit operator Vector4(Color c) => c.ToVector4();

    /// <summary>Линейная интерполяция между двумя цветами.</summary>
    public static Color Lerp(Color a, Color b, float t)
        => new(
            a.R + (b.R - a.R) * t,
            a.G + (b.G - a.G) * t,
            a.B + (b.B - a.B) * t,
            a.A + (b.A - a.A) * t);

    /// <summary>Возвращает копию цвета с изменённой прозрачностью.</summary>
    public Color WithAlpha(float alpha) => new(R, G, B, alpha);

    #region Equality

    /// <summary>Проверяет равенство двух цветов.</summary>
    /// <param name="left">Первый цвет.</param>
    /// <param name="right">Второй цвет.</param>
    /// <returns>True, если цвета равны, иначе false.</returns>
    public static bool operator ==(Color left, Color right) => left.Equals(right);

    /// <summary>Проверяет неравенство двух цветов.</summary>
    /// <param name="left">Первый цвет.</param>
    /// <param name="right">Второй цвет.</param>
    /// <returns>True, если цвета не равны, иначе false.</returns>
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    /// <summary>Проверяет равенство текущего цвета с другим.</summary>
    /// <param name="other">Другой цвет.</param>
    /// <returns>True, если цвета равны, иначе false.</returns>
    public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;

    /// <summary>Проверяет равенство текущего цвета с объектом.</summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>True, если объект является структурой Color и равен текущему цвету, иначе false.</returns>
    public override bool Equals(object? obj) => obj is Color c && Equals(c);

    /// <summary>Возвращает хэш-код текущего цвета.</summary>
    /// <returns>Хэш-код цвета.</returns>
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);

    #endregion

    /// <summary>Возвращает строковое представление цвета.</summary>
    /// <returns>Строковое представление цвета.</returns>
    public override string ToString() => $"Color({R:F2}, {G:F2}, {B:F2}, {A:F2})";
}