using Ignis.Bindings.Vulkan;

namespace Ignis.Platform.Graphics;

internal sealed unsafe class VulkanCommandManager : IDisposable
{
    private readonly nint _device;
    private nint _commandPool;

    private readonly nint[] _commandBuffers = new nint[RenderConfig.MaxFramesInFlight];
    private readonly nint[] _imageAvailableSemaphores = new nint[RenderConfig.MaxFramesInFlight];
    private readonly nint[] _inFlightFences = new nint[RenderConfig.MaxFramesInFlight];

    private nint[] _renderFinishedSemaphores = [];
    private nint[] _imagesInFlight = [];

    public VulkanCommandManager(nint device, int swapchainImageCount)
    {
        _device = device;
        CreateCommandPool();
        CreateSyncObjects(swapchainImageCount);
    }

    public nint GetCommandBuffer(int frameIndex) => _commandBuffers[frameIndex];
    public nint GetImageAvailableSemaphore(int frameIndex) => _imageAvailableSemaphores[frameIndex];
    public nint GetInFlightFence(int frameIndex) => _inFlightFences[frameIndex];
    public nint GetRenderFinishedSemaphore(int imageIndex) => _renderFinishedSemaphores[imageIndex];
    public ref nint GetImageInFlight(int imageIndex) => ref _imagesInFlight[imageIndex];

    private void CreateCommandPool()
    {
        VkCommandPoolCreateInfo poolInfo = new()
        {
            sType = Vk.StructureTypeCommandPoolCreateInfo,
            flags = 0x00000002, // VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT
            queueFamilyIndex = 0
        };

        nint commandPool;
        if (Vk.CreateCommandPool(_device, &poolInfo, null, &commandPool) != 0)
        {
            throw new InvalidOperationException("Ошибка CommandPool.");
        }
        _commandPool = commandPool;

        VkCommandBufferAllocateInfo allocInfo = new()
        {
            sType = Vk.StructureTypeCommandBufferAllocateInfo,
            commandPool = _commandPool,
            level = Vk.CommandBufferLevelPrimary,
            commandBufferCount = RenderConfig.MaxFramesInFlight
        };

        fixed (nint* pCmdBuffers = _commandBuffers)
        {
            if (Vk.AllocateCommandBuffers(_device, &allocInfo, pCmdBuffers) != 0)
            {
                throw new InvalidOperationException("Ошибка CmdBuffer.");
            }
        }
    }

    private void CreateSyncObjects(int swapchainImageCount)
    {
        VkSemaphoreCreateInfo semInfo = new()
        {
            sType = Vk.StructureTypeSemaphoreCreateInfo
        };

        VkFenceCreateInfo fenceInfo = new()
        {
            sType = Vk.StructureTypeFenceCreateInfo,
            flags = Vk.FenceCreateSignaledBit
        };

        for (int i = 0; i < RenderConfig.MaxFramesInFlight; i++)
        {
            nint imgAvail, inFlight;

            if (Vk.CreateSemaphore(_device, &semInfo, null, &imgAvail) != 0 ||
                Vk.CreateFence(_device, &fenceInfo, null, &inFlight) != 0)
            {
                throw new InvalidOperationException("Ошибка CPU Sync.");
            }

            _imageAvailableSemaphores[i] = imgAvail;
            _inFlightFences[i] = inFlight;
        }

        _renderFinishedSemaphores = new nint[swapchainImageCount];
        _imagesInFlight = new nint[swapchainImageCount];

        for (int i = 0; i < swapchainImageCount; i++)
        {
            nint renderFin;
            if (Vk.CreateSemaphore(_device, &semInfo, null, &renderFin) != 0)
            {
                throw new InvalidOperationException("Ошибка GPU Sync.");
            }

            _renderFinishedSemaphores[i] = renderFin;
            _imagesInFlight[i] = 0;
        }
    }

    public void RecreateSyncObjects(int newImageCount)
    {
        foreach (nint sem in _renderFinishedSemaphores)
        {
            if (sem != 0)
                Vk.DestroySemaphore(_device, sem, null);
        }

        VkSemaphoreCreateInfo semInfo = new()
        {
            sType = Vk.StructureTypeSemaphoreCreateInfo
        };

        _renderFinishedSemaphores = new nint[newImageCount];
        _imagesInFlight = new nint[newImageCount];

        for (int i = 0; i < newImageCount; i++)
        {
            nint renderFin;
            if (Vk.CreateSemaphore(_device, &semInfo, null, &renderFin) != 0)
            {
                throw new InvalidOperationException("Ошибка GPU Sync.");
            }

            _renderFinishedSemaphores[i] = renderFin;
            _imagesInFlight[i] = 0;
        }
    }

    public void Dispose()
    {
        if (_device != 0)
        {
            for (int i = 0; i < RenderConfig.MaxFramesInFlight; i++)
            {
                if (_imageAvailableSemaphores[i] != 0)
                    Vk.DestroySemaphore(_device, _imageAvailableSemaphores[i], null);

                if (_inFlightFences[i] != 0)
                    Vk.DestroyFence(_device, _inFlightFences[i], null);
            }

            foreach (nint sem in _renderFinishedSemaphores)
            {
                if (sem != 0)
                    Vk.DestroySemaphore(_device, sem, null);
            }
            _renderFinishedSemaphores = [];

            if (_commandPool != 0)
            {
                Vk.DestroyCommandPool(_device, _commandPool, null);
                _commandPool = 0;
            }
        }
    }
}
