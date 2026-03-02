using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Represents a named gameplay action that can be triggered by multiple input bindings.
    /// Actions abstract raw input (keys, buttons) into gameplay concepts (jump, shoot, interact).
    /// Supports rebinding and serialization for player-customizable controls.
    /// </summary>
    public class InputAction
    {
        /// <summary>
        /// Gets the unique name of this action (e.g., "Jump", "Fire", "Interact").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of action (Pressed, Held, Released, etc.).
        /// Determines how bindings are evaluated.
        /// </summary>
        public InputActionType Type { get; }

        /// <summary>
        /// Gets the list of input bindings that can trigger this action.
        /// Multiple bindings allow keyboard + gamepad support for the same action.
        /// </summary>
        public List<InputBinding> Bindings { get; } = [];

        /// <summary>
        /// Event raised when this action is triggered (for Pressed, Held, Released types).
        /// </summary>
        public event Action<InputAction>? Triggered;

        /// <summary>
        /// Event raised when this action's axis value changes (for Axis type).
        /// Provides the current axis value (-1.0 to 1.0).
        /// </summary>
        public event Action<InputAction, float>? AxisChanged;

        /// <summary>
        /// Initializes a new instance of the InputAction class.
        /// </summary>
        /// <param name="name">The unique name for this action.</param>
        /// <param name="type">The action type (Pressed, Held, Released, etc.).</param>
        public InputAction(string name, InputActionType type)
        {
            Name = name;
            Type = type;
        }

        // ══════════════════════════════════════════════════
        // BINDING METHODS (Fluent API)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Adds a keyboard binding to this action with optional modifier keys.
        /// Returns this instance for method chaining.
        /// </summary>
        /// <param name="key">The main key to bind.</param>
        /// <param name="modifiers">Optional modifier keys (Ctrl, Shift, Alt) that must be held.</param>
        /// <returns>This InputAction for chaining.</returns>
        public InputAction AddKeyBinding(KeyCode key, params KeyCode[] modifiers)
        {
            Bindings.Add(new KeyBinding(key, modifiers));
            return this;
        }

        /// <summary>
        /// Adds a gamepad button binding to this action.
        /// Returns this instance for method chaining.
        /// </summary>
        /// <param name="button">The gamepad button to bind.</param>
        /// <returns>This InputAction for chaining.</returns>
        public InputAction AddGamepadBinding(GamepadButton button)
        {
            Bindings.Add(new GamepadButtonBinding(button));
            return this;
        }

        /// <summary>
        /// Adds a gamepad analog axis binding to this action.
        /// Only valid for Axis-type actions.
        /// Returns this instance for method chaining.
        /// </summary>
        /// <param name="axis">The gamepad axis to bind.</param>
        /// <returns>This InputAction for chaining.</returns>
        public InputAction AddAxisBinding(GamepadAxis axis)
        {
            Bindings.Add(new GamepadAxisBinding(axis));
            return this;
        }

        /// <summary>
        /// Internal method to raise the Triggered event.
        /// Called by InputRegistry when a binding becomes active.
        /// </summary>
        internal void RaiseTrigger() => Triggered?.Invoke(this);

        /// <summary>
        /// Internal method to raise the AxisChanged event.
        /// Called by InputRegistry when an axis binding value changes.
        /// </summary>
        /// <param name="value">The current axis value (-1.0 to 1.0).</param>
        internal void RaiseAxis(float value) => AxisChanged?.Invoke(this, value);
    }

    // ══════════════════════════════════════════════════
    // INPUT BINDING (abstract)
    // ══════════════════════════════════════════════════
    /// <summary>
    /// Abstract base class for input bindings (keyboard, mouse, gamepad).
    /// Defines how a specific input maps to an action and evaluates if it's currently active.
    /// Supports serialization for saving/loading control schemes.
    /// </summary>
    public abstract class InputBinding
    {
        /// <summary>
        /// Checks if this binding is currently active based on the input state and action type.
        /// </summary>
        /// <param name="state">The current input state.</param>
        /// <param name="actionType">The type of action to check for.</param>
        /// <returns>True if the binding is active for the given action type.</returns>
        public abstract bool IsActive(InputState state, InputActionType actionType);

        /// <summary>
        /// Gets the current analog axis value for this binding.
        /// Only relevant for axis-based bindings (gamepad sticks).
        /// </summary>
        /// <param name="state">The current input state.</param>
        /// <returns>The axis value (-1.0 to 1.0), or 0 for non-axis bindings.</returns>
        public virtual float GetAxisValue(InputState state) => 0f;

        /// <summary>
        /// Serializes this binding to a string format for saving.
        /// Format: "Type:Value" (e.g., "Key:W", "Gamepad:A", "Axis:LeftStickX").
        /// </summary>
        /// <returns>A string representation of this binding.</returns>
        public abstract string Serialize();

        /// <summary>
        /// Deserializes a binding from a string format.
        /// Used for loading saved control schemes.
        /// </summary>
        /// <param name="data">The serialized binding string.</param>
        /// <returns>The deserialized InputBinding.</returns>
        /// <exception cref="ArgumentException">Thrown if the format is invalid.</exception>
        public static InputBinding Deserialize(string data)
        {
            var parts = data.Split(':');
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid binding format: {data}");

            var type = parts[0];
            var value = parts[1];

            return type switch
            {
                "Key" => ParseKeyBinding(value),
                "Mouse" => ParseMouseBinding(value),
                "Gamepad" => ParseGamepadBinding(value),
                "Axis" => ParseAxisBinding(value),
                _ => throw new ArgumentException($"Unknown binding type: {type}")
            };
        }

        private static InputBinding ParseKeyBinding(string value)
        {
            var keys = value.Split('+');
            var mainKey = Enum.Parse<KeyCode>(keys[^1]);
            var modifiers = keys.Length > 1
                ? keys[..^1].Select(k => Enum.Parse<KeyCode>(k)).ToArray()
                : [];
            return new KeyBinding(mainKey, modifiers);
        }

        private static InputBinding ParseMouseBinding(string value)
        {
            var button = Enum.Parse<MouseButton>(value);
            return new MouseButtonBinding(button);
        }

        private static InputBinding ParseGamepadBinding(string value)
        {
            var button = Enum.Parse<GamepadButton>(value);
            return new GamepadButtonBinding(button);
        }

        private static InputBinding ParseAxisBinding(string value)
        {
            var axis = Enum.Parse<GamepadAxis>(value);
            return new GamepadAxisBinding(axis);
        }
    }

    // ══════════════════════════════════════════════════
    // KEY INPUT BINDING
    // ══════════════════════════════════════════════════
    /// <summary>
    /// Represents a keyboard key binding with optional modifier keys.
    /// Supports combinations like Ctrl+S, Shift+Space, etc.
    /// </summary>
    public class KeyBinding : InputBinding
    {
        /// <summary>
        /// Gets the main key for this binding.
        /// </summary>
        public KeyCode Key { get; }

        /// <summary>
        /// Gets the modifier keys that must be held (Ctrl, Shift, Alt).
        /// Empty array if no modifiers required.
        /// </summary>
        public KeyCode[] Modifiers { get; }

        /// <summary>
        /// Initializes a new instance of the KeyBinding class.
        /// </summary>
        /// <param name="key">The main key.</param>
        /// <param name="modifiers">Optional modifier keys.</param>
        public KeyBinding(KeyCode key, params KeyCode[] modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Checks if this key binding is active based on the action type.
        /// Verifies that all modifiers are held and the main key state matches the action type.
        /// </summary>
        public override bool IsActive(InputState state, InputActionType actionType)
        {
            if (!Modifiers.All(m => state.CurrentKeys.Contains(m)))
                return false;

            return actionType switch
            {
                InputActionType.Pressed => state.CurrentKeys.Contains(Key) && !state.PreviousKeys.Contains(Key),
                InputActionType.Held => state.CurrentKeys.Contains(Key),
                InputActionType.Released => !state.CurrentKeys.Contains(Key) && state.PreviousKeys.Contains(Key),
                InputActionType.LongPress => state.KeyHoldTimes.TryGetValue(Key, out var t) && t >= 0.5f,
                _ => false
            };
        }

        /// <summary>
        /// Serializes this key binding to string format.
        /// Format: "Key:Modifier1+Modifier2+MainKey" or "Key:MainKey" if no modifiers.
        /// </summary>
        public override string Serialize()
        {
            var modPart = Modifiers.Length > 0
                ? string.Join("+", Modifiers.Select(m => m.ToString())) + "+"
                : "";
            return $"Key:{modPart}{Key}";
        }
    }

    // ══════════════════════════════════════════════════
    // MOUSE BUTTON INPUT BINDING
    // ══════════════════════════════════════════════════
    /// <summary>
    /// Represents a mouse button binding.
    /// </summary>
    public class MouseButtonBinding : InputBinding
    {
        /// <summary>
        /// Gets the mouse button for this binding.
        /// </summary>
        public MouseButton Button { get; }

        /// <summary>
        /// Initializes a new instance of the MouseButtonBinding class.
        /// </summary>
        /// <param name="button">The mouse button.</param>
        public MouseButtonBinding(MouseButton button)
        {
            Button = button;
        }

        /// <summary>
        /// Checks if this mouse button binding is active based on the action type.
        /// </summary>
        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType switch
            {
                InputActionType.Pressed => state.CurrentMouseButtons.Contains(Button) && !state.PreviousMouseButtons.Contains(Button),
                InputActionType.Held => state.CurrentMouseButtons.Contains(Button),
                InputActionType.Released => !state.CurrentMouseButtons.Contains(Button) && state.PreviousMouseButtons.Contains(Button),
                _ => false
            };
        }

        /// <summary>
        /// Serializes this mouse button binding to string format.
        /// </summary>
        public override string Serialize() => $"Mouse:{Button}";
    }

    // ══════════════════════════════════════════════════
    // GAMING BUTTON INPUT BINDING
    // ══════════════════════════════════════════════════
    /// <summary>
    /// Represents a gamepad button binding.
    /// </summary>
    public class GamepadButtonBinding : InputBinding
    {
        /// <summary>
        /// Gets the gamepad button for this binding.
        /// </summary>
        public GamepadButton Button { get; }

        /// <summary>
        /// Initializes a new instance of the GamepadButtonBinding class.
        /// </summary>
        /// <param name="button">The gamepad button.</param>
        public GamepadButtonBinding(GamepadButton button)
        {
            Button = button;
        }

        /// <summary>
        /// Checks if this gamepad button binding is active based on the action type.
        /// </summary>
        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType switch
            {
                InputActionType.Pressed => state.CurrentGamepadButtons.Contains(Button) && !state.PreviousGamepadButtons.Contains(Button),
                InputActionType.Held => state.CurrentGamepadButtons.Contains(Button),
                InputActionType.Released => !state.CurrentGamepadButtons.Contains(Button) && state.PreviousGamepadButtons.Contains(Button),
                _ => false
            };
        }

        /// <summary>
        /// Serializes this gamepad button binding to string format.
        /// </summary>
        public override string Serialize() => $"Gamepad:{Button}";
    }

    // ══════════════════════════════════════════════════
    // GAMING AXIS INPUT BINDING
    // ══════════════════════════════════════════════════
    /// <summary>
    /// Represents a gamepad analog axis binding (stick movement).
    /// Provides continuous values with deadzone support to prevent drift.
    /// </summary>
    public class GamepadAxisBinding : InputBinding
    {
        /// <summary>
        /// Gets the gamepad axis for this binding.
        /// </summary>
        public GamepadAxis Axis { get; }

        /// <summary>
        /// Gets or initializes the deadzone threshold (0.0 to 1.0).
        /// Values below this are treated as zero to prevent stick drift.
        /// Default: 0.15 (15%)
        /// </summary>
        public float Deadzone { get; init; } = 0.15f;

        /// <summary>
        /// Initializes a new instance of the GamepadAxisBinding class.
        /// </summary>
        /// <param name="axis">The gamepad axis.</param>
        public GamepadAxisBinding(GamepadAxis axis)
        {
            Axis = axis;
        }

        /// <summary>
        /// Checks if this axis binding is active (value exceeds deadzone).
        /// Only valid for Axis-type actions.
        /// </summary>
        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType == InputActionType.Axis && MathF.Abs(GetAxisValue(state)) > Deadzone;
        }

        /// <summary>
        /// Gets the current axis value with deadzone applied.
        /// Returns 0 if below deadzone threshold.
        /// </summary>
        public override float GetAxisValue(InputState state)
        {
            if (!state.GamepadAxes.TryGetValue(Axis, out var value)) return 0f;
            return MathF.Abs(value) < Deadzone ? 0f : value;
        }

        /// <summary>
        /// Serializes this axis binding to string format.
        /// </summary>
        public override string Serialize() => $"Axis:{Axis}";
    }
}