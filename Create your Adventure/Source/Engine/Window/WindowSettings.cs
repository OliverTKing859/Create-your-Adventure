using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Window
{
    public sealed class WindowSettings
    {
        public string Title { get; init; } = "Create your Adventure";
        public int Width { get; init; } = 1920;
        public int Height { get; init; } = 1080;
        public int GLMajorVersion { get; init; } = 4;
        public int GLMinorVersion { get; init; } = 6;
        public bool VSync { get; init; } = true;
        public bool Fullscreen { get; init; } = false;
        public bool DebugContext { get; init; } = true;

    }
}
