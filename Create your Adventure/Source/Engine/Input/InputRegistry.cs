using Create_your_Adventure.Source.Debug;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public sealed class InputRegistry
    {
        private readonly Dictionary<string, InputAction> actions = [];

        public int ActionCount => actions.Count;

        public InputAction Register(string name, InputActionType type)
        {
            if (actions.TryGetValue(name, out var existing))
            {
                Logger.Warn($"[INPUT] Action '{name}' already registered");
                return existing;
            }

            var action = new InputAction(name, type);
            actions[name] = action;
            return action;
        }

        public InputAction? Get(string name)
            => actions.TryGetValue(name, out var action) ? action : null;

        public bool IsTriggered(string name, InputState state)
        {
            if (!actions.TryGetValue(name, out var action)) return false;

            foreach (var binding in action.Bindings)
            {
                if (binding.IsActive(state, action.Type))
                    return true;
            }

            return false;
        }

        internal void ProcessActions(InputState state)
        {
            foreach (var action in actions.Values)
            {
                foreach (var binding in action.Bindings)
                {
                    if (binding.IsActive(state, action.Type))
                    {
                        if (action.Type == InputActionType.Axis)
                            action.RaiseAxis(binding.GetAxisValue(state));
                        else
                            action.RaiseTrigger();
                        break;
                    }
                }
            }
        }

        public void RegisterEngineDefaults()
        {
            // ═══ ONLY Engine-relevante actions!
            Register("ToggleCursorLock", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Escape);

            // ═══ Movement (for Gameplay, NOT for camera!)
            Register("MoveForward", InputActionType.Held)
                .AddKeyBinding(KeyCode.W)
                .AddGamepadBinding(GamepadButton.DPadUp);

            Register("MoveBackward", InputActionType.Held)
                .AddKeyBinding(KeyCode.S)
                .AddGamepadBinding(GamepadButton.DPadDown);

            Register("MoveLeft", InputActionType.Held)
                .AddKeyBinding(KeyCode.A)
                .AddGamepadBinding(GamepadButton.DPadLeft);

            Register("MoveRight", InputActionType.Held)
                .AddKeyBinding(KeyCode.D)
                .AddGamepadBinding(GamepadButton.DPadRight);

            Register("Sprint", InputActionType.Held)
                .AddKeyBinding(KeyCode.LeftShift)
                .AddGamepadBinding(GamepadButton.Y);

            Logger.Info($"[INPUT] {ActionCount} engine actions registered");
        }

        public void Clear() => actions.Clear();
    }
}