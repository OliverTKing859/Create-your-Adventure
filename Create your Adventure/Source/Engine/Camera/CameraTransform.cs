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
        // INTERNAL QUATERNION (Computed from Euler angles)
        // ══════════════════════════════════════════════════
        private Quaternion<float> rotation;

        // ══════════════════════════════════════════════════
        // CACHED DIRECTION VECTORS
        // ══════════════════════════════════════════════════
        private Vector3D<float> cachedForward;
        private Vector3D<float> cachedRight;
        private Vector3D<float> cachedUp;

        private bool directionsDirty;

        // ══════════════════════════════════════════════════
        // DIRECTION VECTORS (Quaternion-based)
        // ══════════════════════════════════════════════════
        public readonly Vector3D<float> Forward => cachedForward;
        public readonly Vector3D<float> Right => cachedRight;
        public readonly Vector3D<float> Up => cachedUp;

        public readonly Vector3D<float> ForwardFlat
        {
            get
            {
                var flat = new Vector3D<float>(cachedForward.X, 0f, cachedForward.Z);
                return flat.LengthSquared > 0.0001f
                    ? Vector3D.Normalize(flat)
                    : new Vector3D<float>(0f, 0f, -1f);
            }
        }

        public void UpdateRotation()
        {
            // Convert Euler angles to radians
            float yawRad = Yaw * MathHelper.Deg2Rad;
            float pitchRad = Pitch * MathHelper.Deg2Rad;

            float cosPitch = MathF.Cos(pitchRad);
            float sinPitch = MathF.Sin(pitchRad);
            float cosYaw = MathF.Cos(yawRad);
            float sinYaw = MathF.Sin(yawRad);

            cachedForward = Vector3D.Normalize(new Vector3D<float>(
                cosPitch * sinYaw,      // X: Links/Rechts basierend auf Yaw
                sinPitch,               // Y: Hoch/Runter basierend auf Pitch
                -cosPitch * cosYaw      // Z: Negativ weil OpenGL -Z = forward
            ));

            cachedRight = Vector3D.Normalize(new Vector3D<float>(
                cosYaw,     // X
                0f,         // Y: Immer 0 für horizontale Bewegung
                sinYaw      // Z
            ));

            cachedUp = Vector3D.Normalize(Vector3D.Cross(cachedRight, cachedForward));
        }


        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        public static CameraTransform Default
        {
            get
            {
                var transform = new CameraTransform
                {
                    LocalPosition = new Vector3D<float>(0f, 64f, 0f),
                    Yaw = 0f,
                    Pitch = 0f,
                    Roll = 0f,
                };
                transform.UpdateRotation();
                return transform;
            }
        }
         
        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════

        private static Vector3D<float> TransformVector(Vector3D<float> vector, Quaternion<float> rotation)
        {
            var qv = new Vector3D<float>(rotation.X, rotation.Y, rotation.Z);
            var t = 2f * Vector3D.Cross(qv, vector);
            return vector + rotation.W * t + Vector3D.Cross(qv, t);
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