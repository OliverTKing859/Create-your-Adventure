using Silk.NET.Input;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Provides conversion utilities to translate Silk.NET input types to engine-specific input types.
    /// Separates the engine from direct Silk.NET dependencies for better abstraction and portability.
    /// </summary>
    public static class InputConverter
    {
        /// <summary>
        /// Converts Silk.NET keyboard keys to engine KeyCode enum.
        /// Handles all standard keys including letters, numbers, function keys, modifiers, and special keys.
        /// </summary>
        public static class KeyConverter
        {
            /// <summary>
            /// Converts a Silk.NET Key to an engine KeyCode.
            /// Returns null if the key is not mapped (unknown or unsupported key).
            /// </summary>
            /// <param name="key">The Silk.NET key to convert.</param>
            /// <returns>The corresponding KeyCode, or null if no mapping exists.</returns>
            public static KeyCode? Convert(Key key) => key switch
            {
                // ═══ Letters A-Z
                Key.A => KeyCode.A,
                Key.B => KeyCode.B,
                Key.C => KeyCode.C,
                Key.D => KeyCode.D,
                Key.E => KeyCode.E,
                Key.F => KeyCode.F,
                Key.G => KeyCode.G,
                Key.H => KeyCode.H,
                Key.I => KeyCode.I,
                Key.J => KeyCode.J,
                Key.K => KeyCode.K,
                Key.L => KeyCode.L,
                Key.M => KeyCode.M,
                Key.N => KeyCode.N,
                Key.O => KeyCode.O,
                Key.P => KeyCode.P,
                Key.Q => KeyCode.Q,
                Key.R => KeyCode.R,
                Key.S => KeyCode.S,
                Key.T => KeyCode.T,
                Key.U => KeyCode.U,
                Key.V => KeyCode.V,
                Key.W => KeyCode.W,
                Key.X => KeyCode.X,
                Key.Y => KeyCode.Y,
                Key.Z => KeyCode.Z,

                // ═══ Number row 0-9
                Key.Number0 => KeyCode.Number0,
                Key.Number1 => KeyCode.Number1,
                Key.Number2 => KeyCode.Number2,
                Key.Number3 => KeyCode.Number3,
                Key.Number4 => KeyCode.Number4,
                Key.Number5 => KeyCode.Number5,
                Key.Number6 => KeyCode.Number6,
                Key.Number7 => KeyCode.Number7,
                Key.Number8 => KeyCode.Number8,
                Key.Number9 => KeyCode.Number9,

                // ═══ Modifier keys
                Key.ShiftLeft => KeyCode.LeftShift,
                Key.ShiftRight => KeyCode.RightShift,
                Key.ControlLeft => KeyCode.LeftControl,
                Key.ControlRight => KeyCode.RightControl,
                Key.AltLeft => KeyCode.LeftAlt,
                Key.AltRight => KeyCode.RightAlt,

                // ═══ Navigation keys
                Key.Up => KeyCode.Up,
                Key.Down => KeyCode.Down,
                Key.Left => KeyCode.Left,
                Key.Right => KeyCode.Right,
                Key.Home => KeyCode.Home,
                Key.End => KeyCode.End,
                Key.PageUp => KeyCode.PageUp,
                Key.PageDown => KeyCode.PageDown,
                Key.Insert => KeyCode.Insert,
                Key.Delete => KeyCode.Delete,

                // ═══ Common special keys
                Key.Space => KeyCode.Space,
                Key.Enter => KeyCode.Enter,
                Key.Escape => KeyCode.Escape,
                Key.Tab => KeyCode.Tab,
                Key.Backspace => KeyCode.Backspace,
                Key.CapsLock => KeyCode.CapsLock,
                Key.NumLock => KeyCode.NumLock,
                Key.ScrollLock => KeyCode.ScrollLock,
                Key.PrintScreen => KeyCode.PrintScreen,
                Key.Pause => KeyCode.Pause,

                // ═══ Numpad keys
                Key.Keypad0 => KeyCode.Numpad0,
                Key.Keypad1 => KeyCode.Numpad1,
                Key.Keypad2 => KeyCode.Numpad2,
                Key.Keypad3 => KeyCode.Numpad3,
                Key.Keypad4 => KeyCode.Numpad4,
                Key.Keypad5 => KeyCode.Numpad5,
                Key.Keypad6 => KeyCode.Numpad6,
                Key.Keypad7 => KeyCode.Numpad7,
                Key.Keypad8 => KeyCode.Numpad8,
                Key.Keypad9 => KeyCode.Numpad9,
                Key.KeypadAdd => KeyCode.NumpadAdd,
                Key.KeypadSubtract => KeyCode.NumpadSubtract,
                Key.KeypadMultiply => KeyCode.NumpadMultiply,
                Key.KeypadDivide => KeyCode.NumpadDivide,
                Key.KeypadEnter => KeyCode.NumpadEnter,
                Key.KeypadDecimal => KeyCode.NumpadDecimal,

                // ═══ Symbol keys
                Key.GraveAccent => KeyCode.Grave,
                Key.Minus => KeyCode.Minus,
                Key.Equal => KeyCode.Equals,
                Key.LeftBracket => KeyCode.LeftBracket,
                Key.RightBracket => KeyCode.RightBracket,
                Key.BackSlash => KeyCode.Backslash,
                Key.Semicolon => KeyCode.Semicolon,
                Key.Apostrophe => KeyCode.Apostrophe,
                Key.Comma => KeyCode.Comma,
                Key.Period => KeyCode.Period,
                Key.Slash => KeyCode.Slash,

                _ => null // ═══ Unknown or unsupported key
            };
        }

        /// <summary>
        /// Converts Silk.NET mouse buttons to engine MouseButton enum.
        /// Supports standard buttons (left, right, middle) and additional side buttons.
        /// </summary>
        public static class MouseConverter
        {
            /// <summary>
            /// Converts a Silk.NET MouseButton to an engine MouseButton.
            /// Returns null if the button is not mapped.
            /// </summary>
            /// <param name="button">The Silk.NET mouse button to convert.</param>
            /// <returns>The corresponding MouseButton, or null if no mapping exists.</returns>
            public static MouseButton? Convert(Silk.NET.Input.MouseButton button) => button switch
            {
                Silk.NET.Input.MouseButton.Left => MouseButton.Left,
                Silk.NET.Input.MouseButton.Right => MouseButton.Right,
                Silk.NET.Input.MouseButton.Middle => MouseButton.Middle,
                Silk.NET.Input.MouseButton.Button4 => MouseButton.Button4, // ═══ Side button (back)
                Silk.NET.Input.MouseButton.Button5 => MouseButton.Button5, // ═══ Side button (forward)
                _ => null
            };
        }

        /// <summary>
        /// Converts Silk.NET gamepad button names to engine GamepadButton enum.
        /// Maps Xbox-style controller buttons (A/B/X/Y, bumpers, triggers, d-pad, start/back).
        /// </summary>
        public static class GamepadConverter
        {
            /// <summary>
            /// Converts a Silk.NET ButtonName to an engine GamepadButton.
            /// Returns null if the button is not mapped.
            /// </summary>
            /// <param name="name">The Silk.NET button name to convert.</param>
            /// <returns>The corresponding GamepadButton, or null if no mapping exists.</returns>
            public static GamepadButton? Convert(ButtonName name) => name switch
            {
                // ═══ Face buttons (Xbox layout)
                ButtonName.A => GamepadButton.A,
                ButtonName.B => GamepadButton.B,
                ButtonName.X => GamepadButton.X,
                ButtonName.Y => GamepadButton.Y,

                // ═══ Shoulder buttons
                ButtonName.LeftBumper => GamepadButton.LeftBumper,
                ButtonName.RightBumper => GamepadButton.RightBumper,

                // ═══ Analog stick buttons (press down on sticks)
                ButtonName.LeftStick => GamepadButton.LeftStick,
                ButtonName.RightStick => GamepadButton.RightStick,

                // ═══ D-Pad directional buttons
                ButtonName.DPadUp => GamepadButton.DPadUp,
                ButtonName.DPadDown => GamepadButton.DPadDown,
                ButtonName.DPadLeft => GamepadButton.DPadLeft,
                ButtonName.DPadRight => GamepadButton.DPadRight,

                // ═══ Menu/system buttons
                ButtonName.Start => GamepadButton.Start,
                ButtonName.Back => GamepadButton.Back,
                ButtonName.Home => GamepadButton.Guide, // ═══ Xbox/PlayStation home button
                _ => null
            };
        }
    }
} 