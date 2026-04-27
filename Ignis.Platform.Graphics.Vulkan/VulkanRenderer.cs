using System.Numerics;

using Ignis.Core.Graphics;
using Ignis.Core.Platform.Graphics.Abstraction;

using Silk.NET.Vulkan;

namespace Ignis.Platform.Graphics.Vulkan;

public unsafe class VulkanRenderer : IRenderer
{
    private readonly SurfaceKHR _surface;

    private readonly nint _instancePtr = 0;

    public VulkanRenderer(Func<nint, nint> createSurfaceCallback)
    {
        nint surfaceHandle = createSurfaceCallback(_instancePtr);
        _surface = new SurfaceKHR((ulong)surfaceHandle);
    }

    public bool BeginFrame() => throw new NotImplementedException();
    public bool ClearBackground(Color clearColor) => throw new NotImplementedException();
    public void EndFrame() => throw new NotImplementedException();

    public void DrawCircle(Vector2 position, float radius, Color color) => throw new NotImplementedException();
    public void DrawEllipse(Vector2 position, Vector2 size, float angle, Color color) => throw new NotImplementedException();
    public void DrawRectangle(Vector2 position, Vector2 size, float angle, Color color) => throw new NotImplementedException();
    public void DrawLine(Vector2 firstPoint, Vector2 secondPoint, float thickness, Color color) => throw new NotImplementedException();
    public void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color) => throw new NotImplementedException();
    public void DrawPolygon(ReadOnlySpan<Vector2> points, Color color) => throw new NotImplementedException();
    public void DrawText(string text, Vector2 position, float size, Color color) => throw new NotImplementedException();

    public void UpdateCamera(Vector2 position, Vector2 boundingBox) => throw new NotImplementedException();
    public void Resize(System.Drawing.Size newSize) => throw new NotImplementedException();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}