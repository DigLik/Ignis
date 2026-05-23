using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ignis.Core.CoreTypes.Collections;

public sealed unsafe class NativeList<T>(
    int initialCapacity = 8) : IDisposable where T : unmanaged
{
    private T* _data = (T*)NativeMemory.Alloc(
        (nuint)(initialCapacity * sizeof(T)));

    private bool _isDisposed;

    public int Count { get; private set; }
    public int Capacity { get; private set; } = initialCapacity;

    ~NativeList() => Dispose(false);

    public Span<T> Data
    {
        get
        {
            ThrowIfDisposed();
            return new(_data, Count);
        }
    }

    public ref T this[int index]
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
        Capacity = Capacity == 0 ? 4 : Capacity * 2;
        _data = (T*)NativeMemory.Realloc(
            _data, (nuint)(Capacity * sizeof(T)));
    }

    public void Resize(int newSize, T defaultValue = default)
    {
        ThrowIfDisposed();
        if (newSize > Capacity)
        {
            Capacity = newSize;
            _data = (T*)NativeMemory.Realloc(
                _data, (nuint)(Capacity * sizeof(T)));
        }

        if (newSize > Count)
        {
            for (int i = Count; i < newSize; i++)
            {
                _data[i] = defaultValue;
            }
        }

        Count = newSize;
    }

    public void Clear()
    {
        ThrowIfDisposed();
        Count = 0;
    }

    private void Dispose(bool _)
    {
        if (_isDisposed) return;

        if (_data != null)
        {
            NativeMemory.Free(_data);
            _data = null;
        }

        Capacity = 0;
        Count = 0;
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _isDisposed, typeof(NativeList<T>));
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfOutOfBounds(int index)
    {
        if ((uint)index >= (uint)Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index), $"Индекс {index} вне границ.");
        }
    }
}