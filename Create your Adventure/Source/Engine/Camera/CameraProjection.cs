using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public struct CameraProjection
    {
        // ══════════════════════════════════════════════════
        // PROJECTION PARAMETERS
        // ══════════════════════════════════════════════════
        public float FieldOfView;
        public float NearPlane;
        public float FarPlane;
        public float AspectRatio;

        // ══════════════════════════════════════════════════
        // DYNAMIC FOV MODIFIERS
        // ══════════════════════════════════════════════════
        public float BaseFov;
        public float FovModifier;

        // ══════════════════════════════════════════════════
        // COMPUTED PROPERTIES
        // ══════════════════════════════════════════════════
        public readonly float EffectiveFov => BaseFov + FovModifier;
        public readonly float EffectiveFovRadians => EffectiveFov * MathHelper.Deg2Rad;

        // ══════════════════════════════════════════════════
        // MATRIX GENERATION
        // ══════════════════════════════════════════════════
        public readonly Matrix4X4<float> GetProjectionMatrix()
        {
            float fovRad = EffectiveFovRadians;
            float aspect = AspectRatio > 0f ? AspectRatio : 1f;

            return Matrix4X4.CreatePerspectiveFieldOfView(
                fovRad,
                aspect,
                NearPlane,
                FarPlane
            );
        }

        public void UpdateAspect(int width, int height)
        {
            AspectRatio = height > 0 ? (float)width / height : 1f;
        }

        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        public static CameraProjection Default => new()
        {
            BaseFov = 70f,
            FieldOfView = 70f,
            FovModifier = 0f,
            NearPlane = 0.05f,      // ═══ Very close for first-person
            FarPlane = 2000f,       // ═══ Far enough for large view distances
            AspectRatio = 16f / 9f
        };

        // ══════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════
        public static class Presets
        {
            public static readonly CameraProjection Standard = Default;

            public static readonly CameraProjection Cinematic = new()
            {
                BaseFov = 50f,
                FieldOfView = 50f,
                NearPlane = 0.1f,
                FarPlane = 5000f,
                AspectRatio = 21f / 9f
            };

            public static readonly CameraProjection FirstPerson = new()
            {
                BaseFov = 90f,
                FieldOfView = 90f,
                NearPlane = 0.01f,
                FarPlane = 1500f,
                AspectRatio = 16f / 9f
            };
        }
    }
}