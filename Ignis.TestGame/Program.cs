using Ignis.Core.Numerics;
using Ignis.Platform.Graphics;
using Ignis.Platform.Input;
using Ignis.Platform.Windowing;
using Ignis.TestGame;

using var window = new Window
{
    Title = "Ignis",
    Size = new Vector2Int(1280, 720)
};
using var keyboard = new Keyboard(window);
using var mouse = new Mouse(window);
using var renderer = new Renderer(window)
{
    VSync = VSyncMode.Off
};

var game = new Game(window, keyboard, mouse, renderer);

renderer.RenderRequested += game.UpdateAndRender;

window.WaitClose();
