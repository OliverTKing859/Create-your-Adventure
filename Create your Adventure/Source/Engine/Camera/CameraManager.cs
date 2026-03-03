using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public class CameraManager
    {
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
        // CACHED OUTPUT
        // ══════════════════════════════════════════════════
        private Matrix4X4<float> cachedViewMatrix;
        private Matrix4X4<float> cachedProjectionMatrix;
        private CameraVisibilityContext cachedVisibity;
        private bool matricesDirty = true;

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
            WorldBinding.Initialize(worldX, worldY, worldZ);
            Transform.LocalPosition = new Vector3D<float>(
                worldX % CameraWorldBinding.ChunkSize,
                worldY % CameraWorldBinding.ChunkSize,
                worldZ % CameraWorldBinding.ChunkSize
            );
            matricesDirty = true;
        }

        // ══════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════
        public void Update(
            float dt,
            Vector3D<float> movementInput,
            Vector2D<float> lookInput,
            float verticalInput,
            bool isSprinting
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
            // ═══ IMPORTANT: WorldBinding may return CORRECTED position!
            Transform.LocalPosition = WorldBinding.UpdateFromLocalPosition(Transform.LocalPosition);
            // ▲▲▲ This line MUST write the result back to Transform.LocalPosition!

            // ═══ Mark matrices as dirty
            matricesDirty = true;
        }

        private Vector3D<float> ComputeWorldMovementDirection(Vector3D<float> input)
        {
            if (input.LengthSquared < 0.0001f)
                return Vector3D<float>.Zero;

            // ═══ In fly mode, use full 3D movement
            if (CurrentMode == CameraMotionMode.Fly || CurrentMode == CameraMotionMode.Spectator)
            {
                return Transform.Forward * input.Z +
                       Transform.Right * input.X +
                       Transform.Up * input.Y;
            }

            // ═══ In walk mode, use flat forward
            return Transform.ForwardFlat * input.Z + Transform.Right * input.X;
        }

        private void ApplyRotation(Vector2D<float> delta)
        {
            Transform.Yaw += delta.X;
            Transform.Pitch -= delta.Y;
            Transform.Pitch = MathHelper.Clamp(Transform.Pitch, -Motion.MaxPitch, Motion.MaxPitch);

            // ═══ Handle roll return (for non-roll modes)
            if (!Motion.AllowRoll && MathF.Abs(Transform.Roll) > 0.01f)
            {
                Transform.Roll = MathHelper.ExpDecay(Transform.Roll, 0f, Motion.RollReturnRate, 0.016f);
            }
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
                _ => CameraMotionModel.Default
            };
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
            matricesDirty = true;
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
        // VISIBILITY OUTPUT
        // ══════════════════════════════════════════════════
        public CameraVisibilityContext GetVisibilityContext()
        {
            if (matricesDirty) UpdateMatrices();

            return new CameraVisibilityContext(
                cameraChunk: WorldBinding.CurrentChunk,
                localPosition: Transform.LocalPosition,
                forward: Transform.Forward,
                viewMatrix: cachedViewMatrix,
                projectionMatrix: cachedProjectionMatrix,
                renderDistanceChunks: RenderDistanceChunks,
                farPlane: Projection.FarPlane
            ); 
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
            matricesDirty = true;
        }

        public void SetRotation(float yaw, float pitch, float roll = 0f)
        {
            Transform.Yaw = yaw;
            Transform.Pitch = MathHelper.Clamp(pitch, -Motion.MaxPitch, Motion.MaxPitch);
            Transform.Roll = roll;
            matricesDirty = true;
        }

        public void SetFovModifier(float modifier)
        {
            Projection.FovModifier = modifier;
            matricesDirty = true;
        }
    }
    public enum CameraMotionMode : byte
    {
        Walk,       // ═══ Ground-based, flat forward
        Fly,        // ═══ Free 3D movement
        Glider,     // ═══ Momentum-based, roll enabled
        Cinematic,  // ═══ Slow, smooth for screenshots
        Spectator   // ═══ Like fly but can pass through blocks
    }
}
