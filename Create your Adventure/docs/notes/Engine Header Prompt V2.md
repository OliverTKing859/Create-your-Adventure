# Create Your Adventure — Voxel Engine (C# / Silk.NET / OpenGL 4.6)
Current Version: 0.7.1.1 Alpha

## Tech Stack
- Language: C# (.NET 10, unsafe allowed)
- Windowing/Input: Silk.NET (GLFW backend)
- Graphics: OpenGL 4.6 (Vulkan-compatible abstraction layer)
- UI: ImGui.NET
- Textures: StbImageSharp (RGBA, mipmaps)
- Coordinates: long for chunk coords, int for local (0–15), float for render

## Architecture
Program → Engine → GameLoop → Managers

Program.cs:       Only calls Engine.Start()
GameLoop:         5-phase loop (EarlyUpdate → FixedTick → Scheduler → Update → LateUpdate)
Managers:         All systems, Singleton + Lock pattern
TimeManager:      Single time authority (FixedDt 1/60s, DeltaTime, InterpolationAlpha)

## Active Systems (all Singleton, all API-agnostic interfaces)
- WindowManager      → Window, GL context, resize events
- RenderManager      → IRenderContext, OpenGLRenderContext, viewport, clear
- ShaderManager      → IShaderProgram, GLSL file loading, uniform cache
- TextureManager     → ITexture / ITextureAtlas, block atlas, UV lookup
- MeshManager        → IMesh, VAO/VBO/EBO, GPU instancing, DrawInstanced()
- InputManager       → Keyboard / Mouse / Gamepad, Actions + Direct Queries
- TimeManager        → FixedTick, DeltaTime, WorldTime, InterpolationAlpha
- CameraManager      → Subsystems: Transform, Projection, MotionModel, WorldBinding
- WorldScheduler     → Budget-based task scheduler (8ms/frame), 5 task types
- AssetLoader        → assets/base/ and assets/modded/, caching

## Camera Subsystems
- CameraTransform    → LocalPosition (float), Yaw/Pitch/Roll, Forward/Right/Up
- CameraProjection   → FOV (base + modifier), Near 0.05f, Far 2000f
- CameraMotionModel  → ExpDecay acceleration, 5 motion modes (Walk/Fly/Glider/Cinematic/Spectator)
- CameraWorldBinding → Floating Origin (shift threshold: 256 blocks), ChunkChanged event
- CameraVisibilityContext → ViewMatrix, ProjectionMatrix, VP = P×V, ViewFrustum (6 planes)

## Chunk & World
- ChunkCoord:        struct with long X/Y/Z (±9.2 trillion chunks per axis)
- ChunkState:        Requested → Loading → Meshing → Active → Sleeping → Serializing → Unloaded
- ChunkPriority:     Critical (60TPS) → High (30) → Medium (10) → Low (2) → Background (1)
- WorldRelevanceFilter: RenderDist 16, SimDist 24, LoadDist 32, UnloadDist 40 (in chunks)
- GPU Instancing:    1 draw call per chunk, Matrix4X4[] instance buffer

## Input Rules
- Gameplay:          Must use InputActions (rebindable)
- Camera/Debug/Editor: May use Direct Queries (InputManager.Analyzer)
- Default bindings:  WASD + Space/Ctrl + Shift, Escape = toggle cursor lock
- Time injected:     InputManager.EndFrame(deltaTime) — never self-counted

## Design Rules
- No ECS
- No unnecessary abstraction layers
- All managers initialize in order: Window → Render → Shader → Texture → Mesh → Input → Time → Camera
- All managers dispose in reverse (LIFO)
- Logging: [ENGINE] [OPENGL] [SHADER] [TEXTURE] [CHUNK] [MESH] [INPUT] [CAMERA] categories
- XML documentation on all public methods
- English summaries, English inline comments

## Non-Goals
- No ECS framework
- No overengineering
- No Vulkan yet (but abstraction layer is Vulkan-ready)
- No UI framework beyond ImGui debug overlay