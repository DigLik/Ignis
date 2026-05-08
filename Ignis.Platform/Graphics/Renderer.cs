using System.Numerics;
using System.Runtime.InteropServices;

#if DEBUG
using System.Diagnostics;
using System.Runtime.CompilerServices;
#endif

using Ignis.Bindings.Vulkan;
using Ignis.Core.Graphics;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Graphics;

public sealed unsafe partial class Renderer : IDisposable
{
    private const int MaxFramesInFlight = 3;

    private readonly Window _window;

    private int _currentFrame;

    private nint _instance;
    private nint _surface;
    private nint _physicalDevice;
    private nint _device;
    private nint _queue;

    private nint _swapchain;
    private VkExtent2D _swapchainExtent;
    private int _swapchainImageFormat;
    private nint[] _swapchainImages = [];
    private nint[] _swapchainImageViews = [];

    private nint _commandPool;

    private readonly nint[] _commandBuffers =
        new nint[MaxFramesInFlight];
    private readonly nint[] _imageAvailableSemaphores =
        new nint[MaxFramesInFlight];
    private readonly nint[] _inFlightFences =
        new nint[MaxFramesInFlight];

    private nint[] _renderFinishedSemaphores = [];
    private nint[] _imagesInFlight = [];

    private uint _imageIndex;

#if DEBUG
    private nint _debugMessenger;
#endif

    private static partial Func<nint, string, nint> GetVulkanLoader();
    private partial nint CreatePlatformSurface(nint instance);
    private partial void DisposePlatformSpecific();

    public Renderer(Window window)
    {
        _window = window;
        InitGraphics();
    }

    private void InitGraphics()
    {
        Vk.LoadGlobalFunctions(GetVulkanLoader());

        CreateInstance();
        Vk.LoadInstanceFunctions(_instance);

#if DEBUG
        SetupDebugMessenger();
#endif

        _surface = CreatePlatformSurface(_instance);

        PickPhysicalDevice();
        CreateLogicalDevice();

        CreateSwapchain();
        CreateCommandPool();
        CreateSyncObjects();
    }

#if DEBUG
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint DebugCallback(
        int messageSeverity, int messageType,
        void* pCallbackData, void* pUserData)
    {
        var data =
            (VkDebugUtilsMessengerCallbackDataEXT*)
            pCallbackData;

        string? message = Marshal.PtrToStringUTF8(
            (nint)data->pMessage);

        Debug.WriteLine($"[Vulkan] {message}");
        return Vk.False;
    }

    private void SetupDebugMessenger()
    {
        int msgType =
            Vk.DebugUtilsMessageTypeGeneralBitExt |
            Vk.DebugUtilsMessageTypeValidationBitExt |
            Vk.DebugUtilsMessageTypePerformanceBitExt;

        int msgSeverity =
            Vk.DebugUtilsMessageSeverityWarningBitExt |
            Vk.DebugUtilsMessageSeverityErrorBitExt;

        VkDebugUtilsMessengerCreateInfoEXT info = new()
        {
            sType =
                Vk.StructureTypeDebugUtilsMessengerCreateInfoExt,
            messageSeverity = msgSeverity,
            messageType = msgType,
            pfnUserCallback = &DebugCallback
        };

        nint messenger;
        if (Vk.CreateDebugUtilsMessengerEXT(
            _instance, &info, null, &messenger) != 0)
        {
            throw new InvalidOperationException("Ошибка DebugMessenger.");
        }
        _debugMessenger = messenger;
    }
#endif

    private void CreateInstance()
    {
        VkApplicationInfo appInfo = new()
        {
            sType = Vk.StructureTypeApplicationInfo,
            apiVersion = Vk.ApiVersion13
        };

        nint pExt1 = Marshal.StringToCoTaskMemUTF8(
            "VK_KHR_surface");

        nint pExt2 = Marshal.StringToCoTaskMemUTF8(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            "VK_KHR_win32_surface" : "VK_KHR_xcb_surface");

#if DEBUG
        nint pExt3 = Marshal.StringToCoTaskMemUTF8(
            "VK_EXT_debug_utils");

        nint pLayer = Marshal.StringToCoTaskMemUTF8(
            "VK_LAYER_KHRONOS_validation");

        byte** extensions = stackalloc byte*[3]
        {
            (byte*)pExt1, (byte*)pExt2, (byte*)pExt3
        };

        byte** layers = stackalloc byte*[1]
        {
            (byte*)pLayer
        };
#else
        byte** extensions = stackalloc byte*[2]
        {
            (byte*)pExt1, (byte*)pExt2
        };
#endif

        VkInstanceCreateInfo createInfo = new()
        {
            sType =
                Vk.StructureTypeInstanceCreateInfo,
            pApplicationInfo = &appInfo,

#if DEBUG
            enabledExtensionCount = 3,
            enabledLayerCount = 1,
            ppEnabledLayerNames = layers,
#else
            enabledExtensionCount = 2,
            enabledLayerCount = 0,
            ppEnabledLayerNames = null,
#endif

            ppEnabledExtensionNames = extensions
        };

        nint instance;
        int result = Vk.CreateInstance(
            &createInfo, null, &instance);

        Marshal.FreeCoTaskMem(pExt1);
        Marshal.FreeCoTaskMem(pExt2);

#if DEBUG
        Marshal.FreeCoTaskMem(pExt3);
        Marshal.FreeCoTaskMem(pLayer);
#endif

        if (result != Vk.Success)
            throw new InvalidOperationException("Ошибка VkInstance.");

        _instance = instance;
    }

