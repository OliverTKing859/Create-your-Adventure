using System.Numerics;

namespace Create_your_Adventure.Source.Engine.Input
{
    // ══════════════════════════════════════════════════
    // INPUT CONSTANTS
    // ══════════════════════════════════════════════════
    /// <summary>
    /// 
    /// </summary>
    internal static class InputConstants
    {
        /// <summary>
        /// The duration threshold (in seconds) for detecting long presses.
        /// Default: 0.5 seconds
        /// </summary>
        public const float LongPressThreshold = 0.5f;

        /// <summary>
        /// The default deadzone for analog inputs (gamepad sticks).
        /// Values below this threshold are treated as zero to prevent drift.
        /// </summary>
        public const float DefaultDeadzone = 0.15f;
    }

    /// <summary>
    /// Provides high-level input query methods for gameplay and camera control.
    /// Analyzes the raw input state to detect patterns like presses, releases, holds, and combinations.
    /// Includes helper methods for common game mechanics (movement, looking, combos).
    /// </summary>
    public sealed class InputAnalyzer
    {
        // ═══ Reference to the input state being analyzed
        private readonly InputState state;

        /// <summary>
        /// Initializes a new instance of the InputAnalyzer class.
        /// </summary>
        /// <param name="state">The input state to analyze.</param>
        public InputAnalyzer(InputState state)
        {
            this.state = state;
        }

        // ══════════════════════════════════════════════════
        // KEYBOARD QUERIES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks if a key is currently held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is currently down.</returns>
        public bool IsKeyDown(KeyCode key)
            => state.CurrentKeys.Contains(key);

        /// <summary>
        /// Checks if a key was just pressed this frame (down now, up last frame).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was pressed this frame.</returns>
        public bool IsKeyPressed(KeyCode key)
            => state.CurrentKeys.Contains(key) && !state.PreviousKeys.Contains(key);

        /// <summary>
        /// Checks if a key was just released this frame (up now, down last frame).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was released this frame.</returns>
        public bool IsKeyReleased(KeyCode key)
            => !state.CurrentKeys.Contains(key) && state.PreviousKeys.Contains(key);

        /// <summary>
        /// Checks if a key has been held down for longer than the long-press threshold.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key has been held for at least LongPressThreshold seconds.</returns>
        public bool IsKeyLongPressed(KeyCode key)
            => state.KeyHoldTimes.TryGetValue(key, out var time) && time >= InputConstants.LongPressThreshold;

        /// <summary>
        /// Gets the duration (in seconds) that a key has been held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>The hold duration in seconds, or 0 if the key is not down.</returns>
        public float GetKeyHoldTime(KeyCode key)
            => state.KeyHoldTimes.TryGetValue(key, out var time) ? time : 0f;

        /// <summary>
        /// Checks if a debug key combination is active (modifier held + key pressed).
        /// Useful for developer shortcuts like Ctrl+D, Alt+F, etc.
        /// </summary>
        /// <param name="modifier">The modifier key (Ctrl, Alt, Shift).</param>
        /// <param name="key">The main key that must be pressed.</param>
        /// <returns>True if the modifier is held and the key was just pressed.</returns>
        public bool IsDebugCombo(KeyCode modifier, KeyCode key)
            => state.CurrentKeys.Contains(modifier) && IsKeyPressed(key);

        /// <summary>
        /// Checks if a multi-key combination is active (all modifiers held + main key pressed).
        /// Example: Ctrl+Shift+S for quick save.
        /// </summary>
        /// <param name="mainKey">The main key that must be pressed.</param>
        /// <param name="modifiers">The modifier keys that must all be held.</param>
        /// <returns>True if all modifiers are held and the main key was just pressed.</returns>
        public bool IsKeyCombination(KeyCode mainKey, params KeyCode[] modifiers)
        {
            bool modifiersHeld = modifiers.All(m => state.CurrentKeys.Contains(m));
            return modifiersHeld && IsKeyPressed(mainKey);
        }

        // ══════════════════════════════════════════════════
        // MOUSE QUERIES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks if a mouse button is currently held down.
        /// </summary>
        public bool IsMouseButtonDown(MouseButton btn)
            => state.CurrentMouseButtons.Contains(btn);

        /// <summary>
        /// Checks if a mouse button was just pressed this frame.
        /// </summary>
        public bool IsMouseButtonPressed(MouseButton btn)
            => state.CurrentMouseButtons.Contains(btn) && !state.PreviousMouseButtons.Contains(btn);

        /// <summary>
        /// Checks if a mouse button was just released this frame.
        /// </summary>
        public bool IsMouseButtonReleased(MouseButton btn)
            => !state.CurrentMouseButtons.Contains(btn) && state.PreviousMouseButtons.Contains(btn);

        /// <summary>
        /// Gets the current mouse cursor position in screen coordinates.
        /// </summary>
        public Vector2 GetMousePosition() => state.MousePosition;

