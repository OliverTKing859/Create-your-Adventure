using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public class InputState
    {
        // ═══ Keyboard
        internal readonly HashSet<KeyCode> CurrentKeys = [];
        internal readonly HashSet<KeyCode> PreviousKeys = [];
        internal readonly Dictionary<KeyCode, float> KeyHoldTimes = [];

        // ═══ Mouse
        internal readonly HashSet<MouseButton> CurrentMouseButtons = [];
        internal readonly HashSet<MouseButton> PreviousMouseButtons = [];
        internal Vector2 MousePosition;
        internal Vector2 MouseDelta;
        internal float ScrollDelta;

        // ═══ Gamepad State
        internal readonly HashSet<GamepadButton> CurrentGamepadButtons = [];
        internal readonly HashSet<GamepadButton> PreviousGamepadButtons = [];
        internal readonly Dictionary<GamepadAxis, float> GamepadAxes = [];
        internal readonly Dictionary<GamepadButton, float> TriggerValues = [];

        // ══════════════════════════════════════════════════
        // FRAME MAGAGEMENT (called by InputManager)
        // ══════════════════════════════════════════════════

        internal void BeginFrame()
        {
            // ═══ Save previous frame
            PreviousKeys.Clear();
            foreach (var key in CurrentKeys) PreviousKeys.Add(key);

            PreviousMouseButtons.Clear();
            foreach (var btn in CurrentMouseButtons) PreviousMouseButtons.Add(btn);

            PreviousGamepadButtons.Clear();
            foreach (var btn in CurrentGamepadButtons) PreviousGamepadButtons.Add(btn);

            // ═══ Delta reset
            MouseDelta = Vector2.Zero;
            ScrollDelta = 0f;
        }

        internal void EndFrame(float deltaTime)
        {
            // ═══ Update key hold times
            foreach (var key in CurrentKeys)
            {
                if (!KeyHoldTimes.ContainsKey(key))
                    KeyHoldTimes[key] = 0f;
                KeyHoldTimes[key] += deltaTime;
            }

            // ═══ Remove released keys
            var releasedKeys = KeyHoldTimes.Keys.Where(k => !CurrentKeys.Contains(k)).ToList();
            foreach (var key in releasedKeys)
                KeyHoldTimes.Remove(key);
        }

        // ══════════════════════════════════════════════════
        // SETTERS ()
        // ══════════════════════════════════════════════════

        internal void SetKeyDown(KeyCode key) => CurrentKeys.Add(key);
        internal void SetKeyUp(KeyCode key) => CurrentKeys.Remove(key);

        internal void SetMouseButtonDown(MouseButton btn) => CurrentMouseButtons.Add(btn);
        internal void SetMouseButtonUp(MouseButton btn) => CurrentMouseButtons.Remove(btn);
        internal void SetMousePosition(Vector2 pos) => MousePosition = pos;
        internal void SetMouseDelta(Vector2 delta) => MouseDelta += delta;
        internal void SetScrollDelta(float delta) => ScrollDelta += delta;

        internal void SetGamepadButtonDown(GamepadButton btn) => CurrentGamepadButtons.Add(btn);
        internal void SetGamepadButtonUp(GamepadButton btn) => CurrentGamepadButtons.Remove(btn);
        internal void SetGamepadAxis(GamepadAxis axis, float value) => GamepadAxes[axis] = value;
        internal void SetTriggerValue(GamepadButton trigger, float value) => TriggerValues[trigger] = value;
    }
}