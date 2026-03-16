using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Defines all supported keyboard key codes.
    /// Platform-independent enumeration covering letters, numbers, function keys, modifiers, navigation, and special keys.
    /// </summary>
    public enum KeyCode
    {
        // ═══ Letters A-Z
        A, B, C, D, E, F, G, H, I, J, K, L, M, 
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        // ═══ Number row 0-9
        Number0, Number1, Number2, Number3, Number4,
        Number5, Number6, Number7, Number8, Number9,

        // ═══ Function keys F1-F12
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        // ═══ Modifier keys (Shift, Control, Alt)
        LeftShift, RightShift,
        LeftControl, RightControl,
        LeftAlt, RightAlt,

        // ═══ Navigation keys (arrows, home, end, page up/down)
        Up, Down, Left, Right,
        Home, End, PageUp, PageDown,
        Insert, Delete,

        // Special keys
        Space, Enter, Escape, Tab, Backspace,
        CapsLock, NumLock, ScrollLock,
        PrintScreen, Pause,

        // ═══ Numpad keys (0-9 and operators)
        Numpad0, Numpad1, Numpad2, Numpad3, Numpad4,
        Numpad5, Numpad6, Numpad7, Numpad8, Numpad9,
        NumpadAdd, NumpadSubtract, NumpadMultiply, NumpadDivide,
        NumpadEnter, NumpadDecimal,

        // ═══ Symbol/punctuation keys
        Grave, Minus, Equals, LeftBracket, RightBracket,
        Backslash, Semicolon, Apostrophe, Comma, Period, Slash
    }
}