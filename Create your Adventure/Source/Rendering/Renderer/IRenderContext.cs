using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer
{
    public interface IRenderContext : IDisposable
    {
        void Initialize();

        void BeginFrame();

        void EndFrame();

        void SetViewport(int x, int y, int width, int height);

        void SetClearColor(float r, float g, float b, float a);

        void SetDepthTestEnabled(bool enabled);

        bool IsInitialized { get; }

    }
}
