using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Window;
using Create_your_Adventure.Source.Rendering.Renderer.OpenGL;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer
{
    /// <summary>
    /// Manages the rendering system and provides a singleton interface to the render context.
    /// Handles initialization, frame lifecycle, and viewport management.
    /// </summary>
    public sealed class RendererManager : IDisposable
    {
        // ═══ Singleton instance of the RendererManager ═══
        private static RendererManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization ═══
        private static readonly Lock instanceLock = new();

        // ═══ The active render context (e.g., OpenGL, DirectX, Vulkan)
        private IRenderContext? renderContext;
        // ═══ Flag to track whether this instance has been disposed
        private bool isDisposed;


        // ══════════════════════════════════════════════════
        // RENDERMANAGER INSTANCING
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Gets the singleton instance of the RendererManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
        public static RendererManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new RendererManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the current render context, or null if not initialized.
        /// </summary>
        public IRenderContext? Context => renderContext;

        /// <summary>
        /// Gets a value indicating whether the render context has been initialized.
        /// </summary>
        public bool IsInitialized => renderContext?.IsInitialized ?? false;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private RendererManager()
        {
            // ═══ Intentionally empty - initialization happens in Initialize()
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Initializes the rendering system by creating the render context and setting up the viewport.
        /// Must be called after WindowManager has been initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when WindowManager's GL context is not available.</exception>
        public void Initialize()
        {
            var windowManager = WindowManager.Instance;

            if (windowManager.GlContext is null)
            {
                throw new InvalidOperationException(
                    "WindowManager must be loaded before initializing RenderManager");
            }

            Logger.Info("[RENDER] Initializing RenderManager...");

            // ═══ Create OpenGL context (future: use factory pattern for different rendering APIs)
            renderContext = new OpenGLRenderContext(windowManager.GlContext);
            renderContext.Initialize();

            // ═══ Set initial viewport size to match window dimensions
            var size = windowManager.Size;
            renderContext.SetViewport(0, 0, size.X, size.Y);

            // ═══ Subscribe to window resize events
            windowManager.OnResize += OnWindowResize;

            Logger.Info("[RENDER] RenderManager initialized successfully");
        }

        // ══════════════════════════════════════════════════
        // BEGIN THE FRAME
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Begins a new rendering frame. Clears buffers and prepares the render context.
        /// Should be called at the start of each frame before any rendering operations.
        /// </summary>
        public void BeginFrame()
        {
            renderContext?.BeginFrame();
        }

        // ══════════════════════════════════════════════════
        // ENDING THE FRAME
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Ends the current rendering frame and presents the result to the screen.
        /// Should be called after all rendering operations for the frame are complete.
        /// </summary>
        public void EndFrame()
        {
            renderContext?.EndFrame();
        }

        // ══════════════════════════════════════════════════
        // RESIZE ON WINDOW
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Handles window resize events by updating the viewport dimensions.
        /// </summary>
        /// <param name="size">The new window size in pixels.</param>
        public void OnWindowResize(Vector2D<int> size)
        {
            renderContext?.SetViewport(0, 0, size.X, size.Y);
            Logger.Info($"[RENDER] Viewport resized to {size.X}x{size.Y}");
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Disposes of the RendererManager and releases all associated resources.
        /// Unsubscribes from events and disposes the render context.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            // ═══ Unsubscribe from window resize events
            WindowManager.Instance.OnResize -= OnWindowResize;
            renderContext?.Dispose();

            // ═══ Dispose the render context and release graphics resources
            isDisposed = true;
            Logger.Info("[RENDER] RenderManager disposed");
        }
    }
}