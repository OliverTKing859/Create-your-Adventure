using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Core.Native;
using Silk.NET.Input;

namespace Create_your_Adventure
{
    internal class Program
    {
        private static IWindow window;
        private static GL gl;

        // -------- Input --------

        private static IInputContext input;
        private static IKeyboard keyboard;
        private static IMouse mouse;

        private static bool firstMouse = true;
        private static Vector2 lastMousePosition;

        // -------- Camera --------

        // --- Mouse
        private static float mouseSensitivity = 50.0f;
        private static float mouseSmoothingFactor = 0.6f;
        private static Vector2 mouseDeltaSmoothed = Vector2.Zero;
        private static Vector2 rawMouseDelta = Vector2.Zero;

        // --- Camera Movement
        private static float movementSpeed = 6.0f;
        private static float movementVerticalSpeed = 5.0f;
        private static float sprintMovementSpeedMultiplier = 2.5f;

        // --- Acceleration & Deceleration
        private static float accelerationHorizontalRate = 4.0f;
        private static float decelerationHorizontalRate = 2.0f;
        private static float accelerationVerticalRate = 12.0f;
        private static float decelerationVerticalRate = 8.0f;

        // ---- Velocity ----

        private static float velocityVertical = 0.0f;
        private static Vector3D<float> velocityHorizontal = Vector3D<float>.Zero;

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

        // -------- Camera --------

        private static Vector3D<float> cameraPosition = new(0f, 0f, 3f);
        private static float yaw = -90f;
        private static float pitch = 0f;

        private static Matrix4X4<float> view;
        private static Matrix4X4<float> projection;

        private static int uModel;
        private static int uView;
        private static int uProjection;

        private static float DegreesToRadians(float degrees) => degrees * (MathF.PI / 180.0f);

        // Main ----------------------------------------------------------------

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

        // OnLoad ----------------------------------------------------------------

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

            // -------- Camera --------
            uModel = gl.GetUniformLocation(graphicsProgram, "uModel");
            uView = gl.GetUniformLocation(graphicsProgram, "uView");
            uProjection = gl.GetUniformLocation(graphicsProgram, "uProjection");

            // --------Input Initialation --------

            input = window.CreateInput();

            keyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;
            mouse = input.Mice.Count > 0 ? input.Mice[0] : null;

            if (mouse != null)
            {
                mouse.Cursor.CursorMode = CursorMode.Disabled;
                mouse.MouseMove += OnMouseMove;
            }

