using Create_your_Adventure.Source.Engine.Assets;
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
using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Mesh;
using Create_your_Adventure.Source.Engine.Input;

namespace Create_your_Adventure
{
    internal class Program
    {
        private static IMesh? testCube;
        private static Camera? camera;

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

            // ═══════════════════════════════════════════════════════════
            // ═══ 01 ═══ Renderer Manager
            RendererManager.Instance.Initialize();

            // ═══════════════════════════════════════════════════════════
            // ═══ 02 ═══ Shader Manager
            ShaderManager.Instance.Initialize();

            var vertPath = AssetLoader.GetShaderPath("opengl/basic.vert");
            var fragPath = AssetLoader.GetShaderPath("opengl/basic.frag");

            var shader = ShaderManager.Instance.LoadFromFiles(
                "basic",
                vertPath,
                fragPath
                );

            shader.Use();
            shader.SetUniform("uTexture", 0);

            // ═══════════════════════════════════════════════════════════
            // ═══ 03 ═══ Texture Manager
            TextureManager.Instance.Initialize();
            TextureManager.Instance.BuildBlockAtlas();
            var dirtRegion = TextureManager.Instance.GetBlockUV("dirt");

            // ═══════════════════════════════════════════════════════════
            // ═══ 04 ═══ Mesh Manager
            MeshManager.Instance.Initialize();

            testCube = MeshManager.Instance.CreateCube("testBlock", 1.0f, dirtRegion);

            // ═══════════════════════════════════════════════════════════
            // ═══ 05 ═══ Input Manager
            InputManager.Instance.Initialize();
            InputManager.Instance.LockCursor();

            // ═══════════════════════════════════════════════════════════
            // ═══ 06 ═══ Camera Manager
            camera = new Camera();

            Logger.Info("[ENGINE] All resources loaded successfully");
        }

        // ══════════════════════════════════════════════════
        // ONUPDATE
        // ══════════════════════════════════════════════════
        private static void OnUpdate(double deltaTime)
        {
            float dt = (float)deltaTime;

            camera?.Update(dt);

            // ═══ Input Frame begin
            InputManager.Instance.BeginFrame();

            // ═══ Input Frame End
            InputManager.Instance.EndFrame(dt);



            // Game Logic (Input, Physics, Chunk Management, etc pp 😜)
        }

        // ══════════════════════════════════════════════════
        // ONRENDER
        // ══════════════════════════════════════════════════
        private static unsafe void OnRender(double deltaTime)
        {
            // ═══════════════════════════════════════════════════════════
            RendererManager.Instance.BeginFrame();

            // ═══════════════════════════════════════════════════════════
            // ═══ Rendering with Shader Manager
            var shader = ShaderManager.Instance.UseProgram("basic");

            if (shader is not null && camera is not null)
            {
                var windowSize = WindowManager.Instance.Size;

                shader.SetUniform("uView", camera.GetViewMatrix());
                shader.SetUniform("uProjection", camera.GetProjectionMatrix(
                    windowSize.X,
                    windowSize.Y,
                    fovDegrees: 60f,
                    near: 0.1f,
                    far: 1000f
                    )); 
                shader.SetUniform("uModel", Matrix4X4<float>.Identity);
            }

            // ═══════════════════════════════════════════════════════════
            // ═══ Bind Atlas
            TextureManager.Instance.BindAtlas("blocks", 0);

            // ═══════════════════════════════════════════════════════════
            // ═══ Draw Cube
            testCube?.Draw();

            // ═══════════════════════════════════════════════════════════
            RendererManager.Instance.EndFrame();
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

            InputManager.Instance.Dispose();
            MeshManager.Instance.Dispose();
            TextureManager.Instance.Dispose();
            ShaderManager.Instance.Dispose();
            RendererManager.Instance.Dispose();

            Logger.Info("[ENGINE] Application closed ✓");
        }
    }
}