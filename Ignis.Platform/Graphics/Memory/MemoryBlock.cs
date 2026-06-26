using Ignis.Bindings.Vulkan;

namespace Ignis.Platform.Graphics;

internal sealed unsafe class MemoryBlock : IDisposable
{
    private readonly nint _device;

    public nint DeviceMemory { get; private set; }
    public ulong Size { get; }
    public uint MemoryTypeIndex { get; }
    public bool HostVisible { get; }
    public void* MappedData { get; private set; }

    private readonly List<MemorySegment> _segments = [];

    public bool IsEmpty => _segments.Count == 1 && _segments[0].IsFree;

    public MemoryBlock(nint device, ulong size, uint memoryTypeIndex, bool hostVisible)
    {
        _device = device;
        Size = size;
        MemoryTypeIndex = memoryTypeIndex;
        HostVisible = hostVisible;

        VkMemoryAllocateInfo allocInfo = new()
        {
            sType = Vk.StructureTypeMemoryAllocateInfo,
            allocationSize = size,
            memoryTypeIndex = memoryTypeIndex
        };

        nint memory;
        if (Vk.AllocateMemory(_device, &allocInfo, null, &memory) != 0)
        {
            throw new InvalidOperationException("Не удалось выделить системную память Vulkan.");
        }
        DeviceMemory = memory;

        if (hostVisible)
        {
            void* mapped;
            if (Vk.MapMemory(_device, DeviceMemory, 0, size, 0, &mapped) != 0)
            {
                throw new InvalidOperationException("Не удалось спроецировать память Vulkan.");
            }
            MappedData = mapped;
        }

        // Инициализируем весь блок как один свободный сегмент
        _segments.Add(new MemorySegment
        {
            Offset = 0,
            Size = size,
            IsFree = true
        });
    }

    public Allocation? TryAllocate(ulong size, ulong alignment)
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            if (!segment.IsFree) continue;

            ulong alignedOffset = (segment.Offset + alignment - 1) & ~(alignment - 1);
            ulong padding = alignedOffset - segment.Offset;

            if (segment.Size >= size + padding)
            {
                // Если есть паддинг в начале, отсекаем его как свободный сегмент
                if (padding > 0)
                {
                    var paddingSegment = new MemorySegment
                    {
                        Offset = segment.Offset,
                        Size = padding,
                        IsFree = true
                    };
                    _segments.Insert(i, paddingSegment);
                    segment.Offset = alignedOffset;
                    segment.Size -= padding;
                    i++; // Пропускаем паддинг
                }

                // Если в конце сегмента остается свободное место, отсекаем его
                ulong remaining = segment.Size - size;
                if (remaining > 0)
                {
                    var remainingSegment = new MemorySegment
                    {
                        Offset = segment.Offset + size,
                        Size = remaining,
                        IsFree = true
                    };
                    _segments.Insert(i + 1, remainingSegment);
                    segment.Size = size;
                }

                segment.IsFree = false;
                void* mappedPtr = HostVisible ? (byte*)MappedData + segment.Offset : null;

                return new Allocation(this, DeviceMemory, segment.Offset, segment.Size, mappedPtr, segment);
            }
        }

        return null;
    }

    public void Free(Allocation allocation)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        var segment = (MemorySegment)allocation.AllocatorCookie;
        segment.IsFree = true;

        int idx = _segments.IndexOf(segment);
        if (idx == -1) return;

        // Объединяем со следующим сегментом, если он свободен (coalescing)
        if (idx + 1 < _segments.Count && _segments[idx + 1].IsFree)
        {
            segment.Size += _segments[idx + 1].Size;
            _segments.RemoveAt(idx + 1);
        }

        // Объединяем с предыдущим сегментом, если он свободен
        if (idx - 1 >= 0 && _segments[idx - 1].IsFree)
        {
            _segments[idx - 1].Size += segment.Size;
            _segments.RemoveAt(idx);
        }
    }

    public void Dispose()
    {
        if (DeviceMemory != 0)
        {
            if (MappedData != null)
            {
                Vk.UnmapMemory(_device, DeviceMemory);
                MappedData = null;
            }
            Vk.FreeMemory(_device, DeviceMemory, null);
            DeviceMemory = 0;
        }
    }
}
