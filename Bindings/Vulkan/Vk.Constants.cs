namespace Ignis.Bindings.Vulkan;

internal static unsafe partial class Vk
{
    internal const int Success = 0;
    internal const int False = 0;
    internal const int True = 1;
    internal const int SuboptimalKhr = 1000001003;
    internal const int ErrorOutOfDateKhr = -1000001004;

    internal const uint ApiVersion13 = 4206592;

    internal const uint QueueGraphicsBit = 0x00000001;

    internal const int StructureTypeApplicationInfo = 0;
    internal const int StructureTypeInstanceCreateInfo = 1;
    internal const int StructureTypeDeviceQueueCreateInfo = 2;
    internal const int StructureTypeDeviceCreateInfo = 3;
    internal const int StructureTypeCommandPoolCreateInfo = 39;
    internal const int StructureTypeCommandBufferAllocateInfo = 40;
    internal const int StructureTypeCommandBufferBeginInfo = 42;
    internal const int StructureTypeImageViewCreateInfo = 15;
    internal const int StructureTypeSemaphoreCreateInfo = 9;
    internal const int StructureTypeFenceCreateInfo = 8;
    internal const int StructureTypeSwapchainCreateInfoKhr = 1000001000;
    internal const int StructureTypePresentInfoKhr = 1000001001;
    internal const int StructureTypeWin32SurfaceCreateInfoKhr = 1000009000;
    internal const int StructureTypeDebugUtilsMessengerCreateInfoExt = 1000128004;

    internal const int StructureTypeRenderingInfo = 1000044000;
    internal const int StructureTypeRenderingAttachmentInfo = 1000044001;
    internal const int StructureTypePhysicalDeviceDynamicRenderingFeatures = 1000044003;

    internal const int StructureTypeImageMemoryBarrier2 = 1000314002;
    internal const int StructureTypeDependencyInfo = 1000314003;
    internal const int StructureTypeSubmitInfo2 = 1000314004;
    internal const int StructureTypeSemaphoreSubmitInfo = 1000314005;
    internal const int StructureTypeCommandBufferSubmitInfo = 1000314006;
    internal const int StructureTypePhysicalDeviceSynchronization2Features = 1000314007;

    internal const int ImageLayoutUndefined = 0;
    internal const int ImageLayoutColorAttachmentOptimal = 2;
    internal const int ImageLayoutShaderReadOnlyOptimal = 5;
    internal const int ImageLayoutTransferDstOptimal = 7;
    internal const int ImageLayoutPresentSrcKhr = 1000001002;

    internal const int AttachmentLoadOpClear = 1;
    internal const int AttachmentStoreOpStore = 0;

    internal const int SharingModeExclusive = 0;

    internal const ulong PipelineStage2TopOfPipeBit = 0x00000001ul;
    internal const ulong PipelineStage2ColorAttachmentOutputBit = 0x00000400ul;
    internal const ulong PipelineStage2BottomOfPipeBit = 0x00002000ul;
    internal const ulong PipelineStage2CopyBit = 0x100000000ul;
    internal const ulong PipelineStage2FragmentShaderBit = 0x00000080ul;
    internal const ulong PipelineStage2None = 0ul;

    internal const ulong Access2ColorAttachmentWriteBit = 0x00000100ul;
    internal const ulong Access2TransferWriteBit = 0x00001000ul;
    internal const ulong Access2ShaderReadBit = 0x00000020ul;
    internal const ulong Access2None = 0ul;

    internal const int FormatB8G8R8A8Unorm = 44;
    internal const int ColorSpaceSrgbNonlinearKhr = 0;
    internal const int PresentModeImmediateKhr = 0;
    internal const int PresentModeMailboxKhr = 1;
    internal const int PresentModeFifoKhr = 2;
    internal const int PresentModeFifoRelaxedKhr = 3;
    internal const int CompositeAlphaOpaqueBitKhr = 1;
    internal const uint ImageUsageColorAttachmentBit = 16;
    internal const uint ImageAspectColorBit = 1;
    internal const int ImageViewType2D = 1;

