using Ignis.Bindings.Vulkan;

namespace Ignis.Platform.Graphics;

internal sealed unsafe class VulkanPipeline : IDisposable
{
    private readonly nint _device;

    public nint Pipeline { get; private set; }
    public nint PipelineLayout { get; private set; }

    public nint TaskShaderModule { get; private set; }
    public nint MeshShaderModule { get; private set; }
    public nint FragmentShaderModule { get; private set; }

    public VulkanPipeline(nint device, int swapchainImageFormat, nint descriptorSetLayout)
    {
        _device = device;
        CreatePipeline(swapchainImageFormat, descriptorSetLayout);
    }

    private void CreatePipeline(int swapchainImageFormat, nint descriptorSetLayout)
    {
        fixed (byte* pCode = Shaders.shader_task)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                sType = Vk.StructureTypeShaderModuleCreateInfo,
                codeSize = (nuint)Shaders.shader_task.Length,
                pCode = (uint*)pCode
            };
            nint module;
            if (Vk.CreateShaderModule(_device, &createInfo, null, &module) != 0)
                throw new InvalidOperationException("Не удалось создать модуль Task шейдера.");
            TaskShaderModule = module;
        }

        fixed (byte* pCode = Shaders.shader_mesh)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                sType = Vk.StructureTypeShaderModuleCreateInfo,
                codeSize = (nuint)Shaders.shader_mesh.Length,
                pCode = (uint*)pCode
            };
            nint module;
            if (Vk.CreateShaderModule(_device, &createInfo, null, &module) != 0)
                throw new InvalidOperationException("Не удалось создать модуль Mesh шейдера.");
            MeshShaderModule = module;
        }

        fixed (byte* pCode = Shaders.shader_frag)
        {
            VkShaderModuleCreateInfo createInfo = new()
            {
                sType = Vk.StructureTypeShaderModuleCreateInfo,
                codeSize = (nuint)Shaders.shader_frag.Length,
                pCode = (uint*)pCode
            };
            nint module;
            if (Vk.CreateShaderModule(_device, &createInfo, null, &module) != 0)
                throw new InvalidOperationException("Не удалось создать модуль Fragment шейдера.");
            FragmentShaderModule = module;
        }

        VkPushConstantRange pushRange = new()
        {
            stageFlags = Vk.ShaderStageTaskBitExt | Vk.ShaderStageMeshBitExt,
            offset = 0,
            size = 20
        };

        VkPipelineLayoutCreateInfo layoutInfo = new()
        {
            sType = Vk.StructureTypePipelineLayoutCreateInfo,
            setLayoutCount = 1,
            pSetLayouts = &descriptorSetLayout,
            pushConstantRangeCount = 1,
            pPushConstantRanges = &pushRange
        };

        nint pipLayout;
        if (Vk.CreatePipelineLayout(_device, &layoutInfo, null, &pipLayout) != 0)
            throw new InvalidOperationException("Не удалось создать Layout конвейера.");
        PipelineLayout = pipLayout;

        fixed (byte* pMain = "main\0"u8)
        {
            VkPipelineShaderStageCreateInfo* pStages = stackalloc VkPipelineShaderStageCreateInfo[3];

            pStages[0] = new VkPipelineShaderStageCreateInfo
            {
                sType = Vk.StructureTypePipelineShaderStageCreateInfo,
                stage = Vk.ShaderStageTaskBitExt,
                module = TaskShaderModule,
                pName = pMain
            };

            pStages[1] = new VkPipelineShaderStageCreateInfo
            {
                sType = Vk.StructureTypePipelineShaderStageCreateInfo,
                stage = Vk.ShaderStageMeshBitExt,
                module = MeshShaderModule,
                pName = pMain
            };

            pStages[2] = new VkPipelineShaderStageCreateInfo
            {
                sType = Vk.StructureTypePipelineShaderStageCreateInfo,
                stage = Vk.ShaderStageFragmentBit,
                module = FragmentShaderModule,
                pName = pMain
            };

            VkPipelineViewportStateCreateInfo viewportState = new()
            {
                sType = Vk.StructureTypePipelineViewportStateCreateInfo,
                viewportCount = 1,
                pViewports = null,
                scissorCount = 1,
                pScissors = null
            };

            VkPipelineRasterizationStateCreateInfo rasterizationState = new()
            {
                sType = Vk.StructureTypePipelineRasterizationStateCreateInfo,
                depthClampEnable = 0,
                rasterizerDiscardEnable = 0,
                polygonMode = 0,
                cullMode = 0,
                frontFace = 0,
                depthBiasEnable = 0,
                lineWidth = 1.0f
            };

            VkPipelineMultisampleStateCreateInfo multisampleState = new()
            {
                sType = Vk.StructureTypePipelineMultisampleStateCreateInfo,
                rasterizationSamples = 1,
                sampleShadingEnable = 0
            };

            VkPipelineColorBlendAttachmentState colorBlendAttachment = new()
            {
                colorWriteMask = 0xF,
                blendEnable = 1,
                srcColorBlendFactor = 6,
                dstColorBlendFactor = 7,
                colorBlendOp = 0,
                srcAlphaBlendFactor = 1,
                dstAlphaBlendFactor = 0,
                alphaBlendOp = 0
            };

            VkPipelineColorBlendStateCreateInfo colorBlendState = new()
            {
                sType = Vk.StructureTypePipelineColorBlendStateCreateInfo,
                logicOpEnable = 0,
                attachmentCount = 1,
                pAttachments = &colorBlendAttachment
            };

            int* dynamicStates = stackalloc int[2]
            {
                Vk.DynamicStateViewport,
                Vk.DynamicStateScissor
            };

            VkPipelineDynamicStateCreateInfo dynamicStateInfo = new()
            {
                sType = Vk.StructureTypePipelineDynamicStateCreateInfo,
                dynamicStateCount = 2,
                pDynamicStates = dynamicStates
            };

            int colorFormat = swapchainImageFormat;
            VkPipelineRenderingCreateInfo renderingInfo = new()
            {
                sType = Vk.StructureTypePipelineRenderingCreateInfo,
                colorAttachmentCount = 1,
                pColorAttachmentFormats = &colorFormat
            };

            VkGraphicsPipelineCreateInfo pipelineInfo = new()
            {
                sType = Vk.StructureTypeGraphicsPipelineCreateInfo,
                pNext = &renderingInfo,
                stageCount = 3,
                pStages = pStages,
                pVertexInputState = null,
                pInputAssemblyState = null,
                pTessellationState = null,
                pViewportState = &viewportState,
                pRasterizationState = &rasterizationState,
                pMultisampleState = &multisampleState,
                pDepthStencilState = null,
                pColorBlendState = &colorBlendState,
                pDynamicState = &dynamicStateInfo,
                layout = PipelineLayout,
                basePipelineIndex = -1
            };

            nint pipeline;
            if (Vk.CreateGraphicsPipelines(_device, 0, 1, &pipelineInfo, null, &pipeline) != 0)
                throw new InvalidOperationException("Не удалось создать графический конвейер Mesh Shader.");
            Pipeline = pipeline;
        }
    }

    public void Dispose()
    {
        if (_device != 0)
        {
            if (Pipeline != 0)
            {
                Vk.DestroyPipeline(_device, Pipeline, null);
                Pipeline = 0;
            }
            if (PipelineLayout != 0)
            {
                Vk.DestroyPipelineLayout(_device, PipelineLayout, null);
                PipelineLayout = 0;
            }
            if (TaskShaderModule != 0)
            {
                Vk.DestroyShaderModule(_device, TaskShaderModule, null);
                TaskShaderModule = 0;
            }
            if (MeshShaderModule != 0)
            {
                Vk.DestroyShaderModule(_device, MeshShaderModule, null);
                MeshShaderModule = 0;
            }
            if (FragmentShaderModule != 0)
            {
                Vk.DestroyShaderModule(_device, FragmentShaderModule, null);
                FragmentShaderModule = 0;
            }
        }
    }
}
