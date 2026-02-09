using Create_your_Adventure.Source.Engine.DevDebug;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer.OpenGL
{
    public sealed class OpenGLRenderContext : IRenderContext
    {
        private readonly GL gl;
        private bool isInitialized;
        private bool isDisposed;

        public bool IsInitialized => isInitialized;

        public OpenGLRenderContext(GL glContext)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
        }

        public void Initialize()
        {
            if (isInitialized) return;

            gl.Enable(EnableCap.DebugOutput);
            gl.Enable(EnableCap.DebugOutputSynchronous);

            SetClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            SetDepthTestEnabled(true);

            isInitialized = true;
            Logger.Info("[RENDER] OpenGL context initialized");

        }

        public void BeginFrame()
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void EndFrame()
        {
            // Frame- End Logik (for Double Buffering or others)
        }

        public void SetViewport(int x, int y, int width, int height)
        {
            gl.Viewport(x, y, (uint)width, (uint)height);
        }

        public void SetClearColor(float r, float g, float b, float a)
        {
            gl.ClearColor(r, g, b, a);
        }

        public void SetDepthTestEnabled(bool enabled)
        {
            if (enabled)
            {
                gl.Enable(EnableCap.DepthTest);
            }
            else
            {
                gl.Disable(EnableCap.DepthTest);
            }
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            Logger.Info("[RENDER] OpenGL context disposed");
        }
    }
}