    private void PickPhysicalDevice()
    {
        uint deviceCount;
        Vk.EnumeratePhysicalDevices(
            _instance, &deviceCount, null);

        // CA1508: Анализатор не видит модификации через unmanaged out-параметр
#pragma warning disable CA1508
        if (deviceCount == 0)
        {
            throw new InvalidOperationException("GPU не найден.");
        }
#pragma warning restore CA1508

        nint[] devices = new nint[deviceCount];
        fixed (nint* pDevices = devices)
        {
            Vk.EnumeratePhysicalDevices(
                _instance, &deviceCount, pDevices);
        }
        _physicalDevice = devices[0];
    }

    private void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        VkDeviceQueueCreateInfo queueInfo = new()
        {
            sType =
                Vk.StructureTypeDeviceQueueCreateInfo,
            queueFamilyIndex = 0,
            queueCount = 1,
            pQueuePriorities = &queuePriority
        };

        nint pSwapchainExt = Marshal.StringToCoTaskMemUTF8(
            "VK_KHR_swapchain");

        byte** deviceExtensions = stackalloc byte*[1]
        {
            (byte*)pSwapchainExt
        };

        VkPhysicalDeviceDynamicRenderingFeatures
            dynRendering = new()
            {
                sType =
                Vk.StructureTypePhysicalDeviceDynamicRenderingFeatures,
                dynamicRendering = 1
            };

        VkPhysicalDeviceSynchronization2Features
            sync2 = new()
            {
                sType =
                Vk.StructureTypePhysicalDeviceSynchronization2Features,
                synchronization2 = 1,
                pNext = &dynRendering
            };

        VkDeviceCreateInfo createInfo = new()
        {
            sType =
                Vk.StructureTypeDeviceCreateInfo,
            pNext = &sync2,
            queueCreateInfoCount = 1,
            pQueueCreateInfos = &queueInfo,
            enabledExtensionCount = 1,
            ppEnabledExtensionNames = deviceExtensions
        };

        nint device;
        int result = Vk.CreateDevice(
            _physicalDevice, &createInfo, null, &device);

        Marshal.FreeCoTaskMem(pSwapchainExt);

        if (result != Vk.Success)
            throw new InvalidOperationException("Ошибка VkDevice.");

        _device = device;
        Vk.LoadDeviceFunctions(_device);

