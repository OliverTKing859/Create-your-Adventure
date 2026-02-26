using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    public sealed class GamepadDevice : IInputDevice
    {
        private readonly IGamepad? gamepad;
        private InputState? state;

        public float TriggerThreshold { get; set; } = 0.5f;

        public string Name => gamepad?.Name ?? "No Gamepad";
        public bool IsConnected => gamepad is not null;

        public GamepadDevice(IInputContext context)
        {
            gamepad = context.Gamepads.Count > 0 ? context.Gamepads[0] : null;
        }

        public void Initialize()
        {
            if (IsConnected)
                Logger.Info($"[INPUT] Gamepad initialized: {Name}");
            else
                Logger.Info("[INPUT] No gamepad detected (optional)");
        }

        public void RegisterEvents(InputState inputState)
        {
            if (gamepad is null) return;

            state = inputState;
            gamepad.ButtonDown += OnButtomDown;
            gamepad.ButtonUp += OnButtonUp;
        }

        public void UnregisterEvents()
        {
            if (gamepad is null) return;

            gamepad.ButtonDown -= OnButtomDown;
            gamepad.ButtonUp -= OnButtomUp;
        }

        public void Poll(InputState inputState)
        {
            if (gamepad is null) return;

            var thumbsticks = gamepad.Thumbsticks;
            if (thumbsticks.Count >= 2)
            {
                inputState.SetGamepadAxis(GamepadAxis.LeftStickX, thumbsticks[0].X);
                inputState.SetGamepadAxis(GamepadAxis.LeftStickY, thumbsticks[0].Y);
                inputState.SetGamepadAxis(GamepadAxis.RightStickX, thumbsticks[1].X);
                inputState.SetGamepadAxis(GamepadAxis.RightStickY, thumbsticks[1].Y);
            }

            var triggers = gamepad.Triggers;
            if (triggers.Count >= 2)
            {
                // ═══ Save trigger values for GetTriggerValue()
                inputState.SetTriggerValue(GamepadButton.LeftTrigger, triggers[0].Position);
                inputState.SetTriggerValue(GamepadButton.RightTrigger, triggers[1].Position);

                // ═══ Treat trigger as button (via threshold value)
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

        private void OnButtonDown(IGamepad gp, Button button)
        {
            var btn = GamepadConverter.Convert(button.Name);
            if (btn.HasValue)
                state?.SetGamepadButtonDown(btn.Value);
        }

        private void OnButtonUp(IGamepad gp, Button button)
        {
            var btn = GamepadConverter.Convert(button.Name);
            if (btn.HasValue)
                state?.SetGamepadButtonUp(btn.Value);
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}
