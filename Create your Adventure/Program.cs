using Create_your_Adventure.Source.Engine.Assets;
using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Gamelogic.Camera;
using Create_your_Adventure.Source.GameLogic.Chunk;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using StbImageSharp;
using System.Numerics;
using Create_your_Adventure.Source.Engine.Shader;
using Create_your_Adventure.Source.Engine.Render;
using Create_your_Adventure.Source.Engine.Texture;

namespace Create_your_Adventure
{
    internal class Program
    {
        /*
        // -------- ImGui --------
        private static ImGuiController imGuiController;

        // -------- Camera --------
        private static Camera camera;

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
        private static uint instanceVBO;
        private static uint graphicsProgram;
        private static uint texture;

        // -------- Chunk --------
        private static Chunk chunk;
        
        private static readonly float[] vertices =
        {
            // Position             Texture Coordinates
            // Front Face (0-3)
           -0.5f, -0.5f,  0.5f,     0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,     1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,     1.0f, 1.0f,
           -0.5f,  0.5f,  0.5f,     0.0f, 1.0f,

            // Right Face (4-7)
            0.5f, -0.5f,  0.5f,     0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,     1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,     1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,     0.0f, 1.0f,

            // Back Face (8-11)
            0.5f, -0.5f, -0.5f,     0.0f, 0.0f,
           -0.5f, -0.5f, -0.5f,     1.0f, 0.0f,
           -0.5f,  0.5f, -0.5f,     1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,     0.0f, 1.0f,

            // Left Face (12-15)
           -0.5f, -0.5f, -0.5f,     0.0f, 0.0f,
           -0.5f, -0.5f,  0.5f,     1.0f, 0.0f,
           -0.5f,  0.5f,  0.5f,     1.0f, 1.0f,
           -0.5f,  0.5f, -0.5f,     0.0f, 1.0f,

           // Top Face (16-19)
           -0.5f,  0.5f,  0.5f,     0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,     1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,     1.0f, 1.0f,
           -0.5f,  0.5f, -0.5f,     0.0f, 1.0f,

           // Bottom Face (20-23)
           -0.5f, -0.5f, -0.5f,     0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,     1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,     1.0f, 1.0f,
           -0.5f, -0.5f,  0.5f,     0.0f, 1.0f
        };

        private static readonly uint[] indices =
        {
            // North (Front Face)
            0, 1, 2,
            2, 3, 0,

            // East (Right Face)
            4, 5, 6,
            6, 7, 4,

            // South (Back Face)
            8, 9, 10,
            10, 11, 8,

            // West (Left Face)
            12, 13, 14,
            14, 15, 12,

            // Top (Top Face)
            16, 17, 18,
            18, 19, 16,

            // Bottom (Bottom Face)
            20, 21, 22,
            22, 23, 20
        };

        private static readonly uint stride = 5 * sizeof(float);
        */

        // ══════════════════════════════════════════════════
        // MAIN
        // ══════════════════════════════════════════════════
        static void Main()
        {
            var windowManager = WindowManager.Instance;
            windowManager.Initialize(new WindowSettings
            {
                Title = "Create your Adventure",
                Width = 1920,
                Height = 1080
            });

            windowManager.Loaded += OnLoad;
            windowManager.Updated += OnUpdate;
            windowManager.Rendered += OnRender;
            windowManager.OnClose += OnClose;

            windowManager.Run();


            /*
            Logger.Info("[ENGINE] Initializing application...");

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
            Logger.Info("[WINDOW] Window created (1920x1080, OpenGL 4.6 Core)");

            // -------- Event Subscription --------
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;

            // -------- Run Application --------
            Logger.Info("[ENGINE] Starting main loop...");
            window.Run();
            */
        }