        nint queue;
        Vk.GetDeviceQueue(_device, 0, 0, &queue);
        _queue = queue;
    }

    private void CreateSwapchain()
    {
        VkSurfaceCapabilitiesKHR caps;
        Vk.GetPhysicalDeviceSurfaceCapabilitiesKHR(
            _physicalDevice, _surface, &caps);

        _swapchainImageFormat = Vk.FormatB8G8R8A8Unorm;
        int colorSpace = Vk.ColorSpaceSrgbNonlinearKhr;

        _swapchainExtent = caps.currentExtent;
        if (_swapchainExtent.width == uint.MaxValue)
        {
            _swapchainExtent.width = (uint)_window.Size.Width;
            _swapchainExtent.height = (uint)_window.Size.Height;
        }

        uint imageCount = caps.minImageCount + 1;
        if (caps.maxImageCount > 0 &&
            imageCount > caps.maxImageCount)
        {
            imageCount = caps.maxImageCount;
        }

        VkSwapchainCreateInfoKHR createInfo = new()
        {
            sType =
                Vk.StructureTypeSwapchainCreateInfoKhr,
            surface = _surface,
            minImageCount = imageCount,
            imageFormat = _swapchainImageFormat,
            imageColorSpace = colorSpace,
            imageExtent = _swapchainExtent,
            imageArrayLayers = 1,
            imageUsage =
                Vk.ImageUsageColorAttachmentBit,
            imageSharingMode =
                Vk.SharingModeExclusive,
            preTransform = caps.currentTransform,
            compositeAlpha =
                Vk.CompositeAlphaOpaqueBitKhr,
            presentMode = Vk.PresentModeFifoKhr,
            clipped = 1
        };

        nint swapchain;
        if (Vk.CreateSwapchainKHR(
            _device, &createInfo, null, &swapchain) != 0)
        {
            throw new InvalidOperationException("Ошибка Swapchain.");
        }
        _swapchain = swapchain;

        Vk.GetSwapchainImagesKHR(
            _device, _swapchain, &imageCount, null);

        _swapchainImages = new nint[imageCount];
        fixed (nint* pImages = _swapchainImages)
        {
            Vk.GetSwapchainImagesKHR(
                _device, _swapchain, &imageCount, pImages);
        }

        _swapchainImageViews = new nint[imageCount];
        for (int i = 0; i < imageCount; i++)
        {
            VkImageViewCreateInfo viewInfo = new()
            {
                sType =
                    Vk.StructureTypeImageViewCreateInfo,
                image = _swapchainImages[i],
                viewType = Vk.ImageViewType2D,
                format = _swapchainImageFormat
            };

            viewInfo.subresourceRange.aspectMask =
                Vk.ImageAspectColorBit;
            viewInfo.subresourceRange.baseMipLevel = 0;
            viewInfo.subresourceRange.levelCount = 1;
            viewInfo.subresourceRange.baseArrayLayer = 0;
            viewInfo.subresourceRange.layerCount = 1;

            nint imageView;
            if (Vk.CreateImageView(
                _device, &viewInfo, null, &imageView) != 0)
            {
                throw new InvalidOperationException("Ошибка ImageView.");
            }
            _swapchainImageViews[i] = imageView;
        }
    }

    private void CreateCommandPool()
    {
        VkCommandPoolCreateInfo poolInfo = new()
        {
            sType =
                Vk.StructureTypeCommandPoolCreateInfo,
            flags = 0x00000002,
            queueFamilyIndex = 0
        };

        nint commandPool;
        if (Vk.CreateCommandPool(
            _device, &poolInfo, null, &commandPool) != 0)
        {
            throw new InvalidOperationException("Ошибка CommandPool.");
        }
        _commandPool = commandPool;

        VkCommandBufferAllocateInfo allocInfo = new()
        {
            sType =
                Vk.StructureTypeCommandBufferAllocateInfo,
            commandPool = _commandPool,
            level =
                Vk.CommandBufferLevelPrimary,
            commandBufferCount = MaxFramesInFlight
        };

        fixed (nint* pCmdBuffers = _commandBuffers)
        {
            if (Vk.AllocateCommandBuffers(
                _device, &allocInfo, pCmdBuffers) != 0)
            {
                throw new InvalidOperationException("Ошибка CmdBuffer.");
            }
        }
    }

    private void CreateSyncObjects()
    {
        VkSemaphoreCreateInfo semInfo = new()
        {
            sType =
                Vk.StructureTypeSemaphoreCreateInfo
        };

        VkFenceCreateInfo fenceInfo = new()
        {
            sType =
                Vk.StructureTypeFenceCreateInfo,
            flags =
                Vk.FenceCreateSignaledBit
        };

        for (int i = 0; i < MaxFramesInFlight; i++)
        {
            nint imgAvail, inFlight;

            if (Vk.CreateSemaphore(
                    _device, &semInfo, null, &imgAvail) != 0 ||
                Vk.CreateFence(
                    _device, &fenceInfo, null, &inFlight) != 0)
            {
                throw new InvalidOperationException("Ошибка CPU Sync.");
            }

            _imageAvailableSemaphores[i] = imgAvail;
            _inFlightFences[i] = inFlight;
        }

        int imgCount = _swapchainImages.Length;
        _renderFinishedSemaphores = new nint[imgCount];
        _imagesInFlight = new nint[imgCount];

        for (int i = 0; i < imgCount; i++)
        {
            nint renderFin;
            if (Vk.CreateSemaphore(
                _device, &semInfo, null, &renderFin) != 0)
            {
                throw new InvalidOperationException("Ошибка GPU Sync.");
            }

            _renderFinishedSemaphores[i] = renderFin;
            _imagesInFlight[i] = 0;
        }
    }

    public bool BeginFrame()
    {
        nint fence = _inFlightFences[_currentFrame];
        Vk.WaitForFences(
            _device, 1, &fence, 1, ulong.MaxValue);

        uint imageIndex;
        int result = Vk.AcquireNextImageKHR(
            _device, _swapchain, ulong.MaxValue,
            _imageAvailableSemaphores[_currentFrame],
            0, &imageIndex);

        if (result < 0) return false;
        _imageIndex = imageIndex;

        if (_imagesInFlight[_imageIndex] != 0)
        {
            nint imgFence = _imagesInFlight[_imageIndex];
            Vk.WaitForFences(
                _device, 1, &imgFence, 1, ulong.MaxValue);
        }

        _imagesInFlight[_imageIndex] = fence;

        Vk.ResetFences(_device, 1, &fence);

        nint cmdBuf = _commandBuffers[_currentFrame];
        Vk.ResetCommandBuffer(cmdBuf, 0);

        VkCommandBufferBeginInfo beginInfo = new()
        {
            sType =
                Vk.StructureTypeCommandBufferBeginInfo
        };

        Vk.BeginCommandBuffer(cmdBuf, &beginInfo);
        return true;
    }

    public bool ClearBackground(Color color)
    {
        nint cmdBuf = _commandBuffers[_currentFrame];

        VkImageMemoryBarrier2 barrier = new()
        {
            sType =
                Vk.StructureTypeImageMemoryBarrier2,
            srcStageMask =
                Vk.PipelineStage2TopOfPipeBit,
            srcAccessMask = 0,
            dstStageMask =
                Vk.PipelineStage2ColorAttachmentOutputBit,
            dstAccessMask =
                Vk.Access2ColorAttachmentWriteBit,
            oldLayout = Vk.ImageLayoutUndefined,
            newLayout =
                Vk.ImageLayoutColorAttachmentOptimal,
            image = _swapchainImages[_imageIndex]
        };
        barrier.subresourceRange.aspectMask =
            Vk.ImageAspectColorBit;
        barrier.subresourceRange.levelCount = 1;
        barrier.subresourceRange.layerCount = 1;

        VkDependencyInfo depInfo = new()
        {
            sType = Vk.StructureTypeDependencyInfo,
            imageMemoryBarrierCount = 1,
            pImageMemoryBarriers = &barrier
        };

        Vk.CmdPipelineBarrier2(cmdBuf, &depInfo);

        VkClearValue clearValue = new();
        clearValue.color.float32_0 = color.R;
        clearValue.color.float32_1 = color.G;
        clearValue.color.float32_2 = color.B;
        clearValue.color.float32_3 = color.A;

        VkRenderingAttachmentInfo colorAttachment = new()
        {
            sType =
                Vk.StructureTypeRenderingAttachmentInfo,
            imageView =
                _swapchainImageViews[_imageIndex],
            imageLayout =
                Vk.ImageLayoutColorAttachmentOptimal,
            loadOp =
                Vk.AttachmentLoadOpClear,
            storeOp =
                Vk.AttachmentStoreOpStore,
            clearValue = clearValue
        };

        VkRenderingInfo renderingInfo = new()
        {
            sType =
                Vk.StructureTypeRenderingInfo
        };
        renderingInfo.renderArea.extent = _swapchainExtent;
        renderingInfo.layerCount = 1;
        renderingInfo.colorAttachmentCount = 1;
        renderingInfo.pColorAttachments = &colorAttachment;

        Vk.CmdBeginRendering(cmdBuf, &renderingInfo);

        return true;
    }

    public void EndFrame()
    {
        nint cmdBuf = _commandBuffers[_currentFrame];

        Vk.CmdEndRendering(cmdBuf);

        VkImageMemoryBarrier2 barrier = new()
        {
            sType =
                Vk.StructureTypeImageMemoryBarrier2,
            srcStageMask =
                Vk.PipelineStage2ColorAttachmentOutputBit,
            srcAccessMask =
                Vk.Access2ColorAttachmentWriteBit,
            dstStageMask =
                Vk.PipelineStage2BottomOfPipeBit,
            dstAccessMask = 0,
            oldLayout =
                Vk.ImageLayoutColorAttachmentOptimal,
            newLayout =
                Vk.ImageLayoutPresentSrcKhr,
            image = _swapchainImages[_imageIndex]
        };
        barrier.subresourceRange.aspectMask =
            Vk.ImageAspectColorBit;
        barrier.subresourceRange.levelCount = 1;
        barrier.subresourceRange.layerCount = 1;

        VkDependencyInfo depInfo = new()
        {
            sType = Vk.StructureTypeDependencyInfo,
            imageMemoryBarrierCount = 1,
            pImageMemoryBarriers = &barrier
        };

        Vk.CmdPipelineBarrier2(cmdBuf, &depInfo);

        Vk.EndCommandBuffer(cmdBuf);

        nint waitSem =
            _imageAvailableSemaphores[_currentFrame];
        nint signalSem =
            _renderFinishedSemaphores[_imageIndex];

        VkCommandBufferSubmitInfo cmdInfo = new()
        {
            sType =
                Vk.StructureTypeCommandBufferSubmitInfo,
            commandBuffer = cmdBuf
        };

        VkSemaphoreSubmitInfo waitInfo = new()
        {
            sType =
                Vk.StructureTypeSemaphoreSubmitInfo,
            semaphore = waitSem,
            stageMask =
                Vk.PipelineStage2ColorAttachmentOutputBit
        };

        VkSemaphoreSubmitInfo signalInfo = new()
        {
            sType =
                Vk.StructureTypeSemaphoreSubmitInfo,
            semaphore = signalSem,
            stageMask =
                Vk.PipelineStage2ColorAttachmentOutputBit
        };

        VkSubmitInfo2 submitInfo = new()
        {
            sType =
                Vk.StructureTypeSubmitInfo2,
            waitSemaphoreInfoCount = 1,
            pWaitSemaphoreInfos = &waitInfo,
            commandBufferInfoCount = 1,
            pCommandBufferInfos = &cmdInfo,
            signalSemaphoreInfoCount = 1,
            pSignalSemaphoreInfos = &signalInfo
        };

        nint fence = _inFlightFences[_currentFrame];

        Vk.QueueSubmit2(_queue, 1, &submitInfo, fence);

        nint swapchain = _swapchain;
        uint imgIdx = _imageIndex;

        VkPresentInfoKHR presentInfo = new()
        {
            sType =
                Vk.StructureTypePresentInfoKhr,
            waitSemaphoreCount = 1,
            pWaitSemaphores = &signalSem,
            swapchainCount = 1,
            pSwapchains = &swapchain,
            pImageIndices = &imgIdx
        };

        Vk.QueuePresentKHR(_queue, &presentInfo);

        _currentFrame =
            (_currentFrame + 1) % MaxFramesInFlight;
    }

