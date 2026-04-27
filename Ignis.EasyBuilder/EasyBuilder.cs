using Ignis.Context.GLFW;
using Ignis.Core;
using Ignis.Platform.Graphics.Vulkan;
using Ignis.Platform.Input.GLFW;
using Ignis.Platform.Window.GLFW;

using Silk.NET.Core.Native;

namespace Ignis.EasyBuilder;

public static class EasyBuilder
{
    public static IgnisEngine Build()
    {
        var context = new GlfwContext();
        var window = new GlfwWindow(context);
        nint windowHandle = window.WindowHandle;

        return new IgnisEngine
        {
            Window = window,
            Renderer = new VulkanRenderer((instancePtr) =>
            {
                VkHandle vkInstance = new VkHandle(instancePtr);
                VkNonDispatchableHandle vkSurface;

                unsafe
                {
                    context.Api.CreateWindowSurface(
                        vkInstance,
                        (Silk.NET.GLFW.WindowHandle*)windowHandle,
                        null,
                        &vkSurface);
                }

                return (nint)vkSurface.Handle;
            }),
            Keyboard = new GlfwKeyboard(context, window),
            Mouse = new GlfwMouse(context, window),
        };
    }
}