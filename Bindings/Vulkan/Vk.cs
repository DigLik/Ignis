using System.Runtime.InteropServices;

namespace Ignis.Bindings.Vulkan;

internal static unsafe class Vk
{
    #region Constants

    internal const int Success = 0;
    internal const int False = 0;
    internal const int True = 1;

    internal const uint ApiVersion13 = 4206592;

    internal const uint QueueGraphicsBit = 0x00000001;

    internal const int StructureTypeApplicationInfo = 0;
    internal const int
        StructureTypeInstanceCreateInfo = 1;
    internal const int
        StructureTypeDeviceQueueCreateInfo = 2;
    internal const int
        StructureTypeDeviceCreateInfo = 3;
    internal const int
        StructureTypeCommandPoolCreateInfo = 39;
    internal const int
        StructureTypeCommandBufferAllocateInfo = 40;
    internal const int
        StructureTypeCommandBufferBeginInfo = 42;
    internal const int
        StructureTypeImageViewCreateInfo = 15;
    internal const int
        StructureTypeSemaphoreCreateInfo = 9;
    internal const int
        StructureTypeFenceCreateInfo = 8;
    internal const int
        StructureTypeSwapchainCreateInfoKhr = 1000001000;
    internal const int
        StructureTypePresentInfoKhr = 1000001001;
    internal const int
        StructureTypeWin32SurfaceCreateInfoKhr = 1000009000;
    internal const int
        StructureTypeDebugUtilsMessengerCreateInfoExt =
        1000128004;

    internal const int
        StructureTypeRenderingInfo = 1000044000;
    internal const int
        StructureTypeRenderingAttachmentInfo = 1000044001;
    internal const int
        StructureTypePhysicalDeviceDynamicRenderingFeatures =
        1000044003;

    internal const int
        StructureTypeImageMemoryBarrier2 = 1000314002;
    internal const int
        StructureTypeDependencyInfo = 1000314003;
    internal const int
        StructureTypeSubmitInfo2 = 1000314004;
    internal const int
        StructureTypeSemaphoreSubmitInfo = 1000314005;
    internal const int
        StructureTypeCommandBufferSubmitInfo = 1000314006;
    internal const int
        StructureTypePhysicalDeviceSynchronization2Features =
        1000314007;

    internal const int ImageLayoutUndefined = 0;
    internal const int
        ImageLayoutColorAttachmentOptimal = 2;
    internal const int
        ImageLayoutPresentSrcKhr = 1000001002;

    internal const int AttachmentLoadOpClear = 1;
    internal const int AttachmentStoreOpStore = 0;

    internal const int SharingModeExclusive = 0;

    internal const ulong
        PipelineStage2TopOfPipeBit = 0x00000001ul;
    internal const ulong
        PipelineStage2ColorAttachmentOutputBit =
        0x00000400ul;
    internal const ulong
        PipelineStage2BottomOfPipeBit = 0x00002000ul;
    internal const ulong
        Access2ColorAttachmentWriteBit = 0x00000100ul;

    internal const int FormatB8G8R8A8Unorm = 44;
    internal const int ColorSpaceSrgbNonlinearKhr = 0;
    internal const int PresentModeFifoKhr = 2;
    internal const int CompositeAlphaOpaqueBitKhr = 1;
    internal const uint ImageUsageColorAttachmentBit = 16;
    internal const uint ImageAspectColorBit = 1;
    internal const int ImageViewType2D = 1;

    internal const int FenceCreateSignaledBit = 0x00000001;
    internal const int CommandBufferLevelPrimary = 0;

    internal const int
        DebugUtilsMessageSeverityVerboseBitExt = 0x00000001;
    internal const int
        DebugUtilsMessageSeverityInfoBitExt = 0x00000010;
    internal const int
        DebugUtilsMessageSeverityWarningBitExt = 0x00000100;
    internal const int
        DebugUtilsMessageSeverityErrorBitExt = 0x00001000;

    internal const int
        DebugUtilsMessageTypeGeneralBitExt = 0x00000001;
    internal const int
        DebugUtilsMessageTypeValidationBitExt = 0x00000002;
    internal const int
        DebugUtilsMessageTypePerformanceBitExt = 0x00000004;

    #endregion

    #region Function Pointers

    internal static Func<nint, string, nint>
        GetInstanceProcAddr
    { get; set; } = null!;

    private static delegate* unmanaged[Cdecl]<
        nint, byte*, nint> _getDeviceProcAddr;

