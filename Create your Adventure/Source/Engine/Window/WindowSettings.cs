using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Window
{
    /// <summary>
    /// Configuration settings for window creation and OpenGL context initialization.
    /// Provides sensible defaults for a modern OpenGL application.
    /// </summary>
    public sealed class WindowSettings
    {
        /// <summary>
        /// Gets or initializes the title displayed in the window's title bar.
        /// Default: "Create your Adventure"
        /// </summary>
        public string Title { get; init; } = "Create your Adventure";

        /// <summary>
        /// Gets or initializes the width of the window in pixels.
        /// Default: 1920 (Full HD width)
        /// </summary>
        public int Width { get; init; } = 1920;

        /// <summary>
        /// Gets or initializes the height of the window in pixels.
        /// Default: 1080 (Full HD height)
        /// </summary>
        public int Height { get; init; } = 1080;

        /// <summary>
        /// Gets or initializes the major version number of the OpenGL context.
        /// Default: 4 (OpenGL 4.x)
        /// </summary>
        public int GLMajorVersion { get; init; } = 4;

        /// <summary>
        /// Gets or initializes the minor version number of the OpenGL context.
        /// Default: 6 (OpenGL 4.6)
        /// </summary>
        public int GLMinorVersion { get; init; } = 6;

        /// <summary>
        /// Gets or initializes whether vertical synchronization (VSync) is enabled.
        /// When enabled, frame rate is limited to the monitor's refresh rate to prevent screen tearing.
        /// Default: true
        /// </summary>
        public bool VSync { get; init; } = true;

        /// <summary>
        /// Gets or initializes whether the window starts in fullscreen mode.
        /// Default: false (windowed mode)
        /// </summary>
        public bool Fullscreen { get; init; } = false;

        /// <summary>
        /// Gets or initializes whether to create an OpenGL debug context.
        /// Debug contexts provide additional error checking and validation, useful during development.
        /// Should be disabled in release builds for better performance.
        /// Default: true
        /// </summary>
        public bool DebugContext { get; init; } = true;
    }
}
