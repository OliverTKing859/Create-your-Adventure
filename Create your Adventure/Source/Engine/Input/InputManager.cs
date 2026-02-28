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
        // COMPONENTS
        // ══════════════════════════════════════════════════

        private readonly InputState state = new();
        private readonly InputRegistry registry = new();
        private InputAnalyzer? analyzer;

        // ═══ Devices
        private KeyboardDevice? keyboard;
        private MouseDevice? mouse;
        private GamepadDevice? gamepad;

        // ═══ State
        private CursorMode currentCursorMode = CursorMode.Visible;
        private bool isInitialized;
        private bool isDisposed;

        // ══════════════════════════════════════════════════
        // PUBLIC ACCESS
        // ══════════════════════════════════════════════════

        public InputAnalyzer Analyzer => analyzer ?? throw new InvalidOperationException("Not initialized");

        public InputRegistry Registry => registry;

        public bool IsInitialized => isInitialized;

        public CursorMode CurrentCursorMode => currentCursorMode;

        public bool HasKeyboard => keyboard?.IsConnected ?? false;

        public bool HasMouse => mouse?.IsConnected ?? false;

        public bool HasGamepad => gamepad?.IsConnected ?? false;

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

            var context = WindowManager.Instance.InputContext
                ?? throw new InvalidOperationException("WindowManager must be loaded first");

            // ═══ Create devices
            keyboard = new KeyboardDevice(context);
            mouse = new MouseDevice(context);
            gamepad = new GamepadDevice(context);

            // ═══ Initialize Devices
            keyboard.Initialize();
            mouse.Initialize();
            gamepad.Initialize();

            // ═══ Register Events
            keyboard.RegisterEvents(state);
            mouse.RegisterEvents(state);
            gamepad.RegisterEvents(state);

            // ═══ Create analyzer
            analyzer = new InputAnalyzer(state);

            // ═══ Register engine defaults
            registry.RegisterEngineDefaults();

            isInitialized = true;
            Logger.Info("[INPUT] InputManager initialized");
        }

        // ══════════════════════════════════════════════════
        // FRAME UPDATE (Time is injected!)
        // ══════════════════════════════════════════════════

        public void BeginFrame()
        {
            state.BeginFrame();
        }

        public void EndFrame(float deltaTime)
        {
            // ═══ Gamepad axes pollen
            gamepad?.Poll(state);

            // ═══ Process actions
            registry.ProcessActions(state);

            // ═══ Finalize state
            state.EndFrame(deltaTime);
        }

        // ══════════════════════════════════════════════════
        // CURSOR CONTROL
        // ══════════════════════════════════════════════════

        public void SetCursorMode(CursorMode mode)
        {
            currentCursorMode = mode;
            mouse?.SetCursorMode(mode);
            Logger.Info($"[INPUT] Cursor mode: {mode}");
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
        // DIRECT INPUT QUERIES (Shortcuts for Camera/Debug)
        // ══════════════════════════════════════════════════

        // ═══ Keyboard

        public bool IsKeyDown(KeyCode key) => analyzer!.IsKeyDown(key);

        public bool IsKeyPressed(KeyCode key) => analyzer!.IsKeyPressed(key);

        public bool IsDebugCombo(KeyCode mod, KeyCode key) => analyzer!.IsDebugCombo(mod, key);

        // ═══ Mouse
        public Vector2 GetMouseDelta() => analyzer!.GetMouseDelta();

        public Vector2 GetMousePosition() => analyzer!.GetMousePosition();

        // ═══ Gamepad

        public bool IsGamepadButtonDown(GamepadButton btn) => analyzer!.IsGamepadButtonDown(btn);

        public Vector2 GetLeftStick() => analyzer!.GetLeftStick();

        public Vector2 GetRightStick() => analyzer!.GetRightStick();

        // Camera Helpers
        public Vector2 GetMovementVector() => analyzer!.GetMovementVector();
        public Vector2 GetLookVector() => analyzer!.GetLookVector();

        // ══════════════════════════════════════════════════
        // ACTION QUERIES (for Gameplay)
        // ══════════════════════════════════════════════════

        public bool IsActionTriggered(string name) => registry.IsTriggered(name, state);

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════

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