using Ignis.Bindings.Windows;
using Ignis.Core.Input;

namespace Ignis.Platform.Input;

public sealed unsafe partial class Keyboard
{
    private partial void InitPlatformKeyboard()
    {
        var rid = new User32.RAWINPUTDEVICE
        {
            usUsagePage = 0x01,
            usUsage = 0x06,
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
            if (header->dwType == User32.RIM_TYPEKEYBOARD)
            {
                var kb = (User32.RAWKEYBOARD*)(buffer + sizeof(User32.RAWINPUTHEADER));

                int code = kb->VKey;
                if (code == 255) return;

                if (code == User32.VK_SHIFT) code = kb->MakeCode == 0x36 ? User32.VK_RSHIFT : User32.VK_LSHIFT;
                if (code == User32.VK_CONTROL) code = (kb->Flags & User32.RI_KEY_E0) != 0 ? User32.VK_RCONTROL : User32.VK_LCONTROL;
                if (code == User32.VK_MENU) code = (kb->Flags & User32.RI_KEY_E0) != 0 ? User32.VK_RMENU : User32.VK_LMENU;

                if (code is >= 0 and < MaxKeys)
                {
                    bool isUp = (kb->Flags & User32.RI_KEY_BREAK) != 0;
                    bool wasDown;

                    wasDown = _rawCurrentStates[code] == 1;
                    _rawCurrentStates[code] = isUp ? (byte)0 : (byte)1;

                    if (isUp)
                    {
                        if (wasDown)
                        {
                            KeyReleased?.Invoke(new Key(code));
                        }
                    }
                    else
                    {
                        if (!wasDown)
                        {
                            KeyPressed?.Invoke(new Key(code));
                        }
                        else
                        {
                            KeyDown?.Invoke(new Key(code));
                        }
                    }
                }
            }
        }
    }

    private partial void DisposePlatform()
        => _window.RawInputReceived -= ProcessRawInput;
}