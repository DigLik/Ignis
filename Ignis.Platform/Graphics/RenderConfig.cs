namespace Ignis.Platform.Graphics;

internal static class RenderConfig
{
    public const int MaxFramesInFlight = 3;
    public const int MaxShapesPerFrame = 20_000;
    public const int GpuShapeSize = 48;
    public const int RingBufferFrameSize = MaxShapesPerFrame * GpuShapeSize; // 960_000
    public const int TotalRingBufferSize = MaxFramesInFlight * RingBufferFrameSize;
}
