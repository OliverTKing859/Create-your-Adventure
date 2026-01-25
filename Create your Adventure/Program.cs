using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Core.Native;

namespace Create_your_Adventure
{
    internal class Program
    {
        private static IWindow window;
        private static GL gl;

        private static uint vao;
        private static uint vbo;
        private static uint ebo;
        private static uint graphicsProgram;

        private static readonly float[] vertices =
        {
            // Position             // Color
            0.0f,   0.5f,   0.0f,   0.0f, 0.0f, 2.0f,   // Blue
            0.5f,  -0.5f,   0.0f,   0.0f, 2.0f, 0.0f,   // Green
           -0.5f,  -0.5f,   0.0f,   2.0f, 0.0f, 0.0f    // Red
        };

        private static readonly uint[] indices =
        {
            0, 1, 2
        };

        private static readonly uint stride = 6 * sizeof(float);

        static void Main()
        {
            var options = WindowOptions.Default;
            options.Title = "Create your Adventure";
            options.Size = new Vector2D<int>(1920, 1080);
            options.API = new GraphicsAPI(
                ContextAPI.OpenGL,
                ContextProfile.Core,
                ContextFlags.Debug |
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

        private static unsafe void OnLoad()
        {
            // -------- OpenGL loading --------

            gl = GL.GetApi(window);

            gl.Enable(EnableCap.DebugOutput);
            gl.Enable(EnableCap.DebugOutputSynchronous);

            gl.DebugMessageCallback(DebugCallback, null);

            CenterWindow(window);

            // -------- OpenGL State --------

            gl.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            gl.Enable(EnableCap.DepthTest);

            // -------- VAO --------
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // -------- VBO --------

            vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                vertices,
                BufferUsageARB.StaticDraw
                );

            // -------- EBO --------

            ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                indices,
                BufferUsageARB.StaticDraw
                );

            // -------- Vertex Attribute --------

            gl.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                stride,
                0
                );
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(
                1,
                3,
                VertexAttribPointerType.Float,
                false,
                stride,
                3 * sizeof(float)
                );

            gl.EnableVertexAttribArray(1);

            // -------- Shader --------
            graphicsProgram = CreateGraphicsProgram();

        }

        private static void OnUpdate(double deltaTime)
        {
            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)
        }

        private static unsafe void OnRender(double deltaTime)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            gl.UseProgram(graphicsProgram);
            gl.BindVertexArray(vao);

            gl.DrawElements(
                PrimitiveType.Triangles,
                3,
                DrawElementsType.UnsignedInt,
                null
                );

            // Render Code (IKingsRenderer,OpenGL, Vulkan🌋)
        }

        private static void OnClose()
        {
            gl.DeleteBuffer(ebo);
            gl.DeleteBuffer(vbo);
            gl.DeleteVertexArray(vao);
            gl.DeleteProgram(graphicsProgram);
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
        private static void DebugCallback(
            GLEnum source,
            GLEnum type,
            int id,
            GLEnum severity,
            int length,
            nint message,
            nint userParam)
        {
            if (severity == GLEnum.DebugSeverityNotification)
                return;

            Console.WriteLine($"[GL] {severity}: {SilkMarshal.PtrToString(message)}");
        }

        private const string VertexShaderSource = @"
        #version 460 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        out vec3 vColor;

        void main()
        {
            gl_Position = vec4(aPosition, 0.75);
            vColor = aColor;
        }
        ";

        private const string FragmentShaderSource = @"
        #version 460 core

        in vec3 vColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(vColor, 1.0);
        }
        ";

        private static uint CreateGraphicsProgram()
        {
            uint vertex = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertex, VertexShaderSource);
            gl.CompileShader(vertex);

            uint fragment = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragment, FragmentShaderSource);
            gl.CompileShader(fragment);

            uint createProgram = gl.CreateProgram();
            gl.AttachShader(createProgram, vertex);
            gl.AttachShader(createProgram, fragment);
            gl.LinkProgram(createProgram);

            gl.DeleteShader(vertex);
            gl.DeleteShader(fragment);

            return createProgram;
        }
    }
}