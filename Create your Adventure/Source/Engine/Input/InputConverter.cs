using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public static class InputConverter
    {
        public static class KeyConverter
        {
            public static KeyCode? Convert(Key key) => key switch
            {
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

                Key.ShiftLeft => KeyCode.LeftShift,
                Key.ShiftRight => KeyCode.RightShift,
                Key.ControlLeft => KeyCode.LeftControl,
                Key.ControlRight => KeyCode.RightControl,
                Key.AltLeft => KeyCode.LeftAlt,
                Key.AltRight => KeyCode.RightAlt,

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
                Key.KeypadSubtract => KeyCode.NumpadSubstract,
                Key.KeypadMultiply => KeyCode.NumpadMultiply,
                Key.KeypadDivide => KeyCode.NumpadDivide,
                Key.KeypadEnter => KeyCode.NumpadEnter,
                Key.KeypadDecimal => KeyCode.NumpadDecimal,

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

                _ => null
            };
        }

        public static class MouseConverter
        {
            public static MouseButton? Convert(Silk.NET.Input.MouseButton button) => button switch
            {
                Silk.NET.Input.MouseButton.Left => MouseButton.Left,
                Silk.NET.Input.MouseButton.Right => MouseButton.Right,
                Silk.NET.Input.MouseButton.Middle => MouseButton.Middle,
                Silk.NET.Input.MouseButton.Button4 => MouseButton.Button4,
                Silk.NET.Input.MouseButton.Button5 => MouseButton.Button5,
                _ => null
            };
        }

        public static class GamepadConverter
        {
            public static GamepadButton? Convert(ButtonName name) => name switch
            {
                ButtonName.A => GamepadButton.A,
                ButtonName.B => GamepadButton.B,
                ButtonName.X => GamepadButton.X,
                ButtonName.Y => GamepadButton.Y,

                ButtonName.LeftBumper => GamepadButton.LeftBumper,
                ButtonName.RightBumper => GamepadButton.RightBumper,
                ButtonName.LeftStick => GamepadButton.LeftStick,
                ButtonName.RightStick => GamepadButton.RightStick,

                ButtonName.DPadUp => GamepadButton.DPadUp,
                ButtonName.DPadDown => GamepadButton.DPadDown,
                ButtonName.DPadLeft => GamepadButton.DPadLeft,
                ButtonName.DPadRight => GamepadButton.DPadRight,

                ButtonName.Start => GamepadButton.Start,
                ButtonName.Back => GamepadButton.Back,
                ButtonName.Home => GamepadButton.Guide,
                _ => null
            };
        }
    }
}