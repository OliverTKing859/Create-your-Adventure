using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Create_your_Adventure.source.Gamelogic.Camera;

namespace Create_your_Adventure
{
    internal class Program
    {
        // -------- Window & GL --------
        private static IWindow window;
        private static GL gl;

        // -------- Input --------
        private static IInputContext input;
        private static IKeyboard keyboard;
        private static IMouse mouse;

        // -------- Camera --------
        private static Camera camera;

        // -------- Mouse Position --------
        private static bool firstMouse = true;
        private static Vector2 lastMousePosition;

        // -------- Matrices --------
        private static Matrix4X4<float> view;
        private static Matrix4X4<float> projection;

        // -------- Uniform Locations --------
        private static int uModel;
        private static int uView;
        private static int uProjection;

        // -------- OpenGL pipeline --------
        private static uint vao;
        private static uint vbo;
        private static uint ebo;
        private static uint graphicsProgram;

        private static readonly float[] vertices =
        {
            // Position             // Color

            // Front face
           -0.5f, -0.5f,  0.5f,     2.0f, 0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,     0.0f, 2.0f, 0.0f,
            0.5f,  0.5f,  0.5f,     0.0f, 0.0f, 2.0f,
           -0.5f,  0.5f,  0.5f,     2.0f, 2.0f, 0.0f,

            // Back face
           -0.5f, -0.5f, -0.5f,     2.0f, 0.0f, 2.0f,
            0.5f, -0.5f, -0.5f,     0.0f, 2.0f, 2.0f,
            0.5f,  0.5f, -0.5f,     2.0f, 2.0f, 2.0f,
           -0.5f,  0.5f, -0.5f,     2.0f, 0.0f, 0.0f
        };

        private static readonly uint[] indices =
        {
            // North
            0, 1, 2,
            2, 3, 0,

            // East
            1, 5, 6,
            6, 2, 1,

            // South
            5, 4, 7,
            7, 6, 5,

            // West
            4, 0, 3,
            3, 7, 4,

            // Top
            3, 2, 6,
            6, 7, 3,

            // Bottom
            4, 5, 1,
            1, 0, 4
        };

        private static readonly uint stride = 6 * sizeof(float);

        // MAIN ----------------------------------------------------------------
        static void Main()
        {
            // -------- Window Creation --------
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

            // -------- Event Subscription --------
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;

            // -------- Run Application --------
            window.Run();

        }

        // ONLOAD ----------------------------------------------------------------
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
            gl.UseProgram(graphicsProgram);
            uModel = gl.GetUniformLocation(graphicsProgram, "uModel");
            uView = gl.GetUniformLocation(graphicsProgram, "uView");
            uProjection = gl.GetUniformLocation(graphicsProgram, "uProjection");

            // -------- Camera --------
            camera = new Camera();

            // --------Input Initialation --------
            input = window.CreateInput();

            keyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;
            mouse = input.Mice.Count > 0 ? input.Mice[0] : null;

            // -------- Mouse Setup --------
            if (mouse != null)
            {
                mouse.Cursor.CursorMode = CursorMode.Disabled;
                mouse.MouseMove += OnMouseMove;
            }

            // -------- Initial Projection --------
            projection = camera.CreatePerspective(window.Size.X, window.Size.Y, 60f, 0.1f, 100f);
            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }
        }

        // ONUPDATE ----------------------------------------------------------------
        private static void OnUpdate(double deltaTime)
        {

            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)

            // -------- Camera (WASD/Space/Strg/Shift Input) --------
            camera.Update(
                deltaTime,
                keyboard.IsKeyPressed(Key.W),
                keyboard.IsKeyPressed(Key.A),
                keyboard.IsKeyPressed(Key.S),
                keyboard.IsKeyPressed(Key.D),
                keyboard.IsKeyPressed(Key.Space),
                keyboard.IsKeyPressed(Key.ControlLeft),
                keyboard.IsKeyPressed(Key.ShiftLeft)
                );
        }

        // ONRENDER ----------------------------------------------------------------
        private static unsafe void OnRender(double deltaTime)
        {
            // -------- Rendering --------
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            gl.UseProgram(graphicsProgram);
            gl.BindVertexArray(vao);

            // --- Matrices
            var view = camera.GetViewMatrix();
            var model = Matrix4X4<float>.Identity;

            // --- Update Uniforms
            gl.UniformMatrix4(uModel, 1, false, (float*)&model);
            gl.UniformMatrix4(uView, 1, false, (float*)&view);
            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }

            // --- Draw Call
            gl.DrawElements(
                PrimitiveType.Triangles,
                36,
                DrawElementsType.UnsignedInt,
                null
                );
        }

        // ONCLOSE ----------------------------------------------------------------
        private static void OnClose()
        {
            if (mouse != null)
            {
                mouse.MouseMove -= OnMouseMove;
            }

            gl.DeleteBuffer(ebo);
            gl.DeleteBuffer(vbo);
            gl.DeleteVertexArray(vao);
            gl.DeleteProgram(graphicsProgram);
            // Cleanup (Buffer, Shader, etc.)
        }

        // WINDOW ----------------------------------------------------------------
        public static void CenterWindow(IWindow window)
        {
            // -------- Get Monitor --------
            var monitor = window.Monitor;
            if (monitor == null)
            {
                return;
            }

            // --- Calculate Centered Position
            var size = window.Size;
            var bounds = monitor.Bounds.Size;

            // --- Set Position
            window.Position = new Vector2D<int>(
                (bounds.X - size.X) / 2,
                (bounds.Y - size.Y) / 2
                );
        }

        // OPENGL DEBUG ----------------------------------------------------------------
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

        // SHADER ----------------------------------------------------------------
        private const string VertexShaderSource = @"
        #version 460 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;

        out vec3 vColor;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
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

        // CREATE GRAPHICS PROGRAM ----------------------------------------------------------------
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

        // MOUSE MOVE EVENT ----------------------------------------------------------------
        private static void OnMouseMove(IMouse mouse, Vector2 mousePosition)
        {
            camera.OnMouseMove(mousePosition);
        }
    }
}