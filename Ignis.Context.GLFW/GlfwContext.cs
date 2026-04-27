using Ignis.Core.Context;

using Silk.NET.GLFW;

namespace Ignis.Context.GLFW;

public sealed class GlfwContext : INativeContext<Glfw>
{
    public Glfw Api { get; }

    private bool _isDisposed;

    public GlfwContext()
    {
        Api = Glfw.GetApi();

        if (!Api.Init())
            throw new Exception("Не удалось инициализировать GLFW.");
    }

    ~GlfwContext() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        Api.Terminate();

        if (disposing)
            Api.Dispose();

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}