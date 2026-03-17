using System.Numerics;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Stores the raw input state for all devices (keyboard, mouse, gamepad).
    /// Tracks current and previous frame states to enable edge detection (pressed, released).
    /// Manages frame-by-frame updates including hold times, deltas, and analog values.
    /// Internal class - accessed through InputAnalyzer for queries.
    /// </summary>
    public class InputState
    {
        // ═══ Keyboard state tracking
        internal readonly HashSet<KeyCode> CurrentKeys = [];
        internal readonly HashSet<KeyCode> PreviousKeys = [];
        internal readonly Dictionary<KeyCode, float> KeyHoldTimes = [];
        private readonly List<KeyCode> keysToRemove = new();

        // ═══ Mouse state tracking
        internal readonly HashSet<MouseButton> CurrentMouseButtons = [];
        internal readonly HashSet<MouseButton> PreviousMouseButtons = [];
        internal Vector2 MousePosition;
        internal Vector2 MouseDelta;
        internal float ScrollDelta;

        // ═══ Gamepad state tracking
        internal readonly HashSet<GamepadButton> CurrentGamepadButtons = [];
        internal readonly HashSet<GamepadButton> PreviousGamepadButtons = [];
        internal readonly Dictionary<GamepadAxis, float> GamepadAxes = [];
        internal readonly Dictionary<GamepadButton, float> TriggerValues = [];

        // ══════════════════════════════════════════════════
        // FRAME MAGAGEMENT (called by InputManager)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Called at the start of each frame to prepare for new input.
        /// Copies current state to previous state and resets delta values.
        /// </summary>
        internal void BeginFrame()
        {
            // ═══ Save previous frame state for edge detection
            PreviousKeys.Clear();
            PreviousKeys.UnionWith(CurrentKeys);

            PreviousMouseButtons.Clear();
            PreviousMouseButtons.UnionWith(CurrentMouseButtons);

            PreviousGamepadButtons.Clear();
            PreviousGamepadButtons.UnionWith(CurrentGamepadButtons);
        }

        /// <summary>
        /// Called at the end of each frame to finalize input state.
        /// Updates key hold times and cleans up released keys.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame in seconds.</param>
        internal void EndFrame(float deltaTime)
        {
            // ═══ Update hold times for all currently pressed keys
            foreach (var key in CurrentKeys)
            {
                if (!KeyHoldTimes.ContainsKey(key))
                    KeyHoldTimes[key] = 0f;
                KeyHoldTimes[key] += deltaTime;
            }

            // ═══ Remove hold time tracking for released keys (avoid LINQ allocation)
            keysToRemove.Clear();
            foreach (var k in KeyHoldTimes.Keys)
            {
                if (!CurrentKeys.Contains(k))
                    keysToRemove.Add(k);
            }
            foreach (var key in keysToRemove)
                KeyHoldTimes.Remove(key);

            // ═══ Reset delta values (accumulated during frame)
            MouseDelta = Vector2.Zero;
            ScrollDelta = 0f;
        }

        // ══════════════════════════════════════════════════
        // SETTERS ()
        // ══════════════════════════════════════════════════
        /// <summary>Records a key press event.</summary>
        internal void SetKeyDown(KeyCode key) => CurrentKeys.Add(key);

        /// <summary>Records a key release event.</summary>
        internal void SetKeyUp(KeyCode key) => CurrentKeys.Remove(key);

        // ══════════════════════════════════════════════════
        /// <summary>Records a mouse button press event.</summary>
        internal void SetMouseButtonDown(MouseButton btn) => CurrentMouseButtons.Add(btn);

        /// <summary>Records a mouse button release event.</summary>
        internal void SetMouseButtonUp(MouseButton btn) => CurrentMouseButtons.Remove(btn);

        /// <summary>Updates the absolute mouse cursor position.</summary>
        internal void SetMousePosition(Vector2 pos) => MousePosition = pos;

        /// <summary>Accumulates mouse movement delta (can be called multiple times per frame).</summary>
        internal void SetMouseDelta(Vector2 delta) => MouseDelta += delta;

        /// <summary>Accumulates scroll wheel delta (can be called multiple times per frame).</summary>
        internal void SetScrollDelta(float delta) => ScrollDelta += delta;

        // ══════════════════════════════════════════════════
        /// <summary>Records a gamepad button press event.</summary>
        internal void SetGamepadButtonDown(GamepadButton btn) => CurrentGamepadButtons.Add(btn);

        /// <summary>Records a gamepad button release event.</summary>
        internal void SetGamepadButtonUp(GamepadButton btn) => CurrentGamepadButtons.Remove(btn);

        /// <summary>Updates a gamepad analog axis value (-1.0 to 1.0).</summary>
        internal void SetGamepadAxis(GamepadAxis axis, float value) => GamepadAxes[axis] = value;

        /// <summary>Updates a gamepad trigger analog value (0.0 to 1.0).</summary>
        internal void SetTriggerValue(GamepadButton trigger, float value) => TriggerValues[trigger] = value;
    }
}