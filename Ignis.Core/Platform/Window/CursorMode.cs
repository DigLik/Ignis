namespace Ignis.Core.Platform.Window;

/// <summary>Режим курсора мыши.</summary>
public enum CursorMode : byte
{
    /// <summary>Курсор свободно перемещается и виден.</summary>
    CursorFree,
    /// <summary>Курсор заблокирован и скрыт. Предоставляет неограниченное относительное перемещение.</summary>
    CursorLocked,
    /// <summary>Курсор скрыт, но ограничен областью окна.</summary>
    CursorGrabbed
}