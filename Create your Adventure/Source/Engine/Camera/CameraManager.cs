using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Input;
using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Central manager for the camera system, coordinating all camera subsystems.
    /// Combines transform, projection, motion, and world binding into a unified interface.
    /// Handles frame updates, input processing, matrix generation, and visibility culling.
    /// Uses caching to avoid redundant matrix calculations and supports multiple motion modes.
    /// Solves the "Far Lands" problem through proper floating-point origin shifting.
    /// </summary>
    public class CameraManager
    {
        // ══════════════════════════════════════════════════
        // SINGLETON
        // ══════════════════════════════════════════════════
        // ═══ Singleton instance of the CameraManager
        private static CameraManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the CameraManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
        public static CameraManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new CameraManager();
                    }
                }

                return instance;
            }
        }
        // ══════════════════════════════════════════════════
        // SUBSYSTEMS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the camera's position and rotation transform.
        /// Contains local position, Euler angles (Yaw/Pitch/Roll), and computed direction vectors.
        /// </summary>
        public CameraTransform Transform;

        /// <summary>
        /// Gets or sets the camera's projection parameters.
        /// Defines FOV, aspect ratio, near/far planes, and dynamic FOV modifiers.
        /// </summary>
        public CameraProjection Projection;

        /// <summary>
        /// Gets or sets the camera's motion model.
        /// Controls movement speed, acceleration, look sensitivity, and smoothing behavior.
        /// </summary>
        public CameraMotionModel Motion;

        /// <summary>
        /// Gets the world binding that manages infinite world coordinates and origin shifting.
        /// Prevents floating-point precision loss in large worlds (the "Far Lands" problem).
        /// </summary>
        public CameraWorldBinding WorldBinding { get; }

        // ══════════════════════════════════════════════════
        // CONFIGURATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the current camera motion mode (Walk, Fly, Glider, etc.).
        /// Each mode uses different motion presets optimized for that gameplay scenario.
        /// </summary>
        public CameraMotionMode CurrentMode { get; private set; }

        /// <summary>
        /// Gets or sets the render distance in chunks.
        /// Chunks beyond this distance are not loaded or rendered.
        /// Typical values: 8-32 chunks (128-512 blocks).
        /// </summary>
        public int RenderDistanceChunks { get; set; } = 16;

        // ══════════════════════════════════════════════════
        // STATE
        // ══════════════════════════════════════════════════
        // ═══ Flag to track whether the camera has been initialized with a world position
        private bool isInitialized;

        /// <summary>
        /// Gets a value indicating whether the camera has been initialized.
        /// Camera must be initialized before use.
        /// </summary>
        public bool IsInitialized => isInitialized;

        // ══════════════════════════════════════════════════
        // CACHED OUTPUT
        // ══════════════════════════════════════════════════
        // ═══ Cached view and projection matrices to avoid recalculation every frame
        private Matrix4X4<float> cachedViewMatrix;
        private Matrix4X4<float> cachedProjectionMatrix;

        // ═══ Cached visibility context for culling (includes frustum and render distance)
        private CameraVisibilityContext? cachedVisibility;

        // ═══ Dirty flags to track when caches need updating
        private bool matricesDirty = true;
        private bool visibilityDirty = true;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the CameraManager with default settings.
        /// All subsystems are created with default values - call Initialize() to set actual world position.
        /// </summary>
        public CameraManager()
        {
            Transform = CameraTransform.Default;
            Projection = CameraProjection.Default;
            Motion = CameraMotionModel.Default;
            WorldBinding = new CameraWorldBinding();
            CurrentMode = CameraMotionMode.Walk;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the camera at a specific world position.
        /// Sets up world binding and calculates initial local position.
        /// Must be called before camera updates begin.
        /// </summary>
        /// <param name="worldX">Absolute world X coordinate in blocks.</param>
        /// <param name="worldY">Absolute world Y coordinate in blocks.</param>
        /// <param name="worldZ">Absolute world Z coordinate in blocks.</param>
        public void Initialize(long worldX, long worldY, long worldZ)
        {
            if (isInitialized)
            {
                Logger.Warn("[CAMERA] CameraManager already initialized");
                return;
            }

            // ═══ Initialize world binding with spawn position
            WorldBinding.Initialize(worldX, worldY, worldZ);

            // ═══ Calculate initial local position within chunk (0-16 range)
            Transform.LocalPosition = new Vector3D<float>(
                worldX % CameraWorldBinding.ChunkSize,
                worldY % CameraWorldBinding.ChunkSize,
                worldZ % CameraWorldBinding.ChunkSize
            );

            // ═══ Invalidate caches to force recalculation on first frame
            InvalidateCache();

            isInitialized = true;
            Logger.Info($"[CAMERA] CameraManager initialized at ({worldX}, {worldY}, {worldZ})");
        }

        // ══════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Updates the camera for the current frame based on input and delta time.
        /// Processes motion, rotation, position changes, and handles origin shifting.
        /// Call this every frame with current input state.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        /// <param name="movementInput">Normalized movement input (X=strafe, Y=vertical, Z=forward/back).</param>
        /// <param name="lookInput">Mouse delta for camera rotation (X=yaw, Y=pitch).</param>
        /// <param name="verticalInput">Vertical input for fly mode (-1=down, 0=none, 1=up).</param>
        /// <param name="isSprinting">Whether sprint modifier should be applied.</param>
        public void Update(
            float dt,
            Vector3D<float> movementInput,
            Vector2D<float> lookInput,
            float verticalInput = 0f,
            bool isSprinting = false
            )
        {
            // ═══ Apply motion model to compute velocities from input
            Motion.ApplyMovementInput(
                ComputeWorldMovementDirection(movementInput),
                isSprinting,
                dt
            );
            Motion.ApplyVerticalInput(verticalInput, dt);

            // ═══ Process camera rotation with smoothing
            var rotationDelta = Motion.ApplyLookInput(lookInput, dt);
            ApplyRotation(rotationDelta, dt);

            // ═══ Integrate velocities to get position change
            var positionDelta = Motion.ComputePositionDelta(dt);
            Transform.LocalPosition += positionDelta;

            // ═══ Update world binding and handle potential origin shift
            // ═══ CRITICAL: If origin shifts, position is corrected to prevent "Far Lands" bug
            Transform.LocalPosition = WorldBinding.UpdateFromLocalPosition(Transform.LocalPosition);

            // ═══ Mark cached matrices as dirty for next getter call
            InvalidateCache();
        }

        /// <summary>
        /// Convenience method that pulls input directly from InputManager.
        /// Automatically queries movement, look, vertical, and sprint inputs.
        /// Use this for standard player-controlled camera updates.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        public void UpdateFromInput(float dt)
        {
            var input = InputManager.Instance;

            // ═══ Get normalized movement vector from WASD or gamepad stick
            var move = input.GetMovementVector();
            var look = input.GetLookVector();
            var verticalMove = input.GetVerticalMovement();
            bool sprint = input.IsActionTriggered("Sprint");

            // ═══ Convert 2D movement to 3D (Y=0 for horizontal movement)
            Update(
                dt,
                movementInput: new Vector3D<float>(move.X, 0f, move.Y),
                lookInput: new Vector2D<float>(look.X, look.Y),
                verticalInput: verticalMove,
                isSprinting: sprint
            );
        }

        /// <summary>
        /// Converts input direction to world-space movement direction based on camera orientation.
        /// Handles different motion modes (grounded vs. flying) by using ForwardFlat or full Forward.
        /// </summary>
        /// <param name="input">Normalized input direction (X=strafe, Y=vertical, Z=forward).</param>
        /// <returns>World-space movement direction.</returns>
        private Vector3D<float> ComputeWorldMovementDirection(Vector3D<float> input)
        {
            // ═══ No input = no movement
            if (input.LengthSquared < 0.0001f)
                return Vector3D<float>.Zero;

            // ═══ Flying modes: use full 3D orientation (can move in look direction)
            if (CurrentMode == CameraMotionMode.Fly || CurrentMode == CameraMotionMode.Spectator)
            {
                return Transform.Forward * input.Z +
                       Transform.Right * input.X +
                       Transform.Up * input.Y;
            }

            // ═══ Grounded modes: use flat forward (ignore pitch, always horizontal)
            // ═══ Prevents moving up/down when looking up/down while walking
            return Transform.ForwardFlat * input.Z + Transform.Right * input.X;
        }

        /// <summary>
        /// Applies rotation delta to camera transform with proper clamping and normalization.
        /// Handles yaw wrapping, pitch clamping (prevents gimbal lock), and roll return.
        /// </summary>
        /// <param name="delta">Rotation delta in degrees (X=yaw, Y=pitch).</param>
        /// <param name="dt">Delta time for roll return smoothing.</param>
        private void ApplyRotation(Vector2D<float> delta, float dt)
        {
            // ═══ Apply yaw rotation (horizontal look)
            Transform.Yaw += delta.X;

            // ═══ Normalize yaw to [-180, 180] to prevent unbounded growth
            // ═══ Using double modulo to handle both positive and negative correctly
            Transform.Yaw = ((Transform.Yaw + 180f) % 360f + 360f) % 360f - 180f;

            // ═══ Apply pitch rotation (vertical look) - inverted for natural mouse feel
            Transform.Pitch -= delta.Y;

            // ═══ Clamp pitch to prevent looking past vertical (gimbal lock)
            // ═══ Typical limit: ±89° (just below straight up/down)
            Transform.Pitch = MathHelper.Clamp(Transform.Pitch, -Motion.MaxPitch, Motion.MaxPitch);

            // ═══ Auto-return roll to zero if roll is disabled (standard FPS camera)
            // ═══ Uses exponential decay for smooth return to level
            if (!Motion.AllowRoll && MathF.Abs(Transform.Roll) > 0.01f)
            {
                Transform.Roll = MathHelper.ExpDecay(Transform.Roll, 0f, Motion.RollReturnRate, dt);
            }

            // ═══ Normalize roll to [-180, 180]
            Transform.Roll = ((Transform.Roll + 180) % 360f + 360f) % 360f - 180f;

            // ═══ Recalculate direction vectors from updated Euler angles
            Transform.UpdateRotation();
        }

        // ══════════════════════════════════════════════════
        // MODE SWITCHING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Switches the camera to a different motion mode.
        /// Each mode has different speed, acceleration, and control characteristics.
        /// Instantly swaps the motion model to the appropriate preset.
        /// </summary>
        /// <param name="mode">The target motion mode.</param>
        public void SetMotionMode(CameraMotionMode mode)
        {
            if (CurrentMode == mode) return;

            CurrentMode = mode;

            // ═══ Swap motion model to mode-specific preset
            Motion = mode switch
            {
                CameraMotionMode.Walk => CameraMotionModel.Presets.Walk,
                CameraMotionMode.Fly => CameraMotionModel.Presets.Fly,
                CameraMotionMode.Glider => CameraMotionModel.Presets.Glider,
                CameraMotionMode.Cinematic => CameraMotionModel.Presets.Cinematic,
                CameraMotionMode.Spectator => CameraMotionModel.Presets.Fly,
                CameraMotionMode.Debug => CameraMotionModel.Presets.Debug,
                _ => CameraMotionModel.Default
            };

            Logger.Info($"[CAMERA] Motion mode changed to {mode}");
        }

        // ══════════════════════════════════════════════════
        // MATRIX GENERATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the view matrix (world-to-camera transform).
        /// Cached and only recalculated when camera moves or rotates.
        /// </summary>
        /// <returns>The current view matrix.</returns>
        public Matrix4X4<float> GetViewMatrix()
        {
            if (matricesDirty) UpdateMatrices();
            return cachedViewMatrix;
        }

        /// <summary>
        /// Gets the projection matrix (camera-to-clip-space transform).
        /// Cached and only recalculated when projection parameters change.
        /// </summary>
        /// <returns>The current projection matrix.</returns>
        public Matrix4X4<float> GetProjectionMatrix()
        {
            if (matricesDirty) UpdateMatrices();
            return cachedProjectionMatrix;
        }

        /// <summary>
        /// Updates the aspect ratio when window is resized.
        /// Invalidates caches to force projection matrix recalculation.
        /// </summary>
        /// <param name="width">New window width in pixels.</param>
        /// <param name="height">New window height in pixels.</param>
        public void UpdateAspectRatio(int width, int height)
        {
            Projection.UpdateAspect(width, height);
            InvalidateCache();
        }

        /// <summary>
        /// Recalculates view and projection matrices from current camera state.
        /// Called automatically when matrices are dirty and requested.
        /// </summary>
        private void UpdateMatrices()
        {
            var pos = Transform.LocalPosition;
            var target = pos + Transform.Forward;
            var up = Transform.Up;

            cachedViewMatrix = Matrix4X4.CreateLookAt(pos, target, up);
            cachedProjectionMatrix = Projection.GetProjectionMatrix();
            matricesDirty = false;
        }

        // ══════════════════════════════════════════════════
        // CACHE MANAGEMENT
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Marks all cached data as dirty, forcing recalculation on next access.
        /// Called whenever camera state changes (position, rotation, projection).
        /// </summary>
        private void InvalidateCache()
        {
            matricesDirty = true;
            visibilityDirty = true;
        }

        /// <summary>
        /// Updates the cached visibility context with current camera state.
        /// Extracts frustum planes and combines all data needed for culling.
        /// Call this once per frame before rendering.
        /// </summary>
        public void UpdateVisibilityCache()
        {
            // ═══ Ensure matrices are up-to-date before building visibility context
            if (matricesDirty) UpdateMatrices();

            // ═══ Build visibility context with all culling data
            cachedVisibility = new CameraVisibilityContext(
                cameraChunk: WorldBinding.CurrentChunk,
                localPosition: Transform.LocalPosition,
                forward: Transform.Forward,
                viewMatrix: cachedViewMatrix,
                projectionMatrix: cachedProjectionMatrix,
                renderDistanceChunks: RenderDistanceChunks,
                farPlane: Projection.FarPlane
            );

            visibilityDirty = false;
        }

        // ══════════════════════════════════════════════════
        // VISIBILITY OUTPUT
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the current visibility context for culling operations.
        /// Contains frustum planes, matrices, and render distance for visibility tests.
        /// Automatically updates if dirty.
        /// </summary>
        /// <returns>The current camera visibility context.</returns>
        public CameraVisibilityContext GetVisibilityContext()
        {
            if (visibilityDirty || cachedVisibility is null)
            {
                UpdateVisibilityCache();
            }

            return cachedVisibility!.Value;
        }

        /// <summary>
        /// Quick visibility test for a chunk based on distance from camera.
        /// More sophisticated tests (frustum culling) are done by the renderer.
        /// </summary>
        /// <param name="chunkCoord">The chunk coordinate to test.</param>
        /// <param name="chunkWorldSize">Size of chunks in blocks (typically 16).</param>
        /// <returns>True if chunk is within render distance, false otherwise.</returns>
        public bool IsChunkVisible(ChunkCoord chunkCoord, float chunkWorldSize)
        {
            var visibility = GetVisibilityContext();

            // ═══ Distance-based culling (squared to avoid sqrt)
            long distSq = chunkCoord.DistanceSquaredTo(visibility.CameraChunk);
            if (distSq > (long)RenderDistanceChunks * RenderDistanceChunks)
                return false;

            return true;
        }

        // ══════════════════════════════════════════════════
        // DIRECT SETTERS (for teleportation, cutscenes, etc.)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Instantly teleports the camera to a new world position.
        /// Resets velocity to prevent momentum carry-over.
        /// Used for respawning, fast travel, or entering portals.
        /// </summary>
        /// <param name="worldX">Target world X coordinate.</param>
        /// <param name="worldY">Target world Y coordinate.</param>
        /// <param name="worldZ">Target world Z coordinate.</param>
        public void TeleportTo(long worldX, long worldY, long worldZ)
        {
            // ═══ Reinitialize world binding at new position
            WorldBinding.Initialize(worldX, worldY, worldZ);

            // ═══ Convert to local coordinates
            Transform.LocalPosition = WorldBinding.WorldToLocal(worldX, worldY, worldZ);

            // ═══ Reset all motion to prevent carrying momentum through teleport
            Motion.Velocity = Vector3D<float>.Zero;
            Motion.VerticalVelocity = 0f;

            InvalidateCache();
        }

        /// <summary>
        /// Directly sets camera rotation (for cutscenes, spawning, etc.).
        /// Bypasses motion model smoothing - instant rotation change.
        /// Clamps pitch to prevent gimbal lock.
        /// </summary>
        /// <param name="yaw">Horizontal rotation in degrees.</param>
        /// <param name="pitch">Vertical rotation in degrees (will be clamped).</param>
        /// <param name="roll">Tilt rotation in degrees (default 0 for level camera).</param>
        public void SetRotation(float yaw, float pitch, float roll = 0f)
        {
            Transform.Yaw = yaw;
            Transform.Pitch = MathHelper.Clamp(pitch, -Motion.MaxPitch, Motion.MaxPitch);
            Transform.Roll = roll;

            // ═══ Ensure direction vectors are recalculated from new rotation
            Transform.UpdateRotation();

            InvalidateCache();
        }

        /// <summary>
        /// Sets a temporary FOV modifier (for sprint, zoom, damage effects, etc.).
        /// Added to base FOV to calculate effective FOV for rendering.
        /// </summary>
        /// <param name="modifier">FOV adjustment in degrees (positive = wider, negative = narrower).</param>
        public void SetFovModifier(float modifier)
        {
            Projection.FovModifier = modifier;
            InvalidateCache();
        }
    }

    /// <summary>
    /// Defines different camera motion modes with distinct control characteristics.
    /// Each mode uses a different motion preset optimized for specific gameplay scenarios.
    /// </summary>
    public enum CameraMotionMode : byte
    {
        /// <summary>Standard grounded walking with horizontal-only forward direction.</summary>
        Walk,

        /// <summary>Free 3D flight with full directional movement.</summary>
        Fly,

        /// <summary>Gliding with momentum, air resistance, and roll for banking turns.</summary>
        Glider,

        /// <summary>Slow, smooth movement for screenshots and cinematic sequences.</summary>
        Cinematic,

        /// <summary>Like fly mode but can pass through blocks (debug/creative).</summary>
        Spectator,

        /// <summary>Fast, responsive movement for level editing and debugging.</summary>
        Debug
    }
}