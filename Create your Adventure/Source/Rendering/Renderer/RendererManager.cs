using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Window;
using Create_your_Adventure.Source.Rendering.Renderer.OpenGL;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Renderer
{
    public sealed class RendererManager : IDisposable
    {
        private static RendererManager? instance;
        private static readonly Lock instanceLock = new();

        private IRenderContext? renderContext;
        private bool isDisposed;

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

        public IRenderContext? Context => renderContext;
        public bool IsInitialized => renderContext?.IsInitialized ?? false;

        private RendererManager()
        { 
            // Constructor
        }

        public void Initialize()
        {
            var windowManager = WindowManager.Instance;

            if (windowManager.GlContext is null)
            {
                throw new InvalidOperationException(
                    "WindowManager must be loaded before initializing RenderManager");
            }

            Logger.Info("[RENDER] Initializing RenderManager...");

            // OpenGL-Kontext erstellen (später: Factory-Pattern für verschiedene APIs)
            renderContext = new OpenGLRenderContext(windowManager.GlContext);
            renderContext.Initialize();

            var size = windowManager.Size;
            renderContext.SetViewport(0, 0, size.X, size.Y);

            windowManager.OnResize += OnWindowResize;

            Logger.Info("[RENDER] RenderManager initialized successfully");
        }

        public void BeginFrame()
        {
            renderContext?.BeginFrame();
        }

        public void EndFrame()
        {
            renderContext?.EndFrame();
        }

        public void OnWindowResize(Vector2D<int> size)
        {
            renderContext?.SetViewport(0, 0, size.X, size.Y);
            Logger.Info($"[RENDER] Viewport resized to {size.X}x{size.Y}");
        }

        public void Dispose()
        {
            if (isDisposed) return;

            WindowManager.Instance.OnResize -= OnWindowResize;
            renderContext?.Dispose();

            isDisposed = true;
            Logger.Info("[RENDER] RenderManager disposed");
        }
    }
}
