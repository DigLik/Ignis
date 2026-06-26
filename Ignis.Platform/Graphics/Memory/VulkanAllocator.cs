namespace Ignis.Platform.Graphics;

internal sealed class VulkanAllocator(nint device, ulong blockSize = 16 * 1024 * 1024) : IDisposable
{
    private readonly nint _device = device;
    private readonly ulong _blockSize = blockSize;
    private readonly List<MemoryBlock> _blocks = [];
    private readonly Lock _lock = new();

    public Allocation Allocate(ulong size, ulong alignment, uint memoryTypeIndex, bool hostVisible)
    {
        lock (_lock)
        {
            // Пытаемся выделить память в существующих блоках
            foreach (var block in _blocks)
            {
                if (block.MemoryTypeIndex == memoryTypeIndex)
                {
                    var alloc = block.TryAllocate(size, alignment);
                    if (alloc != null) return alloc;
                }
            }

            // Создаем новый блок
            ulong sizeToAllocate = Math.Max(size, _blockSize);
            var newBlock = new MemoryBlock(_device, sizeToAllocate, memoryTypeIndex, hostVisible);
            _blocks.Add(newBlock);

            var allocation = newBlock.TryAllocate(size, alignment)
                ?? throw new InvalidOperationException("Не удалось выделить память в новом блоке.");
            return allocation;
        }
    }

    public void Free(Allocation allocation)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        lock (_lock)
        {
            allocation.Block.Free(allocation);
            if (allocation.Block.IsEmpty)
            {
                _blocks.Remove(allocation.Block);
                allocation.Block.Dispose();
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var block in _blocks)
            {
                block.Dispose();
            }
            _blocks.Clear();
        }
    }
}
