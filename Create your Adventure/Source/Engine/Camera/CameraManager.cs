using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Input;
using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public class CameraManager
    {
        // ══════════════════════════════════════════════════
        // SINGLETON
        // ══════════════════════════════════════════════════
        private static CameraManager? instance;
        private static readonly Lock instanceLock = new();

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
        public CameraTransform Transform;
        public CameraProjection Projection;
        public CameraMotionModel Motion;
        public CameraWorldBinding WorldBinding { get; }

        // ══════════════════════════════════════════════════
        // CONFIGURATION
        // ══════════════════════════════════════════════════
        public CameraMotionMode CurrentMode { get; private set; }
        public int RenderDistanceChunks { get; set; } = 16;

        // ══════════════════════════════════════════════════
        // STATE
        // ══════════════════════════════════════════════════
        private bool isInitialized;
        public bool IsInitialized => isInitialized;

        // ══════════════════════════════════════════════════
        // CACHED OUTPUT
        // ══════════════════════════════════════════════════
        private Matrix4X4<float> cachedViewMatrix;
        private Matrix4X4<float> cachedProjectionMatrix;
        private CameraVisibilityContext? cachedVisibility;
        private bool matricesDirty = true;
        private bool visibilityDirty = true;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
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
        public void Initialize(long worldX, long worldY, long worldZ)
        {
            if (isInitialized)
            {
                Logger.Warn("[CAMERA] CameraManager already initialized");
                return;
            }

            WorldBinding.Initialize(worldX, worldY, worldZ);
            Transform.LocalPosition = new Vector3D<float>(
                worldX % CameraWorldBinding.ChunkSize,
                worldY % CameraWorldBinding.ChunkSize,
                worldZ % CameraWorldBinding.ChunkSize
            );
            InvalidateCache();

            isInitialized = true;
            Logger.Info($"[CAMERA] CameraManager initialized at ({worldX}, {worldY}, {worldZ})");
        }

        // ══════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════
        public void Update(
            float dt,
            Vector3D<float> movementInput,
            Vector2D<float> lookInput,
            float verticalInput = 0f,
            bool isSprinting = false
            )
        {
            // ═══ Process motion
            Motion.ApplyMovementInput(
                ComputeWorldMovementDirection(movementInput),
                isSprinting,
                dt
            );
            Motion.ApplyVerticalInput(verticalInput, dt);

            // ═══ Process rotation
            var rotationDelta = Motion.ApplyLookInput(lookInput, dt);
            ApplyRotation(rotationDelta);

            // ═══ Apply position delta
            var positionDelta = Motion.ComputePositionDelta(dt);
            Transform.LocalPosition += positionDelta;

            // ═══ Update world binding + handle potential origin shift
            Transform.LocalPosition = WorldBinding.UpdateFromLocalPosition(Transform.LocalPosition);

            // ═══ Mark matrices as dirty
            InvalidateCache();
        }

        public void UpdateFromInput(float dt)
        {
            var input = InputManager.Instance;

            var move = input.GetMovementVector();
            var look = input.GetLookVector();
            var verticalMove = input.GetVerticalMovement();
            bool sprint = input.IsActionTriggered("Sprint");

            Update(
                dt,
                movementInput: new Vector3D<float>(move.X, 0f, move.Y),
                lookInput: new Vector2D<float>(look.X, look.Y),
                verticalInput: verticalMove,
                isSprinting: sprint
            );
        }

        private Vector3D<float> ComputeWorldMovementDirection(Vector3D<float> input)
        {
            if (input.LengthSquared < 0.0001f)
                return Vector3D<float>.Zero;

            if (CurrentMode == CameraMotionMode.Fly || CurrentMode == CameraMotionMode.Spectator)
            {
                return Transform.Forward * input.Z +
                       Transform.Right * input.X +
                       Transform.Up * input.Y;
            }

            return Transform.ForwardFlat * input.Z + Transform.Right * input.X;
        }

        private void ApplyRotation(Vector2D<float> delta)
        {
            Logger.Info($"[CAMERA] delta=({delta.X:F3}, {delta.Y:F3}) yaw={Transform.Yaw:F1} pitch={Transform.Pitch:F1}, roll={Transform.Roll:F1}");

            Transform.Yaw -= delta.X;

            Transform.Pitch -= delta.Y;

            Transform.Pitch = MathHelper.Clamp(Transform.Pitch, -Motion.MaxPitch, Motion.MaxPitch);

            if (!Motion.AllowRoll && MathF.Abs(Transform.Roll) > 0.01f)
            {
                Transform.Roll = MathHelper.ExpDecay(Transform.Roll, 0f, Motion.RollReturnRate, 0.016f);
            }

            Transform.UpdateRotation();
        }

        // ══════════════════════════════════════════════════
        // MODE SWITCHING
        // ══════════════════════════════════════════════════
        public void SetMotionMode(CameraMotionMode mode)
        {
            if (CurrentMode == mode) return;

            CurrentMode = mode;
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
        public Matrix4X4<float> GetViewMatrix()
        {
            if (matricesDirty) UpdateMatrices();
            return cachedViewMatrix;
        }

        public Matrix4X4<float> GetProjectionMatrix()
        {
            if (matricesDirty) UpdateMatrices();
            return cachedProjectionMatrix;
        }

        public void UpdateAspectRatio(int width, int height)
        {
            Projection.UpdateAspect(width, height);
            InvalidateCache();
        }

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
        private void InvalidateCache()
        {
            matricesDirty = true;
            visibilityDirty = true;
        }
        public void UpdateVisibilityCache()
        {
            if (matricesDirty) UpdateMatrices();

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
        public CameraVisibilityContext GetVisibilityContext()
        {
            if (visibilityDirty || cachedVisibility is null)
            {
                UpdateVisibilityCache();
            }

            return cachedVisibility!.Value;
        }

        public bool IsChunkVisible(ChunkCoord chunkCoord, float chunkWorldSize)
        {
            var visibility = GetVisibilityContext();

            long distSq = chunkCoord.DistanceSquaredTo(visibility.CameraChunk);
            if (distSq > (long)RenderDistanceChunks * RenderDistanceChunks)
                return false;

            return true;
        }

        // ══════════════════════════════════════════════════
        // DIRECT SETTERS (for teleportation, cutscenes, etc.)
        // ══════════════════════════════════════════════════
        public void TeleportTo(long worldX, long worldY, long worldZ)
        {
            WorldBinding.Initialize(worldX, worldY, worldZ);
            Transform.LocalPosition = WorldBinding.WorldToLocal(worldX, worldY, worldZ);
            Motion.Velocity = Vector3D<float>.Zero;
            Motion.VerticalVelocity = 0f;
            InvalidateCache();
        }

        public void SetRotation(float yaw, float pitch, float roll = 0f)
        {
            Transform.Yaw = yaw;
            Transform.Pitch = MathHelper.Clamp(pitch, -Motion.MaxPitch, Motion.MaxPitch);
            Transform.Roll = roll;
            InvalidateCache();
        }

        public void SetFovModifier(float modifier)
        {
            Projection.FovModifier = modifier;
            InvalidateCache();
        }
    }
    public enum CameraMotionMode : byte
    {
        Walk,       // ═══ Ground-based, flat forward
        Fly,        // ═══ Free 3D movement
        Glider,     // ═══ Momentum-based, roll enabled
        Cinematic,  // ═══ Slow, smooth for screenshots
        Spectator,  // ═══ Like fly but can pass through blocks
        Debug       // ═══ Debug Cam
    }
}