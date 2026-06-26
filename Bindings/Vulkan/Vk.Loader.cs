namespace Ignis.Bindings.Vulkan;

internal static unsafe partial class Vk
{
    internal static void LoadGlobalFunctions(nint pfnGetInstanceProcAddr)
    {
        if (pfnGetInstanceProcAddr == 0)
            throw new ArgumentNullException(nameof(pfnGetInstanceProcAddr));

        _getInstanceProcAddr = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, byte*, nint>)pfnGetInstanceProcAddr;

        CreateInstance = (delegate* unmanaged[Cdecl, SuppressGCTransition]<VkInstanceCreateInfo*, nint*, nint*, int>)
            GetInstanceProcAddr(nint.Zero, "vkCreateInstance");
    }

    internal static void LoadInstanceFunctions(nint instance)
    {
        DestroyInstance = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint*, void>)
            GetInstanceProcAddr(instance, "vkDestroyInstance");

        CreateDebugUtilsMessengerEXT = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDebugUtilsMessengerCreateInfoEXT*, nint*, nint*, int>)
            GetInstanceProcAddr(instance, "vkCreateDebugUtilsMessengerEXT");

        DestroyDebugUtilsMessengerEXT = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetInstanceProcAddr(instance, "vkDestroyDebugUtilsMessengerEXT");

        EnumeratePhysicalDevices = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint*, nint*, int>)
            GetInstanceProcAddr(instance, "vkEnumeratePhysicalDevices");

        GetPhysicalDeviceQueueFamilyProperties = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint*, VkQueueFamilyProperties*, void>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceQueueFamilyProperties");

        CreateWin32SurfaceKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkWin32SurfaceCreateInfoKHR*, nint*, nint*, int>)
            GetInstanceProcAddr(instance, "vkCreateWin32SurfaceKHR");

        DestroySurfaceKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetInstanceProcAddr(instance, "vkDestroySurfaceKHR");

        GetPhysicalDeviceSurfaceSupportKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint, uint*, int>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceSurfaceSupportKHR");

        GetPhysicalDeviceSurfaceCapabilitiesKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkSurfaceCapabilitiesKHR*, int>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");

        GetPhysicalDeviceSurfaceFormatsKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, VkSurfaceFormatKHR*, int>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceSurfaceFormatsKHR");

        GetPhysicalDeviceSurfacePresentModesKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, int*, int>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceSurfacePresentModesKHR");

        CreateDevice = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDeviceCreateInfo*, nint*, nint*, int>)
            GetInstanceProcAddr(instance, "vkCreateDevice");

        _getDeviceProcAddr = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, byte*, nint>)
            GetInstanceProcAddr(instance, "vkGetDeviceProcAddr");

        GetPhysicalDeviceMemoryProperties = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPhysicalDeviceMemoryProperties*, void>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceMemoryProperties");

        GetPhysicalDeviceProperties = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, void*, void>)
            GetInstanceProcAddr(instance, "vkGetPhysicalDeviceProperties");
    }

    internal static void LoadDeviceFunctions(nint device)
    {
        DestroyDevice = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyDevice");

        GetDeviceQueue = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, nint*, void>)
            GetDeviceProcAddr(device, "vkGetDeviceQueue");

        DeviceWaitIdle = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int>)
            GetDeviceProcAddr(device, "vkDeviceWaitIdle");

        CreateSwapchainKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSwapchainCreateInfoKHR*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateSwapchainKHR");

        DestroySwapchainKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroySwapchainKHR");

        GetSwapchainImagesKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, nint*, int>)
            GetDeviceProcAddr(device, "vkGetSwapchainImagesKHR");

        CreateImageView = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkImageViewCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateImageView");

        DestroyImageView = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyImageView");

        CreateCommandPool = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandPoolCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateCommandPool");

        DestroyCommandPool = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyCommandPool");

        AllocateCommandBuffers = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandBufferAllocateInfo*, nint*, int>)
            GetDeviceProcAddr(device, "vkAllocateCommandBuffers");

        CreateSemaphore = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSemaphoreCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateSemaphore");

        DestroySemaphore = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroySemaphore");

        CreateFence = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkFenceCreateInfo*, nint*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateFence");

        DestroyFence = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void>)
            GetDeviceProcAddr(device, "vkDestroyFence");

        WaitForFences = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint*, uint, ulong, int>)
            GetDeviceProcAddr(device, "vkWaitForFences");

        ResetFences = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint*, int>)
            GetDeviceProcAddr(device, "vkResetFences");

        ResetCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, int>)
            GetDeviceProcAddr(device, "vkResetCommandBuffer");

        BeginCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandBufferBeginInfo*, int>)
            GetDeviceProcAddr(device, "vkBeginCommandBuffer");

        EndCommandBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int>)
            GetDeviceProcAddr(device, "vkEndCommandBuffer");

        CmdPipelineBarrier2 = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDependencyInfo*, void>)
            GetDeviceProcAddr(device, "vkCmdPipelineBarrier2");

        CmdBeginRendering = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkRenderingInfo*, void>)
            GetDeviceProcAddr(device, "vkCmdBeginRendering");

        CmdEndRendering = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, void>)
            GetDeviceProcAddr(device, "vkCmdEndRendering");

        QueueSubmit2 = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, VkSubmitInfo2*, nint, int>)
            GetDeviceProcAddr(device, "vkQueueSubmit2");

        AcquireNextImageKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, ulong, nint, nint, uint*, int>)
            GetDeviceProcAddr(device, "vkAcquireNextImageKHR");

        QueuePresentKHR = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPresentInfoKHR*, int>)
            GetDeviceProcAddr(device, "vkQueuePresentKHR");

        CreateShaderModule = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkShaderModuleCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateShaderModule");

        DestroyShaderModule = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyShaderModule");

        CreatePipelineLayout = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPipelineLayoutCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreatePipelineLayout");

        DestroyPipelineLayout = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyPipelineLayout");

        CreateGraphicsPipelines = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint, VkGraphicsPipelineCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateGraphicsPipelines");

        DestroyPipeline = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyPipeline");

        CreateBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkBufferCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateBuffer");

        DestroyBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyBuffer");

        GetBufferMemoryRequirements = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkMemoryRequirements*, void>)
            GetDeviceProcAddr(device, "vkGetBufferMemoryRequirements");

        AllocateMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkMemoryAllocateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkAllocateMemory");

        FreeMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkFreeMemory");

        BindBufferMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, ulong, int>)
            GetDeviceProcAddr(device, "vkBindBufferMemory");

        MapMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, ulong, ulong, uint, void**, int>)
            GetDeviceProcAddr(device, "vkMapMemory");

        UnmapMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void>)
            GetDeviceProcAddr(device, "vkUnmapMemory");

        CreateDescriptorSetLayout = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorSetLayoutCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateDescriptorSetLayout");

        DestroyDescriptorSetLayout = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyDescriptorSetLayout");

        CreateDescriptorPool = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorPoolCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateDescriptorPool");

        DestroyDescriptorPool = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyDescriptorPool");

        AllocateDescriptorSets = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorSetAllocateInfo*, nint*, int>)
            GetDeviceProcAddr(device, "vkAllocateDescriptorSets");

        UpdateDescriptorSets = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, VkWriteDescriptorSet*, uint, void*, void>)
            GetDeviceProcAddr(device, "vkUpdateDescriptorSets");

        CmdBindPipeline = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int, nint, void>)
            GetDeviceProcAddr(device, "vkCmdBindPipeline");

        CmdSetViewport = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, VkViewport*, void>)
            GetDeviceProcAddr(device, "vkCmdSetViewport");

        CmdSetScissor = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, VkRect2D*, void>)
            GetDeviceProcAddr(device, "vkCmdSetScissor");

        CmdPushConstants = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint, uint, uint, void*, void>)
            GetDeviceProcAddr(device, "vkCmdPushConstants");

        CmdBindDescriptorSets = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int, nint, uint, uint, nint*, uint, uint*, void>)
            GetDeviceProcAddr(device, "vkCmdBindDescriptorSets");

        CmdDrawMeshTasksEXT = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, uint, void>)
            GetDeviceProcAddr(device, "vkCmdDrawMeshTasksEXT");

        CmdCopyBuffer = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, uint, VkBufferCopy*, void>)
            GetDeviceProcAddr(device, "vkCmdCopyBuffer");

        CreateImage = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkImageCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateImage");

        DestroyImage = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroyImage");

        GetImageMemoryRequirements = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkMemoryRequirements*, void>)
            GetDeviceProcAddr(device, "vkGetImageMemoryRequirements");

        BindImageMemory = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, ulong, int>)
            GetDeviceProcAddr(device, "vkBindImageMemory");

        CreateSampler = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSamplerCreateInfo*, void*, nint*, int>)
            GetDeviceProcAddr(device, "vkCreateSampler");

        DestroySampler = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void>)
            GetDeviceProcAddr(device, "vkDestroySampler");

        CmdCopyBufferToImage = (delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, uint, uint, VkBufferImageCopy*, void>)
            GetDeviceProcAddr(device, "vkCmdCopyBufferToImage");
    }
}
