using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;
using static Create_your_Adventure.Source.Engine.Input.InputConverter;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    /// <summary>
    /// Manages keyboard input through event-driven key press and release handling.
    /// Wraps Silk.NET's IKeyboard and converts native key codes to engine-specific key codes.
    /// Uses event-based input (no polling required).
    /// </summary>
    public sealed class KeyboardDevice : IInputDevice
    {
        // ═══ The underlying Silk.NET keyboard interface
        private readonly IKeyboard? keyboard;
        // ═══ Reference to the input state for recording key events
        private InputState? state;

        /// <summary>
        /// Gets the name of the keyboard device.
        /// </summary>
        public string Name => keyboard?.Name ?? "Unknown Keyboard";

        /// <summary>
        /// Gets a value indicating whether a keyboard is connected.
        /// </summary>
        public bool IsConnected => keyboard is not null;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the KeyboardDevice class.
        /// Attempts to use the first available keyboard from the input context.
        /// </summary>
        /// <param name="context">The Silk.NET input context containing available keyboards.</param>
        public KeyboardDevice(IInputContext context)
        {
            keyboard = context.Keyboards.Count > 0 ? context.Keyboards[0] : null;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the keyboard device and logs its status.
        /// </summary>
        public void Initialize()
        {
            if (IsConnected)
                Logger.Info($"[INPUT] Keyboard initialized: {Name}");
            else
                Logger.Warn("[INPUT] No keyboard detected");
        }

        // ══════════════════════════════════════════════════
        // REGISTER EVENTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Registers keyboard event handlers for key press and release events.
        /// </summary>
        /// <param name="inputState">The input state object to update with key events.</param>
        public void RegisterEvents(InputState inputState)
        {
            if (keyboard is null) return;

            state = inputState;
            keyboard.KeyDown += OnKeyDown;
            keyboard.KeyUp += OnKeyUp;
        }

        /// <summary>
        /// Unregisters all keyboard event handlers.
        /// </summary>
        public void UnregisterEvents()
        {
            if (keyboard is null) return;
            keyboard.KeyDown -= OnKeyDown;
            keyboard.KeyUp -= OnKeyUp;
        }

        // ══════════════════════════════════════════════════
        // POLL
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Polling is not required for keyboard input (event-driven).
        /// </summary>
        /// <param name="state">Unused for keyboard.</param>
        public void Poll(InputState state) { } // ═══ Keyboard uses event-driven input

        // ══════════════════════════════════════════════════
        // ON KEYS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Internal handler for key press events.
        /// Converts Silk.NET key codes to engine key codes and updates input state.
        /// </summary>
        private void OnKeyDown(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = KeyConverter.Convert(key);
            if (keyCode.HasValue)
                state?.SetKeyDown(keyCode.Value);
        }

        /// <summary>
        /// Internal handler for key release events.
        /// Converts Silk.NET key codes to engine key codes and updates input state.
        /// </summary>
        private void OnKeyUp(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = KeyConverter.Convert(key);
            if (keyCode.HasValue)
                state?.SetKeyUp(keyCode.Value);
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases resources and unregisters event handlers.
        /// </summary>
        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}