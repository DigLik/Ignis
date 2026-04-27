namespace Ignis.Core.Context;

/// <summary>
/// Контракт нативного контекста платформы.
/// Предоставляет доступ к нативному API (например, GLFW) и управляет его жизненным циклом.
/// </summary>
/// <typeparam name="T">Тип нативного API.</typeparam>
public interface INativeContext<T> : IDisposable
{
    /// <summary>Нативное API платформы.</summary>
    T Api { get; }
}