using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Encapsulates all camera-related data needed for visibility determination and culling.
    /// Contains camera position, view matrices, frustum planes, and render distance settings.
    /// Immutable snapshot used by chunk managers and renderers to determine what should be drawn.
    /// Extracted once per frame to avoid repeated matrix calculations during culling.
    /// </summary>
    public readonly struct CameraVisibilityContext
    {
        // ══════════════════════════════════════════════════
        // POSITION DATA
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the chunk coordinate where the camera is currently located.
        /// Used as the origin point for chunk loading/unloading and LOD calculations.
        /// </summary>
        public readonly ChunkCoord CameraChunk;

        /// <summary>
        /// Gets the local position of the camera within its chunk (0-16 block units).
        /// Combined with CameraChunk to determine absolute world position.
        /// </summary>
        public readonly Vector3D<float> LocalPosition;

        /// <summary>
        /// Gets the normalized forward direction vector the camera is facing.
        /// Used for directional culling and determining chunk priority (load chunks in front first).
        /// </summary>
        public readonly Vector3D<float> Forward;

        // ══════════════════════════════════════════════════
        // MATRICES (for culling & rendering)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the view matrix that transforms world space to camera space.
        /// Combines camera position and rotation into a single transformation.
        /// </summary>
        public readonly Matrix4X4<float> ViewMatrix;

        /// <summary>
        /// Gets the projection matrix that transforms camera space to clip space.
        /// Defines the perspective projection with FOV, aspect ratio, and clipping planes.
        /// </summary>
        public readonly Matrix4X4<float> ProjectionMatrix;

        /// <summary>
        /// Gets the combined view-projection matrix (view * projection).
        /// Pre-multiplied for performance - used for frustum extraction and GPU transforms.
        /// Silk.NET uses row-major matrices, so order matters: view * projection.
        /// </summary>
        public readonly Matrix4X4<float> ViewProjectionMatrix;

        // ══════════════════════════════════════════════════
        // FRUSTUM
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the view frustum extracted from the view-projection matrix.
        /// Contains 6 planes (left, right, top, bottom, near, far) for fast culling tests.
        /// Used to determine if chunks/entities are visible before rendering.
        /// </summary>
        public readonly ViewFrustum Frustum;

        // ══════════════════════════════════════════════════
        // VIEW DISTANCES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the render distance in chunks (horizontal distance from camera).
        /// Chunks beyond this distance are not loaded or rendered.
        /// Typical values: 8-32 chunks (128-512 blocks).
        /// </summary>
        public readonly int RenderDistanceChunks;

        /// <summary>
        /// Gets the far clipping plane distance in blocks.
        /// Objects beyond this distance are clipped and not rendered.
        /// Should be slightly larger than RenderDistanceChunks * 16 to avoid pop-in.
        /// </summary>
        public readonly float FarPlane;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a new camera visibility context with all necessary data for culling.
        /// Automatically computes view-projection matrix and extracts frustum planes.
        /// </summary>
        /// <param name="cameraChunk">The chunk coordinate containing the camera.</param>
        /// <param name="localPosition">Camera position within its chunk (0-16 block units).</param>
        /// <param name="forward">Normalized forward direction vector.</param>
        /// <param name="viewMatrix">View matrix (world to camera transform).</param>
        /// <param name="projectionMatrix">Projection matrix (camera to clip space).</param>
        /// <param name="renderDistanceChunks">Maximum render distance in chunks.</param>
        /// <param name="farPlane">Far clipping plane distance in blocks.</param>
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

            // ═══ Compute combined view-projection matrix
            // ═══ Silk.NET Matrix4X4 is row-major, so order is view * projection
            // ═══ This matches GPU shader convention and frustum extraction algorithms
            ViewProjectionMatrix = viewMatrix * projectionMatrix;
            RenderDistanceChunks = renderDistanceChunks;
            FarPlane = farPlane;

            // ═══ Extract 6 frustum planes from the combined matrix
            // ═══ Done once here to avoid repeated extraction during culling loops
            Frustum = ViewFrustum.ExtractFromMatrix(ViewProjectionMatrix);
        }
    }

    /// <summary>
    /// Represents a view frustum defined by 6 planes (left, right, top, bottom, near, far).
    /// Used for fast visibility culling - objects outside the frustum are not rendered.
    /// Planes are extracted from the view-projection matrix using the Gribb-Hartmann method.
    /// Supports AABB (box) and sphere intersection tests for chunk and entity culling.
    /// </summary>
    public readonly struct ViewFrustum
    {
        /// <summary>Gets the left frustum plane (objects to the left are outside).</summary>
        public readonly Plane3D Left;

        /// <summary>Gets the right frustum plane (objects to the right are outside).</summary>
        public readonly Plane3D Right;

        /// <summary>Gets the bottom frustum plane (objects below are outside).</summary>
        public readonly Plane3D Bottom;

        /// <summary>Gets the top frustum plane (objects above are outside).</summary>
        public readonly Plane3D Top;

        /// <summary>Gets the near frustum plane (objects too close are clipped).</summary>
        public readonly Plane3D Near;

        /// <summary>Gets the far frustum plane (objects too far are clipped).</summary>
        public readonly Plane3D Far;

        /// <summary>
        /// Private constructor - use ExtractFromMatrix() to create frustums.
        /// </summary>
        private ViewFrustum(Plane3D left, Plane3D right, Plane3D bottom, Plane3D top, Plane3D near, Plane3D far)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
            Near = near;
            Far = far;
        }

        /// <summary>
        /// Extracts frustum planes from a view-projection matrix using the Gribb-Hartmann method.
        /// Each plane is computed by adding/subtracting matrix rows and normalizing.
        /// This method works for both OpenGL (column-major) and DirectX (row-major) conventions.
        /// </summary>
        /// <param name="vp">The combined view-projection matrix.</param>
        /// <returns>A new ViewFrustum with extracted and normalized planes.</returns>
        public static ViewFrustum ExtractFromMatrix(Matrix4X4<float> vp)
        {
            // ═══ Left Plane: add column 4 and column 1
            // ═══ Objects with positive distance are inside, negative are outside
            var left = new Plane3D(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            ).Normalized();

            // ═══ Right Plane: subtract column 1 from column 4
            var right = new Plane3D(
                vp.M14 - vp.M11,
                vp.M24 - vp.M21,
                vp.M34 - vp.M31,
                vp.M44 - vp.M41
            ).Normalized();

            // ═══ Bottom Plane: add column 4 and column 2
            var bottom = new Plane3D(
                vp.M14 + vp.M12,
                vp.M24 + vp.M22,
                vp.M34 + vp.M32,
                vp.M44 + vp.M42
            ).Normalized();

            // ═══ Top Plane: subtract column 2 from column 4
            var top = new Plane3D(
                vp.M14 - vp.M12,
                vp.M24 - vp.M22,
                vp.M34 - vp.M32,
                vp.M44 - vp.M42
            ).Normalized();

            // ═══ Near Plane: add column 4 and column 3
            var near = new Plane3D(
                vp.M14 + vp.M13,
                vp.M24 + vp.M23,
                vp.M34 + vp.M33,
                vp.M44 + vp.M43
            ).Normalized();

            // ═══ Far Plane: subtract column 3 from column 4
            var far = new Plane3D(
                vp.M14 - vp.M13,
                vp.M24 - vp.M23,
                vp.M34 - vp.M33,
                vp.M44 - vp.M43
            ).Normalized();

            return new ViewFrustum(left, right, bottom, top, near, far);
        }

        /// <summary>
        /// Tests if an axis-aligned bounding box (AABB) intersects the frustum.
        /// Uses the P-vertex method for efficient conservative culling.
        /// Returns false only if the box is completely outside the frustum.
        /// </summary>
        /// <param name="aabb">The bounding box to test (typically a chunk AABB).</param>
        /// <returns>True if the box might be visible (inside or intersecting), false if completely outside.</returns>
        public bool Intersects(Box3D<float> aabb)
        {
            // ═══ Test against all 6 planes
            // ═══ Box is outside if it's on the negative side of ANY plane
            return TestPlane(Left, aabb) &&
                   TestPlane(Right, aabb) &&
                   TestPlane(Bottom, aabb) &&
                   TestPlane(Top, aabb) &&
                   TestPlane(Near, aabb) &&
                   TestPlane(Far, aabb);
        }

        /// <summary>
        /// Tests if a sphere intersects the frustum.
        /// Faster than AABB test but less tight fit (more false positives).
        /// Good for entity culling where precision is less critical.
        /// </summary>
        /// <param name="center">The center point of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>True if the sphere might be visible (inside or intersecting), false if completely outside.</returns>
        public bool Intersects(Vector3D<float> center, float radius)
        {
            // ═══ Sphere is outside if center distance to any plane is more negative than -radius
            // ═══ This means the entire sphere is on the negative (outside) side of the plane
            return Left.DistanceTo(center) >= -radius &&
                   Right.DistanceTo(center) >= -radius &&
                   Bottom.DistanceTo(center) >= -radius &&
                   Top.DistanceTo(center) >= -radius &&
                   Near.DistanceTo(center) >= -radius &&
                   Far.DistanceTo(center) >= -radius;
        }

        /// <summary>
        /// Tests if an AABB is on the positive (inside) side of a plane using the P-vertex method.
        /// P-vertex is the corner of the box farthest in the direction of the plane normal.
        /// If even the P-vertex is outside, the entire box is outside.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <param name="aabb">The bounding box to test.</param>
        /// <returns>True if the box is at least partially on the positive side (inside).</returns>
        private static bool TestPlane(Plane3D plane, Box3D<float> aabb)
        {
            // ═══ Find the P-vertex: the corner farthest in the plane normal direction
            // ═══ If normal component is positive, use max; if negative, use min
            var pVertex = new Vector3D<float>(
                plane.Normal.X >= 0 ? aabb.Max.X : aabb.Min.X,
                plane.Normal.Y >= 0 ? aabb.Max.Y : aabb.Min.Y,
                plane.Normal.Z >= 0 ? aabb.Max.Z : aabb.Min.Z
            );

            // ═══ If P-vertex (farthest point) is outside, entire box is outside
            return plane.DistanceTo(pVertex) >= 0;
        }
    }

    /// <summary>
    /// Represents a 3D plane defined by a normal vector and distance from origin.
    /// Uses the equation: Normal·Point + Distance = 0 for points on the plane.
    /// Positive distance = point is on the positive side (normal direction).
    /// Negative distance = point is on the negative side (opposite normal).
    /// </summary>
    public readonly struct Plane3D
    {
        /// <summary>
        /// Gets the normal vector of the plane (perpendicular to the plane surface).
        /// Should be normalized (length = 1) for accurate distance calculations.
        /// </summary>
        public readonly Vector3D<float> Normal;

        /// <summary>
        /// Gets the signed distance from the origin to the plane along the normal.
        /// Positive = plane is in the direction of the normal from origin.
        /// Negative = plane is opposite to the normal direction from origin.
        /// </summary>
        public readonly float Distance;

        /// <summary>
        /// Creates a plane from individual components (a, b, c, d).
        /// Plane equation: ax + by + cz + d = 0
        /// </summary>
        /// <param name="a">X component of normal vector.</param>
        /// <param name="b">Y component of normal vector.</param>
        /// <param name="c">Z component of normal vector.</param>
        /// <param name="d">Distance from origin.</param>
        public Plane3D(float a, float b, float c, float d)
        {
            Normal = new Vector3D<float>(a, b, c);
            Distance = d;
        }

        /// <summary>
        /// Creates a plane from a normal vector and distance.
        /// </summary>
        /// <param name="normal">The plane normal (should be normalized for accurate distances).</param>
        /// <param name="distance">Signed distance from origin along the normal.</param>
        public Plane3D(Vector3D<float> normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }

        /// <summary>
        /// Returns a new plane with normalized normal vector and adjusted distance.
        /// Essential for accurate distance calculations and frustum culling.
        /// Division by normal length scales both normal and distance proportionally.
        /// </summary>
        /// <returns>A new plane with unit-length normal.</returns>
        public Plane3D Normalized()
        {
            float length = Normal.Length;

            // ═══ Avoid division by zero for degenerate planes
            if (length < 0.0001f) return this;

            // ═══ Scale both normal and distance by the same factor
            // ═══ Maintains plane equation consistency: (N/|N|)·P + (D/|N|) = 0
            return new Plane3D(
                Normal / length,
                Distance / length
            );
        }

        /// <summary>
        /// Calculates the signed distance from a point to this plane.
        /// Positive = point is on the normal side (front/inside).
        /// Negative = point is on the opposite side (back/outside).
        /// Zero = point is exactly on the plane.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>Signed distance from point to plane.</returns>
        public float DistanceTo(Vector3D<float> point)
        {
            // ═══ Dot product projects point onto normal, distance adjusts for plane position
            // ═══ Formula: distance = Normal·Point + Distance
            return Vector3D.Dot(Normal, point) + Distance;
        }
    }
}
