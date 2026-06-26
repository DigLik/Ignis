using System.Diagnostics;
using System.Numerics;

using Ignis.Bindings.Windows;
using Ignis.Core.Input;

namespace Ignis.Platform.Input;

public sealed unsafe partial class Mouse
{
    private readonly Stopwatch _moveStopwatch = Stopwatch.StartNew();

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

    private void ProcessButton(ushort flags, ushort downFlag, ushort upFlag, MouseButton button)
    {
        int code = button.Code;
        bool triggerPressed = false;
        bool triggerDown = false;
        bool triggerReleased = false;

        if ((flags & downFlag) != 0)
        {
            bool wasDown = _rawCurrentStates[code] == 1;
            _rawCurrentStates[code] = 1;
            if (!wasDown) triggerPressed = true;
            else triggerDown = true;
        }
        if ((flags & upFlag) != 0)
        {
            bool wasDown = _rawCurrentStates[code] == 1;
            _rawCurrentStates[code] = 0;
            if (wasDown) triggerReleased = true;
        }

        if (triggerPressed) ButtonPressed?.Invoke(button);
        if (triggerDown) ButtonDown?.Invoke(button);
        if (triggerReleased) ButtonReleased?.Invoke(button);
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

                ProcessButton(flags, User32.RI_MOUSE_LEFT_BUTTON_DOWN, User32.RI_MOUSE_LEFT_BUTTON_UP, MouseButton.Left);
                ProcessButton(flags, User32.RI_MOUSE_RIGHT_BUTTON_DOWN, User32.RI_MOUSE_RIGHT_BUTTON_UP, MouseButton.Right);
                ProcessButton(flags, User32.RI_MOUSE_MIDDLE_BUTTON_DOWN, User32.RI_MOUSE_MIDDLE_BUTTON_UP, MouseButton.Middle);
                ProcessButton(flags, User32.RI_MOUSE_BUTTON_4_DOWN, User32.RI_MOUSE_BUTTON_4_UP, MouseButton.XButton1);
                ProcessButton(flags, User32.RI_MOUSE_BUTTON_5_DOWN, User32.RI_MOUSE_BUTTON_5_UP, MouseButton.XButton2);

                if ((flags & User32.RI_MOUSE_WHEEL) != 0)
                {
                    short wheelDelta = (short)mouse->usButtonData;
                    float delta = wheelDelta / 120f;
                    _rawWheelDeltaY += delta;
                    WheelScrolled?.Invoke(delta);
                }

                if (User32.GetCursorPos(out var pt))
                {
                    User32.ScreenToClient(_window.Handle, ref pt);

                    float w = _window.Size.X;
                    float h = _window.Size.Y;

                    if (w > 0 && h > 0)
                    {
                        float nx = (pt.X / w) * 2f - 1f;
                        float ny = -((pt.Y / h) * 2f - 1f);
                        var newPos = new Vector2(nx, ny);

                        bool positionChanged = false;
                        if (_rawMousePosition != newPos)
                        {
                            _rawMousePosition = newPos;
                            positionChanged = true;
                        }

                        if (positionChanged)
                        {
                            double elapsedMs = _moveStopwatch.Elapsed.TotalMilliseconds;
                            double minIntervalMs = 1000.0 / MotionPollRate;
                            if (elapsedMs >= minIntervalMs)
                            {
                                _moveStopwatch.Restart();
                                Moved?.Invoke(newPos);
                            }
                        }
                    }
                }
            }
        }
    }

    private partial void DisposePlatform() => _window.RawInputReceived -= ProcessRawInput;
}