    internal const int FenceCreateSignaledBit = 0x00000001;
    internal const int CommandBufferLevelPrimary = 0;

    internal const int DebugUtilsMessageSeverityVerboseBitExt = 0x00000001;
    internal const int DebugUtilsMessageSeverityInfoBitExt = 0x00000010;
    internal const int DebugUtilsMessageSeverityWarningBitExt = 0x00000100;
    internal const int DebugUtilsMessageSeverityErrorBitExt = 0x00001000;

    internal const int DebugUtilsMessageTypeGeneralBitExt = 0x00000001;
    internal const int DebugUtilsMessageTypeValidationBitExt = 0x00000002;
    internal const int DebugUtilsMessageTypePerformanceBitExt = 0x00000004;

    internal const uint ApiVersion14 = 4210688;

    internal const int StructureTypePhysicalDeviceMeshShaderFeaturesExt = 1000328000;
    internal const int StructureTypeSamplerCreateInfo = 31;
    internal const int StructureTypeImageCreateInfo = 14;
    internal const int StructureTypeDescriptorSetLayoutCreateInfo = 32;
    internal const int StructureTypeDescriptorPoolCreateInfo = 33;
    internal const int StructureTypeDescriptorSetAllocateInfo = 34;
    internal const int StructureTypeWriteDescriptorSet = 35;
    internal const int StructureTypeValidationFeaturesExt = 1000247000;
    internal const int StructureTypeShaderModuleCreateInfo = 16;
    internal const int StructureTypePhysicalDeviceFeatures2 = 1000059000;
    internal const int StructureTypePipelineShaderStageCreateInfo = 18;
    internal const int StructureTypePipelineLayoutCreateInfo = 30;
    internal const int StructureTypeGraphicsPipelineCreateInfo = 28;
    internal const int StructureTypePipelineVertexInputStateCreateInfo = 19;
    internal const int StructureTypePipelineInputAssemblyStateCreateInfo = 20;
    internal const int StructureTypePipelineViewportStateCreateInfo = 22;
    internal const int StructureTypePipelineRasterizationStateCreateInfo = 23;
    internal const int StructureTypePipelineMultisampleStateCreateInfo = 24;
    internal const int StructureTypePipelineColorBlendStateCreateInfo = 26;
    internal const int StructureTypePipelineDynamicStateCreateInfo = 27;
    internal const int StructureTypeBufferCreateInfo = 12;
    internal const int StructureTypeMemoryAllocateInfo = 5;
    internal const int StructureTypePipelineRenderingCreateInfo = 1000044002;

    internal const uint ShaderStageVertexBit = 0x00000001;
    internal const uint ShaderStageFragmentBit = 0x00000010;
    internal const uint ShaderStageMeshBitExt = 0x00000080;
    internal const uint ShaderStageTaskBitExt = 0x00000040;

    internal const int DescriptorTypeStorageBuffer = 7;

    internal const int PrimitiveTopologyTriangleList = 3;
    internal const int PolygonModeFill = 0;
    internal const int CullModeNone = 0;
    internal const int FrontFaceCounterClockwise = 1;
    internal const int SampleCount1Bit = 0x00000001;

    internal const int BlendFactorSrcAlpha = 6;
    internal const int BlendFactorOneMinusSrcAlpha = 7;
    internal const int BlendFactorOne = 1;
    internal const int BlendFactorZero = 0;
    internal const int BlendOpAdd = 0;

    internal const int DynamicStateViewport = 0;
    internal const int DynamicStateScissor = 1;

    internal const uint BufferUsageStorageBufferBit = 0x00000020;
    internal const uint BufferUsageTransferSrcBit = 0x00000001;
    internal const uint BufferUsageTransferDstBit = 0x00000002;

    internal const uint MemoryPropertyDeviceLocalBit = 0x00000001;
    internal const uint MemoryPropertyHostVisibleBit = 0x00000002;
    internal const uint MemoryPropertyHostCoherentBit = 0x00000004;

    internal const int ValidationFeatureEnableSynchronizationValidationExt = 1;
    internal const int ValidationFeatureEnableGpuAssistedExt = 0;
}
