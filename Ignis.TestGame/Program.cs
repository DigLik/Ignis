using System.Numerics;

using Ignis.Core.Graphics;
using Ignis.Core.Input;
using Ignis.Platform.Graphics;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;

internal sealed class Program
{
    private static void Main()
    {
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
            Vector2 windowCenter = new(windowSize.X / 2f, windowSize.Y / 2f);
            var minSize = MathF.Min(windowSize.X, windowSize.Y);

            var halfMinSize = minSize / 2f;

            if (!renderer.BeginFrame())
                continue;

            renderer.UpdateCamera(Vector2.Zero, windowCenter * 2);

            renderer.ClearBackground(Color.Black);
            renderer.DrawRectangle(windowCenter, windowCenter, 0, Color.White);
            renderer.DrawCircle(windowCenter, halfMinSize, Color.Black);

            renderer.EndFrame();
        }
    }
}