namespace Ignis.Core.Numerics;

/// <summary>Двумерный вектор с целочисленными компонентами.</summary>
/// <param name="x">Компонент X вектора.</param>
/// <param name="y">Компонент Y вектора.</param>
public readonly struct Vector2Int(int x, int y) : IEquatable<Vector2Int>
{
    /// <summary>Компонент X вектора.</summary>
    public int X { get; } = x;

    /// <summary>Компонент Y вектора.</summary>
    public int Y { get; } = y;

    /// <summary>Вектор со значениями (0, 0).</summary>
    public static Vector2Int Zero => new(0, 0);

    /// <summary>Вектор со значениями (1, 1).</summary>
    public static Vector2Int One => new(1, 1);

    /// <summary>Проверяет равенство текущего вектора с другим.</summary>
    /// <param name="other">Другой вектор.</param>
    /// <returns>True, если компоненты векторов равны, иначе false.</returns>
    public bool Equals(Vector2Int other) => X == other.X && Y == other.Y;

    /// <summary>Проверяет равенство текущего вектора с объектом.</summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>True, если объект является Vector2Int и равен текущему вектору, иначе false.</returns>
    public override bool Equals(object? obj) => obj is Vector2Int other && Equals(other);

    /// <summary>Возвращает хэш-код текущего вектора.</summary>
    /// <returns>Хэш-код вектора.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>Проверяет равенство двух векторов.</summary>
    /// <param name="left">Первый вектор.</param>
    /// <param name="right">Второй вектор.</param>
    /// <returns>True, если векторы равны, иначе false.</returns>
    public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);

    /// <summary>Проверяет неравенство двух векторов.</summary>
    /// <param name="left">Первый вектор.</param>
    /// <param name="right">Второй вектор.</param>
    /// <returns>True, если векторы не равны, иначе false.</returns>
    public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);

    /// <summary>Возвращает строковое представление вектора.</summary>
    /// <returns>Строковое представление вектора.</returns>
    public override string ToString() => $"<{X}, {Y}>";
}