        // ══════════════════════════════════════════════════
        // ONLOAD
        // ══════════════════════════════════════════════════
        private static unsafe void OnLoad()
        {
            Logger.Info("[ENGINE] Loading resources...");

            // ═══════════════════════════════════════════════════════════
            // INITIALISIERUNG (Order is important!)
            // ═══════════════════════════════════════════════════════════

            // ═══ 01 ═══ Renderer Manager
            RendererManager.Instance.Initialize();

            // ═══ 02 ═══ Shader Manager
            ShaderManager.Instance.Initialize();

            // ═══════════════════════════════════════════════════════════
            // SHADER LOAD FROM FILES (with AssetLoader)
            // ═══════════════════════════════════════════════════════════

            var vertPath = AssetLoader.GetShaderPath("opengl/basic.vert");
            var fragPath = AssetLoader.GetShaderPath("opengl/basic.frag");

            // ═══ Shader load from files and compile
            var basicShader = ShaderManager.Instance.LoadFromFiles(
                "basic", // name for cache
                vertPath,
                fragPath
                );

            // ═══ shader activate and set uniforms
            basicShader.Use();
            basicShader.SetUniform("uTexture", 0);

            // ═══ 02 ═══ Texture Manager
            TextureManager.Instance.Initialize();

            // ═══════════════════════════════════════════════════════════
            // BUILD BLOCK ATLAS
            // ═══════════════════════════════════════════════════════════
            var blockAtlas = TextureManager.Instance.BuildBlockAtlas();

            // ═══ Test: Get UV coordinates for a block
            if (blockAtlas.HasTexture("dirt"))
            {
                var dirtRegion = TextureManager.Instance.GetBlockUV("dirt");
                Logger.Info($"[ENGINE] Dirt UV: ({dirtRegion.U0:F3}, {dirtRegion.V0:F3}) - ({dirtRegion.U1:F3}, {dirtRegion.V1:F3})");
            }

            Logger.Info($"[ENGINE] TextureManager has {TextureManager.Instance.CachedAtlasCount} atlas(es) cached");
            Logger.Info($"[ENGINE] ShaderManager has {ShaderManager.Instance.CachedProgramCount} program(s) cached");
            Logger.Info("[ENGINE] All resources loaded successfully");

            /*var gl = WindowManager.Instance.GlContext;
            var windowManager = WindowManager.Instance;

            gl.Enable(EnableCap.DebugOutput);
            gl.Enable(EnableCap.DebugOutputSynchronous);

            gl.DebugMessageCallback(DebugCallback, null);
            Logger.Info("[OPENGL] Debug output enabled");

            CenterWindow(window);
            
            // --------Input Initialation --------
            input = window.CreateInput();

            keyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;
            mouse = input.Mice.Count > 0 ? input.Mice[0] : null;

            if (keyboard == null)
            {
                Logger.Warn("[INPUT] No keyboard detected");
            }
            else
            {
                Logger.Info("[INPUT] Keyboard initialized");
            }

            // -------- Mouse Setup --------
            if (mouse != null)
            {
                mouse.Cursor.CursorMode = CursorMode.Disabled;
                mouse.MouseMove += OnMouseMove;
                Logger.Info("[INPUT] Mouse initialized (cursor locked)");
            }
            else
            {
                Logger.Warn("[INPUT] No mouse detected");
            }

            // -------- OpenGL State --------
            gl.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            gl.Enable(EnableCap.DepthTest);
            Logger.Info("[OPENGL] Depth testing enabled");

            // -------- Camera --------
            camera = new Camera();

            // -------- VAO --------
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);
            Logger.Info("[OPENGL] Vertex Array Object created");

            // -------- Instancing Setup --------
            chunk = new Chunk(new Vector3D<long>(0, 0, 0));

            // -------- Instance VBO --------
            instanceVBO = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, instanceVBO);

            fixed (Matrix4X4<float>* ptr = chunk.InstanceMatrices)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (nuint)(chunk.InstanceCount * sizeof(Matrix4X4<float>)),
                    ptr,
                    BufferUsageARB.StaticDraw
                    );
            }
            Logger.Info($"[OPENGL] Instance VBO created ({chunk.InstanceCount} instances)");

            // -------- Instance Attributes (Matrix 4x4 = 4x vec4) --------

            uint mat4Size = (uint)sizeof(Matrix4X4<float>);

            for (uint i = 0; i < 4; i++)
            {
                gl.EnableVertexAttribArray(2 + i);
                gl.VertexAttribPointer(
                    2 + i,
                    4,
                    VertexAttribPointerType.Float,
                    false,
                    mat4Size,
                    (void*)(i * sizeof(Vector4))
                    );
                gl.VertexAttribDivisor(2 + i, 1);
            }
            Logger.Info("[OPENGL] Instance attributes configured");

            // -------- VBO --------
            vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                vertices,
                BufferUsageARB.StaticDraw
                );
            Logger.Info("[OPENGL] Vertex Buffer Object created");

            // -------- EBO --------
            ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                indices,
                BufferUsageARB.StaticDraw
                );
            Logger.Info("[OPENGL] Element Buffer Object created");

            // -------- Vertex Attribute --------
            // --- Position Attribute
            gl.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                stride,
                0
                );
            gl.EnableVertexAttribArray(0);

            // --- Texture Coordinate Attribute
            gl.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                stride,
                3 * sizeof(float)
                );
            gl.EnableVertexAttribArray(1);
            Logger.Info("[OPENGL] Vertex attributes configured");

            // -------- Texture --------
            texture = LoadTexture(AssetLoader.GetTexturePath("dirt.png"));

            if (texture == 0)
            {
                Logger.Error("[TEXTURE] Failed to load dirt.png - rendering may fail");
            }

            // -------- Shader --------
            graphicsProgram = CreateGraphicsProgram();
            gl.UseProgram(graphicsProgram);
            uModel = gl.GetUniformLocation(graphicsProgram, "uModel");
            uView = gl.GetUniformLocation(graphicsProgram, "uView");
            uProjection = gl.GetUniformLocation(graphicsProgram, "uProjection");
            Logger.Info("[SHADER] Graphics program created and uniforms located");

            // -------- Texture Uniform --------
            int uTexture = gl.GetUniformLocation(graphicsProgram, "uTexture");
            gl.Uniform1(uTexture, 0);

            // -------- Initial Projection --------
            projection = camera.CreatePerspective(window.Size.X, window.Size.Y, 60f, 0.1f, 100f);
            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }

            // -------- ImGui Controller initialize
            imGuiController = new ImGuiController(gl, window, input);
            Logger.Info("[IMGUI] ImGui controller initialized");
            */
        }

