using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public struct CameraTransform
    {
        // ══════════════════════════════════════════════════
        // POSITION (Local to origin chunk, in block units)
        // ══════════════════════════════════════════════════
        public Vector3D<float> LocalPosition;

        // ══════════════════════════════════════════════════
        // ROTATION (Euler angles in degrees)
        // ══════════════════════════════════════════════════
        public float Yaw;   // ═══ Horizontal rotation (-180 to 180)
        public float Pitch; // ═══ Vertical rotation (-90 to 90)
        public float Roll;  // ═══ Tilt rotation (-180 to 180) – for gliders, etc.

        // ══════════════════════════════════════════════════
        // DIRECTION VECTORS (Computed on demand)
        // ══════════════════════════════════════════════════
        public readonly Vector3D<float> Forward
        {
            get
            {
                float yawRad = Yaw * MathHelper.Deg2Rad;
                float pitchRad = Pitch * MathHelper.Deg2Rad;

                return Vector3D.Normalize(new Vector3D<float>(
                    MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                    MathF.Sin(pitchRad),
                    MathF.Sin(yawRad) * MathF.Cos(pitchRad)
                ));
            }
        }

        public readonly Vector3D<float> Right
        {
            get
            {
                var forward = Forward;
                var worldUp = Vector3D<float>.UnitY;
                var right = Vector3D.Normalize(Vector3D.Cross(forward, worldUp));

                // ═══ Apply roll rotation around forward axis
                if (MathF.Abs(Roll) > 0.001f)
                {
                    right = RotateAroundAxis(right, forward, Roll * MathHelper.Deg2Rad);
                }

                return right;
            }
        }

        public readonly Vector3D<float> Up => Vector3D.Cross(Right, Forward);

        public readonly Vector3D<float> ForwardFlat
        {
            get
            {
                var forward = Forward;
                var flat = new Vector3D<float>(forward.X, 0f, forward.Z);
                return flat.LengthSquared > 0.0001f
                    ? Vector3D.Normalize(flat)
                    : Vector3D<float>.UnitZ;
            }
        }

        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        public static CameraTransform Default => new()
        {
            LocalPosition = new Vector3D<float>(0f, 64f, 0f),
            Yaw = -90f,
            Pitch = 0f,
            Roll = 0f
        };

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════

        private static Vector3D<float> RotateAroundAxis(Vector3D<float> vector, Vector3D<float> axis, float angleRad)
        {
            float cos = MathF.Cos(angleRad);
            float sin = MathF.Sin(angleRad);
            var dot = Vector3D.Dot(axis, vector);
            var cross = Vector3D.Cross(axis, vector);

            return vector * cos + cross * sin + axis * dot * (1f - cos);
        }
    }

    public static class MathHelper
    {
        public const float Deg2Rad = MathF.PI / 180f;
        public const float Rad2Deg = 180f / MathF.PI;

        public static float Clamp(float value, float min, float max)
            => MathF.Max(min, MathF.Min(max, value));

        public static float Lerp(float a, float b, float t)
            => a + (b - a) * t;

        public static float ExpDecay(float current, float target, float decay, float dt)
            => Lerp(current, target, 1f - MathF.Exp(-decay * dt));

        public static Vector3D<float> ExpDecay(Vector3D<float> current, Vector3D<float> target, float decay, float dt)
            => Vector3D.Lerp(current, target, 1f - MathF.Exp(-decay * dt));
    }
}