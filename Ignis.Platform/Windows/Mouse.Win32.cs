using System.Numerics;

using Ignis.Bindings.Windows;
using Ignis.Core.Input;

namespace Ignis.Platform.Input;

public sealed unsafe partial class Mouse
{
    private float _wheelDeltaAccumulator;

    private partial void InitPlatformMouse()
    {
        var rid = new User32.RAWINPUTDEVICE
        {
            usUsagePage = 0x01,
            usUsage = 0x02,
            dwFlags = 0,
            hwndTarget = _window.Handle
        };
        User32.RegisterRawInputDevices(in rid, 1, (uint)sizeof(User32.RAWINPUTDEVICE));
        _window.RawInputReceived += ProcessRawInput;
    }

    private void ProcessRawInput(nint lParam)
    {
        uint size = 0;
        User32.GetRawInputData(lParam, User32.RID_INPUT, 0, ref size, (uint)sizeof(User32.RAWINPUTHEADER));
        if (size == 0) return;

        byte* buffer = stackalloc byte[(int)size];
        if (User32.GetRawInputData(lParam, User32.RID_INPUT, (nint)buffer, ref size, (uint)sizeof(User32.RAWINPUTHEADER)) == size)
        {
            var header = (User32.RAWINPUTHEADER*)buffer;
            if (header->dwType == User32.RIM_TYPEMOUSE)
            {
                var mouse = (User32.RAWMOUSE*)(buffer + sizeof(User32.RAWINPUTHEADER));
                ushort flags = mouse->usButtonFlags;

                if ((flags & User32.RI_MOUSE_LEFT_BUTTON_DOWN) != 0) _currentStates[MouseButton.Left.Code] = 1;
                if ((flags & User32.RI_MOUSE_LEFT_BUTTON_UP) != 0) _currentStates[MouseButton.Left.Code] = 0;

                if ((flags & User32.RI_MOUSE_RIGHT_BUTTON_DOWN) != 0) _currentStates[MouseButton.Right.Code] = 1;
                if ((flags & User32.RI_MOUSE_RIGHT_BUTTON_UP) != 0) _currentStates[MouseButton.Right.Code] = 0;

                if ((flags & User32.RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0) _currentStates[MouseButton.Middle.Code] = 1;
                if ((flags & User32.RI_MOUSE_MIDDLE_BUTTON_UP) != 0) _currentStates[MouseButton.Middle.Code] = 0;

                if ((flags & User32.RI_MOUSE_BUTTON_4_DOWN) != 0) _currentStates[MouseButton.XButton1.Code] = 1;
                if ((flags & User32.RI_MOUSE_BUTTON_4_UP) != 0) _currentStates[MouseButton.XButton1.Code] = 0;

                if ((flags & User32.RI_MOUSE_BUTTON_5_DOWN) != 0) _currentStates[MouseButton.XButton2.Code] = 1;
                if ((flags & User32.RI_MOUSE_BUTTON_5_UP) != 0) _currentStates[MouseButton.XButton2.Code] = 0;

                if ((flags & User32.RI_MOUSE_WHEEL) != 0)
                {
                    short wheelDelta = (short)mouse->usButtonData;
                    _wheelDeltaAccumulator += wheelDelta / 120f;
                }
            }
        }
    }

    private partial void UpdatePlatform()
    {
        MouseWheelDeltaY = _wheelDeltaAccumulator;
        _wheelDeltaAccumulator = 0;

        if (User32.GetCursorPos(out var pt))
        {
            User32.ScreenToClient(_window.Handle, ref pt);

            float w = _window.Size.Width;
            float h = _window.Size.Height;

            if (w > 0 && h > 0)
            {
                float nx = (pt.X / w) * 2f - 1f;
                float ny = -((pt.Y / h) * 2f - 1f);
                MousePosition = new Vector2(nx, ny);
            }
        }
    }

    private partial void DisposePlatform() => _window.RawInputReceived -= ProcessRawInput;
}