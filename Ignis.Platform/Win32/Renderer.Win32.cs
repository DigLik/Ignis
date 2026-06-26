using Ignis.Bindings.Vulkan;
using Ignis.Bindings.Windows;

namespace Ignis.Platform.Graphics;

public sealed unsafe partial class Renderer
{
    private static partial nint GetVulkanLoader()
        => VulkanLoader.GetProcAddrPointer();

    private partial nint CreatePlatformSurface(nint instance)
    {
        nint hinstance = Kernel32.GetModuleHandleW(null);

        VkWin32SurfaceCreateInfoKHR createInfo = new()
        {
            sType =
                Vk.StructureTypeWin32SurfaceCreateInfoKhr,
            hinstance = hinstance,
            hwnd = _window.Handle
        };

        nint surface;
        if (Vk.CreateWin32SurfaceKHR(
            instance, &createInfo, null, &surface) != 0)
        {
            throw new InvalidOperationException(
                "Ошибка VkSurfaceKHR (Win32).");
        }

        return surface;
    }

#pragma warning disable CA1822
    private partial void DisposePlatformSpecific()
    {
    }
#pragma warning restore CA1822
}