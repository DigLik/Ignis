using System.Drawing;

using Ignis.Core.Window;

namespace Ignis.Platform.Windowing;

#pragma warning disable CA1724
public sealed partial class Window : IDisposable
#pragma warning restore CA1724
{
    internal event Action<nint>? RawInputReceived;

    public nint Handle { get; private set; }

    public string Title
    {
        get;
        set
        {
            field = value;
            SetTitlePlatform(value);
        }
    } = string.Empty;

    private partial void SetTitlePlatform(string title);

    public Size Size { get; set; }
    public Point Position { get; set; }
    public bool IsVisible { get; set; }
    public bool ShouldClose { get; internal set; }
    public bool CursorVisible { get; set; }
    public CursorMode CursorMode { get; set; }
    public bool IsFullscreen { get; set; }

    public event EventHandler? Closed;
    public event EventHandler<SizeChangedEventArgs>? SizeChanged;

    public Window() => InitPlatformWindow();
    private partial void InitPlatformWindow();

    public void Update() => UpdatePlatform();
    private partial void UpdatePlatform();

    public void Close()
    {
        ShouldClose = true;
        ClosePlatform();
    }
    private partial void ClosePlatform();

    public void Dispose()
    {
        DisposePlatform();
        GC.SuppressFinalize(this);
    }
    private partial void DisposePlatform();
}