    internal static nint GetDeviceProcAddr(
        nint device, string name)
    {
        nint pName = Marshal.StringToCoTaskMemUTF8(name);
        nint ptr = _getDeviceProcAddr(device, (byte*)pName);
        Marshal.FreeCoTaskMem(pName);
        return ptr;
    }

    internal static delegate* unmanaged[Cdecl]<
        VkInstanceCreateInfo*, nint*, nint*, int>
        CreateInstance
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint*, void>
        DestroyInstance
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkDebugUtilsMessengerCreateInfoEXT*, nint*,
        nint*, int> CreateDebugUtilsMessengerEXT
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroyDebugUtilsMessengerEXT
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, uint*, nint*, int>
        EnumeratePhysicalDevices
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, uint*, VkQueueFamilyProperties*, void>
        GetPhysicalDeviceQueueFamilyProperties
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkWin32SurfaceCreateInfoKHR*, nint*, nint*, int>
        CreateWin32SurfaceKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroySurfaceKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, uint, nint, uint*, int>
        GetPhysicalDeviceSurfaceSupportKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, VkSurfaceCapabilitiesKHR*, int>
        GetPhysicalDeviceSurfaceCapabilitiesKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, uint*, VkSurfaceFormatKHR*, int>
        GetPhysicalDeviceSurfaceFormatsKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, uint*, int*, int>
        GetPhysicalDeviceSurfacePresentModesKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkDeviceCreateInfo*, nint*, nint*, int>
        CreateDevice
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint*, void> DestroyDevice
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, uint, uint, nint*, void>
        GetDeviceQueue
    { get; set; }

    // Blocking function - DO NOT suppress GC
    internal static delegate* unmanaged[Cdecl]<
        nint, int> DeviceWaitIdle
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkSwapchainCreateInfoKHR*, nint*, nint*, int>
        CreateSwapchainKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroySwapchainKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, uint*, nint*, int>
        GetSwapchainImagesKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkImageViewCreateInfo*, nint*, nint*, int>
        CreateImageView
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroyImageView
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkCommandPoolCreateInfo*, nint*, nint*, int>
        CreateCommandPool
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroyCommandPool
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkCommandBufferAllocateInfo*, nint*, int>
        AllocateCommandBuffers
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkSemaphoreCreateInfo*, nint*, nint*, int>
        CreateSemaphore
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void>
        DestroySemaphore
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkFenceCreateInfo*, nint*, nint*, int>
        CreateFence
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, nint*, void> DestroyFence
    { get; set; }

    // Blocking function - DO NOT suppress GC
    internal static delegate* unmanaged[Cdecl]<
        nint, uint, nint*, uint, ulong, int>
        WaitForFences
    { get; set; }

