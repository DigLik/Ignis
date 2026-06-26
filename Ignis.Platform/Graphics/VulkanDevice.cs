using System.Runtime.InteropServices;
using Ignis.Bindings.Vulkan;

namespace Ignis.Platform.Graphics;

internal sealed unsafe class VulkanDevice : IDisposable
{
    public nint Instance { get; private set; }
    public nint PhysicalDevice { get; private set; }
    public nint Device { get; private set; }
    public nint Queue { get; private set; }

    public VulkanDevice(nint vulkanLoader)
    {
        Vk.LoadGlobalFunctions(vulkanLoader);
        CreateInstance();
        Vk.LoadInstanceFunctions(Instance);
#if DEBUG
        SetupDebugMessenger();
#endif
        PickPhysicalDevice();
        CreateLogicalDevice();
    }

    private void CreateInstance()
    {
        VkApplicationInfo appInfo = new()
        {
            sType = Vk.StructureTypeApplicationInfo,
            apiVersion = Vk.ApiVersion13
        };

        ReadOnlySpan<byte> ext1 = "VK_KHR_surface\0"u8;
        ReadOnlySpan<byte> ext2 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "VK_KHR_win32_surface\0"u8
            : "VK_KHR_xcb_surface\0"u8;

#if DEBUG
        ReadOnlySpan<byte> ext3 = "VK_EXT_debug_utils\0"u8;
        ReadOnlySpan<byte> layer = "VK_LAYER_KHRONOS_validation\0"u8;
#endif

        nint instance;
        int result;

        fixed (byte* pExt1 = ext1)
        fixed (byte* pExt2 = ext2)
#if DEBUG
        fixed (byte* pExt3 = ext3)
        fixed (byte* pLayer = layer)
#endif
        {
#if DEBUG
            byte** extensions = stackalloc byte*[3] { pExt1, pExt2, pExt3 };
            byte** layers = stackalloc byte*[1] { pLayer };
#else
            byte** extensions = stackalloc byte*[2] { pExt1, pExt2 };
#endif

            VkInstanceCreateInfo createInfo = new()
            {
                sType = Vk.StructureTypeInstanceCreateInfo,
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

            result = Vk.CreateInstance(&createInfo, null, &instance);
        }

        if (result != Vk.Success)
            throw new InvalidOperationException("Ошибка VkInstance.");

        Instance = instance;
    }

    private void PickPhysicalDevice()
    {
        uint deviceCount;
        Vk.EnumeratePhysicalDevices(Instance, &deviceCount, null);

        if (deviceCount == 0)
        {
            throw new InvalidOperationException("GPU не найден.");
        }

        nint[] devices = new nint[deviceCount];
        fixed (nint* pDevices = devices)
        {
            Vk.EnumeratePhysicalDevices(Instance, &deviceCount, pDevices);
        }

        PhysicalDevice = devices[0];
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VkPhysicalDeviceMaintenance4Features
    {
        public int sType;
        public void* pNext;
        public int maintenance4;
    }

    private void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        VkDeviceQueueCreateInfo queueInfo = new()
        {
            sType = Vk.StructureTypeDeviceQueueCreateInfo,
            queueFamilyIndex = 0,
            queueCount = 1,
            pQueuePriorities = &queuePriority
        };

        ReadOnlySpan<byte> swapchainExt = "VK_KHR_swapchain\0"u8;
        ReadOnlySpan<byte> meshShaderExt = "VK_EXT_mesh_shader\0"u8;

        VkPhysicalDeviceDescriptorIndexingFeatures descriptorIndexingFeatures = new()
        {
            sType = 1000161001, // VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_FEATURES
            pNext = null,
            shaderSampledImageArrayNonUniformIndexing = 1,
            descriptorBindingSampledImageUpdateAfterBind = 1,
            descriptorBindingPartiallyBound = 1,
            runtimeDescriptorArray = 1
        };

        VkPhysicalDeviceMaintenance4Features maintenance4Features = new()
        {
            sType = 1000413000, // VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_FEATURES
            pNext = &descriptorIndexingFeatures,
            maintenance4 = 1
        };

        VkPhysicalDeviceDynamicRenderingFeatures dynRendering = new()
        {
            sType = Vk.StructureTypePhysicalDeviceDynamicRenderingFeatures,
            pNext = &maintenance4Features,
            dynamicRendering = 1
        };

        VkPhysicalDeviceMeshShaderFeaturesEXT meshFeatures = new()
        {
            sType = Vk.StructureTypePhysicalDeviceMeshShaderFeaturesExt,
            pNext = &dynRendering,
            taskShader = 1,
            meshShader = 1
        };

        VkPhysicalDeviceSynchronization2Features sync2 = new()
        {
            sType = Vk.StructureTypePhysicalDeviceSynchronization2Features,
            synchronization2 = 1,
            pNext = &meshFeatures
        };

        VkPhysicalDeviceFeatures2 features2 = new()
        {
            sType = Vk.StructureTypePhysicalDeviceFeatures2,
            pNext = &sync2
        };

        nint device;
        int result;

        fixed (byte* pSwapchainExt = swapchainExt)
        fixed (byte* pMeshShaderExt = meshShaderExt)
        {
            byte** deviceExtensions = stackalloc byte*[2]
            {
                pSwapchainExt,
                pMeshShaderExt
            };

            VkDeviceCreateInfo createInfo = new()
            {
                sType = Vk.StructureTypeDeviceCreateInfo,
                pNext = &features2,
                queueCreateInfoCount = 1,
                pQueueCreateInfos = &queueInfo,
                enabledExtensionCount = 2,
                ppEnabledExtensionNames = deviceExtensions
            };

            result = Vk.CreateDevice(PhysicalDevice, &createInfo, null, &device);
        }

        if (result != Vk.Success)
            throw new InvalidOperationException("Ошибка VkDevice.");

        Device = device;
        Vk.LoadDeviceFunctions(Device);

        nint queue;
        Vk.GetDeviceQueue(Device, 0, 0, &queue);
        Queue = queue;
    }

    public bool TryFindMemoryType(uint typeFilter, uint properties, out uint memoryTypeIndex)
    {
        VkPhysicalDeviceMemoryProperties memProperties;
        Vk.GetPhysicalDeviceMemoryProperties(PhysicalDevice, &memProperties);

        for (int i = 0; i < memProperties.memoryTypeCount; i++)
        {
            if ((typeFilter & (1 << i)) != 0 &&
                (memProperties.memoryTypes[i].propertyFlags & properties) == properties)
            {
                memoryTypeIndex = (uint)i;
                return true;
            }
        }

        memoryTypeIndex = 0;
        return false;
    }

    public uint FindMemoryType(uint typeFilter, uint properties)
    {
        if (TryFindMemoryType(typeFilter, properties, out uint memoryTypeIndex))
        {
            return memoryTypeIndex;
        }

        throw new InvalidOperationException("Не удалось найти подходящий тип памяти.");
    }

#if DEBUG
    private nint _debugMessenger;

    // Храним делегат статически чтобы GC не собрал его
    private delegate uint DebugCallbackDelegate(int messageSeverity, int messageType, void* pCallbackData, void* pUserData);
    private static readonly DebugCallbackDelegate s_debugCallback = DebugCallbackImpl;

    private static uint DebugCallbackImpl(int messageSeverity, int messageType, void* pCallbackData, void* pUserData)
    {
        VkDebugUtilsMessengerCallbackDataEXT* callbackData = (VkDebugUtilsMessengerCallbackDataEXT*)pCallbackData;
        string message = Marshal.PtrToStringAnsi((nint)callbackData->pMessage) ?? "no message";
        Console.Error.WriteLine($"[Vulkan Validation] {message}");
        return 0;
    }

    private void SetupDebugMessenger()
    {
        if (Vk.CreateDebugUtilsMessengerEXT == null) return;

        nint callbackPtr = Marshal.GetFunctionPointerForDelegate(s_debugCallback);

        VkDebugUtilsMessengerCreateInfoEXT createInfo = new()
        {
            sType = Vk.StructureTypeDebugUtilsMessengerCreateInfoExt,
            messageSeverity = 0x00000100 | 0x00001000, // WARNING | ERROR
            messageType = 0x00000001 | 0x00000002 | 0x00000004, // GENERAL | VALIDATION | PERFORMANCE
            pfnUserCallback = (delegate* unmanaged<int, int, void*, void*, uint>)callbackPtr
        };

        nint messenger;
        int result = Vk.CreateDebugUtilsMessengerEXT(Instance, &createInfo, null, &messenger);
        if (result == Vk.Success)
        {
            _debugMessenger = messenger;
        }
    }
#endif

    public void Dispose()
    {
        if (Device != 0)
        {
            Vk.DestroyDevice(Device, null);
            Device = 0;
        }
#if DEBUG
        if (_debugMessenger != 0)
        {
            if (Vk.DestroyDebugUtilsMessengerEXT != null)
            {
                Vk.DestroyDebugUtilsMessengerEXT(Instance, _debugMessenger, null);
            }
            _debugMessenger = 0;
        }
#endif
        if (Instance != 0)
        {
            Vk.DestroyInstance(Instance, null);
            Instance = 0;
        }
    }
}
