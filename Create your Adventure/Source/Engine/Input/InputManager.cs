using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Input.Devices;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.GLFW;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InputManager : IDisposable
    {
        // ═══ Singleton instance of the InputManager
        private static InputManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the InputManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
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
        // COMPONENTS
        // ══════════════════════════════════════════════════
        // ═══ Core input state storage
        private readonly InputState state = new();
        // ═══ Action registry for gameplay bindings
        private readonly InputRegistry registry = new();
        // ═══ High-level query interface
        private InputAnalyzer? analyzer;

        // ═══ Input devices
        private KeyboardDevice? keyboard;
        private MouseDevice? mouse;
        private GamepadDevice? gamepad;

        // ═══ Manager state
        private CursorMode currentCursorMode = CursorMode.Visible;
        private bool isInitialized;
        private bool isDisposed;

        // ══════════════════════════════════════════════════
        // PUBLIC ACCESS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the input analyzer for high-level input queries.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if accessed before initialization.</exception>
        public InputAnalyzer Analyzer => analyzer ?? throw new InvalidOperationException("Not initialized");

        /// <summary>
        /// Gets the input action registry for managing gameplay actions.
        /// </summary>
        public InputRegistry Registry => registry;

        /// <summary>
        /// Gets a value indicating whether the input manager has been initialized.
        /// </summary>
        public bool IsInitialized => isInitialized;

        /// <summary>
        /// Gets the current cursor display mode.
        /// </summary>
        public CursorMode CurrentCursorMode => currentCursorMode;

        /// <summary>
        /// Gets a value indicating whether a keyboard is connected.
        /// </summary>
        public bool HasKeyboard => keyboard?.IsConnected ?? false;

        /// <summary>
        /// Gets a value indicating whether a mouse is connected.
        /// </summary>
        public bool HasMouse => mouse?.IsConnected ?? false;

        /// <summary>
        /// Gets a value indicating whether a gamepad is connected (optional).
        /// </summary>
        public bool HasGamepad => gamepad?.IsConnected ?? false;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private InputManager()
        { 
        
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the input manager and all connected input devices.
        /// Creates devices, registers event handlers, and sets up default actions.
        /// Must be called after WindowManager initialization.
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
            {
                Logger.Warn("[INPUT] InputManager already initialized");
                return;
            }

            var context = WindowManager.Instance.InputContext
                ?? throw new InvalidOperationException("WindowManager must be loaded first");

            // ═══ Create input devices
            keyboard = new KeyboardDevice(context);
            mouse = new MouseDevice(context);
            gamepad = new GamepadDevice(context);

            // ═══ Initialize each device
            keyboard.Initialize();
            mouse.Initialize();
            gamepad.Initialize();

            // ═══ Register device event handlers
            keyboard.RegisterEvents(state);
            mouse.RegisterEvents(state);
            gamepad.RegisterEvents(state);

            // ═══ Create analyzer for high-level queries
            analyzer = new InputAnalyzer(state);

            // ═══ Register default engine actions
            registry.RegisterEngineDefaults();

            isInitialized = true;
            Logger.Info("[INPUT] InputManager initialized");
        }

        // ══════════════════════════════════════════════════
        // FRAME UPDATE (Time is injected!)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Called at the start of each frame to prepare input state.
        /// Copies current state to previous state for edge detection.
        /// </summary>
        public void BeginFrame()
        {
            state.BeginFrame();
        }

        /// <summary>
        /// Called at the end of each frame to finalize input processing.
        /// Polls gamepad analog inputs, processes actions, and updates hold times.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame in seconds.</param>
        public void EndFrame(float deltaTime)
        {
            // ═══ Poll gamepad analog axes (continuous sampling required)
            gamepad?.Poll(state);

            // ═══ Process all registered actions and trigger events
            registry.ProcessActions(state);

            // ═══ Finalize state (update hold times, cleanup)
            state.EndFrame(deltaTime);
        }

        // ══════════════════════════════════════════════════
        // CURSOR CONTROL
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets the cursor display mode (visible, hidden, locked, etc.).
        /// </summary>
        /// <param name="mode">The desired cursor mode.</param>
        public void SetCursorMode(CursorMode mode)
        {
            currentCursorMode = mode;
            mouse?.SetCursorMode(mode);
            Logger.Info($"[INPUT] Cursor mode: {mode}");
        }

        /// <summary>
        /// Locks the cursor to the center of the window (ideal for FPS camera control).
        /// </summary>
        public void LockCursor() => SetCursorMode(CursorMode.Locked);

        /// <summary>
        /// Unlocks the cursor and makes it visible (standard UI interaction mode).
        /// </summary>
        public void UnlockCursor() => SetCursorMode(CursorMode.Visible);

        /// <summary>
        /// Toggles between locked and visible cursor modes.
        /// </summary>
        public void ToggleCursorLock()
        {
            if (currentCursorMode == CursorMode.Locked)
                UnlockCursor();
            else
                LockCursor();
        }

        // ══════════════════════════════════════════════════
        // DIRECT INPUT QUERIES (Shortcuts for Camera/Debug)
        // ══════════════════════════════════════════════════

        // ═══ Keyboard
        /// <summary>Checks if a key is currently held down.</summary>
        public bool IsKeyDown(KeyCode key) => analyzer!.IsKeyDown(key);

        /// <summary>Checks if a key was just pressed this frame.</summary>
        public bool IsKeyPressed(KeyCode key) => analyzer!.IsKeyPressed(key);

        /// <summary>Checks if a debug key combination is active (modifier + key).</summary>
        public bool IsDebugCombo(KeyCode mod, KeyCode key) => analyzer!.IsDebugCombo(mod, key);

        // ═══ Mouse
        /// <summary>Gets the mouse movement delta since last frame.</summary>
        public Vector2 GetMouseDelta() => analyzer!.GetMouseDelta();

        /// <summary>Gets the current mouse cursor position.</summary>
        public Vector2 GetMousePosition() => analyzer!.GetMousePosition();

        // ═══ Gamepad
        /// <summary>Checks if a gamepad button is currently held down.</summary>
        public bool IsGamepadButtonDown(GamepadButton btn) => analyzer!.IsGamepadButtonDown(btn);

        /// <summary>Gets the left analog stick position with deadzone applied.</summary>
        public Vector2 GetLeftStick() => analyzer!.GetLeftStick();

        /// <summary>Gets the right analog stick position with deadzone applied.</summary>
        public Vector2 GetRightStick() => analyzer!.GetRightStick();

        // Camera Helpers
        /// <summary>
        /// Gets a normalized movement vector based on WASD or left stick.
        /// Helper method for quick camera/character movement implementation.
        /// </summary>
        public Vector2 GetMovementVector() => analyzer!.GetMovementVector();

        /// <summary>
        /// Gets a look/rotation vector based on mouse delta or right stick.
        /// Helper method for quick camera rotation implementation.
        /// </summary>
        public Vector2 GetLookVector() => analyzer!.GetLookVector();

        // ══════════════════════════════════════════════════
        // ACTION QUERIES (for Gameplay)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks if a registered gameplay action is currently triggered.
        /// Use this for gameplay logic instead of direct input queries for better rebinding support.
        /// </summary>
        /// <param name="name">The name of the action to check.</param>
        /// <returns>True if any binding for this action is active.</returns>
        public bool IsActionTriggered(string name) => registry.IsTriggered(name, state);

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases all input resources and unregisters event handlers.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            keyboard?.Dispose();
            mouse?.Dispose();
            gamepad?.Dispose();
            registry.Clear();

            isDisposed = true;

            Logger.Info("[INPUT] InputManager disposed");
        }
    }
}