    // --- HOT PATH: SuppressGCTransition applied ---

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, uint, nint*, int> ResetFences
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, uint, int> ResetCommandBuffer
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, VkCommandBufferBeginInfo*, int>
        BeginCommandBuffer
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, int> EndCommandBuffer
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, VkDependencyInfo*, void>
        CmdPipelineBarrier2
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, VkRenderingInfo*, void>
        CmdBeginRendering
    { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<
        nint, void> CmdEndRendering
    { get; set; }

    // --- END HOT PATH ---

    // Blocking/Heavy functions - DO NOT suppress GC
    internal static delegate* unmanaged[Cdecl]<
        nint, uint, VkSubmitInfo2*, nint, int>
        QueueSubmit2
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, nint, ulong, nint, nint, uint*, int>
        AcquireNextImageKHR
    { get; set; }

    internal static delegate* unmanaged[Cdecl]<
        nint, VkPresentInfoKHR*, int>
        QueuePresentKHR
    { get; set; }

    #endregion

    #region Function Loaders

    internal static void LoadGlobalFunctions(
        Func<nint, string, nint> loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        GetInstanceProcAddr = loader;

        CreateInstance = (delegate* unmanaged[Cdecl]<
            VkInstanceCreateInfo*, nint*, nint*, int>)
            GetInstanceProcAddr(nint.Zero,
            "vkCreateInstance");
    }

    internal static void LoadInstanceFunctions(nint instance)
    {
        DestroyInstance = (delegate* unmanaged[Cdecl]<
            nint, nint*, void>)
            GetInstanceProcAddr(instance,
            "vkDestroyInstance");

        CreateDebugUtilsMessengerEXT = (delegate* unmanaged[Cdecl]<
            nint, VkDebugUtilsMessengerCreateInfoEXT*, nint*,
            nint*, int>)
            GetInstanceProcAddr(instance,
            "vkCreateDebugUtilsMessengerEXT");

        DestroyDebugUtilsMessengerEXT = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetInstanceProcAddr(instance,
            "vkDestroyDebugUtilsMessengerEXT");

        EnumeratePhysicalDevices = (delegate* unmanaged[Cdecl]<
            nint, uint*, nint*, int>)
            GetInstanceProcAddr(instance,
            "vkEnumeratePhysicalDevices");

        GetPhysicalDeviceQueueFamilyProperties = (delegate* unmanaged[Cdecl]<
            nint, uint*, VkQueueFamilyProperties*, void>)
            GetInstanceProcAddr(instance,
            "vkGetPhysicalDeviceQueueFamilyProperties");

        CreateWin32SurfaceKHR = (delegate* unmanaged[Cdecl]<
            nint, VkWin32SurfaceCreateInfoKHR*, nint*,
            nint*, int>)
            GetInstanceProcAddr(instance,
            "vkCreateWin32SurfaceKHR");

        DestroySurfaceKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetInstanceProcAddr(instance,
            "vkDestroySurfaceKHR");

        GetPhysicalDeviceSurfaceSupportKHR = (delegate* unmanaged[Cdecl]<
            nint, uint, nint, uint*, int>)
            GetInstanceProcAddr(instance,
            "vkGetPhysicalDeviceSurfaceSupportKHR");

        GetPhysicalDeviceSurfaceCapabilitiesKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, VkSurfaceCapabilitiesKHR*, int>)
            GetInstanceProcAddr(instance,
            "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");

        GetPhysicalDeviceSurfaceFormatsKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, uint*, VkSurfaceFormatKHR*, int>)
            GetInstanceProcAddr(instance,
            "vkGetPhysicalDeviceSurfaceFormatsKHR");

        GetPhysicalDeviceSurfacePresentModesKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, uint*, int*, int>)
            GetInstanceProcAddr(instance,
            "vkGetPhysicalDeviceSurfacePresentModesKHR");

        CreateDevice = (delegate* unmanaged[Cdecl]<
            nint, VkDeviceCreateInfo*, nint*, nint*, int>)
            GetInstanceProcAddr(instance, "vkCreateDevice");

        _getDeviceProcAddr = (delegate* unmanaged[Cdecl]<
            nint, byte*, nint>)
            GetInstanceProcAddr(instance,
            "vkGetDeviceProcAddr");
    }

    internal static void LoadDeviceFunctions(nint device)
    {
        DestroyDevice = (delegate* unmanaged[Cdecl]<
            nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyDevice");

        GetDeviceQueue = (delegate* unmanaged[Cdecl]<
            nint, uint, uint, nint*, void>)
            GetDeviceProcAddr(device, "vkGetDeviceQueue");

        DeviceWaitIdle = (delegate* unmanaged[Cdecl]<
            nint, int>)
            GetDeviceProcAddr(device, "vkDeviceWaitIdle");

        CreateSwapchainKHR = (delegate* unmanaged[Cdecl]<
            nint, VkSwapchainCreateInfoKHR*, nint*,
            nint*, int>)
            GetDeviceProcAddr(device, "vkCreateSwapchainKHR");

        DestroySwapchainKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroySwapchainKHR");

        GetSwapchainImagesKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, uint*, nint*, int>)
            GetDeviceProcAddr(device,
            "vkGetSwapchainImagesKHR");

        CreateImageView = (delegate* unmanaged[Cdecl]<
            nint, VkImageViewCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateImageView");

        DestroyImageView = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyImageView");

        CreateCommandPool = (delegate* unmanaged[Cdecl]<
            nint, VkCommandPoolCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateCommandPool");

        DestroyCommandPool = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyCommandPool");

        AllocateCommandBuffers = (delegate* unmanaged[Cdecl]<
            nint, VkCommandBufferAllocateInfo*, nint*, int>)
            GetDeviceProcAddr(device,
            "vkAllocateCommandBuffers");

        CreateSemaphore = (delegate* unmanaged[Cdecl]<
            nint, VkSemaphoreCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateSemaphore");

        DestroySemaphore = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroySemaphore");

        CreateFence = (delegate* unmanaged[Cdecl]<
            nint, VkFenceCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateFence");

        DestroyFence = (delegate* unmanaged[Cdecl]<
            nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyFence");

        WaitForFences = (delegate* unmanaged[Cdecl]<
            nint, uint, nint*, uint, ulong, int>)
            GetDeviceProcAddr(device, "vkWaitForFences");

        ResetFences = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, uint, nint*, int>)
            GetDeviceProcAddr(device, "vkResetFences");

        ResetCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, uint, int>)
            GetDeviceProcAddr(device, "vkResetCommandBuffer");

        BeginCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, VkCommandBufferBeginInfo*, int>)
            GetDeviceProcAddr(device, "vkBeginCommandBuffer");

        EndCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, int>)
            GetDeviceProcAddr(device, "vkEndCommandBuffer");

        CmdPipelineBarrier2 = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, VkDependencyInfo*, void>)
            GetDeviceProcAddr(device, "vkCmdPipelineBarrier2");

        CmdBeginRendering = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, VkRenderingInfo*, void>)
            GetDeviceProcAddr(device, "vkCmdBeginRendering");

        CmdEndRendering = (delegate* unmanaged[Cdecl, SuppressGCTransition]<
            nint, void>)
            GetDeviceProcAddr(device, "vkCmdEndRendering");

        QueueSubmit2 = (delegate* unmanaged[Cdecl]<
            nint, uint, VkSubmitInfo2*, nint, int>)
            GetDeviceProcAddr(device, "vkQueueSubmit2");

        AcquireNextImageKHR = (delegate* unmanaged[Cdecl]<
            nint, nint, ulong, nint, nint, uint*, int>)
            GetDeviceProcAddr(device, "vkAcquireNextImageKHR");

        QueuePresentKHR = (delegate* unmanaged[Cdecl]<
            nint, VkPresentInfoKHR*, int>)
            GetDeviceProcAddr(device, "vkQueuePresentKHR");
    }

    #endregion
}

