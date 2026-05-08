using System.Numerics;

using Ignis.Core.Graphics;
using Ignis.Core.Input;
using Ignis.Platform.Graphics;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

using var window = new Window { Title = "Ignis" };
using var keyboard = new Keyboard(window);
using var mouse = new Mouse(window);
using var renderer = new Renderer(window);

while (!window.ShouldClose)
{
    keyboard.Update();
    mouse.Update();
    window.Update();

    if (keyboard.IsKeyPressed(Key.Escape))
        window.Close();

    var windowSize = window.Size;
    Vector2 windowCenter = new(windowSize.Width / 2f, windowSize.Height / 2f);
    float minSize = MathF.Min(windowSize.Width, windowSize.Height);

    float halfMinSize = minSize / 2f;

    if (!renderer.BeginFrame())
        continue;

    renderer.UpdateCamera(Vector2.Zero, windowCenter * 2);

    renderer.ClearBackground(Color.Black);
    renderer.DrawRectangle(windowCenter, windowCenter, 0, Color.White);
    renderer.DrawCircle(windowCenter, halfMinSize, Color.Black);

    renderer.EndFrame();
}