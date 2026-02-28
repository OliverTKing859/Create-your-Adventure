using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public sealed class InputAnalyzer
    {
        private readonly InputState state;

        public const float LongPressThreshold = 0.5f;
        public const float DefaultDeadzone = 0.15f;

        public InputAnalyzer(InputState state)
        {
            this.state = state;
        }

        // ══════════════════════════════════════════════════
        // KEYBOARD QUERIES
        // ══════════════════════════════════════════════════

        public bool IsKeyDown(KeyCode key)
            => state.CurrentKeys.Contains(key);

        public bool IsKeyPressed(KeyCode key)
            => state.CurrentKeys.Contains(key) && !state.PreviousKeys.Contains(key);

        public bool IsKeyReleased(KeyCode key)
            => !state.CurrentKeys.Contains(key) && state.PreviousKeys.Contains(key);

        public bool IsKeyLongPressed(KeyCode key)
            => state.KeyHoldTimes.TryGetValue(key, out var time) && time >= LongPressThreshold;

        public float GetKeyHoldTime(KeyCode key)
            => state.KeyHoldTimes.TryGetValue(key, out var time) ? time : 0f;

        public bool IsDebugCombo(KeyCode modifier, KeyCode key)
            => state.CurrentKeys.Contains(modifier) && IsKeyPressed(key);

        public bool IsKeyCombination(KeyCode mainKey, params KeyCode[] modifiers)
        {
            bool modifiersHeld = modifiers.All(m => state.CurrentKeys.Contains(m));
            return modifiersHeld && IsKeyPressed(mainKey);
        }

        // ══════════════════════════════════════════════════
        // MOUSE QUERIES
        // ══════════════════════════════════════════════════

        public bool IsMouseButtonDown(MouseButton btn)
            => state.CurrentMouseButtons.Contains(btn);

        public bool IsMouseButtonPressed(MouseButton btn)
            => state.CurrentMouseButtons.Contains(btn) && !state.PreviousMouseButtons.Contains(btn);

        public bool IsMouseButtonReleased(MouseButton btn)
            => !state.CurrentMouseButtons.Contains(btn) && state.PreviousMouseButtons.Contains(btn);

        public Vector2 GetMousePosition() => state.MousePosition;
        public Vector2 GetMouseDelta() => state.MouseDelta;
        public float GetScrollDelta() => state.ScrollDelta;

        // ══════════════════════════════════════════════════
        // GAMEPAD QUERIES
        // ══════════════════════════════════════════════════

        public bool IsGamepadButtonDown(GamepadButton btn)
            => state.CurrentGamepadButtons.Contains(btn);

        public bool IsGamepadButtonPressed(GamepadButton btn)
            => state.CurrentGamepadButtons.Contains(btn) && !state.PreviousGamepadButtons.Contains(btn);

        public float GetAxis(GamepadAxis axis, float deadzone = DefaultDeadzone)
        {
            if (!state.GamepadAxes.TryGetValue(axis, out var value)) return 0f;
            return MathF.Abs(value) < deadzone ? 0f : value;
        }

        public Vector2 GetLeftStick(float deadzone = DefaultDeadzone) => new(
            GetAxis(GamepadAxis.LeftStickX, deadzone),
            GetAxis(GamepadAxis.LeftStickY, deadzone)
        );

        public Vector2 GetRightStick(float deadzone = DefaultDeadzone) => new(
            GetAxis(GamepadAxis.RightStickX, deadzone),
            GetAxis(GamepadAxis.RightStickY, deadzone)
        );

        // ══════════════════════════════════════════════════
        // CAMERA HELPERS (Direct Queries!)
        // ══════════════════════════════════════════════════

        public Vector2 GetMovementVector()
        {
            Vector2 movement = Vector2.Zero;

            if (IsKeyDown(KeyCode.W)) movement.Y += 1f;
            if (IsKeyDown(KeyCode.S)) movement.Y -= 1f;
            if (IsKeyDown(KeyCode.A)) movement.X -= 1f;
            if (IsKeyDown(KeyCode.D)) movement.X += 1f;

            if (movement != Vector2.Zero)
                return Vector2.Normalize(movement);

            // ═══ Fallback: Gamepad
            var stick = GetLeftStick();
            return stick != Vector2.Zero ? stick : Vector2.Zero;
        }

        public Vector2 GetLookVector()
        {
            var delta = GetMouseDelta();
            if (delta != Vector2.Zero) return delta;

            return GetRightStick();
        }
    }
}