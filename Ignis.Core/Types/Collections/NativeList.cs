using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ignis.Core.Collections;

/// <summary>Список элементов в неуправляемой памяти.</summary>
/// <typeparam name="T">Тип неуправляемого элемента.</typeparam>
/// <param name="initialCapacity">Начальная емкость списка.</param>
public sealed unsafe class NativeList<T>(
    int initialCapacity = 8) : IDisposable where T : unmanaged
{
    private T* _data = (T*)NativeMemory.Alloc(
        (nuint)(initialCapacity * sizeof(T)));

    private bool _isDisposed;

    /// <summary>Количество элементов в списке.</summary>
    public int Count { get; private set; }

    /// <summary>Текущая емкость (выделенная память в количестве элементов) списка.</summary>
    public int Capacity { get; private set; } = initialCapacity;

    /// <summary>Деструктор класса <see cref="NativeList{T}"/>.</summary>
    ~NativeList() => Dispose(false);

    /// <summary>Возвращает данные списка в виде <see cref="Span{T}"/>.</summary>
    public Span<T> Data
    {
        get
        {
            ThrowIfDisposed();
            return new(_data, Count);
        }
    }

    /// <summary>Возвращает ссылку на элемент по указанному индексу.</summary>
    /// <param name="index">Индекс элемента.</param>
    /// <returns>Ссылка на элемент.</returns>
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

    /// <summary>Добавляет элемент в список.</summary>
    /// <param name="item">Элемент для добавления.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        ThrowIfDisposed();
        if (Count == Capacity) Grow();
        _data[Count++] = item;
    }

    /// <summary>Удаляет элемент по индексу путем его замены последним элементом списка (быстрое удаление без сохранения порядка).</summary>
    /// <param name="index">Индекс удаляемого элемента.</param>
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

    /// <summary>Изменяет размер списка. Если новый размер больше текущего, список заполняется значением по умолчанию.</summary>
    /// <param name="newSize">Новый размер списка.</param>
    /// <param name="defaultValue">Значение по умолчанию для новых элементов.</param>
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

    /// <summary>Очищает список (сбрасывает количество элементов в 0, но сохраняет емкость).</summary>
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

    /// <summary>Освобождает все ресурсы, используемые списком.</summary>
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