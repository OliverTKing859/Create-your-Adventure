using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public class InputAction
    {
        public string Name { get; }

        public InputActionType Type { get; }

        public List<InputBinding> Bindings { get; } = [];

        public event Action<InputAction>? Triggered;

        public event Action<InputAction, float>? AxisChanged;

        public InputAction(string name, InputActionType type)
        {
            Name = name;
            Type = type;
        }

        public InputAction AddKeyBinding(KeyCode key, params KeyCode[] modifiers)
        {
            Bindings.Add(new KeyBinding(key, modifiers));
            return this;
        }

        public InputAction AddGamepadBinding(GamepadButton button)
        {
            Bindings.Add(new GamepadButtonBinding(button));
            return this;
        }

        public InputAction AddAxisBinding(GamepadAxis axis)
        {
            Bindings.Add(new GamepadAxisBinding(axis));
            return this;
        }

        internal void RaiseTrigger() => Triggered?.Invoke(this);
        internal void RaiseAxis(float value) => AxisChanged?.Invoke(this, value);
    }

    public abstract class InputBinding
    {
        public abstract bool IsActive(InputState state, InputActionType actionType);

        public virtual float GetAxisValue(InputState state) => 0f;

        public abstract string Serialize();

        public static InputBinding Deserialize(string data)
        {
            var parts = data.Split(':');
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

        // ═══ Additional parse methods
    }

    public class KeyBinding : InputBinding
    {
        public KeyCode Key { get; }
        public KeyCode[] Modifiers { get; }

        public KeyBinding(KeyCode key, params KeyCode[] modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override bool IsActive(InputState state, InputActionType actionType)
        {
            if (!Modifiers.All(m => state.IsKeyDown(m)))
                return false;

            return actionType switch
            {
                InputActionType.Pressed => state.IsKeyPressed(Key),
                InputActionType.Held => state.IsKeyDown(Key),
                InputActionType.Released => state.IsKeyReleased(Key),
                InputActionType.LongPress => state.IsKeyLongPressed(Key),
                _ => false
            };
        }

        public override string Serialize()
        {
            var modPart = Modifiers.Length > 0
                ? string.Join("+", Modifiers.Select(m => m.ToString())) + "+"
                : "";
            return $"Key:{modPart}{Key}";
        }
    }

    public class MouseButtonBinding : InputBinding
    {
        public MouseButton Button { get; }

        public MouseButtonBinding(MouseButton button)
        {
            Button = button;
        }

        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType switch
            {
                InputActionType.Pressed => state.IsMouseButtonPressed(Button),
                InputActionType.Held => state.IsMouseButtonDown(Button),
                InputActionType.Released => state.IsMouseButtonReleased(Button),
                _ => false
            };
        }

        public override string Serialize() => $"Mouse:{Button}";
    }

    public class GamepadButtonBinding : InputBinding
    {
        public GamepadButton Button { get; }

        public GamepadButtonBinding(GamepadButton button)
        {
            Button = button;
        }

        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType switch
            {
                InputActionType.Pressed => state.IsGamepadButtonPressed(Button),
                InputActionType.Held => state.IsGamepadButtonPressed(Button),
                _ => false
            };
        }

        public override string Serialize() => $"Gamepad:{Button}";
    }

    public class GamepadAxisBinding : InputBinding
    {
        public GamepadAxis Axis { get; }
        public float Deadzone { get; init; } = 0.15f;

        public GamepadAxisBinding(GamepadAxis axis)
        {
            Axis = axis;
        }

        public override bool IsActive(InputState state, InputActionType actionType)
        {
            return actionType == InputActionType.Axis && MathF.Abs(GetAxisValue(state)) > Deadzone;
        }

        public override float GetAxisValue(InputState state)
        {
            return state.GetGamepadAxis(Axis, Deadzone);
        }

        public override string Serialize() => $"Axis:{Axis}";
    }
}