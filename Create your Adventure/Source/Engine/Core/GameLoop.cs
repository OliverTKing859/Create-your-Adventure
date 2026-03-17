using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Assets;
using Create_your_Adventure.Source.Engine.Camera;
using Create_your_Adventure.Source.Engine.Input;
using Create_your_Adventure.Source.Engine.Mesh;
using Create_your_Adventure.Source.Engine.Render;
using Create_your_Adventure.Source.Engine.Shader;
using Create_your_Adventure.Source.Engine.Texture;
using Create_your_Adventure.Source.Engine.Time;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Core
{
    public sealed class GameLoop
    {
        // ══════════════════════════════════════════════════
        // FIELDS
        // ══════════════════════════════════════════════════
        private readonly WindowManager windowManager;
        private IMesh? testCube;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        public GameLoop(WindowManager windowManager)
        {
            this.windowManager = windowManager;

            windowManager.Loaded += OnLoad;
            windowManager.Updated += OnUpdate;
            windowManager.Rendered += OnRender;
            windowManager.OnResize += OnResize;
            windowManager.OnClose += OnClose;

            Logger.Info("[GAMELOOP] GameLoop initialized and events wired");
        }

        // ══════════════════════════════════════════════════
        // ONLOAD
        // ══════════════════════════════════════════════════
        private unsafe void OnLoad()
        {
            Logger.Info("[GAMELOOP] Loading resources...");

            // ═══════════════════════════════════════════════════════════
            // MANAGER INITIALIZATION (Order is important!)
            // ═══════════════════════════════════════════════════════════

            // ═══ 01 ═══ Renderer Manager
            RendererManager.Instance.Initialize();

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

            // ═══ 03 ═══ Texture Manager
            TextureManager.Instance.Initialize();
            TextureManager.Instance.BuildBlockAtlas();
            var dirtRegion = TextureManager.Instance.GetBlockUV("dirt");

            // ═══ 04 ═══ Mesh Manager
            MeshManager.Instance.Initialize();
            testCube = MeshManager.Instance.CreateCube("testBlock", 1.0f, dirtRegion);

            // ═══ 05 ═══ Input Manager
            InputManager.Instance.Initialize();
            InputManager.Instance.LockCursor();

            // ═══ 06 ═══ Time Manager (already available as singleton)
            // TimeManager requires no explicit initialization

            // ═══ 07 ═══ Camera Manager
            CameraManager.Instance.Initialize(worldX: 0, worldY: 0, worldZ: 0);
            CameraManager.Instance.SetMotionMode(CameraMotionMode.Debug);

            var size = windowManager.Size;
            CameraManager.Instance.UpdateAspectRatio(size.X, size.Y);

            Logger.Info("[GAMELOOP] All resources loaded successfully");
        }

        // ══════════════════════════════════════════════════
        // ONUPDATE
        // ══════════════════════════════════════════════════
        private void OnUpdate(double deltaTime)
        {
            // ══════════════════════════════════════════════════
            // PHASE 1: FRAME BEGIN (Time & Input)
            // ══════════════════════════════════════════════════
            TimeManager.Instance.BeginFrame(deltaTime);
            InputManager.Instance.BeginFrame();

            // ══════════════════════════════════════════════════
            // PHASE 2: FIXED SIMULATION TICKS
            // ══════════════════════════════════════════════════
            int ticksThisFrame = TimeManager.Instance.ConsumeFixedTicks();
            float fixedDt = (float)TimeManager.Instance.FixedDeltaTime;

            for (int i = 0; i < ticksThisFrame; i++)
            {
                // ═══ Physics, Player Logic, Chunk Simulation here
                // ═══ PhysicsSystem.Tick(fixedDt);
            }

            // ══════════════════════════════════════════════════
            // PHASE 3: VARIABLE UPDATE (Camera, UI)
            // ══════════════════════════════════════════════════
            float dt = (float)TimeManager.Instance.FrameDeltaTime;

            // [DEBUG] Camera tick
            CameraManager.Instance.UpdateFromInput(dt);

            // ══════════════════════════════════════════════════
            // PHASE 4: FRAME END (Input finalization)
            // ══════════════════════════════════════════════════
            InputManager.Instance.EndFrame(dt);
        }

        // ══════════════════════════════════════════════════
        // ONRENDER
        // ══════════════════════════════════════════════════
        private unsafe void OnRender(double rawDeltaTime)
        {
            RendererManager.Instance.BeginFrame();

            // ═══ Rendering with Shader Manager
            var shader = ShaderManager.Instance.UseProgram("basic");
            var camera = CameraManager.Instance;

            if (shader is not null)
            {
                shader.SetUniform("uView", camera.GetViewMatrix());
                shader.SetUniform("uProjection", camera.GetProjectionMatrix());
                shader.SetUniform("uModel", Matrix4X4<float>.Identity);
            }

            // ═══ Bind Atlas
            TextureManager.Instance.BindAtlas("blocks", 0);

            // ═══ Draw Cube
            testCube?.Draw();

            RendererManager.Instance.EndFrame();
        }

        // ══════════════════════════════════════════════════
        // ONRESIZE
        // ══════════════════════════════════════════════════
        private void OnResize(Vector2D<int> size)
        {
            CameraManager.Instance.UpdateAspectRatio(size.X, size.Y);
        }

        // ══════════════════════════════════════════════════
        // ONCLOSE
        // ══════════════════════════════════════════════════
        private void OnClose()
        {
            Logger.Info("[GAMELOOP] Shutting down...");

            // ═══════════════════════════════════════════════════════════
            // DISPOSE (Reverse order - LIFO)
            // ═══════════════════════════════════════════════════════════

            // ═══ 07 ═══ Camera Manager (no dispose needed, stateless singleton)
            // ═══ 06 ═══ Time Manager (no dispose needed)
            // ═══ 05 ═══ Input Manager
            InputManager.Instance.Dispose();
            // ═══ 04 ═══ Mesh Manager
            MeshManager.Instance.Dispose();
            // ═══ 03 ═══ Texture Manager
            TextureManager.Instance.Dispose();
            // ═══ 02 ═══ Shader Manager
            ShaderManager.Instance.Dispose();
            // ═══ 01 ═══ Renderer Manager
            RendererManager.Instance.Dispose();

            Logger.Info("[GAMELOOP] All systems disposed ✓");
        }
    }
}
