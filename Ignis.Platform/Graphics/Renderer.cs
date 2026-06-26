using System.Numerics;
using Ignis.Bindings.Vulkan;
using Ignis.Core.Numerics;
using Ignis.Core.Graphics;
using Ignis.Core.Window;
using Ignis.Platform.Windowing;

namespace Ignis.Platform.Graphics;

/// <summary>Представляет двумерный рендерер на базе графического API Vulkan.</summary>
public sealed unsafe partial class Renderer : IDisposable
{
    private VulkanDevice _device = null!;
    private VulkanSwapchain _swapchain = null!;
    private VulkanCommandManager _commands = null!;
    private VulkanAllocator _allocator = null!;
    private ShapeBatcher _batcher = null!;
    private VulkanPipeline _pipeline = null!;

    private nint _surface;
    private uint _imageIndex;
    private int _currentFrame;
    private bool _renderPassActive;

    private Vector2 _cameraPosition;
    private Vector2 _cameraSize = new(100f, 100f);

    private readonly Window _window;
    private VSyncMode _vsync = VSyncMode.On;

    /// <summary>Режим вертикальной синхронизации рендерера.</summary>
    public VSyncMode VSync
    {
        get => _vsync;
        set
        {
            if (_vsync == value) return;

            _vsync = value;

            if (_device.Device != 0)
                Resize(_window.Size);
        }
    }

    /// <summary>Событие, возникающее на ранней фазе кадра рендеринга (например, для очистки состояний или обновления логики перед основным рендером).</summary>
    public event Action? EarlyRenderRequested;

    /// <summary>Основное событие рендеринга кадра.</summary>
    public event Action? RenderRequested;

    /// <summary>Событие, возникающее на поздней фазе кадра рендеринга (например, для отрисовки пользовательского интерфейса поверх игровой сцены).</summary>
    public event Action? LateRenderRequested;

    /// <summary>Максимальное количество кадров в секунду (FPS). Значения меньше или равные нулю отключают ограничение.</summary>
    public int MaxFPS { get; set; }

    /// <summary>Время, прошедшее с момента начала предыдущего кадра, в секундах.</summary>
    public float DeltaTime { get; private set; }

    private long _lastFrameTicks;

    private readonly System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();

    private static partial nint GetVulkanLoader();
    private partial nint CreatePlatformSurface(nint instance);
    partial void DisposePlatformSpecific();

    /// <summary>Создает и инициализирует новый экземпляр класса <see cref="Renderer"/> для указанного окна.</summary>
    /// <param name="window">Окно, в котором будет производиться отрисовка.</param>
    public Renderer(Window window)
    {
        _window = window;
        _window.SizeChanged += OnWindowSizeChanged;
        _window.ResizeEnded += OnWindowResizeEnded;
        InitGraphics();

        _lastFrameTicks = _stopwatch.ElapsedTicks;

        _window.FrameTick += OnFrameTick;
    }

    private void InitGraphics()
    {
        _device = new VulkanDevice(GetVulkanLoader());
        _surface = CreatePlatformSurface(_device.Instance);
        _swapchain = new VulkanSwapchain(_device, _surface, _window.Size, _vsync, _window.InSizeMove);
        _commands = new VulkanCommandManager(_device.Device, _swapchain.Images.Length);
        _allocator = new VulkanAllocator(_device.Device);
        _batcher = new ShapeBatcher(_device, _allocator);
        _pipeline = new VulkanPipeline(_device.Device, _swapchain.ImageFormat, _batcher.DescriptorSetLayout);
    }

    /// <summary>Начинает запись команд для нового кадра.</summary>
    /// <returns>True, если кадр успешно начат и готов к отрисовке, иначе false.</returns>
    public bool BeginFrame()
    {
        _renderPassActive = false;
        nint fence = _commands.GetInFlightFence(_currentFrame);
        Vk.WaitForFences(_device.Device, 1, &fence, 1, ulong.MaxValue);

        uint imageIndex;
        int result = Vk.AcquireNextImageKHR(
            _device.Device, _swapchain.Handle, ulong.MaxValue,
            _commands.GetImageAvailableSemaphore(_currentFrame),
            0, &imageIndex);

        if (result == Vk.ErrorOutOfDateKhr)
        {
            Resize(_window.Size);
            return false;
        }
        else if (result != Vk.Success && result != Vk.SuboptimalKhr)
        {
            return false;
        }

        _imageIndex = imageIndex;

        nint imgFence = _commands.GetImageInFlight((int)_imageIndex);
        if (imgFence != 0)
            Vk.WaitForFences(_device.Device, 1, &imgFence, 1, ulong.MaxValue);

        _commands.GetImageInFlight((int)_imageIndex) = fence;
        Vk.ResetFences(_device.Device, 1, &fence);

        nint cmdBuf = _commands.GetCommandBuffer(_currentFrame);
        Vk.ResetCommandBuffer(cmdBuf, 0);

        VkCommandBufferBeginInfo beginInfo = new()
        {
            sType = Vk.StructureTypeCommandBufferBeginInfo
        };

        Vk.BeginCommandBuffer(cmdBuf, &beginInfo);
        return true;
    }

