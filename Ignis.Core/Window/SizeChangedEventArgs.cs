using System.Drawing;

namespace Ignis.Core.Window;

/// <summary>Аргументы события изменения размера окна.</summary>
public sealed class SizeChangedEventArgs(Size size) : EventArgs
{
    /// <summary>Новый размер окна.</summary>
    public Size Size { get; } = size;
}