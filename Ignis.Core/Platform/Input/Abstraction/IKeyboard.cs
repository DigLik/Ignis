namespace Ignis.Core.Platform.Input.Abstraction;

/// <summary>
/// Контракт клавиатуры. Предоставляет состояние клавиш с поддержкой переходов (Pressed/Released/Held).
/// </summary>
public interface IKeyboard : IDisposable
{
    /// <summary>Обновляет внутреннее состояние клавиатуры. Вызывается каждый кадр.</summary>
    void Update();

    /// <summary>Возвращает текущее состояние указанной клавиши.</summary>
    /// <param name="key">Клавиша для проверки.</param>
    InputState GetKeyState(Key key);
}