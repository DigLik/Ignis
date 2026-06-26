namespace Ignis.Core.Input;

/// <summary>Клавиша клавиатуры, идентифицируемая виртуальным кодом клавиши.</summary>
/// <param name="code">Виртуальный код клавиши.</param>
public readonly struct Key(int code) : IEquatable<Key>
{
    /// <summary>Виртуальный код клавиши.</summary>
    public int Code { get; } = code;

    #region Letters
    /// <summary>Клавиша A.</summary>
    public static readonly Key A = new(65);
    /// <summary>Клавиша B.</summary>
    public static readonly Key B = new(66);
    /// <summary>Клавиша C.</summary>
    public static readonly Key C = new(67);
    /// <summary>Клавиша D.</summary>
    public static readonly Key D = new(68);
    /// <summary>Клавиша E.</summary>
    public static readonly Key E = new(69);
    /// <summary>Клавиша F.</summary>
    public static readonly Key F = new(70);
    /// <summary>Клавиша G.</summary>
    public static readonly Key G = new(71);
    /// <summary>Клавиша H.</summary>
    public static readonly Key H = new(72);
    /// <summary>Клавиша I.</summary>
    public static readonly Key I = new(73);
    /// <summary>Клавиша J.</summary>
    public static readonly Key J = new(74);
    /// <summary>Клавиша K.</summary>
    public static readonly Key K = new(75);
    /// <summary>Клавиша L.</summary>
    public static readonly Key L = new(76);
    /// <summary>Клавиша M.</summary>
    public static readonly Key M = new(77);
    /// <summary>Клавиша N.</summary>
    public static readonly Key N = new(78);
    /// <summary>Клавиша O.</summary>
    public static readonly Key O = new(79);
    /// <summary>Клавиша P.</summary>
    public static readonly Key P = new(80);
    /// <summary>Клавиша Q.</summary>
    public static readonly Key Q = new(81);
    /// <summary>Клавиша R.</summary>
    public static readonly Key R = new(82);
    /// <summary>Клавиша S.</summary>
    public static readonly Key S = new(83);
    /// <summary>Клавиша T.</summary>
    public static readonly Key T = new(84);
    /// <summary>Клавиша U.</summary>
    public static readonly Key U = new(85);
    /// <summary>Клавиша V.</summary>
    public static readonly Key V = new(86);
    /// <summary>Клавиша W.</summary>
    public static readonly Key W = new(87);
    /// <summary>Клавиша X.</summary>
    public static readonly Key X = new(88);
    /// <summary>Клавиша Y.</summary>
    public static readonly Key Y = new(89);
    /// <summary>Клавиша Z.</summary>
    public static readonly Key Z = new(90);
    #endregion

    #region Digits
    /// <summary>Клавиша D0.</summary>
    public static readonly Key D0 = new(48);
    /// <summary>Клавиша D1.</summary>
    public static readonly Key D1 = new(49);
    /// <summary>Клавиша D2.</summary>
    public static readonly Key D2 = new(50);
    /// <summary>Клавиша D3.</summary>
    public static readonly Key D3 = new(51);
    /// <summary>Клавиша D4.</summary>
    public static readonly Key D4 = new(52);
    /// <summary>Клавиша D5.</summary>
    public static readonly Key D5 = new(53);
    /// <summary>Клавиша D6.</summary>
    public static readonly Key D6 = new(54);
    /// <summary>Клавиша D7.</summary>
    public static readonly Key D7 = new(55);
    /// <summary>Клавиша D8.</summary>
    public static readonly Key D8 = new(56);
    /// <summary>Клавиша D9.</summary>
    public static readonly Key D9 = new(57);
    #endregion

    #region Function keys
    /// <summary>Клавиша F1.</summary>
    public static readonly Key F1 = new(112);
    /// <summary>Клавиша F2.</summary>
    public static readonly Key F2 = new(113);
    /// <summary>Клавиша F3.</summary>
    public static readonly Key F3 = new(114);
    /// <summary>Клавиша F4.</summary>
    public static readonly Key F4 = new(115);
    /// <summary>Клавиша F5.</summary>
    public static readonly Key F5 = new(116);
    /// <summary>Клавиша F6.</summary>
    public static readonly Key F6 = new(117);
    /// <summary>Клавиша F7.</summary>
    public static readonly Key F7 = new(118);
    /// <summary>Клавиша F8.</summary>
    public static readonly Key F8 = new(119);
    /// <summary>Клавиша F9.</summary>
    public static readonly Key F9 = new(120);
    /// <summary>Клавиша F10.</summary>
    public static readonly Key F10 = new(121);
    /// <summary>Клавиша F11.</summary>
    public static readonly Key F11 = new(122);
    /// <summary>Клавиша F12.</summary>
    public static readonly Key F12 = new(123);
    /// <summary>Клавиша F13.</summary>
    public static readonly Key F13 = new(124);
    /// <summary>Клавиша F14.</summary>
    public static readonly Key F14 = new(125);
    /// <summary>Клавиша F15.</summary>
    public static readonly Key F15 = new(126);
    /// <summary>Клавиша F16.</summary>
    public static readonly Key F16 = new(127);
    /// <summary>Клавиша F17.</summary>
    public static readonly Key F17 = new(128);
    /// <summary>Клавиша F18.</summary>
    public static readonly Key F18 = new(129);
    /// <summary>Клавиша F19.</summary>
    public static readonly Key F19 = new(130);
    /// <summary>Клавиша F20.</summary>
    public static readonly Key F20 = new(131);
    /// <summary>Клавиша F21.</summary>
    public static readonly Key F21 = new(132);
    /// <summary>Клавиша F22.</summary>
    public static readonly Key F22 = new(133);
    /// <summary>Клавиша F23.</summary>
    public static readonly Key F23 = new(134);
    /// <summary>Клавиша F24.</summary>
    public static readonly Key F24 = new(135);
    #endregion

    #region Numpad
    /// <summary>Клавиша Numpad0.</summary>
    public static readonly Key Numpad0 = new(96);
    /// <summary>Клавиша Numpad1.</summary>
    public static readonly Key Numpad1 = new(97);
    /// <summary>Клавиша Numpad2.</summary>
    public static readonly Key Numpad2 = new(98);
    /// <summary>Клавиша Numpad3.</summary>
    public static readonly Key Numpad3 = new(99);
    /// <summary>Клавиша Numpad4.</summary>
    public static readonly Key Numpad4 = new(100);
    /// <summary>Клавиша Numpad5.</summary>
    public static readonly Key Numpad5 = new(101);
    /// <summary>Клавиша Numpad6.</summary>
    public static readonly Key Numpad6 = new(102);
    /// <summary>Клавиша Numpad7.</summary>
    public static readonly Key Numpad7 = new(103);
    /// <summary>Клавиша Numpad8.</summary>
    public static readonly Key Numpad8 = new(104);
    /// <summary>Клавиша Numpad9.</summary>
    public static readonly Key Numpad9 = new(105);
    /// <summary>Клавиша NumpadMultiply.</summary>
    public static readonly Key NumpadMultiply = new(106);
    /// <summary>Клавиша NumpadAdd.</summary>
    public static readonly Key NumpadAdd = new(107);
    /// <summary>Клавиша NumpadSeparator.</summary>
    public static readonly Key NumpadSeparator = new(108);
    /// <summary>Клавиша NumpadSubtract.</summary>
    public static readonly Key NumpadSubtract = new(109);
    /// <summary>Клавиша NumpadDecimal.</summary>
    public static readonly Key NumpadDecimal = new(110);
    /// <summary>Клавиша NumpadDivide.</summary>
    public static readonly Key NumpadDivide = new(111);
    /// <summary>Клавиша NumLock.</summary>
    public static readonly Key NumLock = new(144);
    #endregion

    #region Control keys
    /// <summary>Клавиша Backspace.</summary>
    public static readonly Key Backspace = new(8);
    /// <summary>Клавиша Tab.</summary>
    public static readonly Key Tab = new(9);
    /// <summary>Клавиша Enter.</summary>
    public static readonly Key Enter = new(13);
    /// <summary>Клавиша Shift.</summary>
    public static readonly Key Shift = new(16);
    /// <summary>Клавиша Control.</summary>
    public static readonly Key Control = new(17);
    /// <summary>Клавиша Alt.</summary>
    public static readonly Key Alt = new(18);
    /// <summary>Клавиша Pause.</summary>
    public static readonly Key Pause = new(19);
    /// <summary>Клавиша CapsLock.</summary>
    public static readonly Key CapsLock = new(20);
    /// <summary>Клавиша Escape.</summary>
    public static readonly Key Escape = new(27);
    /// <summary>Клавиша Space.</summary>
    public static readonly Key Space = new(32);
    /// <summary>Клавиша PrintScreen.</summary>
    public static readonly Key PrintScreen = new(44);
    /// <summary>Клавиша Insert.</summary>
    public static readonly Key Insert = new(45);
    /// <summary>Клавиша Delete.</summary>
    public static readonly Key Delete = new(46);
    /// <summary>Клавиша ScrollLock.</summary>
    public static readonly Key ScrollLock = new(145);
    /// <summary>Клавиша ContextMenu.</summary>
    public static readonly Key ContextMenu = new(93);
    #endregion

    #region Navigation
    /// <summary>Клавиша PageUp.</summary>
    public static readonly Key PageUp = new(33);
    /// <summary>Клавиша PageDown.</summary>
    public static readonly Key PageDown = new(34);
    /// <summary>Клавиша End.</summary>
    public static readonly Key End = new(35);
    /// <summary>Клавиша Home.</summary>
    public static readonly Key Home = new(36);
    /// <summary>Клавиша Left.</summary>
    public static readonly Key Left = new(37);
    /// <summary>Клавиша Up.</summary>
    public static readonly Key Up = new(38);
    /// <summary>Клавиша Right.</summary>
    public static readonly Key Right = new(39);
    /// <summary>Клавиша Down.</summary>
    public static readonly Key Down = new(40);
    #endregion

    #region System
    /// <summary>Клавиша LeftWindows.</summary>
    public static readonly Key LeftWindows = new(91);
    /// <summary>Клавиша RightWindows.</summary>
    public static readonly Key RightWindows = new(92);
    /// <summary>Клавиша LeftShift.</summary>
    public static readonly Key LeftShift = new(160);
    /// <summary>Клавиша RightShift.</summary>
    public static readonly Key RightShift = new(161);
    /// <summary>Клавиша LeftControl.</summary>
    public static readonly Key LeftControl = new(162);
    /// <summary>Клавиша RightControl.</summary>
    public static readonly Key RightControl = new(163);
    /// <summary>Клавиша LeftAlt.</summary>
    public static readonly Key LeftAlt = new(164);
    /// <summary>Клавиша RightAlt.</summary>
    public static readonly Key RightAlt = new(165);
    #endregion

    #region Punctuation & symbols (US layout)
    /// <summary>Клавиша Semicolon.</summary>
    public static readonly Key Semicolon = new(186);    // ;
    /// <summary>Клавиша Equal.</summary>
    public static readonly Key Equal = new(187);        // =
    /// <summary>Клавиша Comma.</summary>
    public static readonly Key Comma = new(188);        // ,
    /// <summary>Клавиша Minus.</summary>
    public static readonly Key Minus = new(189);        // -
    /// <summary>Клавиша Period.</summary>
    public static readonly Key Period = new(190);       // .
    /// <summary>Клавиша Slash.</summary>
    public static readonly Key Slash = new(191);        // /
    /// <summary>Клавиша Backtick.</summary>
    public static readonly Key Backtick = new(192);     // `
    /// <summary>Клавиша LeftBracket.</summary>
    public static readonly Key LeftBracket = new(219);  // [
    /// <summary>Клавиша Backslash.</summary>
    public static readonly Key Backslash = new(220);    // \
    /// <summary>Клавиша RightBracket.</summary>
    public static readonly Key RightBracket = new(221); // ]
    /// <summary>Клавиша Quote.</summary>
    public static readonly Key Quote = new(222);        // '
    #endregion

    /// <summary>Проверяет равенство двух клавиш.</summary>
    /// <param name="left">Первая клавиша.</param>
    /// <param name="right">Вторая клавиша.</param>
    /// <returns>True, если клавиши равны, иначе false.</returns>
    public static bool operator ==(Key left, Key right) => left.Code == right.Code;

    /// <summary>Проверяет неравенство двух клавиш.</summary>
    /// <param name="left">Первая клавиша.</param>
    /// <param name="right">Вторая клавиша.</param>
    /// <returns>True, если клавиши не равны, иначе false.</returns>
    public static bool operator !=(Key left, Key right) => left.Code != right.Code;

    /// <summary>Проверяет равенство текущей клавиши с другой.</summary>
    /// <param name="other">Другая клавиша для сравнения.</param>
    /// <returns>True, если клавиши равны, иначе false.</returns>
    public bool Equals(Key other) => Code == other.Code;

    /// <summary>Проверяет равенство текущей клавиши с объектом.</summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>True, если объект является клавишей Key и равен текущей клавише, иначе false.</returns>
    public override bool Equals(object? obj) => obj is Key key && Equals(key);

    /// <summary>Возвращает хэш-код текущей клавиши.</summary>
    /// <returns>Хэш-код клавиши.</returns>
    public override int GetHashCode() => Code;

    /// <summary>Возвращает строковое представление клавиши.</summary>
    /// <returns>Строковое представление клавиши.</returns>
    public override string ToString() => $"{nameof(Key)}:{Code}";

    /// <summary>Явное преобразование клавиши в её целочисленный код.</summary>
    /// <param name="key">Клавиша.</param>
    public static explicit operator int(Key key) => key.Code;

    /// <summary>Явное преобразование целочисленного кода в клавишу.</summary>
    /// <param name="code">Целочисленный код.</param>
    public static explicit operator Key(int code) => new(code);

    /// <summary>Преобразует клавишу в целочисленный код.</summary>
    /// <returns>Целочисленный код клавиши.</returns>
    public int ToInt32() => Code;

    /// <summary>Создает клавишу из её целочисленного кода.</summary>
    /// <param name="code">Целочисленный код клавиши.</param>
    /// <returns>Экземпляр клавиши.</returns>
    public static Key FromInt32(int code) => new(code);
}