#region Structs

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkApplicationInfo
{
    public int sType;
    public void* pNext;
    public byte* pApplicationName;
    public uint applicationVersion;
    public byte* pEngineName;
    public uint engineVersion;
    public uint apiVersion;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkInstanceCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public VkApplicationInfo* pApplicationInfo;
    public uint enabledLayerCount;
    public byte** ppEnabledLayerNames;
    public uint enabledExtensionCount;
    public byte** ppEnabledExtensionNames;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDebugUtilsMessengerCreateInfoEXT
{
    public int sType;
    public void* pNext;
    public uint flags;
    public int messageSeverity;
    public int messageType;
    public delegate* unmanaged[Cdecl]<
        int, int, void*, void*, uint> pfnUserCallback;
    public void* pUserData;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDebugUtilsMessengerCallbackDataEXT
{
    public int sType;
    public void* pNext;
    public uint flags;
    public byte* pMessageIdName;
    public int messageIdNumber;
    public byte* pMessage;
    public uint queueLabelCount;
    public void* pQueueLabels;
    public uint cmdBufLabelCount;
    public void* pCmdBufLabels;
    public uint objectCount;
    public void* pObjects;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkWin32SurfaceCreateInfoKHR
{
    public int sType;
    public void* pNext;
    public uint flags;
    public nint hinstance;
    public nint hwnd;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDeviceQueueCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint queueFamilyIndex;
    public uint queueCount;
    public float* pQueuePriorities;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct
    VkPhysicalDeviceSynchronization2Features
{
    public int sType;
    public void* pNext;
    public uint synchronization2;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct
    VkPhysicalDeviceDynamicRenderingFeatures
{
    public int sType;
    public void* pNext;
    public uint dynamicRendering;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDeviceCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint queueCreateInfoCount;
    public VkDeviceQueueCreateInfo* pQueueCreateInfos;
    public uint enabledLayerCount;
    public byte** ppEnabledLayerNames;
    public uint enabledExtensionCount;
    public byte** ppEnabledExtensionNames;
    public void* pEnabledFeatures;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent2D
{
    public uint width;
    public uint height;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkOffset2D
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkRect2D
{
    public VkOffset2D offset;
    public VkExtent2D extent;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSurfaceCapabilitiesKHR
{
    public uint minImageCount;
    public uint maxImageCount;
    public VkExtent2D currentExtent;
    public VkExtent2D minImageExtent;
    public VkExtent2D maxImageExtent;
    public uint maxImageArrayLayers;
    public uint supportedTransforms;
    public int currentTransform;
    public uint supportedCompositeAlpha;
    public uint supportedUsageFlags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSurfaceFormatKHR
{
    public int format;
    public int colorSpace;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkSwapchainCreateInfoKHR
{
    public int sType;
    public void* pNext;
    public uint flags;
    public nint surface;
    public uint minImageCount;
    public int imageFormat;
    public int imageColorSpace;
    public VkExtent2D imageExtent;
    public uint imageArrayLayers;
    public uint imageUsage;
    public int imageSharingMode;
    public uint queueFamilyIndexCount;
    public uint* pQueueFamilyIndices;
    public int preTransform;
    public int compositeAlpha;
    public int presentMode;
    public uint clipped;
    public nint oldSwapchain;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkComponentMapping
{
    public int r;
    public int g;
    public int b;
    public int a;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkImageSubresourceRange
{
    public uint aspectMask;
    public uint baseMipLevel;
    public uint levelCount;
    public uint baseArrayLayer;
    public uint layerCount;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkImageViewCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public nint image;
    public int viewType;
    public int format;
    public VkComponentMapping components;
    public VkImageSubresourceRange subresourceRange;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkCommandPoolCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint queueFamilyIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkCommandBufferAllocateInfo
{
    public int sType;
    public void* pNext;
    public nint commandPool;
    public int level;
    public uint commandBufferCount;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkCommandBufferBeginInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public void* pInheritanceInfo;
}

[StructLayout(LayoutKind.Explicit)]
internal struct VkClearColorValue
{
    [FieldOffset(0)] public float float32_0;
    [FieldOffset(4)] public float float32_1;
    [FieldOffset(8)] public float float32_2;
    [FieldOffset(12)] public float float32_3;
}

[StructLayout(LayoutKind.Explicit)]
internal struct VkClearValue
{
    [FieldOffset(0)] public VkClearColorValue color;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkRenderingAttachmentInfo
{
    public int sType;
    public void* pNext;
    public nint imageView;
    public int imageLayout;
    public int resolveMode;
    public nint resolveImageView;
    public int resolveImageLayout;
    public int loadOp;
    public int storeOp;
    public VkClearValue clearValue;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkRenderingInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public VkRect2D renderArea;
    public uint layerCount;
    public uint viewMask;
    public uint colorAttachmentCount;
    public VkRenderingAttachmentInfo* pColorAttachments;
    public VkRenderingAttachmentInfo* pDepthAttachment;
    public VkRenderingAttachmentInfo* pStencilAttachment;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkSemaphoreCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkFenceCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkImageMemoryBarrier2
{
    public int sType;
    public void* pNext;
    public ulong srcStageMask;
    public ulong srcAccessMask;
    public ulong dstStageMask;
    public ulong dstAccessMask;
    public int oldLayout;
    public int newLayout;
    public uint srcQueueFamilyIndex;
    public uint dstQueueFamilyIndex;
    public nint image;
    public VkImageSubresourceRange subresourceRange;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDependencyInfo
{
    public int sType;
    public void* pNext;
    public uint dependencyFlags;
    public uint memoryBarrierCount;
    public void* pMemoryBarriers;
    public uint bufferMemoryBarrierCount;
    public void* pBufferMemoryBarriers;
    public uint imageMemoryBarrierCount;
    public VkImageMemoryBarrier2* pImageMemoryBarriers;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkSemaphoreSubmitInfo
{
    public int sType;
    public void* pNext;
    public nint semaphore;
    public ulong value;
    public ulong stageMask;
    public uint deviceIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkCommandBufferSubmitInfo
{
    public int sType;
    public void* pNext;
    public nint commandBuffer;
    public uint deviceMask;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkSubmitInfo2
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint waitSemaphoreInfoCount;
    public VkSemaphoreSubmitInfo* pWaitSemaphoreInfos;
    public uint commandBufferInfoCount;
    public VkCommandBufferSubmitInfo* pCommandBufferInfos;
    public uint signalSemaphoreInfoCount;
    public VkSemaphoreSubmitInfo* pSignalSemaphoreInfos;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPresentInfoKHR
{
    public int sType;
    public void* pNext;
    public uint waitSemaphoreCount;
    public nint* pWaitSemaphores;
    public uint swapchainCount;
    public nint* pSwapchains;
    public uint* pImageIndices;
    public int* pResults;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkQueueFamilyProperties
{
    public uint queueFlags;
    public uint queueCount;
    public uint timestampValidBits;
    public VkExtent2D minImageTransferGranularity;
}

#endregion