using System.Runtime.InteropServices;

namespace Ignis.Bindings.Vulkan;

internal static unsafe partial class Vk
{
    private static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, byte*, nint> _getInstanceProcAddr;

    internal static nint GetInstanceProcAddr(nint instance, string name)
    {
        nint pName = Marshal.StringToCoTaskMemUTF8(name);
        nint ptr = _getInstanceProcAddr(instance, (byte*)pName);
        Marshal.FreeCoTaskMem(pName);
        return ptr;
    }

    private static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, byte*, nint> _getDeviceProcAddr;

    internal static nint GetDeviceProcAddr(nint device, string name)
    {
        nint pName = Marshal.StringToCoTaskMemUTF8(name);
        nint ptr = _getDeviceProcAddr(device, (byte*)pName);
        Marshal.FreeCoTaskMem(pName);
        return ptr;
    }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<VkInstanceCreateInfo*, nint*, nint*, int> CreateInstance { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint*, void> DestroyInstance { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDebugUtilsMessengerCreateInfoEXT*, nint*, nint*, int> CreateDebugUtilsMessengerEXT { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroyDebugUtilsMessengerEXT { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint*, nint*, int> EnumeratePhysicalDevices { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint*, VkQueueFamilyProperties*, void> GetPhysicalDeviceQueueFamilyProperties { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkWin32SurfaceCreateInfoKHR*, nint*, nint*, int> CreateWin32SurfaceKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroySurfaceKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint, uint*, int> GetPhysicalDeviceSurfaceSupportKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkSurfaceCapabilitiesKHR*, int> GetPhysicalDeviceSurfaceCapabilitiesKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, VkSurfaceFormatKHR*, int> GetPhysicalDeviceSurfaceFormatsKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, int*, int> GetPhysicalDeviceSurfacePresentModesKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDeviceCreateInfo*, nint*, nint*, int> CreateDevice { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint*, void> DestroyDevice { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, nint*, void> GetDeviceQueue { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int> DeviceWaitIdle { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSwapchainCreateInfoKHR*, nint*, nint*, int> CreateSwapchainKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroySwapchainKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint*, nint*, int> GetSwapchainImagesKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkImageViewCreateInfo*, nint*, nint*, int> CreateImageView { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroyImageView { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandPoolCreateInfo*, nint*, nint*, int> CreateCommandPool { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroyCommandPool { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandBufferAllocateInfo*, nint*, int> AllocateCommandBuffers { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSemaphoreCreateInfo*, nint*, nint*, int> CreateSemaphore { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroySemaphore { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkFenceCreateInfo*, nint*, nint*, int> CreateFence { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint*, void> DestroyFence { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint*, uint, ulong, int> WaitForFences { get; set; }

    // --- HOT PATH: SuppressGCTransition applied ---
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, nint*, int> ResetFences { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, int> ResetCommandBuffer { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkCommandBufferBeginInfo*, int> BeginCommandBuffer { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int> EndCommandBuffer { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDependencyInfo*, void> CmdPipelineBarrier2 { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkRenderingInfo*, void> CmdBeginRendering { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, void> CmdEndRendering { get; set; }
    // --- END HOT PATH ---

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, VkSubmitInfo2*, nint, int> QueueSubmit2 { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, ulong, nint, nint, uint*, int> AcquireNextImageKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPresentInfoKHR*, int> QueuePresentKHR { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, void*, void> GetPhysicalDeviceProperties { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPhysicalDeviceMemoryProperties*, void> GetPhysicalDeviceMemoryProperties { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkShaderModuleCreateInfo*, void*, nint*, int> CreateShaderModule { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyShaderModule { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkPipelineLayoutCreateInfo*, void*, nint*, int> CreatePipelineLayout { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyPipelineLayout { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint, VkGraphicsPipelineCreateInfo*, void*, nint*, int> CreateGraphicsPipelines { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyPipeline { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkBufferCreateInfo*, void*, nint*, int> CreateBuffer { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyBuffer { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkMemoryRequirements*, void> GetBufferMemoryRequirements { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkMemoryAllocateInfo*, void*, nint*, int> AllocateMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> FreeMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, ulong, int> BindBufferMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, ulong, ulong, uint, void**, int> MapMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void> UnmapMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorSetLayoutCreateInfo*, void*, nint*, int> CreateDescriptorSetLayout { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyDescriptorSetLayout { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorPoolCreateInfo*, void*, nint*, int> CreateDescriptorPool { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyDescriptorPool { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkDescriptorSetAllocateInfo*, nint*, int> AllocateDescriptorSets { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, VkWriteDescriptorSet*, uint, void*, void> UpdateDescriptorSets { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int, nint, void> CmdBindPipeline { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, VkViewport*, void> CmdSetViewport { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, VkRect2D*, void> CmdSetScissor { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, uint, uint, uint, void*, void> CmdPushConstants { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, int, nint, uint, uint, nint*, uint, uint*, void> CmdBindDescriptorSets { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, uint, uint, uint, void> CmdDrawMeshTasksEXT { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, uint, VkBufferCopy*, void> CmdCopyBuffer { get; set; }

    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkImageCreateInfo*, void*, nint*, int> CreateImage { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroyImage { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, VkMemoryRequirements*, void> GetImageMemoryRequirements { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, ulong, int> BindImageMemory { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, VkSamplerCreateInfo*, void*, nint*, int> CreateSampler { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, void*, void> DestroySampler { get; set; }
    internal static delegate* unmanaged[Cdecl, SuppressGCTransition]<nint, nint, nint, uint, uint, VkBufferImageCopy*, void> CmdCopyBufferToImage { get; set; }
}
