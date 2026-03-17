using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Represents the position and rotation of a camera in 3D space.
    /// Uses Euler angles (Yaw, Pitch, Roll) for intuitive control and quaternions for accurate rotation calculations.
    /// Caches direction vectors (Forward, Right, Up) for performance optimization.
    /// </summary>
    public struct CameraTransform
    {
        // ══════════════════════════════════════════════════
        // POSITION (Local to origin chunk, in block units)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the local position of the camera relative to the origin chunk.
        /// Measured in block units for voxel-based world positioning.
        /// </summary>
        public Vector3D<float> LocalPosition;

        // ══════════════════════════════════════════════════
        // ROTATION (Euler angles in degrees)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the horizontal rotation in degrees.
        /// Range: -180 to 180 (0 = facing forward, 90 = facing right, -90 = facing left).
        /// </summary>
        public float Yaw;

        /// <summary>
        /// Gets or sets the vertical rotation in degrees.
        /// Range: -90 to 90 (-90 = looking straight down, 0 = looking forward, 90 = looking straight up).
        /// Clamping to this range prevents gimbal lock.
        /// </summary>
        public float Pitch;

        /// <summary>
        /// Gets or sets the tilt rotation in degrees.
        /// Range: -180 to 180 (0 = level, positive = roll right).
        /// Typically used for flight mechanics, gliders, or camera effects.
        /// </summary>
        public float Roll;

        // ══════════════════════════════════════════════════
        // INTERNAL QUATERNION (Computed from Euler angles)
        // ══════════════════════════════════════════════════
        // ═══ Stores the computed rotation as a quaternion for accurate vector transformations
        // ═══ Prevents gimbal lock and provides smooth interpolation
        private Quaternion<float> rotation;

        // ══════════════════════════════════════════════════
        // CACHED DIRECTION VECTORS
        // ══════════════════════════════════════════════════

        // ═══ Pre-calculated direction vectors to avoid repeated quaternion calculations
        private Vector3D<float> cachedForward;
        private Vector3D<float> cachedRight;
        private Vector3D<float> cachedUp;

        // ══════════════════════════════════════════════════
        // DIRECTION VECTORS (Quaternion-based)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the forward direction vector (where the camera is looking).
        /// Includes pitch component - points up/down when looking up/down.
        /// Cached for performance - call UpdateRotation() after changing Yaw/Pitch/Roll.
        /// </summary>
        public readonly Vector3D<float> Forward => cachedForward;

        /// <summary>
        /// Gets the right direction vector (perpendicular to forward, points to the camera's right side).
        /// Cached for performance - call UpdateRotation() after changing Yaw/Pitch/Roll.
        /// </summary>
        public readonly Vector3D<float> Right => cachedRight;

        /// <summary>
        /// Gets the up direction vector (perpendicular to forward and right, points upward relative to camera).
        /// Cached for performance - call UpdateRotation() after changing Yaw/Pitch/Roll.
        /// </summary>
        public readonly Vector3D<float> Up => cachedUp;

        /// <summary>
        /// Gets the forward direction projected onto the horizontal plane (Y = 0).
        /// Ignores pitch - always parallel to the ground.
        /// Useful for horizontal character movement where you want to move forward regardless of where you're looking vertically.
        /// </summary>
        public readonly Vector3D<float> ForwardFlat
        {
            get
            {
                // ═══ Project forward vector onto XZ plane by zeroing Y component
                var flat = new Vector3D<float>(cachedForward.X, 0f, cachedForward.Z);

                // ═══ Normalize if length is significant, otherwise return default forward (-Z)
                // ═══ Prevents division by zero when looking straight up/down
                return flat.LengthSquared > 0.0001f
                    ? Vector3D.Normalize(flat)
                    : new Vector3D<float>(0f, 0f, -1f);
            }
        }

        /// <summary>
        /// Recalculates the rotation quaternion and direction vectors from Euler angles.
        /// Must be called after modifying Yaw, Pitch, or Roll for changes to take effect.
        /// Updates Forward, Right, Up, and ForwardFlat properties.
        /// </summary>
        public void UpdateRotation()
        {
            // ═══ Convert Euler angles from degrees to radians for trigonometric functions
            float yawRad = Yaw * MathHelper.Deg2Rad;
            float pitchRad = Pitch * MathHelper.Deg2Rad;
            float rollRad = Roll * MathHelper.Deg2Rad;

            // ═══ Create individual quaternions for each rotation axis
            // ═══ Yaw rotates around Y-axis (negative for correct direction)
            var qYaw = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitY, -yawRad);

            // ═══ Pitch rotates around X-axis
            var qPitch = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitX, pitchRad);

            // ═══ Roll rotates around Z-axis
            var qRoll = Quaternion<float>.CreateFromAxisAngle(Vector3D<float>.UnitZ, rollRad);


            // ═══ Combine rotations in YXZ order (Yaw → Pitch → Roll) and normalize
            // ═══ This order prevents gimbal lock for typical camera movement
            rotation = Quaternion<float>.Normalize(qYaw * qPitch * qRoll);


            // ═══ Pre-calculate trigonometric values for optimization
            float cosPitch = MathF.Cos(pitchRad);
            float sinPitch = MathF.Sin(pitchRad);
            float cosYaw = MathF.Cos(yawRad);
            float sinYaw = MathF.Sin(yawRad);

            // ═══ Transform base direction vectors using the rotation quaternion
            // ═══ Forward: -Z axis in world space (OpenGL convention)
            cachedForward = Vector3D.Normalize(TransformVector(new Vector3D<float>(0f, 0f, -1f), rotation));

            // ═══ Right: +X axis in world space
            cachedRight = Vector3D.Normalize(TransformVector(new Vector3D<float>(1f, 0f, 0f), rotation));

            // ═══ Up: +Y axis in world space
            cachedUp = Vector3D.Normalize(TransformVector(new Vector3D<float>(0f, 1f, 0f), rotation));
        }


        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a default camera transform with standard initial values.
        /// Position: (0, 64, 0) - typical spawn height in voxel games.
        /// Rotation: Yaw=0, Pitch=0, Roll=0 - looking forward and level.
        /// Direction vectors are pre-calculated.
        /// </summary>
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

                // ═══ Initialize direction vectors from default rotation
                transform.UpdateRotation();
                return transform;
            }
        }

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// Uses the formula: v' = v + 2*w*(qv × v) + 2*(qv × (qv × v))
        /// More efficient than converting quaternion to matrix for single vector transforms.
        /// </summary>
        /// <param name="vector">The vector to transform.</param>
        /// <param name="q">The rotation quaternion.</param>
        /// <returns>The rotated vector.</returns>
        private static Vector3D<float> TransformVector(Vector3D<float> vector, Quaternion<float> q)
        {
            // ═══ Extract vector part of quaternion (imaginary components)
            var qv = new Vector3D<float>(q.X, q.Y, q.Z);

            // ═══ Calculate intermediate cross product: 2 * (qv × v)
            var t = 2f * Vector3D.Cross(qv, vector);

            // ═══ Apply quaternion rotation formula
            // ═══ v' = v + w*t + qv × t
            return vector + q.W * t + Vector3D.Cross(qv, t);
        }
    }

    /// <summary>
    /// Provides common mathematical utility functions for game calculations.
    /// Includes angle conversions, clamping, interpolation, and exponential decay.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Conversion factor from degrees to radians.
        /// Multiply degrees by this value to get radians.
        /// </summary>
        public const float Deg2Rad = MathF.PI / 180f;

        /// <summary>
        /// Conversion factor from radians to degrees.
        /// Multiply radians by this value to get degrees.
        /// </summary>
        public const float Rad2Deg = 180f / MathF.PI;

        /// <summary>
        /// Clamps a value between a minimum and maximum range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The clamped value within [min, max].</returns>
        public static float Clamp(float value, float min, float max)
            => MathF.Max(min, MathF.Min(max, value));

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation factor (0.0 to 1.0).</param>
        /// <returns>The interpolated value between a and b.</returns>
        public static float Lerp(float a, float b, float t)
            => a + (b - a) * t;

        /// <summary>
        /// Performs exponential decay interpolation between current and target values.
        /// Provides smooth, framerate-independent transitions that slow down as they approach the target.
        /// Useful for camera smoothing, spring physics, and damped animations.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The target value to approach.</param>
        /// <param name="decay">The decay rate (higher = faster convergence, typical values: 5-20).</param>
        /// <param name="dt">Delta time in seconds (time since last frame).</param>
        /// <returns>The new current value after decay interpolation.</returns>
        public static float ExpDecay(float current, float target, float decay, float dt)
            => Lerp(current, target, 1f - MathF.Exp(-decay * dt));

        /// <summary>
        /// Performs exponential decay interpolation between current and target vectors.
        /// Provides smooth, framerate-independent transitions for 3D positions and movements.
        /// Each component is interpolated independently using the same decay rate.
        /// </summary>
        /// <param name="current">The current vector.</param>
        /// <param name="target">The target vector to approach.</param>
        /// <param name="decay">The decay rate (higher = faster convergence, typical values: 5-20).</param>
        /// <param name="dt">Delta time in seconds (time since last frame).</param>
        /// <returns>The new current vector after decay interpolation.</returns>
        public static Vector3D<float> ExpDecay(Vector3D<float> current, Vector3D<float> target, float decay, float dt)
            => Vector3D.Lerp(current, target, 1f - MathF.Exp(-decay * dt));
    }
}