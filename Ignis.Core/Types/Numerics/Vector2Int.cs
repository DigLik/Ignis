namespace Ignis.Core.CoreTypes.Numerics;

public readonly struct Vector2Int(int x, int y) : IEquatable<Vector2Int>
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public static Vector2Int Zero => new(0, 0);
    public static Vector2Int One => new(1, 1);

    public bool Equals(Vector2Int other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Vector2Int other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);
    public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);

    public override string ToString() => $"<{X}, {Y}>";
}