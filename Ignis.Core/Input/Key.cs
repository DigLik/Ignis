namespace Ignis.Core.Input;

/// <summary>Клавиша клавиатуры, идентифицируемая виртуальным кодом клавиши.</summary>
public readonly struct Key(int code) : IEquatable<Key>
{
    /// <summary>Виртуальный код клавиши.</summary>
    public int Code { get; } = code;

    #region Letters
    public static readonly Key A = new(65);
    public static readonly Key B = new(66);
    public static readonly Key C = new(67);
    public static readonly Key D = new(68);
    public static readonly Key E = new(69);
    public static readonly Key F = new(70);
    public static readonly Key G = new(71);
    public static readonly Key H = new(72);
    public static readonly Key I = new(73);
    public static readonly Key J = new(74);
    public static readonly Key K = new(75);
    public static readonly Key L = new(76);
    public static readonly Key M = new(77);
    public static readonly Key N = new(78);
    public static readonly Key O = new(79);
    public static readonly Key P = new(80);
    public static readonly Key Q = new(81);
    public static readonly Key R = new(82);
    public static readonly Key S = new(83);
    public static readonly Key T = new(84);
    public static readonly Key U = new(85);
    public static readonly Key V = new(86);
    public static readonly Key W = new(87);
    public static readonly Key X = new(88);
    public static readonly Key Y = new(89);
    public static readonly Key Z = new(90);
    #endregion

    #region Digits
    public static readonly Key D0 = new(48);
    public static readonly Key D1 = new(49);
    public static readonly Key D2 = new(50);
    public static readonly Key D3 = new(51);
    public static readonly Key D4 = new(52);
    public static readonly Key D5 = new(53);
    public static readonly Key D6 = new(54);
    public static readonly Key D7 = new(55);
    public static readonly Key D8 = new(56);
    public static readonly Key D9 = new(57);
    #endregion

    #region Function keys
    public static readonly Key F1 = new(112);
    public static readonly Key F2 = new(113);
    public static readonly Key F3 = new(114);
    public static readonly Key F4 = new(115);
    public static readonly Key F5 = new(116);
    public static readonly Key F6 = new(117);
    public static readonly Key F7 = new(118);
    public static readonly Key F8 = new(119);
    public static readonly Key F9 = new(120);
    public static readonly Key F10 = new(121);
    public static readonly Key F11 = new(122);
    public static readonly Key F12 = new(123);
    public static readonly Key F13 = new(124);
    public static readonly Key F14 = new(125);
    public static readonly Key F15 = new(126);
    public static readonly Key F16 = new(127);
    public static readonly Key F17 = new(128);
    public static readonly Key F18 = new(129);
    public static readonly Key F19 = new(130);
    public static readonly Key F20 = new(131);
    public static readonly Key F21 = new(132);
    public static readonly Key F22 = new(133);
    public static readonly Key F23 = new(134);
    public static readonly Key F24 = new(135);
    #endregion

    #region Numpad
    public static readonly Key Numpad0 = new(96);
    public static readonly Key Numpad1 = new(97);
    public static readonly Key Numpad2 = new(98);
    public static readonly Key Numpad3 = new(99);
    public static readonly Key Numpad4 = new(100);
    public static readonly Key Numpad5 = new(101);
    public static readonly Key Numpad6 = new(102);
    public static readonly Key Numpad7 = new(103);
    public static readonly Key Numpad8 = new(104);
    public static readonly Key Numpad9 = new(105);
    public static readonly Key NumpadMultiply = new(106);
    public static readonly Key NumpadAdd = new(107);
    public static readonly Key NumpadSeparator = new(108);
    public static readonly Key NumpadSubtract = new(109);
    public static readonly Key NumpadDecimal = new(110);
    public static readonly Key NumpadDivide = new(111);
    public static readonly Key NumLock = new(144);
    #endregion

    #region Control keys
    public static readonly Key Backspace = new(8);
    public static readonly Key Tab = new(9);
    public static readonly Key Enter = new(13);
    public static readonly Key Shift = new(16);
    public static readonly Key Control = new(17);
    public static readonly Key Alt = new(18);
    public static readonly Key Pause = new(19);
    public static readonly Key CapsLock = new(20);
    public static readonly Key Escape = new(27);
    public static readonly Key Space = new(32);
    public static readonly Key PrintScreen = new(44);
    public static readonly Key Insert = new(45);
    public static readonly Key Delete = new(46);
    public static readonly Key ScrollLock = new(145);
    public static readonly Key ContextMenu = new(93);
    #endregion

    #region Navigation
    public static readonly Key PageUp = new(33);
    public static readonly Key PageDown = new(34);
    public static readonly Key End = new(35);
    public static readonly Key Home = new(36);
    public static readonly Key Left = new(37);
    public static readonly Key Up = new(38);
    public static readonly Key Right = new(39);
    public static readonly Key Down = new(40);
    #endregion

    #region System
    public static readonly Key LeftWindows = new(91);
    public static readonly Key RightWindows = new(92);
    public static readonly Key LeftShift = new(160);
    public static readonly Key RightShift = new(161);
    public static readonly Key LeftControl = new(162);
    public static readonly Key RightControl = new(163);
    public static readonly Key LeftAlt = new(164);
    public static readonly Key RightAlt = new(165);
    #endregion

    #region Punctuation & symbols (US layout)
    public static readonly Key Semicolon = new(186);    // ;
    public static readonly Key Equal = new(187);        // =
    public static readonly Key Comma = new(188);        // ,
    public static readonly Key Minus = new(189);        // -
    public static readonly Key Period = new(190);       // .
    public static readonly Key Slash = new(191);        // /
    public static readonly Key Backtick = new(192);     // `
    public static readonly Key LeftBracket = new(219);  // [
    public static readonly Key Backslash = new(220);    // \
    public static readonly Key RightBracket = new(221); // ]
    public static readonly Key Quote = new(222);        // '
    #endregion

    public static bool operator ==(Key left, Key right) => left.Code == right.Code;
    public static bool operator !=(Key left, Key right) => left.Code != right.Code;

    public bool Equals(Key other) => Code == other.Code;
    public override bool Equals(object? obj) => obj is Key key && Equals(key);
    public override int GetHashCode() => Code;
    public override string ToString() => $"{nameof(Key)}:{Code}";

    public static explicit operator int(Key key) => key.Code;
    public static explicit operator Key(int code) => new(code);

    public int ToInt32() => Code;
    public static Key FromInt32(int code) => new(code);
}