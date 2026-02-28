using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;
using static Create_your_Adventure.Source.Engine.Input.InputConverter;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    public sealed class KeyboardDevice : IInputDevice
    {
        private readonly IKeyboard? keyboard;
        private InputState? state;

        public string Name => keyboard?.Name ?? "Unknown Keyboard";
        public bool IsConnected => keyboard is not null;

        public KeyboardDevice(IInputContext context)
        {
            keyboard = context.Keyboards.Count > 0 ? context.Keyboards[0] : null;
        }

        public void Initialize()
        {
            if (IsConnected)
                Logger.Info($"[INPUT] Keyboard initialized: {Name}");
            else
                Logger.Warn("[INPUT] No keyboard detected");
        }

        public void RegisterEvents(InputState inputState)
        {
            if (keyboard is null) return;

            state = inputState;
            keyboard.KeyDown += OnKeyDown;
            keyboard.KeyUp += OnKeyUp;
        }

        public void UnregisterEvents()
        {
            if (keyboard is null) return;
            keyboard.KeyDown -= OnKeyDown;
            keyboard.KeyUp -= OnKeyUp;
        }

        public void Poll(InputState state) { } // ═══ Keyboard does not require polling

        private void OnKeyDown(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = KeyConverter.Convert(key);
            if (keyCode.HasValue)
                state?.SetKeyDown(keyCode.Value);
        }

        private void OnKeyUp(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = KeyConverter.Convert(key);
            if (keyCode.HasValue)
                state?.SetKeyUp(keyCode.Value);
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}