        // ══════════════════════════════════════════════════
        // ONUPDATE
        // ══════════════════════════════════════════════════
        private static void OnUpdate(double deltaTime)
        {

            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)

            /*
            // -------- Update ImGui
            imGuiController.Update((float)deltaTime);

            // --- Update FPS Counter
            DebugDisplay.Update(deltaTime);

            // -------- Update Camera (WASD/Space/Strg/Shift Input) --------
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
            */
        }

        // ══════════════════════════════════════════════════
        // ONRENDER
        // ══════════════════════════════════════════════════
        private static unsafe void OnRender(double deltaTime)
        {

            RendererManager.Instance.BeginFrame();

            // ═══════════════════════════════════════════════════════════ RENDERING MIT SHADER MANAGER
            var shader = ShaderManager.Instance.UseProgram("basic");

            if (shader is not null)
            {
                // Später: Uniforms setzen, Draw Calls, etc.
                // shader.SetUniform("uView", viewMatrix);
                // shader.SetUniform("uProjection", projectionMatrix);
            }

            TextureManager.Instance.BindAtlas("blocks", 0);

            RendererManager.Instance.EndFrame();

            /*

            // -------- Rendering --------
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            gl.UseProgram(graphicsProgram);
            gl.BindVertexArray(vao);

            // --- Bind Texture
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);

            // --- View Matrix
            var view = camera.GetViewMatrix();

            // --- Update Uniforms
            gl.UniformMatrix4(uView, 1, false, (float*)&view);

            fixed (Matrix4X4<float>* pointerProjection = &projection)
            {
                gl.UniformMatrix4(uProjection, 1, false, (float*)pointerProjection);
            }

            // --- Instanced Draw Call (1 Call for all Cubes)
            gl.DrawElementsInstanced(
                PrimitiveType.Triangles,
                36,
                DrawElementsType.UnsignedInt,
                null,
                (uint)chunk.InstanceCount
                );

            DebugDisplay.RenderImGui();
            imGuiController.Render();

            */
        }

