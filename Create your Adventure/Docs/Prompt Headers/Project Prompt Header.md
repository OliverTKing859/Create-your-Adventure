Prompt Header

🔧 TECH STACK
Language  : C# (.NET 10), unsafe allowed
Graphics  : OpenGL 4.6 via Silk.NET, Vulkan abstraction layer prepared
Windowing : Silk.NET.Windowing + Silk.NET.Input
Math      : Silk.NET.Maths (Vector2D/3D, Matrix4X4<float>, Quaternion<float>)

🏗️ EXISTING SYSTEMS — DO NOT REIMPLEMENT
Before writing any new code, check if the system already exists below.
Singletons (all thread-safe via Lock / Double-Check)
SingletonFile hintRoleWindowManagerwindowSilk.NET window + GL/Input context, lifecycle eventsRendererManagerrenderIRenderContext (OpenGL), BeginFrame/EndFrame, ViewportShaderManagershaderLoad, compile, cache IShaderProgram; UseProgram state-trackingTextureManagertextureLoad, cache, bind ITexture + ITextureAtlas; GetBlockUV()MeshManagermeshCreate, cache, draw IMesh (VAO/VBO/EBO via OpenGLMesh)InputManagerinputKeyboardDevice, MouseDevice, GamepadDevice; BeginFrame/EndFrameTimeManagertimeDeltaTime, FixedTick (60 TPS), WorldTime, InterpolationAlphaCameraManagercameraView/Projection matrices, CameraVisibilityContext, motion modes
World / Chunk Systems
ClassRoleWorldSchedulerBudget-based frame scheduler (~8 ms), 5 weighted task queuesWorldRelevanceFilterDistance + frustum culling → ShouldLoad/Render/Simulate/UnloadChunkJobChunk work-item: state, priority, tick-rate, metadataChunkCoordImmutable 3D chunk coordinate (long X/Y/Z)ChunkPriorityCritical / High / Medium / Low / BackgroundChunkMetadataHasWater, HasActiveEntities, Biome, Temperature
Camera Subsystems
Class/StructRoleCameraTransformPosition, Euler angles, quaternion directionsCameraProjectionFoV, Near/Far, AspectRatio, projection matrixCameraMotionModelAcceleration, smoothing, presets per modeCameraWorldBindingChunk tracking, origin-shift, coord conversionCameraVisibilityContextRendering snapshot: matrices, frustum, render distanceViewFrustum6-plane frustum, AABB + sphere intersection testsCameraMotionModeWalk / Fly / Glider / Cinematic / Spectator / Debug
Asset & Utility
ClassRoleAssetLoaderTyped path lookup (Audio/Model/Shader/Texture), modded → base fallback, path cacheLoggerColor-coded console output (Info/Warn/Error)
Interfaces (use these, do not bypass)

IMesh — Bind(), Draw(), DrawInstanced(), Create(vertices, indices, layout)
IShaderProgram — Compile(), UseProgram(), SetUniform(...)
ITexture / ITextureAtlas — Load, Bind, GetRegion/GetBlockUV
IRenderContext — Initialize(), BeginFrame(), EndFrame(), SetViewport(), Dispose()

Key Data Types

VertexLayout / VertexAttribute / VertexAttributeType — vertex format description
AtlasRegion — UV coords from texture atlas
TextureSettings — immutable record (filter, wrap, mipmap; presets: PixelArt, Smooth, Atlas)
ChunkState — Requested → Loading → Meshing → Ready → Unloaded

🔗 CODE INTEGRATION RULE

When implementing new functionality:

1. Prefer extending existing classes.
2. Only create new classes if absolutely necessary.
3. Follow the existing folder structure.
4. Do not duplicate existing systems.

🔄 GAMELOOP ORDER (OnLoad init / OnClose dispose LIFO)
01 WindowManager  →  02 RendererManager  →  03 ShaderManager
→  04 TextureManager  →  05 MeshManager  →  06 InputManager  →  07 TimeManager
→  08 CameraManager

OnUpdate:
  1. Time.BeginFrame + Input.BeginFrame
  2. ConsumeFixedTicks  →  [Physics stub]
  3. Camera.Update
  4. Input.EndFrame

OnRender:
  RendererManager.BeginFrame → ShaderManager.UseProgram → Camera uniforms
  → TextureManager.BindAtlas → mesh.Draw → RendererManager.EndFrame
  
🧱 ARCHITECTURE RULES — STRICT

Never create alternative managers.
Never bypass managers to access OpenGL directly.
Never instantiate OpenGLMesh, OpenGLTexture, etc. outside the Render backend.

Always use:

RendererManager
ShaderManager
TextureManager
MeshManager
InputManager
TimeManager
CameraManager

Graphics code must always use interfaces:

IMesh
IShaderProgram
ITexture
ITextureAtlas
IRenderContext

Do not introduce new global state.
Do not introduce static helper classes for rendering.

⚠️ KNOWN BUGS — DO NOT WORK AROUND, FIX AT SOURCE
PriorityBugLocation🔴skipNextDelta set but never read → first-frame delta spikeMouseDevice🔴DoublePress defined but never implemented in IsActive()All binding classes🔴boundTexture/boundAtlas assignments swapped in BindTexture()TextureManager🔴currentY *= textureSize should be += in atlas row-wrapOpenGLTextureAtlas.Build()🟡LongPressThreshold defined twiceKeyBinding + InputAnalyzer🟡LastTaskTimings stores taskBudgetMs instead of taskUsedMsWorldScheduler🟡GetChunkAABB() mixes local + absolute coordsWorldRelevanceFilter

📐 CODING CONVENTIONS

All managers: Singleton with static Instance, thread-safe init
Graphics abstraction: always code against interfaces (IMesh, IShaderProgram, etc.), never OpenGLMesh directly
Backend factory pattern already prepared for Vulkan/DirectX — do not hardcode OpenGL paths in new code
Asset paths: always resolve via AssetLoader.Get*Path(), never hardcode paths
Logging: use static Logger (Info/Warn/Error), no Console.WriteLine
unsafe is allowed where performance-critical (buffer uploads, pointer math)
Math types: use Silk.NET.Maths (Vector3D<float>, Matrix4X4<float>, etc.)

🚧 STUBS WAITING FOR IMPLEMENTATION (safe to implement next)

ProcessChunkLoad(ChunkJob) — WorldScheduler, only sets state, no worldgen/disk logic
ProcessChunkMesh(ChunkJob) — WorldScheduler, only sets state, no mesh generation
ProcessBlockUpdatesWithBudget(double) — empty body
ProcessLightningWithBudget(double) — empty body
ProcessEntitiesWithBudget(double) — empty body
Physics slot in GameLoop OnUpdate Phase 2 — placeholder comments only
WorldRelevanceFilter — not yet wired into GameLoop
Frustum culling in CameraManager.IsChunkVisible() — ViewFrustum ready, not connected

📦 RESPONSE FORMAT

When generating code:

• Provide complete classes, not fragments.
• Include using statements.
• Follow KingsEngine naming conventions.
• Do not generate pseudocode.
• Ensure code compiles in .NET 10.

KingsEngine · C# .NET 10 · Silk.NET · OpenGL 4.6 · Vulkan abstraction layer