        /// <summary>
        /// Gets the mouse movement delta since last frame (useful for camera control).
        /// </summary>
        public Vector2 GetMouseDelta() => state.MouseDelta;

        /// <summary>
        /// Gets the scroll wheel delta since last frame (positive = scroll up, negative = scroll down).
        /// </summary>
        public float GetScrollDelta() => state.ScrollDelta;

        // ══════════════════════════════════════════════════
        // GAMEPAD QUERIES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks if a gamepad button is currently held down.
        /// </summary>
        public bool IsGamepadButtonDown(GamepadButton btn)
            => state.CurrentGamepadButtons.Contains(btn);

        /// <summary>
        /// Checks if a gamepad button was just pressed this frame.
        /// </summary>
        public bool IsGamepadButtonPressed(GamepadButton btn)
            => state.CurrentGamepadButtons.Contains(btn) && !state.PreviousGamepadButtons.Contains(btn);

        /// <summary>
        /// Gets the value of a gamepad analog axis with deadzone applied.
        /// Returns 0 if the value is below the deadzone threshold to prevent drift.
        /// </summary>
        /// <param name="axis">The axis to query (left/right stick X/Y).</param>
        /// <param name="deadzone">The deadzone threshold (0.0 to 1.0). Default is 0.15.</param>
        /// <returns>The axis value (-1.0 to 1.0), or 0 if below deadzone.</returns>
        public float GetAxis(GamepadAxis axis, float deadzone = InputConstants.DefaultDeadzone)
        {
            if (!state.GamepadAxes.TryGetValue(axis, out var value)) return 0f;
            return MathF.Abs(value) < deadzone ? 0f : value;
        }

        /// <summary>
        /// Gets the left analog stick position as a 2D vector with deadzone applied.
        /// </summary>
        /// <param name="deadzone">The deadzone threshold. Default is 0.15.</param>
        /// <returns>A vector where X is horizontal (-1 left, 1 right) and Y is vertical (-1 down, 1 up).</returns>
        public Vector2 GetLeftStick(float deadzone = InputConstants.DefaultDeadzone) => new(
            GetAxis(GamepadAxis.LeftStickX, deadzone),
            GetAxis(GamepadAxis.LeftStickY, deadzone)
        );

        /// <summary>
        /// Gets the right analog stick position as a 2D vector with deadzone applied.
        /// </summary>
        /// <param name="deadzone">The deadzone threshold. Default is 0.15.</param>
        /// <returns>A vector where X is horizontal (-1 left, 1 right) and Y is vertical (-1 down, 1 up).</returns>
        public Vector2 GetRightStick(float deadzone = InputConstants.DefaultDeadzone) => new(
            GetAxis(GamepadAxis.RightStickX, deadzone),
            GetAxis(GamepadAxis.RightStickY, deadzone)
        );

        // ══════════════════════════════════════════════════
        // CAMERA HELPERS (Direct Queries!)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets a normalized movement vector based on WASD keys or left gamepad stick.
        /// Returns a diagonal-corrected vector for smooth 8-directional movement.
        /// </summary>
        /// <returns>A normalized movement vector, or Vector2.Zero if no movement input.</returns>
        public Vector2 GetMovementVector()
        {
            Vector2 movement = Vector2.Zero;

            // ═══ Check WASD keys for keyboard movement
            if (IsKeyDown(KeyCode.W)) movement.Y += 1f;
            if (IsKeyDown(KeyCode.S)) movement.Y -= 1f;
            if (IsKeyDown(KeyCode.A)) movement.X -= 1f;
            if (IsKeyDown(KeyCode.D)) movement.X += 1f;

            // ═══ Normalize diagonal movement to prevent faster diagonal movement
            if (movement != Vector2.Zero)
                return Vector2.Normalize(movement);

            // ═══ Fallback to gamepad left stick if no keyboard input
            var stick = GetLeftStick();
            return stick != Vector2.Zero ? stick : Vector2.Zero;
        }

        /// <summary>
        /// Determines the vertical movement direction based on the current keyboard input.
        /// </summary>
        /// <returns>A value indicating the vertical movement direction: 1 if the Space key is pressed, -1 if the Left Control
        /// key is pressed, or 0 if neither key is pressed.</returns>
        public float GetVerticalMovement()
        {
            if (IsKeyDown(KeyCode.Space)) return 1f;
            if (IsKeyDown(KeyCode.LeftControl)) return -1f;
            return 0f;
        }

        /// <summary>
        /// Gets a look/rotation vector based on mouse delta or right gamepad stick.
        /// Prioritizes mouse input over gamepad for camera control.
        /// </summary>
        /// <returns>A vector representing camera rotation input.</returns>
        public Vector2 GetLookVector()
        {
            var delta = GetMouseDelta();
            if (delta != Vector2.Zero) return new Vector2(delta.X, delta.Y);

            // ═══ Fallback to gamepad right stick if no mouse movement
            return GetRightStick();
        }
    }
}