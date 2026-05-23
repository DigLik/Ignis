using Ignis.Core.CoreTypes.Numerics;

namespace Ignis.Core.Window;

/// <summary>Аргументы события изменения размера окна.</summary>
public sealed class SizeChangedEventArgs(Vector2Int size) : EventArgs
{
    /// <summary>Новый размер окна.</summary>
    public Vector2Int Size { get; } = size;
}