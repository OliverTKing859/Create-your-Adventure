using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public class InputState
    {
        // ═══ Keyboard State
        private readonly HashSet<KeyCode> currentKeys = [];
        private readonly HashSet<KeyCode> previousKeys = [];

        // ═══ Mouse State
        private readonly HashSet<MouseButton> currentMouseButtons = [];
        private readonly HashSet<MouseButton> previousMouseButtons = [];

        // ═══ Mouse Position & Delta
        public Vector2 MousePosition { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        public float ScrollDelta { get; private set; }

        // ═══ Gamepad State
        private readonly HashSet<GamepadButton> currentGamepadButtons = [];
        private readonly HashSet<GamepadButton> previousGamepadButtons = [];
        private readonly Dictionary<GamepadAxis, float> GamepadAxes = [];

        // ═══ Long Press Tracking
        private readonly Dictionary<KeyCode, float> keyHoldTimes = [];
        private readonly Dictionary<MouseButton, float> mouseHoldTimes = [];
        private readonly Dictionary<GamepadButton, float> gamepadHoldTimes = [];

        // ═══ Double Press Tracking
        private readonly Dictionary<KeyCode, (float LastPressTime, int PressCount)> doublePressTracking = [];

        public const float LongPressThreshold = 0.5f;   // Seconds for LongPress
        public const float DoublePressWindow = 0.3f;    // Seconds for DoublePress

        // ══════════════════════════════════════════════════
        // FRAME UPDATE
        // ══════════════════════════════════════════════════

        public void BeginFrame()
        {
            // ═══ Save previous frame
            previousKeys.Clear();
            foreach (var key in currentKeys) previousKeys.Add(key);

            previousMouseButtons.Clear();
            foreach (var btn in currentMouseButtons) previousMouseButtons.Add(btn);

            previousGamepadButtons.Clear();
            foreach (var btn in currentGamepadButtons) previousGamepadButtons.Add(btn);

            // ═══ Delta reset
            MouseDelta = Vector2.Zero;
            ScrollDelta = 0f;
        }

        public void EndFrame(float deltaTime)
        {
            // ═══ Update key hold times
            foreach (var key in currentKeys)
            {
                if (!keyHoldTimes.ContainsKey(key))
                    keyHoldTimes[key] = 0f;
                keyHoldTimes[key] += deltaTime;
            }

            // ═══ Remove released keys
            var releasedKeys = keyHoldTimes.Keys.Where(k => !currentKeys.Contains(k)).ToList();
            foreach (var key in releasedKeys)
                keyHoldTimes.Remove(key);

            // ══════════════════════════════════════════════════

            // ═══ Same for mouse and gamepad...
        }

        // ══════════════════════════════════════════════════
        // KEYBOARD QUERIES
        // ══════════════════════════════════════════════════

        public bool IsKeyDown(KeyCode key) 
            => currentKeys.Contains(key);

        public bool IsKeyPressed(KeyCode key)
            => currentKeys.Contains(key) && !previousKeys.Contains(key);

        public bool IsKeyReleased(KeyCode key)
            => !currentKeys.Contains(key) && previousKeys.Contains(key);

        public bool IsKeyLongPressed(KeyCode key)
            => keyHoldTimes.TryGetValue(key, out var time) && time >= LongPressThreshold;

        public float GetKeyHoldTime(KeyCode key)
            => keyHoldTimes.TryGetValue(key, out var time) ? time : 0f;

        public bool IsKeyCombinationDown(params KeyCode[] keys)
            => keys.All(k => currentKeys.Contains(k));

        public bool IsKeyCombinationPressed(KeyCode mainKey, params KeyCode[] modifiers)
        {
            bool modifiersHeld = modifiers.All(m => currentKeys.Contains(m));
            bool mainPressed = IsKeyPressed(mainKey);
            return modifiersHeld && mainPressed;
        }

        public bool IsKeyDoublePressed(KeyCode key, float currentTime)
        {
            if (!IsKeyPressed(key)) return false;

            if (doublePressTracking.TryGetValue(key, out var data))
            {
                if (currentTime - data.LastPressTime <= DoublePressWindow)
                {
                    doublePressTracking[key] = (currentTime, 0); // ═══ Reset
                    return true;
                }
            }

            doublePressTracking[key] = (currentTime, 1);
            return false;
        }

        // ══════════════════════════════════════════════════
        // MOUSE QUERIES
        // ══════════════════════════════════════════════════

        public bool IsMouseButtonDown(MouseButton button) => currentMouseButtons.Contains(button);
        public bool IsMouseButtonPressed(MouseButton button)
            => currentMouseButtons.Contains(button) && !previousMouseButtons.Contains(button);
        public bool IsMouseButtonReleased(MouseButton button)
            => !currentMouseButtons.Contains(button) && previousMouseButtons.Contains(button);

        public Vector2 GetMousePosition() => MousePosition;
        public Vector2 GetMouseDelta() => MouseDelta;
        public float GetScrollDelta() => ScrollDelta;

        // ══════════════════════════════════════════════════
        // GAMEPAD QUERIES
        // ══════════════════════════════════════════════════

        public bool IsGamepadButtonDown(GamepadButton button) => currentGamepadButtons.Contains(button);
        public bool IsGamepadButtonPressed(GamepadButton button)
            => currentGamepadButtons.Contains(button) && !previousGamepadButtons.Contains(button);

        public float GetGamepadAxis(GamepadAxis axis, float deadzone = 0.15f)
        {
            if (!GamepadAxes.TryGetValue(axis, out var value)) return 0f;
            return MathF.Abs(value) < deadzone ? 0f : value;
        }

        public Vector2 GetLeftStick(float deadzone = 0.15f) => new(
            GetGamepadAxis(GamepadAxis.LeftStickX, deadzone),
            GetGamepadAxis(GamepadAxis.LeftStickY, deadzone)
        );

        public Vector2 GetRightStick(float deadzone = 0.15f) => new(
            GetGamepadAxis(GamepadAxis.LeftStickX, deadzone),
            GetGamepadAxis(GamepadAxis.LeftStickY, deadzone)
        );

        // ══════════════════════════════════════════════════
        // INTERNAL UPDATE METHODS (called up by devices)
        // ══════════════════════════════════════════════════

        internal void SetKeyDown(KeyCode key) => currentKeys.Add(key);
        internal void SetKeyUp(KeyCode key) => currentKeys.Remove(key);

        internal void SetMouseButtonDown(MouseButton button) => currentMouseButtons.Add(button);
        internal void SetMouseButtonUp(MouseButton button) => currentMouseButtons.Remove(button);
        internal void SetMousePosition(Vector2 position) => MousePosition += position;
        internal void SetMouseDelta(Vector2 delta) => MouseDelta += delta;
        internal void SetScrollDelta(float delta) => ScrollDelta += delta;

        internal void SetGamepadButtonDown(GamepadButton button) => currentGamepadButtons.Add(button);
        internal void SetGamepadButtonUp(GamepadButton button) => currentGamepadButtons.Remove(button);
        internal void SetGamepadAxis(GamepadAxis axis, float value) => GamepadAxes[axis] = value;
    }
}