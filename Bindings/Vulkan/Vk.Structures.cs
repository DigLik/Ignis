using System.Runtime.InteropServices;

namespace Ignis.Bindings.Vulkan;

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
    public delegate* unmanaged<int, int, void*, void*, uint> pfnUserCallback;
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
internal unsafe struct VkPhysicalDeviceSynchronization2Features
{
    public int sType;
    public void* pNext;
    public uint synchronization2;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceDynamicRenderingFeatures
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

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryType
{
    public uint propertyFlags;
    public uint heapIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryHeap
{
    public ulong size;
    public uint flags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryType_16
{
    private VkMemoryType _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15;
    public VkMemoryType this[int index]
    {
        get
        {
            unsafe
            {
                fixed (VkMemoryType* p = &_0) return p[index];
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryType_32
{
    private VkMemoryType _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15,
                         _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31;
    public VkMemoryType this[int index]
    {
        get
        {
            unsafe
            {
                fixed (VkMemoryType* p = &_0) return p[index];
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryHeap_16
{
    private VkMemoryHeap _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15;
    public VkMemoryHeap this[int index]
    {
        get
        {
            unsafe
            {
                fixed (VkMemoryHeap* p = &_0) return p[index];
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPhysicalDeviceMemoryProperties
{
    public uint memoryTypeCount;
    public VkMemoryType_32 memoryTypes;
    public uint memoryHeapCount;
    public VkMemoryHeap_16 memoryHeaps;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkValidationFeaturesEXT
{
    public int sType;
    public void* pNext;
    public uint enabledValidationFeatureCount;
    public int* pEnabledValidationFeatures;
    public uint disabledValidationFeatureCount;
    public int* pDisabledValidationFeatures;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceMeshShaderFeaturesEXT
{
    public int sType;
    public void* pNext;
    public uint taskShader;
    public uint meshShader;
    public uint multiviewMeshShader;
    public uint primitiveFragmentShadingRateMeshShader;
    public uint meshShaderQueries;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDescriptorSetLayoutBinding
{
    public uint binding;
    public int descriptorType;
    public uint descriptorCount;
    public uint stageFlags;
    public nint* pImmutableSamplers;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDescriptorSetLayoutCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint bindingCount;
    public VkDescriptorSetLayoutBinding* pBindings;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorPoolSize
{
    public int type;
    public uint descriptorCount;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDescriptorPoolCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint maxSets;
    public uint poolSizeCount;
    public VkDescriptorPoolSize* pPoolSizes;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDescriptorSetAllocateInfo
{
    public int sType;
    public void* pNext;
    public nint descriptorPool;
    public uint descriptorSetCount;
    public nint* pSetLayouts;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorBufferInfo
{
    public nint buffer;
    public ulong offset;
    public ulong range;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkWriteDescriptorSet
{
    public int sType;
    public void* pNext;
    public nint dstSet;
    public uint dstBinding;
    public uint dstArrayElement;
    public uint descriptorCount;
    public int descriptorType;
    public void* pImageInfo;
    public VkDescriptorBufferInfo* pBufferInfo;
    public void* pTexelBufferView;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkShaderModuleCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public nuint codeSize;
    public uint* pCode;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPushConstantRange
{
    public uint stageFlags;
    public uint offset;
    public uint size;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineLayoutCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint setLayoutCount;
    public nint* pSetLayouts;
    public uint pushConstantRangeCount;
    public VkPushConstantRange* pPushConstantRanges;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineRenderingCreateInfo
{
    public int sType;
    public void* pNext;
    public uint viewMask;
    public uint colorAttachmentCount;
    public int* pColorAttachmentFormats;
    public int depthAttachmentFormat;
    public int stencilAttachmentFormat;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputBindingDescription
{
    public uint binding;
    public uint stride;
    public int inputRate;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputAttributeDescription
{
    public uint location;
    public uint binding;
    public int format;
    public uint offset;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineVertexInputStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint vertexBindingDescriptionCount;
    public VkVertexInputBindingDescription* pVertexBindingDescriptions;
    public uint vertexAttributeDescriptionCount;
    public VkVertexInputAttributeDescription* pVertexAttributeDescriptions;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineInputAssemblyStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public int topology;
    public uint primitiveRestartEnable;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineViewportStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint viewportCount;
    public void* pViewports;
    public uint scissorCount;
    public void* pScissors;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineRasterizationStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint depthClampEnable;
    public uint rasterizerDiscardEnable;
    public int polygonMode;
    public uint cullMode;
    public int frontFace;
    public uint depthBiasEnable;
    public float depthBiasConstantFactor;
    public float depthBiasClamp;
    public float depthBiasSlopeFactor;
    public float lineWidth;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineMultisampleStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public int rasterizationSamples;
    public uint sampleShadingEnable;
    public float minSampleShading;
    public uint* pSampleMask;
    public uint alphaToCoverageEnable;
    public uint alphaToOneEnable;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPipelineColorBlendAttachmentState
{
    public uint blendEnable;
    public int srcColorBlendFactor;
    public int dstColorBlendFactor;
    public int colorBlendOp;
    public int srcAlphaBlendFactor;
    public int dstAlphaBlendFactor;
    public int alphaBlendOp;
    public uint colorWriteMask;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineColorBlendStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint logicOpEnable;
    public int logicOp;
    public uint attachmentCount;
    public VkPipelineColorBlendAttachmentState* pAttachments;
    public float blendConstants_0;
    public float blendConstants_1;
    public float blendConstants_2;
    public float blendConstants_3;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineDynamicStateCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint dynamicStateCount;
    public int* pDynamicStates;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPipelineShaderStageCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint stage;
    public nint module;
    public byte* pName;
    public void* pSpecializationInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkGraphicsPipelineCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public uint stageCount;
    public VkPipelineShaderStageCreateInfo* pStages;
    public VkPipelineVertexInputStateCreateInfo* pVertexInputState;
    public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
    public void* pTessellationState;
    public VkPipelineViewportStateCreateInfo* pViewportState;
    public VkPipelineRasterizationStateCreateInfo* pRasterizationState;
    public VkPipelineMultisampleStateCreateInfo* pMultisampleState;
    public void* pDepthStencilState;
    public VkPipelineColorBlendStateCreateInfo* pColorBlendState;
    public VkPipelineDynamicStateCreateInfo* pDynamicState;
    public nint layout;
    public nint renderPass;
    public uint subpass;
    public nint basePipelineHandle;
    public int basePipelineIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkBufferCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public ulong size;
    public uint usage;
    public int sharingMode;
    public uint queueFamilyIndexCount;
    public uint* pQueueFamilyIndices;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryRequirements
{
    public ulong size;
    public ulong alignment;
    public uint memoryTypeBits;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkMemoryAllocateInfo
{
    public int sType;
    public void* pNext;
    public ulong allocationSize;
    public uint memoryTypeIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkViewport
{
    public float x;
    public float y;
    public float width;
    public float height;
    public float minDepth;
    public float maxDepth;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkBufferCopy
{
    public ulong srcOffset;
    public ulong dstOffset;
    public ulong size;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPhysicalDeviceFeatures
{
    public unsafe fixed uint features[55];
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceFeatures2
{
    public int sType;
    public void* pNext;
    public VkPhysicalDeviceFeatures features;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent3D
{
    public uint width;
    public uint height;
    public uint depth;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkImageCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public int imageType;
    public int format;
    public VkExtent3D extent;
    public uint mipLevels;
    public uint arrayLayers;
    public int samples;
    public int tiling;
    public uint usage;
    public int sharingMode;
    public uint queueFamilyIndexCount;
    public uint* pQueueFamilyIndices;
    public int initialLayout;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkSamplerCreateInfo
{
    public int sType;
    public void* pNext;
    public uint flags;
    public int magFilter;
    public int minFilter;
    public int mipmapMode;
    public int addressModeU;
    public int addressModeV;
    public int addressModeW;
    public float mipLodBias;
    public uint anisotropyEnable;
    public float maxAnisotropy;
    public uint compareEnable;
    public int compareOp;
    public float minLod;
    public float maxLod;
    public int borderColor;
    public uint unnormalizedCoordinates;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkImageSubresourceLayers
{
    public uint aspectMask;
    public uint mipLevel;
    public uint baseArrayLayer;
    public uint layerCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkOffset3D
{
    public int x;
    public int y;
    public int z;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkBufferImageCopy
{
    public ulong bufferOffset;
    public uint bufferRowLength;
    public uint bufferImageHeight;
    public VkImageSubresourceLayers imageSubresource;
    public VkOffset3D imageOffset;
    public VkExtent3D imageExtent;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorImageInfo
{
    public nint sampler;
    public nint imageView;
    public int imageLayout;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceDescriptorIndexingFeatures
{
    public int sType;
    public void* pNext;
    public uint shaderInputAttachmentArrayDynamicIndexing;
    public uint shaderUniformTexelBufferArrayDynamicIndexing;
    public uint shaderStorageTexelBufferArrayDynamicIndexing;
    public uint shaderUniformBufferArrayNonUniformIndexing;
    public uint shaderSampledImageArrayNonUniformIndexing;
    public uint shaderStorageBufferArrayNonUniformIndexing;
    public uint shaderStorageImageArrayNonUniformIndexing;
    public uint shaderInputAttachmentArrayNonUniformIndexing;
    public uint shaderUniformTexelBufferArrayNonUniformIndexing;
    public uint shaderStorageTexelBufferArrayNonUniformIndexing;
    public uint descriptorBindingUniformBufferUpdateAfterBind;
    public uint descriptorBindingSampledImageUpdateAfterBind;
    public uint descriptorBindingStorageImageUpdateAfterBind;
    public uint descriptorBindingStorageBufferUpdateAfterBind;
    public uint descriptorBindingUniformTexelBufferUpdateAfterBind;
    public uint descriptorBindingStorageTexelBufferUpdateAfterBind;
    public uint descriptorBindingUpdateUnusedWhilePending;
    public uint descriptorBindingPartiallyBound;
    public uint descriptorBindingVariableDescriptorCount;
    public uint runtimeDescriptorArray;
}
