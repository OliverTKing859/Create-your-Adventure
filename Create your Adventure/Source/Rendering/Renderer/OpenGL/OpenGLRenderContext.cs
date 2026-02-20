using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Render;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer.OpenGL
{
    /// <summary>
    /// OpenGL implementation of the IRenderContext interface.
    /// Manages OpenGL-specific rendering operations including initialization, frame management,
    /// and state configuration for the graphics pipeline.
    /// </summary>
    public sealed class OpenGLRenderContext : IRenderContext
    {
        // ═══ The Silk.NET OpenGL context wrapper
        private readonly GL gl;
        // ═══ Flag to track whether the context has been initialized
        private bool isInitialized;
        // ═══ Flag to track whether this instance has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets a value indicating whether the OpenGL context has been successfully initialized.
        /// </summary>
        public bool IsInitialized => isInitialized;

        /// <summary>
        /// Initializes a new instance of the OpenGLRenderContext with the specified OpenGL context.
        /// </summary>
        /// <param name="glContext">The Silk.NET OpenGL context to use for rendering operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when the glContext parameter is null.</exception>
        public OpenGLRenderContext(GL glContext)
        {
            // ═══ Validate that a valid OpenGL context was provided
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the OpenGL rendering context and configures default rendering states.
        /// Enables debug output, sets the default clear color, and activates depth testing.
        /// This method is idempotent and can be safely called multiple times.
        /// </summary>
        public void Initialize()
        {
            // Prevent re-initialization if already initialized
            if (isInitialized) return;

            // ═══ Enable OpenGL debug output for error detection and performance warnings
            gl.Enable(EnableCap.DebugOutput);
            // ═══ Make debug output synchronous for easier debugging (callbacks happen immediately)
            gl.Enable(EnableCap.DebugOutputSynchronous);

            // ═══ Set default clear color to dark gray (RGB: 0.05, 0.05, 0.05)
            SetClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            // ═══ Enable depth testing by default for proper 3D rendering
            SetDepthTestEnabled(true);

            isInitialized = true;
            Logger.Info("[RENDER] OpenGL context initialized");

        }

        // ══════════════════════════════════════════════════
        // BEGIN FRAME
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Begins a new rendering frame by clearing the color and depth buffers.
        /// Should be called at the start of each frame before any rendering operations.
        /// </summary>
        public void BeginFrame()
        {
            // ═══ Clear both color and depth buffers to prepare for new frame
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        // ══════════════════════════════════════════════════
        // END FRAME
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Ends the current rendering frame.
        /// In OpenGL, buffer swapping is typically handled by the windowing system,
        /// so this method is currently a no-op but can be extended for future needs.
        /// </summary>
        public void EndFrame()
        {
            // ═══ Frame-end logic (for double buffering or other post-frame operations)
            // ═══ Currently handled by the window manager's swap buffers call
        }

        // ══════════════════════════════════════════════════
        // SET VIEW
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets the OpenGL viewport to define the rendering area.
        /// This determines which portion of the window will display rendered content.
        /// </summary>
        /// <param name="x">The X coordinate of the viewport's lower-left corner in pixels.</param>
        /// <param name="y">The Y coordinate of the viewport's lower-left corner in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        public void SetViewport(int x, int y, int width, int height)
        {
            // ═══ Update the OpenGL viewport to match the specified dimensions
            gl.Viewport(x, y, (uint)width, (uint)height);
        }

        // ══════════════════════════════════════════════════
        // CLEAR COLOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets the background color used when clearing the screen.
        /// This color will be applied at the start of each frame during BeginFrame().
        /// </summary>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <param name="a">Alpha component (0.0 to 1.0).</param>
        public void SetClearColor(float r, float g, float b, float a)
        {
            // ═══ Set the OpenGL clear color for the color buffer
            gl.ClearColor(r, g, b, a);
        }

        // ══════════════════════════════════════════════════
        // DEPTHTEST ENABLE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Enables or disables depth testing in the OpenGL pipeline.
        /// Depth testing ensures that closer objects occlude farther objects in 3D scenes.
        /// </summary>
        /// <param name="enabled">True to enable depth testing, false to disable it.</param>
        public void SetDepthTestEnabled(bool enabled)
        {
            if (enabled)
            {
                // ═══ Enable depth testing for proper 3D rendering
                gl.Enable(EnableCap.DepthTest);
            }
            else
            {
                // ═══ Disable depth testing (useful for 2D rendering or UI overlays)
                gl.Disable(EnableCap.DepthTest);
            }
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Disposes of the OpenGL render context and releases all associated resources.
        /// The underlying GL context is managed by Silk.NET and will be disposed separately.
        /// </summary>
        public void Dispose()
        {
            // ═══ Prevent double disposal
            if (isDisposed) return;

            isDisposed = true;
            Logger.Info("[RENDER] OpenGL context disposed");

            // ═══ Note: The GL context itself is owned by Silk.NET.Windowing and will be
            // ═══       disposed when the window is closed. We only track our disposal state here.
        }
    }
}
