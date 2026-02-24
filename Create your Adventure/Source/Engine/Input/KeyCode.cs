using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public enum KeyCode
    {
        // ═══ Letters
        A, B, C, D, E, F, G, H, I, J, K, L, M, 
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        // ═══ Numbers
        Number0, Number1, Number2, Number3, Number4,
        Number5, Number6, Number7, Number8, Number9,

        // ═══ Functionskeys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        // ═══ Modifications
        LeftShift, RightShift,
        LeftControl, RightControl,
        LeftAlt, RightAlt,

        // ═══ Navigations
        Up, Down, Left, Right,
        Home, End, PageUp, PageDown,
        Insert, Delete,

        // Special
        Space, Enter, Escape, Tab, Backspace,
        CapsLock, NumLock, ScrollLock,
        PrintScreen, Pause,

        // ═══ Numpad
        Numpad0, Numpad1, Numpad2, Numpad3, Numpad4,
        Numpad5, Numpad6, Numpad7, Numpad8, Numpad9,
        NumpadAdd, NumpadSubstract, NumpadMultiply, NumpadDivide,
        NumpadEnter, NumpadDecimal,

        // ═══ Miscellaneous
        Grave, Minus, Equals, LeftBracket, RightBracket,
        Backslash, Semicolon, Apostrophe, Comma, Period, Slash
    }
}