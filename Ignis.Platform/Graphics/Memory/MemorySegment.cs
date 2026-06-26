namespace Ignis.Platform.Graphics;

internal sealed class MemorySegment
{
    public ulong Offset { get; set; }
    public ulong Size { get; set; }
    public bool IsFree { get; set; }
}
