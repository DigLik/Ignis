using System.Numerics;
using System.Runtime.InteropServices;
using Ignis.Bindings.Vulkan;
using Ignis.Core.Collections;

namespace Ignis.Platform.Graphics;

[StructLayout(LayoutKind.Sequential)]
internal struct GPUShape
{
    public int Type;
    private readonly float _pad;
    public Vector2 P1;
    public Vector2 P2;
    public float Angle;
    public float Thickness;
    public Vector4 Color;
}

[StructLayout(LayoutKind.Sequential)]
internal struct PushConstants
{
    public Vector2 CameraPos;
    public Vector2 CameraSize;
    public uint TotalShapes;
}

internal sealed unsafe class ShapeBatcher : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanAllocator _allocator;
    private readonly NativeList<GPUShape> _shapes = new();
    private int _maxShapesPerFrame = 20000;

    // Ресурсы текстур шрифта
    private readonly nint[] _fontImages = new nint[96];
    private readonly Allocation[] _fontImageAllocations = new Allocation[96];
    private readonly nint[] _fontImageViews = new nint[96];
    private nint _fontSampler;
    private nint _fontStagingBuffer;
    private Allocation _fontStagingBufferAllocation = null!;
    private bool _fontUploaded;

    private nint _descriptorSetLayout;
    private nint _descriptorPool;
    private readonly nint[] _descriptorSets = new nint[RenderConfig.MaxFramesInFlight];

    private nint _ringBuffer;
    private Allocation _ringBufferAllocation = null!;
    private void* _mappedRingBufferData;

    public nint DescriptorSetLayout => _descriptorSetLayout;

    public ShapeBatcher(VulkanDevice device, VulkanAllocator allocator)
    {
        _device = device;
        _allocator = allocator;

        CreateFontResources();
        CreateDescriptorResources();
        CreateRingBuffer();
    }

    public void Add(GPUShape shape)
    {
        _shapes.Add(shape);
    }

    public void UploadFontIfNeeded(nint cmdBuf)
    {
        if (!_fontUploaded)
        {
            for (int i = 0; i < 96; i++)
            {
                VkImageMemoryBarrier2 barrier1 = new()
                {
                    sType = Vk.StructureTypeImageMemoryBarrier2,
                    srcStageMask = Vk.PipelineStage2None,
                    srcAccessMask = Vk.Access2None,
                    dstStageMask = Vk.PipelineStage2CopyBit,
                    dstAccessMask = Vk.Access2TransferWriteBit,
                    oldLayout = Vk.ImageLayoutUndefined,
                    newLayout = Vk.ImageLayoutTransferDstOptimal,
                    image = _fontImages[i]
                };
                barrier1.subresourceRange.aspectMask = 1;
                barrier1.subresourceRange.levelCount = 1;
                barrier1.subresourceRange.layerCount = 1;

                VkDependencyInfo depInfo1 = new()
                {
                    sType = Vk.StructureTypeDependencyInfo,
                    imageMemoryBarrierCount = 1,
                    pImageMemoryBarriers = &barrier1
                };
                Vk.CmdPipelineBarrier2(cmdBuf, &depInfo1);

                VkBufferImageCopy region = new()
                {
                    bufferOffset = (ulong)i * 64,
                    bufferRowLength = 0,
                    bufferImageHeight = 0
                };
                region.imageSubresource.aspectMask = 1;
                region.imageSubresource.layerCount = 1;
                region.imageExtent = new VkExtent3D { width = 8, height = 8, depth = 1 };

                Vk.CmdCopyBufferToImage(cmdBuf, _fontStagingBuffer, _fontImages[i], Vk.ImageLayoutTransferDstOptimal, 1, &region);

                VkImageMemoryBarrier2 barrier2 = new()
                {
                    sType = Vk.StructureTypeImageMemoryBarrier2,
                    srcStageMask = Vk.PipelineStage2CopyBit,
                    srcAccessMask = Vk.Access2TransferWriteBit,
                    dstStageMask = Vk.PipelineStage2FragmentShaderBit,
                    dstAccessMask = Vk.Access2ShaderReadBit,
                    oldLayout = Vk.ImageLayoutTransferDstOptimal,
                    newLayout = Vk.ImageLayoutShaderReadOnlyOptimal,
                    image = _fontImages[i]
                };
                barrier2.subresourceRange.aspectMask = 1;
                barrier2.subresourceRange.levelCount = 1;
                barrier2.subresourceRange.layerCount = 1;

                VkDependencyInfo depInfo2 = new()
                {
                    sType = Vk.StructureTypeDependencyInfo,
                    imageMemoryBarrierCount = 1,
                    pImageMemoryBarriers = &barrier2
                };
                Vk.CmdPipelineBarrier2(cmdBuf, &depInfo2);
            }

            _fontUploaded = true;
        }
    }

    public void FlushAndDraw(nint cmdBuf, int frameIndex, Vector2 cameraPos, Vector2 cameraSize, VkExtent2D extent,
        nint pipeline, nint pipelineLayout)
    {
        if (_shapes.Count == 0) return;

        if (_shapes.Count > _maxShapesPerFrame)
        {
            Vk.DeviceWaitIdle(_device.Device);
            int newMaxShapes = (int)(_shapes.Count * 1.5f);
            RecreateRingBuffer(newMaxShapes);
        }

        int shapesToDraw = _shapes.Count;
        ulong ringBufferFrameSize = (ulong)_maxShapesPerFrame * RenderConfig.GpuShapeSize;

        ulong frameOffset = (ulong)frameIndex * ringBufferFrameSize;
        byte* destPtr = (byte*)_mappedRingBufferData + frameOffset;

        var shapesSpan = _shapes.Data;
        fixed (GPUShape* pShapes = shapesSpan)
        {
            Buffer.MemoryCopy(pShapes, destPtr, ringBufferFrameSize, (ulong)shapesToDraw * RenderConfig.GpuShapeSize);
        }

        Vk.CmdBindPipeline(cmdBuf, 0, pipeline);

        nint set = _descriptorSets[frameIndex];
        Vk.CmdBindDescriptorSets(cmdBuf, 0, pipelineLayout, 0, 1, &set, 0, null);

        VkViewport viewport = new()
        {
            x = 0.0f,
            y = (float)extent.height,
            width = (float)extent.width,
            height = -(float)extent.height,
            minDepth = 0.0f,
            maxDepth = 1.0f
        };
        Vk.CmdSetViewport(cmdBuf, 0, 1, &viewport);

        VkRect2D scissor = new()
        {
            offset = new VkOffset2D { x = 0, y = 0 },
            extent = extent
        };
        Vk.CmdSetScissor(cmdBuf, 0, 1, &scissor);

        PushConstants push;
        push.CameraPos = cameraPos;
        push.CameraSize = cameraSize;
        push.TotalShapes = (uint)shapesToDraw;

        Vk.CmdPushConstants(cmdBuf, pipelineLayout, Vk.ShaderStageTaskBitExt | Vk.ShaderStageMeshBitExt, 0, 20, &push);

        Vk.CmdDrawMeshTasksEXT(cmdBuf, ((uint)shapesToDraw + 31) / 32, 1, 1);

        _shapes.Clear();
    }

    private void CreateDescriptorResources()
    {
        VkDescriptorSetLayoutBinding* bindings = stackalloc VkDescriptorSetLayoutBinding[2];
        bindings[0] = new VkDescriptorSetLayoutBinding
        {
            binding = 0,
            descriptorType = Vk.DescriptorTypeStorageBuffer,
            descriptorCount = 1,
            stageFlags = Vk.ShaderStageTaskBitExt | Vk.ShaderStageMeshBitExt,
            pImmutableSamplers = null
        };
        bindings[1] = new VkDescriptorSetLayoutBinding
        {
            binding = 1,
            descriptorType = 1, // VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER
            descriptorCount = 96,
            stageFlags = 0x00000010, // VK_SHADER_STAGE_FRAGMENT_BIT
            pImmutableSamplers = null
        };

        VkDescriptorSetLayoutCreateInfo layoutInfo = new()
        {
            sType = Vk.StructureTypeDescriptorSetLayoutCreateInfo,
            bindingCount = 2,
            pBindings = bindings
        };

        nint layout;
        if (Vk.CreateDescriptorSetLayout(_device.Device, &layoutInfo, null, &layout) != 0)
            throw new InvalidOperationException("Не удалось создать Layout дескрипторов.");
        _descriptorSetLayout = layout;

        VkDescriptorPoolSize* poolSizes = stackalloc VkDescriptorPoolSize[2];
        poolSizes[0] = new VkDescriptorPoolSize
        {
            type = Vk.DescriptorTypeStorageBuffer,
            descriptorCount = (uint)RenderConfig.MaxFramesInFlight
        };
        poolSizes[1] = new VkDescriptorPoolSize
        {
            type = 1, // VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER
            descriptorCount = (uint)RenderConfig.MaxFramesInFlight * 96
        };

        VkDescriptorPoolCreateInfo poolInfo = new()
        {
            sType = Vk.StructureTypeDescriptorPoolCreateInfo,
            maxSets = (uint)RenderConfig.MaxFramesInFlight,
            poolSizeCount = 2,
            pPoolSizes = poolSizes
        };

        nint pool;
        if (Vk.CreateDescriptorPool(_device.Device, &poolInfo, null, &pool) != 0)
            throw new InvalidOperationException("Не удалось создать Pool дескрипторов.");
        _descriptorPool = pool;

        fixed (nint* pLayouts = new nint[RenderConfig.MaxFramesInFlight]
                   { _descriptorSetLayout, _descriptorSetLayout, _descriptorSetLayout })
        {
            VkDescriptorSetAllocateInfo allocInfo = new()
            {
                sType = Vk.StructureTypeDescriptorSetAllocateInfo,
                descriptorPool = _descriptorPool,
                descriptorSetCount = RenderConfig.MaxFramesInFlight,
                pSetLayouts = pLayouts
            };

            fixed (nint* pSets = _descriptorSets)
            {
                if (Vk.AllocateDescriptorSets(_device.Device, &allocInfo, pSets) != 0)
                    throw new InvalidOperationException("Не удалось выделить дескрипторные сеты.");
            }
        }
    }

    private void CreateRingBuffer()
    {
        ulong ringBufferFrameSize = (ulong)_maxShapesPerFrame * RenderConfig.GpuShapeSize;
        ulong size = (ulong)RenderConfig.MaxFramesInFlight * ringBufferFrameSize;

        VkBufferCreateInfo bufferInfo = new()
        {
            sType = Vk.StructureTypeBufferCreateInfo,
            size = size,
            usage = Vk.BufferUsageStorageBufferBit,
            sharingMode = Vk.SharingModeExclusive
        };

        nint buffer;
        if (Vk.CreateBuffer(_device.Device, &bufferInfo, null, &buffer) != 0)
            throw new InvalidOperationException("Не удалось создать Ring Buffer.");
        _ringBuffer = buffer;

        VkMemoryRequirements memReq;
        Vk.GetBufferMemoryRequirements(_device.Device, _ringBuffer, &memReq);

        uint desiredProperties = Vk.MemoryPropertyHostVisibleBit | Vk.MemoryPropertyHostCoherentBit | Vk.MemoryPropertyDeviceLocalBit;
        uint fallbackProperties = Vk.MemoryPropertyHostVisibleBit | Vk.MemoryPropertyHostCoherentBit;

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        if (!_device.TryFindMemoryType(memReq.memoryTypeBits, desiredProperties, out uint memType))
        {
            memType = _device.FindMemoryType(memReq.memoryTypeBits, fallbackProperties);
            Console.WriteLine("[Vulkan] Resizable BAR is NOT active for RingBuffer (Using standard HostVisible memory).");
        }
        else
        {
            Console.WriteLine("[Vulkan] Resizable BAR is ACTIVE for RingBuffer.");
        }
#pragma warning restore CA1303

        _ringBufferAllocation = _allocator.Allocate(memReq.size, memReq.alignment, memType, hostVisible: true);

        Vk.BindBufferMemory(_device.Device, _ringBuffer, _ringBufferAllocation.DeviceMemory,
            _ringBufferAllocation.Offset);
        _mappedRingBufferData = _ringBufferAllocation.MappedData;

        UpdateDescriptorSets();
    }

    private void UpdateDescriptorSets()
    {
        ulong ringBufferFrameSize = (ulong)_maxShapesPerFrame * RenderConfig.GpuShapeSize;
        VkDescriptorImageInfo* imageInfos = stackalloc VkDescriptorImageInfo[96];
        VkWriteDescriptorSet* writes = stackalloc VkWriteDescriptorSet[2];

        for (uint i = 0; i < RenderConfig.MaxFramesInFlight; i++)
        {
            VkDescriptorBufferInfo bufferInfoDesc = new()
            {
                buffer = _ringBuffer,
                offset = i * ringBufferFrameSize,
                range = ringBufferFrameSize
            };

            for (int t = 0; t < 96; t++)
            {
                imageInfos[t] = new VkDescriptorImageInfo
                {
                    sampler = _fontSampler,
                    imageView = _fontImageViews[t],
                    imageLayout = 5 // VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL
                };
            }

            writes[0] = new VkWriteDescriptorSet
            {
                sType = Vk.StructureTypeWriteDescriptorSet,
                dstSet = _descriptorSets[i],
                dstBinding = 0,
                dstArrayElement = 0,
                descriptorCount = 1,
                descriptorType = Vk.DescriptorTypeStorageBuffer,
                pBufferInfo = &bufferInfoDesc
            };
            writes[1] = new VkWriteDescriptorSet
            {
                sType = Vk.StructureTypeWriteDescriptorSet,
                dstSet = _descriptorSets[i],
                dstBinding = 1,
                dstArrayElement = 0,
                descriptorCount = 96,
                descriptorType = 1, // VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER
                pImageInfo = imageInfos
            };

            Vk.UpdateDescriptorSets(_device.Device, 2, writes, 0, null);
        }
    }

    private void CreateFontResources()
    {
        ulong stagingSize = 96 * 8 * 8;
        VkBufferCreateInfo stagingBufferInfo = new()
        {
            sType = Vk.StructureTypeBufferCreateInfo,
            size = stagingSize,
            usage = 0x00000001, // VK_BUFFER_USAGE_TRANSFER_SRC_BIT
            sharingMode = 0
        };

        nint stagingBuffer;
        if (Vk.CreateBuffer(_device.Device, &stagingBufferInfo, null, &stagingBuffer) != 0)
            throw new InvalidOperationException("Не удалось создать staging буфер для шрифта.");
        _fontStagingBuffer = stagingBuffer;

        VkMemoryRequirements stagingMemReq;
        Vk.GetBufferMemoryRequirements(_device.Device, _fontStagingBuffer, &stagingMemReq);

        uint stagingMemType = _device.FindMemoryType(stagingMemReq.memoryTypeBits,
            Vk.MemoryPropertyHostVisibleBit | Vk.MemoryPropertyHostCoherentBit);

        _fontStagingBufferAllocation = _allocator.Allocate(stagingMemReq.size, stagingMemReq.alignment, stagingMemType,
            hostVisible: true);
        Vk.BindBufferMemory(_device.Device, _fontStagingBuffer, _fontStagingBufferAllocation.DeviceMemory,
            _fontStagingBufferAllocation.Offset);

        byte* pDst = (byte*)_fontStagingBufferAllocation.MappedData;
        for (int charIdx = 0; charIdx < 96; charIdx++)
        {
            for (int row = 0; row < 8; row++)
            {
                byte rowByte = Renderer.FontData[charIdx * 8 + row];
                for (int col = 0; col < 8; col++)
                {
                    bool pixelOn = (rowByte & (1 << (7 - col))) != 0;
                    pDst[charIdx * 64 + row * 8 + col] = pixelOn ? (byte)0xFF : (byte)0x00;
                }
            }
        }

        VkSamplerCreateInfo samplerInfo = new()
        {
            sType = Vk.StructureTypeSamplerCreateInfo,
            magFilter = 0,
            minFilter = 0,
            mipmapMode = 0,
            addressModeU = 2,
            addressModeV = 2,
            addressModeW = 2,
            borderColor = 3,
            maxLod = 1.0f
        };

        nint sampler;
        if (Vk.CreateSampler(_device.Device, &samplerInfo, null, &sampler) != 0)
            throw new InvalidOperationException("Не удалось создать сэмплер для шрифта.");
        _fontSampler = sampler;

        for (int i = 0; i < 96; i++)
        {
            VkImageCreateInfo imageInfo = new()
            {
                sType = Vk.StructureTypeImageCreateInfo,
                imageType = 1,
                format = 9, // VK_FORMAT_R8_UNORM
                extent = new VkExtent3D { width = 8, height = 8, depth = 1 },
                mipLevels = 1,
                arrayLayers = 1,
                samples = 1,
                tiling = 0,
                usage = 0x00000002 | 0x00000004,
                sharingMode = 0,
                initialLayout = 0
            };

            nint image;
            if (Vk.CreateImage(_device.Device, &imageInfo, null, &image) != 0)
                throw new InvalidOperationException($"Не удалось создать VkImage для символа {i}.");
            _fontImages[i] = image;

            VkMemoryRequirements imgMemReq;
            Vk.GetImageMemoryRequirements(_device.Device, image, &imgMemReq);

            uint imgMemType = _device.FindMemoryType(imgMemReq.memoryTypeBits, 1);

            _fontImageAllocations[i] =
                _allocator.Allocate(imgMemReq.size, imgMemReq.alignment, imgMemType, hostVisible: false);
            Vk.BindImageMemory(_device.Device, image, _fontImageAllocations[i].DeviceMemory,
                _fontImageAllocations[i].Offset);

            VkImageViewCreateInfo viewInfo = new()
            {
                sType = Vk.StructureTypeImageViewCreateInfo,
                image = image,
                viewType = Vk.ImageViewType2D,
                format = 9
            };
            viewInfo.subresourceRange.aspectMask = 1;
            viewInfo.subresourceRange.levelCount = 1;
            viewInfo.subresourceRange.layerCount = 1;

            nint imageView;
            if (Vk.CreateImageView(_device.Device, &viewInfo, null, &imageView) != 0)
                throw new InvalidOperationException($"Не удалось создать VkImageView для символа {i}.");
            _fontImageViews[i] = imageView;
        }
    }

    private void RecreateRingBuffer(int newMaxShapes)
    {
        if (_ringBuffer != 0)
        {
            Vk.DestroyBuffer(_device.Device, _ringBuffer, null);
            _ringBuffer = 0;
        }

        if (_ringBufferAllocation != null)
        {
            _allocator.Free(_ringBufferAllocation);
            _ringBufferAllocation = null!;
        }

        _maxShapesPerFrame = newMaxShapes;
        CreateRingBuffer();
    }

    public void Dispose()
    {
        _shapes.Dispose();

        for (int i = 0; i < 96; i++)
        {
            if (_fontImageViews[i] != 0)
            {
                Vk.DestroyImageView(_device.Device, _fontImageViews[i], null);
            }

            if (_fontImages[i] != 0)
            {
                Vk.DestroyImage(_device.Device, _fontImages[i], null);
            }

            if (_fontImageAllocations[i] != null)
            {
                _allocator.Free(_fontImageAllocations[i]);
            }
        }

        if (_fontSampler != 0)
        {
            Vk.DestroySampler(_device.Device, _fontSampler, null);
        }

        if (_fontStagingBuffer != 0)
        {
            Vk.DestroyBuffer(_device.Device, _fontStagingBuffer, null);
        }

        if (_fontStagingBufferAllocation != null)
        {
            _allocator.Free(_fontStagingBufferAllocation);
        }

        if (_device.Device != 0)
        {
            if (_ringBuffer != 0)
            {
                Vk.DestroyBuffer(_device.Device, _ringBuffer, null);
                _ringBuffer = 0;
            }

            if (_ringBufferAllocation != null)
            {
                _allocator.Free(_ringBufferAllocation);
                _ringBufferAllocation = null!;
            }

            if (_descriptorPool != 0)
            {
                Vk.DestroyDescriptorPool(_device.Device, _descriptorPool, null);
                _descriptorPool = 0;
            }

            if (_descriptorSetLayout != 0)
            {
                Vk.DestroyDescriptorSetLayout(_device.Device, _descriptorSetLayout, null);
                _descriptorSetLayout = 0;
            }
        }
    }
}
