using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;
using static Create_your_Adventure.Source.Engine.Input.InputConverter;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    /// <summary>
    /// Manages gamepad input including buttons, analog sticks, and triggers.
    /// Combines event-driven button input with polling-based analog input for smooth control.
    /// Triggers can be treated as analog values or digital buttons based on a configurable threshold.
    /// </summary>
    public sealed class GamepadDevice : IInputDevice
    {
        // ═══ The underlying Silk.NET gamepad interface
        private readonly IGamepad? gamepad;
        // ═══ Reference to the input state for recording gamepad events
        private InputState? state;

        /// <summary>
        /// Gets or sets the threshold value (0.0 to 1.0) at which triggers are considered pressed as buttons.
        /// Default: 0.5 (50% pressed)
        /// </summary>
        public float TriggerThreshold { get; set; } = 0.5f;

        /// <summary>
        /// Gets the name of the gamepad device.
        /// </summary>
        public string Name => gamepad?.Name ?? "No Gamepad";

        /// <summary>
        /// Gets a value indicating whether a gamepad is connected.
        /// Gamepad support is optional - the engine works without one.
        /// </summary>
        public bool IsConnected => gamepad is not null;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the GamepadDevice class.
        /// Attempts to use the first available gamepad from the input context.
        /// </summary>
        /// <param name="context">The Silk.NET input context containing available gamepads.</param>
        public GamepadDevice(IInputContext context)
        {
            gamepad = context.Gamepads.Count > 0 ? context.Gamepads[0] : null;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the gamepad device and logs its status.
        /// Gamepad is optional - no error if not detected.
        /// </summary>
        public void Initialize()
        {
            if (IsConnected)
                Logger.Info($"[INPUT] Gamepad initialized: {Name}");
            else
                Logger.Info("[INPUT] No gamepad detected (optional)");
        }

        // ══════════════════════════════════════════════════
        // REGISTER EVENTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Registers gamepad event handlers for button presses and releases.
        /// Analog sticks and triggers are handled via polling in Poll().
        /// </summary>
        /// <param name="inputState">The input state object to update with gamepad events.</param>
        public void RegisterEvents(InputState inputState)
        {
            if (gamepad is null) return;

            state = inputState;
            gamepad.ButtonDown += OnButtonDown;
            gamepad.ButtonUp += OnButtonUp;
        }

        /// <summary>
        /// Unregisters all gamepad event handlers.
        /// </summary>
        public void UnregisterEvents()
        {
            if (gamepad is null) return;

            gamepad.ButtonDown -= OnButtonDown;
            gamepad.ButtonUp -= OnButtonUp;
        }

        // ══════════════════════════════════════════════════
        // POLL
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Polls the gamepad for analog stick and trigger values.
        /// Called every frame to capture continuous analog input.
        /// </summary>
        /// <param name="inputState">The input state object to update with current analog values.</param>
        public void Poll(InputState inputState)
        {
            if (gamepad is null) return;

            // ═══ Poll analog sticks (left and right)
            var thumbsticks = gamepad.Thumbsticks;
            if (thumbsticks.Count >= 2)
            {
                inputState.SetGamepadAxis(GamepadAxis.LeftStickX, thumbsticks[0].X);
                inputState.SetGamepadAxis(GamepadAxis.LeftStickY, thumbsticks[0].Y);
                inputState.SetGamepadAxis(GamepadAxis.RightStickX, thumbsticks[1].X);
                inputState.SetGamepadAxis(GamepadAxis.RightStickY, thumbsticks[1].Y);
            }

            // ═══ Poll triggers (can be used as analog values or digital buttons)
            var triggers = gamepad.Triggers;
            if (triggers.Count >= 2)
            {
                // ═══ Store raw analog trigger values (0.0 to 1.0)
                inputState.SetTriggerValue(GamepadButton.LeftTrigger, triggers[0].Position);
                inputState.SetTriggerValue(GamepadButton.RightTrigger, triggers[1].Position);

                // ═══ Treat triggers as digital buttons based on threshold
                if (triggers[0].Position >= TriggerThreshold)
                    inputState.SetGamepadButtonDown(GamepadButton.LeftTrigger);
                else
                    inputState.SetGamepadButtonUp(GamepadButton.LeftTrigger);

                if (triggers[1].Position >= TriggerThreshold)
                    inputState.SetGamepadButtonDown(GamepadButton.RightTrigger);
                else
                    inputState.SetGamepadButtonUp(GamepadButton.RightTrigger);
            }
        }

        // ══════════════════════════════════════════════════
        // ON BUTTONS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Internal handler for gamepad button press events.
        /// </summary>
        private void OnButtonDown(IGamepad gp, Button button)
        {
            var btn = GamepadConverter.Convert(button.Name);
            if (btn.HasValue)
                state?.SetGamepadButtonDown(btn.Value);
        }

        /// <summary>
        /// Internal handler for gamepad button release events.
        /// </summary>
        private void OnButtonUp(IGamepad gp, Button button)
        {
            var btn = GamepadConverter.Convert(button.Name);
            if (btn.HasValue)
                state?.SetGamepadButtonUp(btn.Value);
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