            // -------- Initial Projection --------
            projection = CreatePerspective(window.Size.X, window.Size.Y, 60f, 0.1f, 100f);
            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }

        }

        // ONUPDATE ----------------------------------------------------------------

        private static void OnUpdate(double deltaTime)
        {

            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)

            float dt = (float)deltaTime;

            Vector3D<float> viewForward = GetViewDirection(yaw, pitch);
            Vector3D<float> viewRight = Vector3D.Normalize(Vector3D.Cross(viewForward, Vector3D<float>.UnitY));

            //Vector3D<float> viewUp = Vector3D<float>.UnitY;

            Vector3D<float> viewForwardHorizontal = Vector3D.Normalize(new Vector3D<float>(viewForward.X, 0, viewForward.Z));

            Vector3D<float> inputDirection = Vector3D<float>.Zero;

            // -------- WASD/Space/Shift Input --------

            if (keyboard.IsKeyPressed(Key.W))
            {
                inputDirection += viewForwardHorizontal;
            }
            if (keyboard.IsKeyPressed(Key.S))
            {
                inputDirection -= viewForwardHorizontal;
            }
            if (keyboard.IsKeyPressed(Key.A ))
            {
                inputDirection -= viewRight;
            }
            if (keyboard.IsKeyPressed(Key.D))
            {
                inputDirection += viewRight;
            }

            if (inputDirection.LengthSquared > 0)
            {
                inputDirection = Vector3D.Normalize(inputDirection);
            }

            // -------- Sprint Input --------

            float speed = movementSpeed;
            if (keyboard.IsKeyPressed(Key.ShiftLeft))
            {
                speed *= sprintMovementSpeedMultiplier;
            }

            Vector3D<float> targetVelocity = inputDirection * speed;

            // -------- Horizontal smoothing --------

            float rate = (inputDirection.LengthSquared > 0) ? accelerationHorizontalRate : decelerationHorizontalRate;
            velocityHorizontal = Vector3D.Lerp(
                velocityHorizontal,
                targetVelocity,
                1.0f - MathF.Exp(-rate * dt)
                );

            // -------- Space/Shift Input --------

            float targetVertical = 0f;

            if (keyboard.IsKeyPressed(Key.Space))
            {
                targetVertical += movementVerticalSpeed;
            }
            if (keyboard.IsKeyPressed(Key.ControlLeft))
            {
                targetVertical -= movementVerticalSpeed;
            }

            float verticalRate = (MathF.Abs(targetVertical) > 0.001f)
                ? accelerationVerticalRate
                : decelerationVerticalRate;

            float verticalLerpFactor = 1.0f - MathF.Exp(-verticalRate * dt);
            velocityVertical = velocityVertical + (targetVertical - velocityVertical) * verticalLerpFactor;

            cameraPosition += velocityHorizontal * dt;
            cameraPosition.Y += velocityVertical * dt;

            // Smoothing
            mouseDeltaSmoothed = Vector2.Lerp(Vector2.Zero, rawMouseDelta, 1 - MathF.Exp(-mouseSmoothingFactor * dt));

            yaw += mouseDeltaSmoothed.X * mouseSensitivity;
            pitch -= mouseDeltaSmoothed.Y * mouseSensitivity;

            // Clamp Pitch
            pitch = Math.Clamp(pitch, -89f, 89f);
            rawMouseDelta = Vector2.Zero;

        }

        // ONRENDER ----------------------------------------------------------------

        private static unsafe void OnRender(double deltaTime)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            gl.UseProgram(graphicsProgram);
            gl.BindVertexArray(vao);

            BuildViewMatrix();

            var model = Matrix4X4<float>.Identity;

            gl.UniformMatrix4(uModel, 1, false, (float*)&model);

            fixed (Matrix4X4<float>* pointerView = &view)
            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uView, 1, false, (float*)pointerView);
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }

            gl.DrawElements(
                PrimitiveType.Triangles,
                36,
                DrawElementsType.UnsignedInt,
                null
                );

            // Render Code (IKingsRenderer,OpenGL, Vulkan🌋)
        }

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

        // Window ----------------------------------------------------------------

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

        // DEBUG ----------------------------------------------------------------

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

        // CAMERA ----------------------------------------------------------------

        private static Vector3D<float> GetViewDirection(float yawDegrees, float pitchDegrees)
        {
            float yaw = DegreesToRadians(yawDegrees);
            float pitch = DegreesToRadians(pitchDegrees);

            float sinPitch = MathF.Sin(pitch);
            float cosPitch = MathF.Cos(pitch);
            float sinYaw = MathF.Sin(yaw);
            float cosYaw = MathF.Cos(yaw);


            return Vector3D.Normalize(new Vector3D<float>(
                x: cosYaw * cosPitch,
                y: sinPitch,
                z: sinYaw * cosPitch
                )
                );
        }

        private static void BuildViewMatrix()
        {
            Vector3D<float> cameraFront = GetViewDirection(yaw, pitch);
            Vector3D<float> cameraRight = Vector3D.Normalize(Vector3D.Cross(cameraFront, Vector3D<float>.UnitY));
            Vector3D<float> cameraUp = Vector3D.Cross(cameraRight, cameraFront);
            view = Matrix4X4.CreateLookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
        }

        private static Matrix4X4<float> CreatePerspective(int width, int height, float fovDegrees, float near, float far)
        {
            float aspect = width <= 0 || height <= 0 ? 1.0f : (float)width / height;
            float fov = DegreesToRadians(fovDegrees);

            return Matrix4X4.CreatePerspectiveFieldOfView(fov, aspect, near, far);
        }

        // Mouse

        private static void OnMouseMove(IMouse mouse, Vector2 mousePosition)
        {
            if (firstMouse)
            {
                lastMousePosition = mousePosition;
                firstMouse = false;
                return;
            }

            rawMouseDelta = mousePosition - lastMousePosition;
            lastMousePosition = mousePosition;

        }
    }
}