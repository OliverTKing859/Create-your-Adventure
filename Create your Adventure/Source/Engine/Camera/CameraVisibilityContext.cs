using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public readonly struct CameraVisibilityContext
    {
        // ══════════════════════════════════════════════════
        // POSITION DATA
        // ══════════════════════════════════════════════════
        public readonly ChunkCoord CameraChunk;
        public readonly Vector3D<float> LocalPosition;
        public readonly Vector3D<float> Forward;

        // ══════════════════════════════════════════════════
        // MATRICES (for culling & rendering)
        // ══════════════════════════════════════════════════
        public readonly Matrix4X4<float> ViewMatrix;
        public readonly Matrix4X4<float> ProjectionMatrix;
        public readonly Matrix4X4<float> ViewProjectionMatrix;

        // ══════════════════════════════════════════════════
        // FRUSTUM
        // ══════════════════════════════════════════════════
        public readonly ViewFrustum Frustum;

        // ══════════════════════════════════════════════════
        // VIEW DISTANCES
        // ══════════════════════════════════════════════════
        public readonly int RenderDistanceChunks;

        public readonly float FarPlane;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        public CameraVisibilityContext(
            ChunkCoord cameraChunk,
            Vector3D<float> localPosition,
            Vector3D<float> forward,
            Matrix4X4<float> viewMatrix,
            Matrix4X4<float> projectionMatrix,
            int renderDistanceChunks,
            float farPlane
            )
        {
            CameraChunk = cameraChunk;
            LocalPosition = localPosition;
            Forward = forward;
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
            ViewProjectionMatrix = viewMatrix * projectionMatrix;
            RenderDistanceChunks = renderDistanceChunks;
            FarPlane = farPlane;

            // ═══ Extract frustum from combined matrix
            Frustum = ViewFrustum.ExtractFromMatrix(ViewProjectionMatrix);
        }
    }

    public readonly struct ViewFrustum
    {
        public readonly Plane3D Left;
        public readonly Plane3D Right;
        public readonly Plane3D Bottom;
        public readonly Plane3D Top;
        public readonly Plane3D Near;
        public readonly Plane3D Far;

        private ViewFrustum(Plane3D left, Plane3D right, Plane3D bottom, Plane3D top, Plane3D near, Plane3D far)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
            Near = near;
            Far = far;
        }

        public static ViewFrustum ExtractFromMatrix(Matrix4X4<float> vp)
        {
            // ═══ Left Plane
            var left = new Plane3D(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            ).Normalized();

            // ═══ Left Plane
            var right = new Plane3D(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            ).Normalized();

            // ═══ Bottom Plane
            var bottom = new Plane3D(
                vp.M14 + vp.M12,
                vp.M24 + vp.M22,
                vp.M34 + vp.M32,
                vp.M44 + vp.M42
            ).Normalized();

            // ═══ Top Plane
            var top = new Plane3D(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            ).Normalized();

            // ═══ Near Plane
            var near = new Plane3D(
                vp.M14 + vp.M13,
                vp.M24 + vp.M23,
                vp.M34 + vp.M33,
                vp.M44 + vp.M43
            ).Normalized();

            // ═══ Far Plane
            var far = new Plane3D(
                vp.M14 + vp.M13,
                vp.M24 + vp.M23,
                vp.M34 + vp.M33,
                vp.M44 + vp.M43
            ).Normalized();

            return new ViewFrustum(left, right, bottom, top, near, far);
        }

        public bool Intersects(Box3D<float> aabb)
        {
            return TestPlane(Left, aabb) &&
                   TestPlane(Right, aabb) &&
                   TestPlane(Bottom, aabb) &&
                   TestPlane(Top, aabb) &&
                   TestPlane(Near, aabb) &&
                   TestPlane(Far, aabb);
        }

        public bool Intersects(Vector3D<float> center, float radius)
        {
            return Left.DistanceTo(center) >= -radius &&
                   Right.DistanceTo(center) >= -radius &&
                   Bottom.DistanceTo(center) >= -radius &&
                   Top.DistanceTo(center) >= -radius &&
                   Near.DistanceTo(center) >= -radius &&
                   Far.DistanceTo(center) >= -radius;
        }

        private static bool TestPlane(Plane3D plane, Box3D<float> aabb)
        {
            // ═══ Get the positive vertex (P-vertex) for the plane normal
            var pVertex = new Vector3D<float>(
                plane.Normal.X >= 0 ? aabb.Max.X : aabb.Min.X,
                plane.Normal.Y >= 0 ? aabb.Max.Y : aabb.Min.Y,
                plane.Normal.Z >= 0 ? aabb.Max.Z : aabb.Min.Z
            );

            return plane.DistanceTo(pVertex) >= 0;
        }
    }

    public readonly struct Plane3D
    {
        public readonly Vector3D<float> Normal;
        public readonly float Distance;

        public Plane3D(float a, float b, float c, float d)
        {
            Normal = new Vector3D<float>(a, b, c);
            Distance = d;
        }

        public Plane3D(Vector3D<float> normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }

        public Plane3D Normalized()
        {
            float length = Normal.Length;
            if (length < 0.0001f) return this;

            return new Plane3D(
                Normal / length,
                Distance / length
            );
        }

        public float DistanceTo(Vector3D<float> point)
        {
            return Vector3D.Dot(Normal, point) + Distance;
        }
    }
}
