using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer
{
    /// <summary>
    /// Defines the contract for a rendering context that abstracts graphics API functionality.
    /// Implementations can provide OpenGL, DirectX, Vulkan, or other rendering backends.
    /// </summary>
    public interface IRenderContext : IDisposable
    {
        /// <summary>
        /// Initializes the render context and prepares it for rendering operations.
        /// Must be called before any other rendering methods.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Begins a new rendering frame by clearing buffers and preparing the context.
        /// Should be called at the start of each frame before any draw calls.
        /// </summary>
        void BeginFrame();

        /// <summary>
        /// Ends the current rendering frame and presents the result to the screen.
        /// Should be called after all draw calls for the frame are complete.
        /// </summary>
        void EndFrame();

        /// <summary>
        /// Sets the viewport area where rendering will occur.
        /// Defines the region of the window that will display the rendered content.
        /// </summary>
        /// <param name="x">The X coordinate of the viewport's lower-left corner in pixels.</param>
        /// <param name="y">The Y coordinate of the viewport's lower-left corner in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        void SetViewport(int x, int y, int width, int height);

        /// <summary>
        /// Sets the color used to clear the screen at the beginning of each frame.
        /// </summary>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <param name="a">Alpha component (0.0 to 1.0).</param>
        void SetClearColor(float r, float g, float b, float a);

        /// <summary>
        /// Enables or disables depth testing for 3D rendering.
        /// When enabled, fragments are tested against the depth buffer to determine visibility.
        /// </summary>
        /// <param name="enabled"></param>
        void SetDepthTestEnabled(bool enabled);

        /// <summary>
        /// Gets a value indicating whether the render context has been successfully initialized.
        /// </summary>
        bool IsInitialized { get; }

    }
}