using Create_your_Adventure.Source.Debug;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Central registry for managing input actions and their bindings.
    /// Stores all registered actions and processes them each frame to trigger events.
    /// Provides fluent API for registering actions with multiple bindings.
    /// </summary>
    public sealed class InputRegistry
    {
        // ═══ Dictionary mapping action names to InputAction instances
        private readonly Dictionary<string, InputAction> actions = [];

        /// <summary>
        /// Gets the total number of registered actions.
        /// </summary>
        public int ActionCount => actions.Count;

        /// <summary>
        /// Registers a new input action with the specified name and type.
        /// Returns the created action for binding configuration (fluent API).
        /// </summary>
        /// <param name="name">Unique name for the action (e.g., "Jump", "Fire").</param>
        /// <param name="type">The action type (Pressed, Held, Released, etc.).</param>
        /// <returns>The registered InputAction for method chaining.</returns>
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

        /// <summary>
        /// Retrieves a registered action by name.
        /// </summary>
        /// <param name="name">The name of the action to retrieve.</param>
        /// <returns>The InputAction if found, null otherwise.</returns>
        public InputAction? Get(string name)
            => actions.TryGetValue(name, out var action) ? action : null;

        /// <summary>
        /// Checks if a registered action is currently triggered based on input state.
        /// Evaluates all bindings for the action and returns true if any are active.
        /// </summary>
        /// <param name="name">The name of the action to check.</param>
        /// <param name="state">The current input state.</param>
        /// <returns>True if any binding for this action is active.</returns>
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

        /// <summary>
        /// Processes all registered actions and raises events for active bindings.
        /// Called by InputManager at the end of each frame.
        /// </summary>
        /// <param name="state">The current input state.</param>
        internal void ProcessActions(InputState state)
        {
            foreach (var action in actions.Values)
            {
                foreach (var binding in action.Bindings)
                {
                    if (binding.IsActive(state, action.Type))
                    {
                        // ═══ Raise appropriate event based on action type
                        if (action.Type == InputActionType.Axis)
                            action.RaiseAxis(binding.GetAxisValue(state));
                        else
                            action.RaiseTrigger();
                        break;  // ═══ Only trigger once per action per frame
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════
        // REGISTER ENGINE DEFAULTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Registers default engine-level actions (movement, cursor lock, etc.).
        /// Called automatically during InputManager initialization.
        /// Game-specific actions should be registered separately.
        /// </summary>
        public void RegisterEngineDefaults()
        {
            // ═══ Cursor control (engine-level)
            Register("ToggleCursorLock", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Escape);

            // ═══ Basic movement actions (can be overridden or extended by game code)
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

            Register("MoveUp", InputActionType.Held)
                .AddKeyBinding(KeyCode.Space)
                .AddGamepadBinding(GamepadButton.RightBumper);

            Register("MoveDown", InputActionType.Held)
                .AddKeyBinding(KeyCode.LeftControl)
                .AddGamepadBinding(GamepadButton.LeftBumper);

            Logger.Info($"[INPUT] {ActionCount} engine actions registered");
        }

        /// <summary>
        /// Clears all registered actions from the registry.
        /// </summary>
        public void Clear() => actions.Clear();
    }
}