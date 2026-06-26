namespace Ignis.Platform.Graphics;

internal sealed unsafe class Allocation(
    MemoryBlock block,
    nint memory,
    ulong offset,
    ulong size,
    void* mappedData,
    object cookie)
{
    public MemoryBlock Block { get; } = block;
    public nint DeviceMemory { get; } = memory;
    public ulong Offset { get; } = offset;
    public ulong Size { get; } = size;
    public unsafe void* MappedData { get; } = mappedData;
    public object AllocatorCookie { get; } = cookie;
}