        // ══════════════════════════════════════════════════
        // ONCLOSE
        // ══════════════════════════════════════════════════
        private static void OnClose()
        {

            Logger.Info("[ENGINE] Shutting down...");

            // ═══════════════════════════════════════════════════════════
            // DISPOSE (Reverse order)
            // ═══════════════════════════════════════════════════════════

            TextureManager.Instance.Dispose();
            ShaderManager.Instance.Dispose();
            RendererManager.Instance.Dispose();

            Logger.Info("[ENGINE] Application closed ✓");

            /*
            imGuiController?.Dispose();

            if (mouse != null)
            {
                mouse.MouseMove -= OnMouseMove;
            }

            gl.DeleteBuffer(instanceVBO);
            gl.DeleteTexture(texture);
            gl.DeleteBuffer(ebo);
            gl.DeleteBuffer(vbo);
            gl.DeleteVertexArray(vao);
            gl.DeleteProgram(graphicsProgram);

            Logger.Info("[OPENGL] Resources cleaned up");
            Logger.Info("[ENGINE] Application closed");

            */
        }
        /*

        // WINDOW ----------------------------------------------------------------
        public static void CenterWindow(IWindow window)
        {
            // -------- Get Monitor --------
            var monitor = window.Monitor;
            if (monitor == null)
            {
                Logger.Warn("[WINDOW] No monitor detected - cannot center window");
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

            Logger.Info("[WINDOW] Window centered on monitor");
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
            
            string messageText = SilkMarshal.PtrToString(message);

            if (severity == GLEnum.DebugSeverityHigh)
            {
                Logger.Error($"[OPENGL] {severity}: {messageText}");
            }
            else if (severity == GLEnum.DebugSeverityMedium)
            {
                Logger.Warn($"[OPENGL] {severity}: {messageText}");
            }
            else
            {
                Logger.Info($"[OPENGL] {severity}: {messageText}");
            }
        }

        // SHADER ----------------------------------------------------------------
        private const string VertexShaderSource = @"
        #version 460 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTextureCoordinates;
        layout (location = 2) in vec4 aInstanceMatrix0;
        layout (location = 3) in vec4 aInstanceMatrix1;
        layout (location = 4) in vec4 aInstanceMatrix2;
        layout (location = 5) in vec4 aInstanceMatrix3;

        out vec2 vTextureCoordinates;

        uniform mat4 uView;
        uniform mat4 uProjection;

        void main()
        {
            mat4 instanceMatrix = mat4(
                aInstanceMatrix0,
                aInstanceMatrix1,
                aInstanceMatrix2,
                aInstanceMatrix3
            );

            gl_Position = uProjection * uView * instanceMatrix * vec4(aPosition, 1.0);
            vTextureCoordinates = aTextureCoordinates;
        }
        ";

        private const string FragmentShaderSource = @"
        #version 460 core

        in vec2 vTextureCoordinates;
        out vec4 FragColor;
        
        uniform sampler2D uTexture;
        
        void main()
        {
            FragColor = texture(uTexture, vTextureCoordinates);
        }
        ";

        // SHADER ERROR CHECKING ----------------------------------------------------------------
        private static void CheckShaderCompileErrors(uint shader, string type)
        {
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = gl.GetShaderInfoLog(shader);
                Logger.Error($"[SHADER] Compilation failed ({type}):\n{infoLog}");
                throw new Exception($"Shader compilation error: {type}");
            }
        }

        private static void CheckProgramLinkErrors(uint program)
        {
            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = gl.GetProgramInfoLog(program);
                Logger.Error($"[SHADER] Linking failed:\n{infoLog}");
                throw new Exception("Shader linking error");
            }
        }

        // CREATE GRAPHICS PROGRAM ----------------------------------------------------------------
        private static uint CreateGraphicsProgram()
        {
            Logger.Info("[SHADER] Compiling vertex shader...");
            uint vertex = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertex, VertexShaderSource);
            gl.CompileShader(vertex);
            CheckShaderCompileErrors(vertex, "VERTEX");
            Logger.Info("[SHADER] Vertex shader compiled successfully");

            Logger.Info("[SHADER] Compiling fragment shader...");
            uint fragment = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragment, FragmentShaderSource);
            gl.CompileShader(fragment);
            CheckShaderCompileErrors(fragment, "FRAGMENT");
            Logger.Info("[SHADER] Fragment shader compiled successfully");

            Logger.Info("[SHADER] Linking shader program...");
            uint createProgram = gl.CreateProgram();
            gl.AttachShader(createProgram, vertex);
            gl.AttachShader(createProgram, fragment);
            gl.LinkProgram(createProgram);
            CheckProgramLinkErrors(createProgram);
            Logger.Info("[SHADER] Shader program linked successfully");

            gl.DeleteShader(vertex);
            gl.DeleteShader(fragment);

            return createProgram;
        }

        // LOAD TEXTURE ----------------------------------------------------------------
        private static unsafe uint LoadTexture(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                Logger.Error("[TEXTURE] Invalid texture path provided");
                return 0;
            }

            Logger.Info($"[TEXTURE] Loading texture: {path}");

            // -------- StbImageSharp Configuration --------
            StbImage.stbi_set_flip_vertically_on_load(1);

            // -------- Load Image --------
            ImageResult image;
            try
            {
                using var stream = File.OpenRead(path);
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            catch (FileNotFoundException)
            {
                Logger.Error($"[TEXTURE] File not found: {path}");
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"[TEXTURE] Loading failed: {ex.Message}");
                return 0;
            }

            // -------- OpenGL Texture Creation --------
            uint textureId = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, textureId);

            // --------- Pixel-Daten hochladen --------
            fixed (byte* ptr = image.Data)
            {
                gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    (uint)image.Width,
                    (uint)image.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    ptr
                    );
            }

            // --------- Texture Parameter --------
            gl.GenerateMipmap(TextureTarget.Texture2D);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.NearestMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

            Logger.Info($"[TEXTURE] Loaded successfully: {path} ({image.Width}x{image.Height})");

            return textureId;
        }

        */

        // MOUSE MOVE EVENT ----------------------------------------------------------------

        /*private static void OnMouseMove(IMouse mouse, Vector2 mousePosition)
        {
            camera.OnMouseMove(mousePosition);
        }*/
    }
}