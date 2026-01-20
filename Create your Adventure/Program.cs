using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;

namespace Create_your_Adventure
{
    internal class Program
    {
        private static IWindow window;
        private static GL gl;
        static void Main()
        {
            var options = WindowOptions.Default;
            options.Title = "Create your Adventure";
            options.Size = new Vector2D<int>(1920, 1080);
            options.API = new GraphicsAPI(
                ContextAPI.OpenGL,
                ContextProfile.Core,
                ContextFlags.ForwardCompatible,
                new APIVersion(4, 6)
                );

            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;

            window.Run();

        }

        private static void OnLoad()
        {
            // OpenGL loading

            gl = GL.GetApi(window);
            CenterWindow(window);

            // OpenGL State

            gl.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            gl.Enable(EnableCap.DepthTest);
        }

        private static void OnUpdate(double deltaTime)
        {
            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)
        }

        private static void OnRender(double deltaTime)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render Code (IKingsRenderer,OpenGL, Vulkan🌋)
        }

        private static void OnClose()
        {
            // Cleanup (Buffer, Shader, etc.)
        }

        public static void CenterWindow(IWindow window)

        {
            var monitor = window.Monitor;
            if (monitor == null)
            {
                return;
            }

            var size = window.Size;
            var bounds = monitor.Bounds.Size;

            window.Position = new Vector2D<int>(
                (bounds.X - size.X) / 2,
                (bounds.Y - size.Y) / 2
                );
        }
    }
}