#pragma warning disable CA1822, IDE0060
    public void DrawCircle(Vector2 p, float r, Color c) { }
    public void DrawEllipse(Vector2 p, Vector2 s, float a, Color c) { }
    public void DrawRectangle(Vector2 p, Vector2 s, float a, Color c) { }
    public void DrawLine(Vector2 p1, Vector2 p2, float t, Color c) { }
    public void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color c) { }
    public void DrawPolygon(ReadOnlySpan<Vector2> p, Color c) { }
    public void DrawText(string t, Vector2 p, float s, Color c) { }
    public void UpdateCamera(Vector2 p, Vector2 b) { }
    public void Resize(System.Drawing.Size newSize) { }
#pragma warning restore CA1822, IDE0060

    public void Dispose()
    {
        if (_device != 0)
        {
            Vk.DeviceWaitIdle(_device);

            for (int i = 0; i < MaxFramesInFlight; i++)
            {
                if (_imageAvailableSemaphores[i] != 0)
                    Vk.DestroySemaphore(
                        _device,
                        _imageAvailableSemaphores[i], null);

                if (_inFlightFences[i] != 0)
                    Vk.DestroyFence(
                        _device, _inFlightFences[i], null);
            }

            foreach (nint sem in _renderFinishedSemaphores)
            {
                if (sem != 0)
                    Vk.DestroySemaphore(_device, sem, null);
            }

            Vk.DestroyCommandPool(_device, _commandPool, null);

            foreach (nint view in _swapchainImageViews)
            {
                if (view != 0)
                    Vk.DestroyImageView(_device, view, null);
            }

            if (_swapchain != 0)
                Vk.DestroySwapchainKHR(
                    _device, _swapchain, null);

            Vk.DestroyDevice(_device, null);
            _device = 0;
        }

#if DEBUG
        if (_debugMessenger != 0)
        {
            Vk.DestroyDebugUtilsMessengerEXT(
                _instance, _debugMessenger, null);
            _debugMessenger = 0;
        }
#endif

        if (_surface != 0)
        {
            Vk.DestroySurfaceKHR(_instance, _surface, null);
            _surface = 0;
        }

        DisposePlatformSpecific();

        if (_instance != 0)
        {
            Vk.DestroyInstance(_instance, null);
            _instance = 0;
        }

        GC.SuppressFinalize(this);
    }
}