    /// <summary>Очищает фон кадра указанным цветом.</summary>
    /// <param name="color">Цвет очистки.</param>
    /// <returns>True, если очистка прошла успешно, иначе false.</returns>
    public bool ClearBackground(Color color)
    {
        nint cmdBuf = _commands.GetCommandBuffer(_currentFrame);

        VkImageMemoryBarrier2 barrier = new()
        {
            sType = Vk.StructureTypeImageMemoryBarrier2,
            srcStageMask = Vk.PipelineStage2TopOfPipeBit,
            srcAccessMask = 0,
            dstStageMask = Vk.PipelineStage2ColorAttachmentOutputBit,
            dstAccessMask = Vk.Access2ColorAttachmentWriteBit,
            oldLayout = Vk.ImageLayoutUndefined,
            newLayout = Vk.ImageLayoutColorAttachmentOptimal,
            image = _swapchain.Images[_imageIndex]
        };
        barrier.subresourceRange.aspectMask = Vk.ImageAspectColorBit;
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
            sType = Vk.StructureTypeRenderingAttachmentInfo,
            imageView = _swapchain.ImageViews[_imageIndex],
            imageLayout = Vk.ImageLayoutColorAttachmentOptimal,
            loadOp = Vk.AttachmentLoadOpClear,
            storeOp = Vk.AttachmentStoreOpStore,
            clearValue = clearValue
        };

        VkRenderingInfo renderingInfo = new()
        {
            sType = Vk.StructureTypeRenderingInfo
        };
        renderingInfo.renderArea.extent = _swapchain.Extent;
        renderingInfo.layerCount = 1;
        renderingInfo.colorAttachmentCount = 1;
        renderingInfo.pColorAttachments = &colorAttachment;

        if (!_renderPassActive)
        {
            _batcher.UploadFontIfNeeded(cmdBuf);

            Vk.CmdBeginRendering(cmdBuf, &renderingInfo);
            _renderPassActive = true;
        }

