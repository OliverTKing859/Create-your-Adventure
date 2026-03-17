using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Create_your_Adventure.Source.Engine.Window
{
    /// <summary>
    /// Manages the application window and provides a singleton interface to window operations.
    /// Handles window creation, OpenGL context initialization, input setup, and lifecycle events.
    /// </summary>
    public sealed class WindowManager : IDisposable
    {
        // ═══ Singleton instance of the WindowManager
        private static WindowManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        // ═══ The underlying Silk.NET window instance
        private IWindow? window;
        // ═══ Flag to track whether this instance has been disposed
        private bool isDisposed;

        // ══════════════════════════════════════════════════
        // PUBLIC PROPERTIES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the singleton instance of the WindowManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
        public static WindowManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new WindowManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the OpenGL context used for rendering operations.
        /// Available after the window has been loaded.
        /// </summary>
        public GL? GlContext { get; private set; }

        /// <summary>
        /// Gets the input context for handling keyboard, mouse, and gamepad input.
        /// Available after the window has been loaded.
        /// </summary>
        public IInputContext? InputContext { get; private set; }

        /// <summary>
        /// Gets the current size of the window in pixels.
        /// Returns Vector2D.Zero if the window is not initialized.
        /// </summary>
        public Vector2D<int> Size => window?.Size ?? Vector2D<int>.Zero;

        /// <summary>
        /// Gets a value indicating whether the window is currently open and not closing.
        /// </summary>
        public bool IsOpen => window?.IsClosing == false;

        // ══════════════════════════════════════════════════
        // EVENTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Event raised when the window has finished loading and is ready for use.
        /// OpenGL and input contexts are initialized at this point.
        /// </summary>
        public event Action? Loaded;

        /// <summary>
        /// Event raised every frame during the update phase.
        /// Use this for game logic, physics, and input processing.
        /// </summary>
        public event Action<double>? Updated;

        /// <summary>
        /// Event raised every frame during the render phase.
        /// Use this for all rendering operations.
        /// </summary>
        public event Action<double>? Rendered;

        /// <summary>
        /// Event raised when the window is closing.
        /// Use this for cleanup operations before the window shuts down.
        /// </summary>
        public event Action? OnClose;

        /// <summary>
        /// Event raised when the window is resized.
        /// Provides the new window size in pixels.
        /// </summary>
        public event Action<Vector2D<int>>? OnResize;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private WindowManager()
        {
            // ═══ Intentionally empty - initialization happens in Initialize()
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the window with the specified settings.
        /// Creates the window, sets up OpenGL context, and subscribes to lifecycle events.
        /// </summary>
        /// <param name="settings">Window configuration settings. If null, uses default settings.</param>
        public void Initialize(WindowSettings? settings = null)
        {
            settings ??= new WindowSettings();

            Logger.Info("[WINDOW] Initializing window manager...");

            // ═══ Configure window options with OpenGL API settings
            var options = WindowOptions.Default with
            {
                Title = settings.Title,
                Size = new Vector2D<int>(settings.Width, settings.Height),
                VSync = settings.VSync,
                WindowState = settings.Fullscreen ? WindowState.Fullscreen : WindowState.Normal,
                API = new GraphicsAPI(
                    ContextAPI.OpenGL,
                    ContextProfile.Core,
                    settings.DebugContext ? ContextFlags.Debug | ContextFlags.ForwardCompatible : ContextFlags.ForwardCompatible,
                    new APIVersion(settings.GLMajorVersion, settings.GLMinorVersion)
                    )
            };

            // ═══ Create the window instance
            window = Silk.NET.Windowing.Window.Create(options);

            // ═══ Event-Subscription
            window.Load += HandleLoad;
            window.Update += HandleUpdate;
            window.Render += HandleRender;
            window.Closing += HandleClose;
            window.Resize += HandleResize;

            Logger.Info($"[WINDOW] Window created ({settings.Width}x{settings.Height}, OpenGL {settings.GLMajorVersion}.{settings.GLMinorVersion})");
        }

        // ══════════════════════════════════════════════════
        // RUNNING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Starts the main application loop.
        /// This method blocks until the window is closed.
        /// Must be called after Initialize().
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the window has not been initialized.</exception>
        public void Run()
        {
            System.ObjectDisposedException.ThrowIf(isDisposed, this);

            if (window is null)
                throw new InvalidOperationException("WindowManager must be initialized before calling Run()");

            Logger.Info("[WINDOW] Starting main loop...");
            window.Run();
        }

        /// <summary>
        /// Closes the window and exits the application loop.
        /// Triggers the OnClose event before closing.
        /// </summary>
        public void Close()
        {
            System.ObjectDisposedException.ThrowIf(isDisposed, this);

            window?.Close();
        }


        // ══════════════════════════════════════════════════
        // CENTER WINDOW
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Centers the window on the primary monitor.
        /// Called automatically after window loading, but can be called manually if needed.
        /// </summary>
        public void CenterWindow()
        {
            System.ObjectDisposedException.ThrowIf(isDisposed, this);

            if (window?.Monitor is not { } monitor)
            {
                Logger.Warn("[WINDOW] No monitor detected - cannot center window");
                return;
            }

            var size = window.Size;
            var bounds = monitor.Bounds.Size;

            // ═══ Calculate centered position
            window.Position = new Vector2D<int>(
                (bounds.X - size.X) / 2,
                (bounds.Y - size.Y) / 2
                );

            Logger.Info("[WINDOW] Window centered on monitor");
        }

        // ══════════════════════════════════════════════════
        // EVENT HANDLER
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Internal handler for the window Load event.
        /// Initializes OpenGL context, input context, centers the window, and raises the Loaded event.
        /// </summary>
        private void HandleLoad()
        {
            // ═══ Initialize OpenGL context from the window
            GlContext = Silk.NET.OpenGL.GL.GetApi(window!);

            // ═══ Create input context for handling user input
            InputContext = window!.CreateInput();

            // ═══ Center the window on the screen
            CenterWindow();

            Logger.Info("[WINDOW] GL context and input initialized");

            // ═══ Notify subscribers that the window is ready
            Loaded?.Invoke();
        }

        /// <summary>
        /// Internal handler for the window Update event. Forwards to subscribers.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        private void HandleUpdate(double deltaTime) => Updated?.Invoke(deltaTime);

        /// <summary>
        /// Internal handler for the window Render event. Forwards to subscribers.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last render in seconds.</param>
        private void HandleRender(double deltaTime) => Rendered?.Invoke(deltaTime);

        /// <summary>
        /// Internal handler for the window Resize event. Forwards to subscribers.
        /// </summary>
        /// <param name="size">The new window size in pixels.</param>
        private void HandleResize(Vector2D<int> size) => OnResize?.Invoke(size);

        /// <summary>
        /// Internal handler for the window Closing event. Logs and forwards to subscribers.
        /// </summary>
        private void HandleClose()
        {
            Logger.Info("[WINDOW] Window closing...");
            OnClose?.Invoke();
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases all resources associated with the window.
        /// Disposes of input context and window instance.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            // ═══ Clear public events to avoid keeping external references
            Loaded = null;
            Updated = null;
            Rendered = null;
            OnClose = null;
            OnResize = null;

            // ═══ Dispose input context first
            InputContext?.Dispose();

            // ═══ Dispose GL context before disposing the window
            GlContext?.Dispose();

            // ═══ Dispose the window
            window?.Dispose();

            isDisposed = true;

            // ═══ Ensure singleton reference cleared in a thread-safe manner
            lock (instanceLock)
            {
                instance = null;
            }

            Logger.Info("[WINDOW] WindowManager disposed");
        }
    }
}
