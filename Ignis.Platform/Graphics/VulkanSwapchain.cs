using Ignis.Bindings.Vulkan;
using Ignis.Core.Numerics;

namespace Ignis.Platform.Graphics;

internal sealed unsafe class VulkanSwapchain : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly nint _surface;

    public nint Handle { get; private set; }
    public nint[] Images { get; private set; } = [];
    public nint[] ImageViews { get; private set; } = [];
    public VkExtent2D Extent { get; private set; }
    public int ImageFormat { get; private set; }

    public VulkanSwapchain(VulkanDevice device, nint surface, Vector2Int windowSize, VSyncMode vsync, bool inSizeMove)
    {
        _device = device;
        _surface = surface;
        Create(windowSize, vsync, inSizeMove, 0);
    }

    private int ChoosePresentMode(uint capsPresentModeCount, int* pPresentModes, VSyncMode vsync, bool inSizeMove)
    {
        int defaultMode = Vk.PresentModeFifoKhr;
        var modes = new List<int>();
        for (int i = 0; i < capsPresentModeCount; i++)
        {
            modes.Add(pPresentModes[i]);
        }

        if (inSizeMove)
        {
            if (modes.Contains(Vk.PresentModeImmediateKhr))
                return Vk.PresentModeImmediateKhr;
            if (modes.Contains(Vk.PresentModeMailboxKhr))
                return Vk.PresentModeMailboxKhr;
        }

        switch (vsync)
        {
            case VSyncMode.Off:
                if (modes.Contains(Vk.PresentModeImmediateKhr))
                    return Vk.PresentModeImmediateKhr;
                if (modes.Contains(Vk.PresentModeMailboxKhr))
                    return Vk.PresentModeMailboxKhr;
                break;

            case VSyncMode.On:
                return Vk.PresentModeFifoKhr;

            case VSyncMode.Relaxed:
                if (modes.Contains(Vk.PresentModeFifoRelaxedKhr))
                    return Vk.PresentModeFifoRelaxedKhr;
                break;

            case VSyncMode.Mailbox:
                if (modes.Contains(Vk.PresentModeMailboxKhr))
                    return Vk.PresentModeMailboxKhr;
                if (modes.Contains(Vk.PresentModeImmediateKhr))
                    return Vk.PresentModeImmediateKhr;
                break;
        }

        return defaultMode;
    }

    private void Create(Vector2Int windowSize, VSyncMode vsync, bool inSizeMove, nint oldSwapchain)
    {
        VkSurfaceCapabilitiesKHR caps;
        Vk.GetPhysicalDeviceSurfaceCapabilitiesKHR(_device.PhysicalDevice, _surface, &caps);

        ImageFormat = Vk.FormatB8G8R8A8Unorm;
        int colorSpace = Vk.ColorSpaceSrgbNonlinearKhr;

        var extent = caps.currentExtent;
        if (extent.width == uint.MaxValue)
        {
            extent.width = (uint)windowSize.X;
            extent.height = (uint)windowSize.Y;
        }
        Extent = extent;

        uint presentModeCount;
        Vk.GetPhysicalDeviceSurfacePresentModesKHR(_device.PhysicalDevice, _surface, &presentModeCount, null);
        int* presentModes = stackalloc int[(int)presentModeCount];
        Vk.GetPhysicalDeviceSurfacePresentModesKHR(_device.PhysicalDevice, _surface, &presentModeCount, presentModes);

        int presentMode = ChoosePresentMode(presentModeCount, presentModes, vsync, inSizeMove);

        uint imageCount = caps.minImageCount + 1;
        
        // Для Mailbox требуется минимум 3 буфера (тройная буферизация), чтобы не блокировать CPU
        if (presentMode == Vk.PresentModeMailboxKhr && imageCount < 3)
        {
            imageCount = 3;
        }
        
        if (caps.maxImageCount > 0 && imageCount > caps.maxImageCount)
        {
            imageCount = caps.maxImageCount;
        }

        VkSwapchainCreateInfoKHR createInfo = new()
        {
            sType = Vk.StructureTypeSwapchainCreateInfoKhr,
            surface = _surface,
            minImageCount = imageCount,
            imageFormat = ImageFormat,
            imageColorSpace = colorSpace,
            imageExtent = Extent,
            imageArrayLayers = 1,
            imageUsage = Vk.ImageUsageColorAttachmentBit,
            imageSharingMode = Vk.SharingModeExclusive,
            preTransform = caps.currentTransform,
            compositeAlpha = Vk.CompositeAlphaOpaqueBitKhr,
            presentMode = presentMode,
            clipped = 1,
            oldSwapchain = oldSwapchain
        };

        nint swapchain;
        if (Vk.CreateSwapchainKHR(_device.Device, &createInfo, null, &swapchain) != 0)
        {
            throw new InvalidOperationException("Ошибка Swapchain.");
        }

        Handle = swapchain;

        Vk.GetSwapchainImagesKHR(_device.Device, Handle, &imageCount, null);
        Images = new nint[imageCount];
        fixed (nint* pImages = Images)
        {
            Vk.GetSwapchainImagesKHR(_device.Device, Handle, &imageCount, pImages);
        }

        ImageViews = new nint[imageCount];
        for (int i = 0; i < imageCount; i++)
        {
            VkImageViewCreateInfo viewInfo = new()
            {
                sType = Vk.StructureTypeImageViewCreateInfo,
                image = Images[i],
                viewType = Vk.ImageViewType2D,
                format = ImageFormat
            };

            viewInfo.subresourceRange.aspectMask = Vk.ImageAspectColorBit;
            viewInfo.subresourceRange.baseMipLevel = 0;
            viewInfo.subresourceRange.levelCount = 1;
            viewInfo.subresourceRange.baseArrayLayer = 0;
            viewInfo.subresourceRange.layerCount = 1;

            nint imageView;
            if (Vk.CreateImageView(_device.Device, &viewInfo, null, &imageView) != 0)
            {
                throw new InvalidOperationException("Ошибка ImageView.");
            }

            ImageViews[i] = imageView;
        }
    }

    public void Recreate(Vector2Int windowSize, VSyncMode vsync, bool inSizeMove)
    {
        if (windowSize.X <= 0 || windowSize.Y <= 0) return;
        if (_device.Device == 0) return;

        Vk.DeviceWaitIdle(_device.Device);

        foreach (nint view in ImageViews)
        {
            if (view != 0)
                Vk.DestroyImageView(_device.Device, view, null);
        }
        ImageViews = [];

        nint oldSwapchain = Handle;

        Create(windowSize, vsync, inSizeMove, oldSwapchain);

        if (oldSwapchain != 0)
        {
            Vk.DestroySwapchainKHR(_device.Device, oldSwapchain, null);
        }
    }

    public void Dispose()
    {
        if (_device.Device != 0)
        {
            foreach (nint view in ImageViews)
            {
                if (view != 0)
                    Vk.DestroyImageView(_device.Device, view, null);
            }
            ImageViews = [];

            if (Handle != 0)
            {
                Vk.DestroySwapchainKHR(_device.Device, Handle, null);
                Handle = 0;
            }
        }
    }
}