        return true;
    }

    /// <summary>Завершает отрисовку кадра, отправляя буфер команд на выполнение графическому процессору и выводя результат на экран.</summary>
    public void EndFrame()
    {
        nint cmdBuf = _commands.GetCommandBuffer(_currentFrame);

        _batcher.FlushAndDraw(cmdBuf, _currentFrame, _cameraPosition, _cameraSize, _swapchain.Extent,
            _pipeline.Pipeline, _pipeline.PipelineLayout);

        if (_renderPassActive)
        {
            Vk.CmdEndRendering(cmdBuf);
            _renderPassActive = false;
        }

        VkImageMemoryBarrier2 barrier = new()
        {
            sType = Vk.StructureTypeImageMemoryBarrier2,
            srcStageMask = Vk.PipelineStage2ColorAttachmentOutputBit,
            srcAccessMask = Vk.Access2ColorAttachmentWriteBit,
            dstStageMask = Vk.PipelineStage2BottomOfPipeBit,
            dstAccessMask = 0,
            oldLayout = Vk.ImageLayoutColorAttachmentOptimal,
            newLayout = Vk.ImageLayoutPresentSrcKhr,
            image = _swapchain.Images[_imageIndex]
        };
        barrier.subresourceRange.aspectMask = Vk.ImageAspectColorBit;
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

        nint waitSem = _commands.GetImageAvailableSemaphore(_currentFrame);
        nint signalSem = _commands.GetRenderFinishedSemaphore((int)_imageIndex);

        VkCommandBufferSubmitInfo cmdInfo = new()
        {
            sType = Vk.StructureTypeCommandBufferSubmitInfo,
            commandBuffer = cmdBuf
        };

        VkSemaphoreSubmitInfo waitInfo = new()
        {
            sType = Vk.StructureTypeSemaphoreSubmitInfo,
            semaphore = waitSem,
            stageMask = Vk.PipelineStage2ColorAttachmentOutputBit
        };

        VkSemaphoreSubmitInfo signalInfo = new()
        {
            sType = Vk.StructureTypeSemaphoreSubmitInfo,
            semaphore = signalSem,
            stageMask = Vk.PipelineStage2ColorAttachmentOutputBit
        };

        VkSubmitInfo2 submitInfo = new()
        {
            sType = Vk.StructureTypeSubmitInfo2,
            waitSemaphoreInfoCount = 1,
            pWaitSemaphoreInfos = &waitInfo,
            commandBufferInfoCount = 1,
            pCommandBufferInfos = &cmdInfo,
            signalSemaphoreInfoCount = 1,
            pSignalSemaphoreInfos = &signalInfo
        };

        nint fence = _commands.GetInFlightFence(_currentFrame);
        Vk.QueueSubmit2(_device.Queue, 1, &submitInfo, fence);

        nint rawSwapchain = _swapchain.Handle;
        uint imgIdx = _imageIndex;

        VkPresentInfoKHR presentInfo = new()
        {
            sType = Vk.StructureTypePresentInfoKhr,
            waitSemaphoreCount = 1,
            pWaitSemaphores = &signalSem,
            swapchainCount = 1,
            pSwapchains = &rawSwapchain,
            pImageIndices = &imgIdx
        };

        int presentRes = Vk.QueuePresentKHR(_device.Queue, &presentInfo);

        if (presentRes == Vk.ErrorOutOfDateKhr || presentRes == Vk.SuboptimalKhr)
            Resize(_window.Size);

        _currentFrame = (_currentFrame + 1) % RenderConfig.MaxFramesInFlight;
    }

    private void OnFrameTick()
    {
        if (_window.ShouldClose) return;

        if (_window.Size.X <= 0 || _window.Size.Y <= 0)
        {
            Thread.Sleep(1);
            return;
        }

        long frameStartTime = _stopwatch.ElapsedTicks;
        DeltaTime = (float)(frameStartTime - _lastFrameTicks) / System.Diagnostics.Stopwatch.Frequency;
        _lastFrameTicks = frameStartTime;

        foreach (var device in _window.InputDevices)
            device.SnapshotFrame();

        if (BeginFrame())
        {
            EarlyRenderRequested?.Invoke();
            RenderRequested?.Invoke();
            LateRenderRequested?.Invoke();
            EndFrame();
        }

        if (MaxFPS > 0)
        {
            long targetTicks = System.Diagnostics.Stopwatch.Frequency / MaxFPS;
            while (true)
            {
                long elapsed = _stopwatch.ElapsedTicks - frameStartTime;
                long remainingTicks = targetTicks - elapsed;

                if (remainingTicks <= 0) break;

                long remainingMs = remainingTicks * 1000 / System.Diagnostics.Stopwatch.Frequency;
                if (remainingMs > 1)
                    Thread.Sleep((int)(remainingMs - 1));
                else
                    Thread.SpinWait(1);
            }
        }
    }

    private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.Size.X > 0 && e.Size.Y > 0)
            Resize(e.Size);
    }

    private void OnWindowResizeEnded(object? sender, EventArgs e)
    {
    }

    /// <summary>Изменяет размер внутренних ресурсов рендеринга (swapchain и др.) в соответствии с новым размером окна.</summary>
    /// <param name="newSize">Новый размер клиентской области окна.</param>
    public void Resize(Vector2Int newSize)
    {
        if (newSize.X <= 0 || newSize.Y <= 0) return;
        if (_device.Device == 0) return;

        _swapchain.Recreate(newSize, _vsync, _window.InSizeMove);
        _commands.RecreateSyncObjects(_swapchain.Images.Length);
    }

    /// <summary>Освобождает все ресурсы, используемые рендерером.</summary>
    public void Dispose()
    {
        _window.FrameTick -= OnFrameTick;
        _window.SizeChanged -= OnWindowSizeChanged;
        _window.ResizeEnded -= OnWindowResizeEnded;

        if (_device.Device != 0)
        {
            Vk.DeviceWaitIdle(_device.Device);

            _pipeline.Dispose();
            _batcher.Dispose();
            _allocator.Dispose();
            _commands.Dispose();
            _swapchain.Dispose();

            if (_surface != 0)
            {
                Vk.DestroySurfaceKHR(_device.Instance, _surface, null);
                _surface = 0;
            }

            _device.Dispose();
        }

        DisposePlatformSpecific();
    }
}