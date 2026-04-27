using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ignis.Core.Utils.Collections;

public unsafe struct NativeList<T>(int initialCapacity = 8) : IDisposable where T : unmanaged
{
    private T* _data = (T*)NativeMemory.Alloc((nuint)(initialCapacity * sizeof(T)));

    public int Count { get; private set; } = 0;
    public int Capacity { get; private set; } = initialCapacity;

    public readonly Span<T> Data
    {
        get
        {
            ThrowIfDisposed();
            return new(_data, Count);
        }
    }

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ThrowIfDisposed();
            ThrowIfOutOfBounds(index);
            return ref _data[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        ThrowIfDisposed();
        if (Count == Capacity) Grow();
        _data[Count++] = item;
    }

    public void RemoveAtSwapBack(int index)
    {
        ThrowIfDisposed();
        ThrowIfOutOfBounds(index);
        _data[index] = _data[Count - 1];
        Count--;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow()
    {
        Capacity *= 2;
        _data = (T*)NativeMemory.Realloc(_data, (nuint)(Capacity * sizeof(T)));
    }

    public void Resize(int newSize, T defaultValue = default)
    {
        ThrowIfDisposed();
        if (newSize > Capacity)
        {
            Capacity = newSize;
            _data = (T*)NativeMemory.Realloc(_data, (nuint)(Capacity * sizeof(T)));
        }

        if (newSize > Count)
            for (int i = Count; i < newSize; i++)
                _data[i] = defaultValue;

        Count = newSize;
    }

    public void Clear()
    {
        ThrowIfDisposed();
        Count = 0;
    }

    public void Dispose()
    {
        if (_data != null)
        {
            NativeMemory.Free(_data);
            _data = null;
        }
        Capacity = 0;
        Count = 0;
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void ThrowIfDisposed()
    {
        if (_data == null)
            throw new ObjectDisposedException(
                nameof(NativeList<>),
                "Коллекция не инициализирована или уже освобождена."
            );
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void ThrowIfOutOfBounds(int index)
    {
        if ((uint)index >= (uint)Count)
            throw new IndexOutOfRangeException($"Индекс {index} находится вне границ. Размер коллекции: {Count}.");
    }
}