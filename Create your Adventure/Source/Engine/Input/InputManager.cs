using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.GLFW;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    public sealed class InputManager : IDisposable
    {
        private static InputManager? instance;
        private static readonly Lock instanceLock = new();

        public static InputManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new InputManager();
                    }
                }

                return instance;
            }
        }

        // ══════════════════════════════════════════════════
        // FIELDS
        // ══════════════════════════════════════════════════

        // ═══ Input State (Frame-gebuffert)
        private readonly InputState state = new();

        // ═══ Registered Actions
        private readonly Dictionary<string, InputAction> actions = [];

        // ═══ Silk.NET Devices (intern)
        private IKeyboard? keyboard;
        private IMouse? mouse;
        private IGamepad? gamepad;

        // ═══ Cursor State
        private CursorMode currentCursorMode = CursorMode.Visible;
        private Vector2 lastMousePosition;

        // ═══ Time Tracking
        private float currentTime;

        private bool isInitialized;
        private bool isDisposed;

        // ══════════════════════════════════════════════════
        // PUBLIC PROPERTIES
        // ══════════════════════════════════════════════════

        public bool IsInitialized => isInitialized;
        public CursorMode CurrentCursorMode => currentCursorMode;
        public bool HasKeyboard => keyboard is not null;
        public bool HasMouse => mouse is not null;
        public bool HasGamepad => gamepad is not null;

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════

        public void Initialize()
        {
            if (isInitialized)
            {
                Logger.Warn("[INPUT] InputManager already initialized");
                return;
            }

            var inputContext = WindowManager.Instance.InputContext;

            if (inputContext is null)
            {
                throw new InvalidOperationException("WindowManager must be loaded before initializing InputManager");
            }

            // ═══ Keyboard Setup
            keyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
            if (keyboard is not null)
            {
                keyboard.KeyDown += OnKeyDown;
                keyboard.KeyUp += OnKeyUp;
                Logger.Info("[INPUT] Keyboard initialized");
            }
            else
            {
                Logger.Warn("[INPUT] No keyboard detected");
            }

            // ═══ Mouse Setup
            mouse = inputContext.Mice.Count > 0 ? inputContext.Mice[0] : null;
            if (mouse is not null)
            {
                mouse.MouseDown += OnMouseDown;
                mouse.MouseUp += OnMouseUp;
                mouse.MouseMove += OnMouseMove;
                mouse.Scroll += OnMouseScroll;
                lastMousePosition = mouse.Position;
                Logger.Info("[INPUT] Mouse initialized");
            }
            else
            {
                Logger.Warn("[INPUT] No mouse detected");
            }

            // ═══ Gamepad Setup
            gamepad = inputContext.Gamepads.Count > 0 ? inputContext.Gamepads[0] : null;
            if (gamepad is not null)
            {
                gamepad.ButtonDown += OnGamepadButtonDown;
                gamepad.ButtonUp += OnGamepadButton;
                Logger.Info($"[INPUT] Gamepad initialized: {gamepad.Name}");
            }
            else
            {
                Logger.Info("[INPUT] No gamepad detected (optional)");
            }

            RegisterDefaultActions();

            isInitialized = true;
            Logger.Info("[INPUT] InputManager initialized");
        }

        // ══════════════════════════════════════════════════
        // FRAME UPDATE
        // ══════════════════════════════════════════════════

        public void BeginFrame()
        {
            state.BeginFrame();
        }

        public void EndFrame(float deltaTime)
        {
            currentTime += deltaTime;

            if (gamepad is not null)
            {
                PollGamepadAxes();
            }

            foreach (var action in actions.Values)
            {
                CheckAction(action);
            }

            state.EndFrame(deltaTime);
        }

        private void PollGamepadAxes()
        {
            var thumbsticks = gamepad!.Thumbsticks;

            if (thumbsticks.Count >= 2)
            {
                state.SetGamepadAxis(GamepadAxis.LeftStickX, thumbsticks[0].X);
                state.SetGamepadAxis(GamepadAxis.LeftStickY, thumbsticks[0].Y);
                state.SetGamepadAxis(GamepadAxis.RightStickX, thumbsticks[1].X);
                state.SetGamepadAxis(GamepadAxis.RightStickY, thumbsticks[1].Y);
            }

            var triggers = gamepad.Triggers;
            if (triggers.Count >= 2)
            {
                state.SetGamepadAxis(GamepadAxis.LeftTrigger, triggers[0].Position);
                state.SetGamepadAxis(GamepadAxis.RightTrigger, triggers[1].Position);
            }
        }

        private void CheckAction(InputAction action)
        {
            foreach (var binding in action.Bindings)
            {
                if (binding.IsActive(state, action.Type))
                {
                    if (action.Type == InputActionType.Axis)
                    {
                        action.RaiseAxis(binding.GetAxisValue(state));
                    }
                    else
                    {
                        action.RaiseTrigger();
                    }

                    break; // ═══ Nur ein Binding pro Frame auslösen 
                }
            }
        }

        // ══════════════════════════════════════════════════
        // CURSOR CONTROL
        // ══════════════════════════════════════════════════

        public void SetCursorMode(CursorMode mode)
        {
            if (mouse is null) return;

            currentCursorMode = mode;

            switch (mode)
            {
                case CursorMode.Visible:
                    mouse.Cursor.CursorMode = Silk.NET.Input.CursorMode.Normal;
                    break;

                case CursorMode.Hidden:
                    mouse.Cursor.CursorMode = Silk.NET.Input.CursorMode.Hidden;
                    break;

                case CursorMode.Locked:
                    mouse.Cursor.CursorMode = Silk.NET.Input.CursorMode.Disabled;
                    break;

                case CursorMode.Confined:
                    mouse.Cursor.CursorMode = Silk.NET.Input.CursorMode.Normal;
                    // TODO: Window-Clipping !!!
                    break;

                case CursorMode.ConfinedHidden:
                    mouse.Cursor.CursorMode = Silk.NET.Input.CursorMode.Hidden;
                    // TODO: Window-Clipping !!!
                    break;
            }

            Logger.Info($"[INPUT] Cursor mode set to: {mode}");
        }

        public void LockCursor() => SetCursorMode(CursorMode.Locked);

        public void UnlockCursor() => SetCursorMode(CursorMode.Visible);

        public void ToggleCursorLock()
        {
            if (currentCursorMode == CursorMode.Locked)
                UnlockCursor();
            else
                LockCursor();
        }

        // ══════════════════════════════════════════════════
        // DIRECT INPUT QUERIES (for Camera, etc.)
        // ══════════════════════════════════════════════════

        // ═══ Keyboard

        public bool IsKeyDown(KeyCode key) => state.IsKeyDown(key);

        public bool IsKeyPressed(KeyCode key) => state.IsKeyPressed(key);

        public bool IsKeyReleased(KeyCode key) => state.IsKeyReleased(key);

        public bool IsKeyLongPressed(KeyCode key) => state.IsKeyLongPressed(key);
        public float GetKeyHoldTime(KeyCode key) => state.GetKeyHoldTime(key);

        // ═══ Keyboard Combi.
        public bool IsKeyCombinationPressed(KeyCode mainKey, params KeyCode[] modifiers)
            => state.IsKeyCombinationPressed(mainKey, modifiers);

        // ══════════════════════════════════════════════════
        // ═══ Mouse

        public bool IsMouseButtonDown(MouseButton button) => state.IsMouseButtonDown(button);

        public bool IsMouseButtonPressed(MouseButton button) => state.IsMouseButtonPressed(button);

        public Vector2 GetMousePosition() => state.GetMousePosition();

        public Vector2 GetMouseDelta() => state.GetMouseDelta();

        public float GetScrollDelta() => state.GetScrollDelta();

        // ══════════════════════════════════════════════════
        // ═══ Gamepad

        public bool IsGamepadButtonDown(GamepadButton button) => state.IsGamepadButtonDown(button);

        public bool IsGamepadButtonPressed(GamepadButton button) => state.IsGamepadButtonPressed(button);

        public float GetGamepadAxis(GamepadAxis axis) => state.GetGamepadAxis(axis);

        public Vector2 GetLeftStick() => state.GetLeftStick();

        public Vector2 GetRightStick() => state.GetRightStick();

        public Vector2 GetMovementVector()
        {
            // ═══ Keyboard has Priority
            Vector2 movement = Vector2.Zero;

            if (IsKeyDown(KeyCode.W)) movement.Y += 1f;
            if (IsKeyDown(KeyCode.S)) movement.Y -= 1f;
            if (IsKeyDown(KeyCode.A)) movement.X -= 1f;
            if (IsKeyDown(KeyCode.D)) movement.X += 1f;

            if (movement != Vector2.Zero)
                return Vector2.Normalize(movement);

            // ═══ Fallback of Gamepad
            var stick = GetLeftStick();
            if (stick != Vector2.Zero)
                return stick;

            return Vector2.Zero;
        }

        // ══════════════════════════════════════════════════
        // ═══ Get Looking
        public Vector2 GetLookVector()
        {
            var mouseDelta = GetMouseDelta();
            if (mouseDelta != Vector2.Zero)
                return mouseDelta;

            return GetRightStick();
        }


        // ══════════════════════════════════════════════════
        // ACTION SYSTEM
        // ══════════════════════════════════════════════════

        public InputAction RegisterAction(string name, InputActionType type)
        {
            if (actions.ContainsKey(name))
            {
                Logger.Warn($"[INPUT] Action '{name}' already registered");
                return actions[name];
            }

            var action = new InputAction(name, type);
            actions[name] = action;
            Logger.Info($"[INPUT] Action '{name}' registered (Type: {type})");
            return action;
        }

        public InputAction? GetAction(string name)
            => actions.TryGetValue(name, out var action) ? action : null;

        public bool IsActionTriggered(string name)
        {
            if (!actions.TryGetValue(name, out var action)) return false;

            foreach (var binding in action.Bindings)
            {
                if (binding.IsActive(state, action.Type))
                    return true;
            }

            return false;
        }

        private void RegisterDefaultActions()
        {
            // ═══ Movement
            RegisterAction("MoveForward", InputActionType.Held)
                .AddKeyBinding(KeyCode.W)
                .AddGamepadBinding(GamepadButton.DPadUp);

            RegisterAction("MoveBackward", InputActionType.Held)
                .AddKeyBinding(KeyCode.S)
                .AddGamepadBinding(GamepadButton.DPadDown);

            RegisterAction("MoveLeft", InputActionType.Held)
                .AddKeyBinding(KeyCode.A)
                .AddGamepadBinding(GamepadButton.DPadLeft);

            RegisterAction("MoveRight", InputActionType.Held)
                .AddKeyBinding(KeyCode.D)
                .AddGamepadBinding(GamepadButton.A);

            RegisterAction("MoveUp", InputActionType.Held)
                .AddKeyBinding(KeyCode.Space)
                .AddGamepadBinding(GamepadButton.RightBumper);

            RegisterAction("MoveDown", InputActionType.Held)
                .AddKeyBinding(KeyCode.LeftControl)
                .AddGamepadBinding(GamepadButton.LeftBumper);

            RegisterAction("Sprint", InputActionType.Held)
                .AddKeyBinding(KeyCode.LeftShift)
                .AddGamepadBinding(GamepadButton.LeftStick);

            // ═══ Camera
            RegisterAction("ToggleCursorLock", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Escape);

            /*
            // ═══ Game Actions
            RegisterAction("Jump", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Space)
                .AddGamepadBinding(GamepadButton.LeftBumper);

            RegisterAction("Interact", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.F)
                .AddGamepadBinding(GamepadButton.Y);

            RegisterAction("Inventory", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Tab)
                .AddGamepadBinding(GamepadButton.X);

            // ═══ System
            RegisterAction("Save", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.S, KeyCode.LeftControl);

            RegisterAction("Load", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.L, KeyCode.LeftControl);

            RegisterAction("Pause", InputActionType.Pressed)
                .AddKeyBinding(KeyCode.Escape)
                .AddGamepadBinding(GamepadButton.Start);*/
            
        Logger.Info($"[INPUT] {actions.Count} default actions registered");

        }

        // ══════════════════════════════════════════════════
        // SETTINGS SAVE/LOAD
        // ══════════════════════════════════════════════════
        /*
        public string ExportBindings()
        {
            var bindings = new Dictionary<string, List<string>>();

            foreach (var (name, action) in actions)
            {
                bindings[name] = action.Bindings.Select(b => b.Serialize()).ToList();
            }

            return System.Text.Json.JsonSerializer.Serialize(bindings, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public void ImportBindings(string json)
        {
            var bindings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

            if (bindings is null) return;

            foreach (var (name, bindingStrings) in bindings)
            {
                if (!actions.TryGetValue(name, out var action)) continue;

                action.Bindings.Clear();
                foreach (var str in bindingStrings)
                {
                    try
                    {
                        action.Bindings.Add(InputBinding.Deserialize(str));
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"[INPUT] Failed to deserialize binding '{str}': {ex.Message}");
                    }
                }
            }

            Logger.Info("[INPUT] Bindings imported from settings");
        }
        */

        // ══════════════════════════════════════════════════
        // EVENT HANDLERS (Silk.NET -> InputState)
        // ══════════════════════════════════════════════════

        private void OnKeyDown(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = ConvertKey(key);
            if (keyCode.HasValue)
                state.SetKeyDown(keyCode.Value);
        }

        private void OnKeyUp(IKeyboard kb, Key key, int scancode)
        {
            var keyCode = ConvertKey(Key);
            if (keyCode.HasValue)
                state.SetKeyDown(keyCode.Value);
        }

        private void OnMouseDown(IMouse m, Silk.NET.Input.MouseButton button)
        {
            var btn = ConvertMouseButton(button);
            if (btn.HasValue)
                state.SetMouseButtonUp(btn.Value);
        }

        private void OnMouseMove(IMouse m, Vector2 position)
        {
            var delta = position - lastMousePosition;
            lastMousePosition = position;

            state.SetMousePosition(position);
            state.SetMouseDelta(delta);
        }

        private void OnMouseScroll(IMouse m, ScrollWheel scroll)
        {
            state.SetScrollDelta(scroll.Y);
        }

        private void OnGamepadButtonDown(IGamepad gp, Button button)
        {
            var btn = ConvertGamepadButton(button.Name);
            if (btn.HasValue)
                state.SetGamepadButtonDown(btn.Value);
        }

        private void OnGamepadButtonUp(IGamepad gp, Button button)
        {
            var btn = ConvertGamepadButton(button.Name);
            if (btn.HasValue)
                state.SetGamepadButtonUp(btn.Value);
        }

        // ══════════════════════════════════════════════════
        // CONVERTERS (Silk.NET -> Custom Enums)
        // ══════════════════════════════════════════════════

        private static KeyCode? ConvertKey(Key key)
        {
            // ═══ Mapping from Silk.NET Key to KeyCode
            return key switch
            {
                Key.A => KeyCode.A,
                Key.B => KeyCode.B,
                Key.C => KeyCode.C,
                Key.D => KeyCode.D,
                Key.E => KeyCode.E,
                Key.F => KeyCode.F,
                Key.G => KeyCode.G,
                Key.H => KeyCode.H,
                Key.I => KeyCode.I,
                Key.J => KeyCode.J,
                Key.K => KeyCode.K,
                Key.L => KeyCode.L,
                Key.M => KeyCode.M,
                Key.N => KeyCode.N,
                Key.O => KeyCode.O,
                Key.P => KeyCode.P,
                Key.Q => KeyCode.Q,
                Key.R => KeyCode.R,
                Key.S => KeyCode.S,
                Key.T => KeyCode.T,
                Key.U => KeyCode.U,
                Key.V => KeyCode.V,
                Key.W => KeyCode.W,
                Key.X => KeyCode.X,
                Key.Y => KeyCode.Y,
                Key.Z => KeyCode.Z,

                Key.Number0 => KeyCode.Number0,
                Key.Number1 => KeyCode.Number1,
                Key.Number2 => KeyCode.Number2,
                Key.Number3 => KeyCode.Number3,
                Key.Number4 => KeyCode.Number4,
                Key.Number5 => KeyCode.Number5,
                Key.Number6 => KeyCode.Number6,
                Key.Number7 => KeyCode.Number7,
                Key.Number8 => KeyCode.Number8,
                Key.Number9 => KeyCode.Number9,

                Key.ShiftLeft => KeyCode.LeftShift,
                Key.ShiftRight => KeyCode.RightShift,
                Key.ControlLeft => KeyCode.LeftControl,
                Key.ControlRight => KeyCode.RightControl,
                Key.AltLeft => KeyCode.LeftAlt,
                Key.AltRight => KeyCode.RightAlt,

                Key.Up => KeyCode.Up,
                Key.Down => KeyCode.Down,
                Key.Left => KeyCode.Left,
                Key.Right => KeyCode.Right,
                Key.Home => KeyCode.Home,
                Key.End => KeyCode.End,
                Key.PageUp => KeyCode.PageUp,
                Key.PageDown => KeyCode.PageDown,
                Key.Insert => KeyCode.Insert,
                Key.Delete => KeyCode.Delete,

                Key.Space => KeyCode.Space,
                Key.Enter => KeyCode.Enter,
                Key.Escape => KeyCode.Escape,
                Key.Tab => KeyCode.Tab,
                Key.Backspace => KeyCode.Backspace,
                Key.CapsLock => KeyCode.CapsLock,
                Key.NumLock => KeyCode.NumLock,
                Key.ScrollLock => KeyCode.ScrollLock,
                Key.PrintScreen => KeyCode.PrintScreen,
                Key.Pause => KeyCode.Pause,

                Key.Keypad0 => KeyCode.Numpad0,
                Key.Keypad1 => KeyCode.Numpad1,
                Key.Keypad2 => KeyCode.Numpad2,
                Key.Keypad3 => KeyCode.Numpad3,
                Key.Keypad4 => KeyCode.Numpad4,
                Key.Keypad5 => KeyCode.Numpad5,
                Key.Keypad6 => KeyCode.Numpad6,
                Key.Keypad7 => KeyCode.Numpad7,
                Key.Keypad8 => KeyCode.Numpad8,
                Key.Keypad9 => KeyCode.Numpad9,
                Key.KeypadAdd => KeyCode.NumpadAdd,
                Key.KeypadSubtract => KeyCode.NumpadSubstract,
                Key.KeypadMultiply => KeyCode.NumpadMultiply,
                Key.KeypadDivide => KeyCode.NumpadDivide,
                Key.KeypadEnter => KeyCode.NumpadEnter,
                Key.KeypadDecimal => KeyCode.NumpadDecimal,

                Key.GraveAccent => KeyCode.Grave,
                Key.Minus => KeyCode.Minus,
                Key.Equal => KeyCode.Equals,
                Key.LeftBracket => KeyCode.LeftBracket,
                Key.RightBracket => KeyCode.RightBracket,
                Key.BackSlash => KeyCode.Backslash,
                Key.Semicolon => KeyCode.Semicolon,
                Key.Apostrophe => KeyCode.Apostrophe,
                Key.Comma => KeyCode.Comma,
                Key.Period => KeyCode.Period,
                Key.Slash => KeyCode.Slash
            };
        }

        private static MouseButton? ConvertMouseButton(Silk.NET.Input.MouseButton button)
        {
            return button switch
            {
                Silk.NET.Input.MouseButton.Left => MouseButton.Left,
                Silk.NET.Input.MouseButton.Right => MouseButton.Right,
                Silk.NET.Input.MouseButton.Middle => MouseButton.Middle,
                Silk.NET.Input.MouseButton.Button4 => MouseButton.Button4,
                Silk.NET.Input.MouseButton.Button5 => MouseButton.Button5
            };
        }

        private static GamepadButton? ConvertGamepadButton(ButtonName name)
        {
            return name switch
            {
                ButtonName.A => GamepadButton.A,
                ButtonName.B => GamepadButton.B,
                ButtonName.X => GamepadButton.X,
                ButtonName.Y => GamepadButton.Y,

                ButtonName.LeftBumper => GamepadButton.LeftBumper,
                ButtonName.RightBumper => GamepadButton.RightBumper,
                ButtonName. => GamepadButton.LeftTrigger,
                ButtonName. => GamepadButton.RightTrigger,

                ButtonName.LeftStick => GamepadButton.LeftStick,
                ButtonName.RightStick => GamepadButton.RightStick,

                ButtonName.DPadUp => GamepadButton.DPadUp,
                ButtonName.DPadDown => GamepadButton.DPadDown,
                ButtonName.DPadLeft => GamepadButton.DPadLeft,
                ButtonName.DPadRight => GamepadButton.DPadRight,

                ButtonName.Start => GamepadButton.Start,
                ButtonName.Back => GamepadButton.Back,
                ButtonName.Home => GamepadButton.Guide
            };
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════

        public void Dispose()
        {
            if (isDisposed) return;

            // ═══ Events log out
            if (keyboard is not null)
            {
                keyboard.KeyDown -= OnKeyDown;
                keyboard.KeyUp -= OnKeyUp;
            }

            if (mouse is not null)
            {
                mouse.MouseDown -= OnMouseDown;
                mouse.MouseUp -= OnMouseUp;
                mouse.MouseMove -= OnMouseScroll;
            }

            if (gamepad is not null)
            {
                gamepad.ButtonDown -= OnGamepadButtonDown;
                gamepad.ButtonUp -= OnGamepadButtonUp;
            }

            actions.Clear();
            isDisposed = true;

            Logger.Info("[INPUT] InputManager disposed");
        }
    }
}
