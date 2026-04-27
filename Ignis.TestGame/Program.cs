using System.Numerics;

using Ignis.Core.Graphics;
using Ignis.Core.Platform.Input;
using Ignis.EasyBuilder;

using var engine = EasyBuilder.Build();

var window = engine.Window;
var keyboard = engine.Keyboard;
var mouse = engine.Mouse;
var renderer = engine.Renderer;

while (!window.ShouldClose)
{
    window.Update();
    keyboard.Update();
    mouse.Update();

    if (keyboard.GetKeyState(Key.Escape) == InputState.Pressed)
        window.Close();

    var windowSize = window.ClientSize;
    Vector2 windowCenter = new(windowSize.Width / 2, windowSize.Height / 2);
    float minSize = MathF.Min(windowSize.Width, windowSize.Height);
    float halfMinSize = minSize / 2;

    if (!renderer.BeginFrame())
        continue;

    renderer.UpdateCamera(Vector2.Zero, windowCenter * 2);

    renderer.ClearBackground(Color.Black);

    renderer.DrawRectangle(windowCenter, windowCenter, 0, Color.White);
    renderer.DrawCircle(windowCenter, halfMinSize, Color.Black);

    renderer.EndFrame();
}