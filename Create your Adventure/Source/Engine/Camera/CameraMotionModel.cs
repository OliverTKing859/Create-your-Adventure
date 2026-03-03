using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public struct CameraMotionModel
    {
        // ══════════════════════════════════════════════════
        // MOVEMENT PARAMETERS
        // ══════════════════════════════════════════════════
        public float MaxSpeed;
        public float SprintMultiplier;
        public float VerticalSpeed;

        // ══════════════════════════════════════════════════
        // ACCELERATION & DAMPING
        // ══════════════════════════════════════════════════
        public float AccelerationRate;
        public float DecelerationRate;
        public float DragCoefficient;

        // ══════════════════════════════════════════════════
        // ROTATION PARAMETERS
        // ══════════════════════════════════════════════════
        public float LookSensitivity;
        public float LookSmoothing;
        public float MaxPitch;
        public bool AllowRoll;
        public float RollReturnRate;

        // ══════════════════════════════════════════════════
        // MOTION STATE (Runtime values)
        // ══════════════════════════════════════════════════+
        public Vector3D<float> Velocity;
        public float VerticalVelocity;
        public Vector2D<float> SmoothedLookDelta;

        // ══════════════════════════════════════════════════
        // MOTION PROCESSING
        // ══════════════════════════════════════════════════
        public void ApplyMovementInput(Vector3D<float> inputDirection, bool isSprinting, float dt)
        {
            float targetSpeed = isSprinting ? MaxSpeed * SprintMultiplier : MaxSpeed;
            var targetVelocity = inputDirection * targetSpeed;

            // ═══ Choose rate based on whether there's input
            float rate = inputDirection.LengthSquared > 0.001f
                ? AccelerationRate
                : DecelerationRate;

            // ═══ Exponential decay for smooth, responsive feel
            Velocity = MathHelper.ExpDecay(Velocity, targetVelocity, rate, dt);
        }

        public void ApplyVerticalInput(float verticalInput, float dt)
        {
            float targetVert = verticalInput * VerticalSpeed;
            VerticalVelocity = MathHelper.ExpDecay(VerticalVelocity, targetVert, AccelerationRate, dt);
        }

        public Vector2D<float> ApplyLookInput(Vector2D<float> rawDelta, float dt)
        {
            float smoothFactor = 1f - MathF.Exp(-LookSmoothing * dt);
            SmoothedLookDelta = Vector2D.Lerp(SmoothedLookDelta, rawDelta, smoothFactor);

            return SmoothedLookDelta * LookSensitivity;
        }

        public readonly Vector3D<float> ComputePositionDelta(float dt)
        {
            return new Vector3D<float>(
                Velocity.X * dt,
                Velocity.Y * dt + VerticalVelocity * dt,
                Velocity.Z * dt
            );
        }

        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        public static CameraMotionModel Default => new()
        {
            MaxSpeed = 6f,
            SprintMultiplier = 2.5f,
            VerticalSpeed = 5f,
            AccelerationRate = 10f,
            DecelerationRate = 8f,
            DragCoefficient = 0f,
            LookSensitivity = 0.1f,
            LookSmoothing = 18f,
            MaxPitch = 89f,
            AllowRoll = false,
            RollReturnRate = 5f,
            Velocity = Vector3D<float>.Zero,
            VerticalVelocity = 0f,
            SmoothedLookDelta = Vector2D<float>.Zero
        };

        // ══════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════
        public static class Presets
        {
            public static readonly CameraMotionModel Walk = Default;

            public static readonly CameraMotionModel Fly = new()
            {
                MaxSpeed = 12f,
                SprintMultiplier = 4f,
                VerticalSpeed = 10f,
                AccelerationRate = 12f,
                DecelerationRate = 6f,
                DragCoefficient = 0.1f,
                LookSensitivity = 0.12f,
                LookSmoothing = 15f,
                MaxPitch = 89f,
                AllowRoll = false,
                RollReturnRate = 5f
            };

            public static readonly CameraMotionModel Glider = new()
            {
                MaxSpeed = 25f,
                SprintMultiplier = 1.5f,
                VerticalSpeed = 0f,           // ═══ No direct vertical in glider
                AccelerationRate = 3f,
                DecelerationRate = 1f,
                DragCoefficient = 0.02f,
                LookSensitivity = 0.08f,
                LookSmoothing = 12f,
                MaxPitch = 85f,
                AllowRoll = true,
                RollReturnRate = 2f
            };

            public static readonly CameraMotionModel Cinematic = new()
            {
                MaxSpeed = 2f,
                SprintMultiplier = 3f,
                VerticalSpeed = 2f,
                AccelerationRate = 4f,
                DecelerationRate = 3f,
                DragCoefficient = 0f,
                LookSensitivity = 0.05f,
                LookSmoothing = 8f,
                MaxPitch = 89f,
                AllowRoll = true,
                RollReturnRate = 1f
            };
        }
    }
}