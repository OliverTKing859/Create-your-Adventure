using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System.Numerics;
using static Create_your_Adventure.Source.Engine.Input.InputConverter;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    /// <summary>
    /// Manages mouse input including button presses, movement, and scroll wheel.
    /// Tracks cursor position, delta movement, and provides cursor mode control (visible, hidden, locked).
    /// Uses event-based input (no polling required).
    /// </summary>
    public sealed class MouseDevice : IInputDevice
    {
        // ═══ The underlying Silk.NET mouse interface
        private readonly IMouse? mouse;
        // ═══ Reference to the input state for recording mouse events
        private InputState? state;
        // ═══ Last known mouse position for calculating delta movement
        private Vector2 lastPosition;
        // ═══ Flag to skip first delta after mode change (prevents jump)
        private bool skipNextDelta;

        /// <summary>
        /// Gets the name of the mouse device.
        /// </summary>
        public string Name => mouse?.Name ?? "Unknown Mouse";

        /// <summary>
        /// Gets a value indicating whether a mouse is connected.
        /// </summary>
        public bool IsConnected => mouse is not null;

        /// <summary>
        /// Gets the raw Silk.NET mouse interface for advanced operations.
        /// </summary>
        public IMouse? RawMouse => mouse;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the MouseDevice class.
        /// Attempts to use the first available mouse from the input context.
        /// </summary>
        /// <param name="context">The Silk.NET input context containing available mice.</param>
        public MouseDevice(IInputContext context)
        {
            mouse = context.Mice.Count > 0 ? context.Mice[0] : null;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the mouse device and stores the initial cursor position.
        /// </summary>
        public void Initialize()
        {
            if (IsConnected)
            {
                lastPosition = mouse!.Position;
                Logger.Info($"[INPUT] Mouse initialized: {Name}");
            }
            else
            {
                Logger.Warn("[INPUT] No mouse detected");
            }
        }

        // ══════════════════════════════════════════════════
        // REGISTER EVENTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Registers mouse event handlers for button presses, movement, and scrolling.
        /// </summary>
        /// <param name="inputState">The input state object to update with mouse events.</param>
        public void RegisterEvents(InputState inputState)
        {
            if (mouse is null) return;

            state = inputState;
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnScroll;
        }

        /// <summary>
        /// Unregisters all mouse event handlers.
        /// </summary>
        public void UnregisterEvents()
        {
            if (mouse is null) return;

            mouse.MouseDown -= OnMouseDown;
            mouse.MouseUp -= OnMouseUp;
            mouse.MouseMove -= OnMouseMove;
            mouse.Scroll -= OnScroll;
        }

        // ══════════════════════════════════════════════════
        // POLL
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Polling is not required for mouse input (event-driven).
        /// </summary>
        /// <param name="state">Unused for mouse.</param>
        public void Poll(InputState state) { } // ═══ Mouse uses event-driven input

        // ══════════════════════════════════════════════════
        // ON MOUSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Internal handler for mouse button press events.
        /// </summary>
        private void OnMouseUp(IMouse m, Silk.NET.Input.MouseButton button)
        {
            var btn = MouseConverter.Convert(button);
            if (btn.HasValue)
                state?.SetMouseButtonUp(btn.Value);
        }

        /// <summary>
        /// Internal handler for mouse button release events.
        /// </summary>
        private void OnMouseDown(IMouse m, Silk.NET.Input.MouseButton button)
        {
            var btn = MouseConverter.Convert(button);
            if (btn.HasValue)
                state?.SetMouseButtonDown(btn.Value);
        }

        /// <summary>
        /// Internal handler for mouse movement events.
        /// Calculates delta movement correctly for both normal and locked cursor modes.
        /// In CursorMode.Disabled, GLFW provides cumulative raw position, not frame delta!
        /// </summary>
        private void OnMouseMove(IMouse m, Vector2 position)
        {
            // ═══ If we need to skip the next delta (e.g. after changing cursor mode),
            // ═══ update lastPosition and current position but do not emit a delta.
            if (skipNextDelta)
            {
                lastPosition = position;
                skipNextDelta = false;
                state?.SetMousePosition(position);
                return;
            }

            var delta = position - lastPosition;
            lastPosition = position;

            state?.SetMousePosition(position);
            state?.SetMouseDelta(delta);
        }

        /// <summary>
        /// Internal handler for mouse scroll wheel events.
        /// </summary>
        private void OnScroll(IMouse m, ScrollWheel scroll)
        {
            state?.SetScrollDelta(scroll.Y);
        }

        // ══════════════════════════════════════════════════
        // CURSOR MODES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets the cursor display mode (visible, hidden, locked, etc.).
        /// Useful for first-person cameras (locked), UI interaction (visible), or cinematic modes (hidden).
        /// </summary>
        /// <param name="mode">The desired cursor mode.</param>
        public void SetCursorMode(CursorMode mode)
        {
            if (mouse is null) return;

            // ═══ Skip next delta to prevent jump when switching modes
            skipNextDelta = true;

            mouse.Cursor.CursorMode = mode switch
            {
                CursorMode.Visible => Silk.NET.Input.CursorMode.Normal,
                CursorMode.Hidden => Silk.NET.Input.CursorMode.Hidden,
                CursorMode.Locked => Silk.NET.Input.CursorMode.Disabled, // ═══ Locks and hides cursor (FPS games)
                CursorMode.Confined => Silk.NET.Input.CursorMode.Normal,
                CursorMode.ConfinedHidden => Silk.NET.Input.CursorMode.Hidden,
                _ => Silk.NET.Input.CursorMode.Normal
            };

            // ═══ Update lastPosition to current position after mode change
            if (mouse.Cursor.CursorMode == Silk.NET.Input.CursorMode.Disabled)
            {
                lastPosition = mouse.Position;
            }
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases resources and unregisters event handlers.
        /// </summary>
        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}