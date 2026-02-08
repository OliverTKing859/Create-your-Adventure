using Create_your_Adventure.Source.Engine.DevDebug;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Create_your_Adventure.Source.Engine.Window
{
    public sealed class WindowManager : IDisposable
    {
        private static WindowManager? instance;
        private static readonly Lock instanceLock = new();

        private IWindow? window;
        private bool isDisposed;

        // -------- Public Properties --------
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

        public GL? GlContext { get; private set; }
        public IInputContext? InputContext { get; private set; }
        public Vector2D<int> Size => window?.Size ?? Vector2D<int>.Zero;
        public bool IsOpen => window?.IsClosing == false;

        // -------- Events --------
        public event Action? Loaded;
        public event Action<double>? Updated;
        public event Action<double>? Rendered;
        public event Action? OnClose;
        public event Action<Vector2D<int>>? OnResize;

        // -------- Private Constructor (Singleton) --------
        public void Initialize(WindowSettings? settings = null)
        {
            settings ??= new WindowSettings();

            Logger.Info("[WINDOW] Initializing window manager...");

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

            window = Silk.NET.Windowing.Window.Create(options);

            // --- Event-Subscription
            window.Load += HandleLoad;
            window.Update += HandleUpdate;
            window.Render += HandleRender;
            window.Closing += HandleClose;
            window.Resize += HandleResize;

            Logger.Info($"[WINDOW] Window created ({settings.Width}x{settings.Height}, OpenGL {settings.GLMajorVersion}.{settings.GLMinorVersion})");
        }

        // -------- Public Methods --------
        public void Run()
        {
            if (window is null)
                throw new InvalidOperationException("WindowManager must be initialized before calling Run()");

            Logger.Info("[WINDOW] Starting main loop...");
            window.Run();
        }

        public void Close()
        {
            window?.Close();
        }

        public void CenterWindow()
        {
            if (window?.Monitor is not { } monitor)
            {
                Logger.Warn("[WINDOW] No monitor detected - cannot center window");
                return;
            }

            var size = window.Size;
            var bounds = monitor.Bounds.Size;

            window.Position = new Vector2D<int>(
                (bounds.X - size.X) / 2,
                (bounds.Y - size.Y) / 2
                );

            Logger.Info("[WINDOW] Window centered on monitor");
        }

        // EVENT HANDLERS ----------------------------------------------------------------
        private void HandleLoad()
        {
            GlContext = Silk.NET.OpenGL.GL.GetApi(window!);
            InputContext = window!.CreateInput();

            CenterWindow();

            Logger.Info("[WINDOW] GL context and input initialized");
            Loaded?.Invoke();
        }

        private void HandleUpdate(double deltaTime) => Updated?.Invoke(deltaTime);
        private void HandleRender(double deltaTime) => Rendered?.Invoke(deltaTime);
        private void HandleResize(Vector2D<int> size) => OnResize?.Invoke(size);

        private void HandleClose()
        {
            Logger.Info("[WINDOW] Window closing...");
            OnClose?.Invoke();
        }

        // -------- Dispose --------

        public void Dispose()
        {
            if (isDisposed) return;

            InputContext?.Dispose();
            window?.Dispose();

            isDisposed = true;
            Logger.Info("[WINDOW] WindowManager disposed");
        }
    }
}
