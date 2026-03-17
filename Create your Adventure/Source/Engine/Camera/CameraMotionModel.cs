using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Defines the motion characteristics and behavior of a camera.
    /// Controls movement speed, acceleration, damping, and look sensitivity.
    /// Supports multiple presets for different camera modes (walk, fly, glider, cinematic).
    /// Uses exponential decay for smooth, framerate-independent movement.
    /// </summary>
    public class CameraMotionModel
    {
        // ══════════════════════════════════════════════════
        // MOVEMENT PARAMETERS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the maximum movement speed in units per second.
        /// This is the base speed before sprint multiplier is applied.
        /// </summary>
        public float MaxSpeed;

        /// <summary>
        /// Gets or sets the speed multiplier applied when sprinting.
        /// Typical values: 1.5-3.0 (1.0 = no sprint boost).
        /// </summary>
        public float SprintMultiplier;

        /// <summary>
        /// Gets or sets the vertical movement speed in units per second.
        /// Used for up/down movement in fly mode or creative flight.
        /// Set to 0 for grounded movement or glider mode.
        /// </summary>
        public float VerticalSpeed;

        // ══════════════════════════════════════════════════
        // ACCELERATION & DAMPING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the acceleration rate when input is applied.
        /// Higher values = faster response to input (snappier controls).
        /// Typical values: 8-15 (used in exponential decay formula).
        /// </summary>
        public float AccelerationRate;

        /// <summary>
        /// Gets or sets the deceleration rate when input is released.
        /// Higher values = faster stopping (less sliding).
        /// Typical values: 6-10 (used in exponential decay formula).
        /// </summary>
        public float DecelerationRate;

        /// <summary>
        /// Gets or sets the continuous drag coefficient applied to velocity.
        /// Simulates air resistance or friction (0 = no drag, higher = more resistance).
        /// Useful for flight or glider modes to prevent infinite acceleration.
        /// </summary>
        public float DragCoefficient;

        // ══════════════════════════════════════════════════
        // ROTATION PARAMETERS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the mouse/look sensitivity multiplier.
        /// Controls how much the camera rotates per pixel of mouse movement.
        /// Typical values: 0.05-0.15 (lower = slower, more precise).
        /// </summary>
        public float LookSensitivity;

        /// <summary>
        /// Gets or sets the look smoothing factor for mouse input.
        /// Higher values = less smoothing, more responsive (use with exponential decay).
        /// Typical values: 10-20 (0 = no smoothing, very high = heavy smoothing).
        /// </summary>
        public float LookSmoothing;

        /// <summary>
        /// Gets or sets the maximum pitch angle in degrees.
        /// Prevents looking too far up/down (prevents gimbal lock).
        /// Typical value: 89° (just below vertical to avoid singularity).
        /// </summary>
        public float MaxPitch;

        /// <summary>
        /// Gets or sets whether roll (tilt) rotation is allowed.
        /// True for flight/glider modes, false for standard FPS cameras.
        /// </summary>
        public bool AllowRoll;

        /// <summary>
        /// Gets or sets the rate at which roll returns to zero when not controlled.
        /// Higher values = faster return to level orientation.
        /// Only applies when AllowRoll is true.
        /// </summary>
        public float RollReturnRate;

        // ══════════════════════════════════════════════════
        // MOTION STATE (Runtime values)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the current horizontal velocity vector.
        /// Updated each frame based on input and acceleration/deceleration.
        /// </summary>
        public Vector3D<float> Velocity;

        /// <summary>
        /// Gets or sets the current vertical velocity (separate from horizontal movement).
        /// Used for up/down movement in fly mode.
        /// </summary>
        public float VerticalVelocity;

        /// <summary>
        /// Gets or sets the smoothed mouse delta for look rotation.
        /// Provides smooth camera rotation by filtering raw input through exponential decay.
        /// </summary>
        public Vector2D<float> SmoothedLookDelta;

        // ══════════════════════════════════════════════════
        // MOTION PROCESSING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Applies horizontal movement input and updates velocity using exponential decay.
        /// Provides smooth, framerate-independent acceleration and deceleration.
        /// </summary>
        /// <param name="inputDirection">Normalized input direction vector (WASD keys).</param>
        /// <param name="isSprinting">Whether sprint modifier should be applied.</param>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        public void ApplyMovementInput(Vector3D<float> inputDirection, bool isSprinting, float dt)
        {
            // ═══ Calculate target speed including sprint multiplier
            float targetSpeed = isSprinting ? MaxSpeed * SprintMultiplier : MaxSpeed;
            var targetVelocity = inputDirection * targetSpeed;

            // ═══ Choose acceleration or deceleration rate based on input presence
            // ═══ Faster acceleration when moving, slower when stopping for smoother feel
            float rate = inputDirection.LengthSquared > 0.001f
                ? AccelerationRate
                : DecelerationRate;

            // ═══ Exponential decay provides smooth, framerate-independent interpolation
            // ═══ Creates natural "ease-in/ease-out" feel without explicit curves
            Velocity = MathHelper.ExpDecay(Velocity, targetVelocity, rate, dt);
        }

        /// <summary>
        /// Applies vertical movement input (up/down in fly mode).
        /// Uses the same exponential decay as horizontal movement for consistency.
        /// </summary>
        /// <param name="verticalInput">Vertical input value (-1 = down, 0 = none, 1 = up).</param>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        public void ApplyVerticalInput(float verticalInput, float dt)
        {
            // ═══ Calculate target vertical velocity
            float targetVert = verticalInput * VerticalSpeed;

            // ═══ Apply same acceleration rate as horizontal movement for consistent feel
            VerticalVelocity = MathHelper.ExpDecay(VerticalVelocity, targetVert, AccelerationRate, dt);
        }

        /// <summary>
        /// Applies look input (mouse delta) with smoothing for camera rotation.
        /// Filters raw mouse movement through linear interpolation based on LookSmoothing.
        /// </summary>
        /// <param name="rawDelta">Raw mouse movement delta from current frame.</param>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        /// <returns>Smoothed and sensitivity-adjusted look delta.</returns>
        public Vector2D<float> ApplyLookInput(Vector2D<float> rawDelta, float dt)
        {
            // ═══ Calculate interpolation factor, clamped to prevent overshooting
            // ═══ Higher LookSmoothing = less smoothing (more responsive)
            float t = MathF.Min(1f, LookSmoothing * dt);

            // ═══ Smooth the delta through linear interpolation
            SmoothedLookDelta = Vector2D.Lerp(SmoothedLookDelta, rawDelta, t);

            // ═══ Apply sensitivity scaling to final output
            return SmoothedLookDelta * LookSensitivity;
        }

        /// <summary>
        /// Computes the total position change for this frame based on current velocities.
        /// Combines horizontal velocity and vertical velocity into a single displacement vector.
        /// </summary>
        /// <param name="dt">Delta time in seconds since last frame.</param>
        /// <returns>Position delta to apply to camera transform.</returns>
        public Vector3D<float> ComputePositionDelta(float dt)
        {
            // ═══ Integrate velocity over time to get displacement
            // ═══ Y component combines horizontal Y-velocity and separate vertical velocity
            return new Vector3D<float>(
                Velocity.X * dt,
                Velocity.Y * dt + VerticalVelocity * dt,
                Velocity.Z * dt
            );
        }

        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a default motion model suitable for standard first-person walking.
        /// Speed: 6 units/sec (comfortable walking pace)
        /// Sprint: 2.5x multiplier (15 units/sec)
        /// Acceleration: Responsive but not twitchy (10/8 rates)
        /// Look: Moderate sensitivity with good smoothing
        /// </summary>
        public static CameraMotionModel Default => new()
        {
            MaxSpeed = 6f,
            SprintMultiplier = 2.5f,
            VerticalSpeed = 5f,
            AccelerationRate = 10f,
            DecelerationRate = 8f,
            DragCoefficient = 0f,       // ═══ No drag for grounded movement
            LookSensitivity = 0.1f,
            LookSmoothing = 18f,
            MaxPitch = 89f,             // ═══ Prevent looking straight up/down
            AllowRoll = false,          // ═══ No roll for standard FPS
            RollReturnRate = 5f,
            Velocity = Vector3D<float>.Zero,
            VerticalVelocity = 0f,
            SmoothedLookDelta = Vector2D<float>.Zero
        };

        // ══════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Provides pre-configured motion models for common camera scenarios.
        /// Each preset is optimized for different gameplay mechanics and feel.
        /// </summary>
        public static class Presets
        {
            /// <summary>
            /// Standard walking motion - same as Default.
            /// Grounded movement with no roll, moderate speeds.
            /// </summary>
            public static readonly CameraMotionModel Walk = Default;

            /// <summary>
            /// Flying motion model for creative/spectator mode.
            /// Speed: 12 units/sec base, 48 with sprint (fast exploration)
            /// Vertical: Full 3D movement with independent up/down control
            /// Acceleration: Snappy response (12) with gradual slowdown (6)
            /// Drag: Slight air resistance (0.1) prevents runaway acceleration
            /// Look: Slightly more sensitive for aerial maneuvering
            /// </summary>
            public static readonly CameraMotionModel Fly = new()
            {
                MaxSpeed = 12f,
                SprintMultiplier = 4f,
                VerticalSpeed = 10f,
                AccelerationRate = 12f,
                DecelerationRate = 6f,
                DragCoefficient = 0.1f,     // ═══ Subtle air resistance
                LookSensitivity = 0.12f,
                LookSmoothing = 15f,
                MaxPitch = 89f,
                AllowRoll = false,
                RollReturnRate = 5f
            };

            /// <summary>
            /// Glider motion model for gliding/flying mechanics.
            /// Speed: 25 units/sec (fast forward momentum)
            /// Sprint: Limited boost (1.5x) - gliders maintain speed naturally
            /// Vertical: Disabled (0) - altitude controlled by pitch and physics
            /// Acceleration: Slow (3) for realistic glider momentum
            /// Deceleration: Very slow (1) - gliders coast smoothly
            /// Drag: Light air resistance (0.02) for realistic flight
            /// Roll: Enabled with slow return (2) for banking turns
            /// Look: Lower sensitivity (0.08) for precise glider control
            /// </summary>
            public static readonly CameraMotionModel Glider = new()
            {
                MaxSpeed = 25f,
                SprintMultiplier = 1.5f,
                VerticalSpeed = 0f,         // ═══ No direct vertical control in glider
                AccelerationRate = 3f,
                DecelerationRate = 1f,
                DragCoefficient = 0.02f,    // ═══ Air resistance for realistic gliding
                LookSensitivity = 0.08f,
                LookSmoothing = 12f,
                MaxPitch = 85f,             // ═══ Slightly limited to prevent stalls
                AllowRoll = true,           // ═══ Banking turns for glider feel
                RollReturnRate = 2f
            };

            /// <summary>
            /// Cinematic motion model for cutscenes and smooth camera work.
            /// Speed: 2 units/sec (slow, deliberate movement)
            /// Sprint: 3x boost available for faster repositioning
            /// Acceleration: Smooth and gradual (4/3) for no jarring movements
            /// Drag: None (0) - precise control for cinematography
            /// Roll: Enabled with slow return (1) for Dutch angles
            /// Look: Low sensitivity (0.05) and smoothing (8) for smooth pans
            /// </summary>
            public static readonly CameraMotionModel Cinematic = new()
            {
                MaxSpeed = 2f,
                SprintMultiplier = 3f,
                VerticalSpeed = 2f,
                AccelerationRate = 4f,
                DecelerationRate = 3f,
                DragCoefficient = 0f,
                LookSensitivity = 0.05f,    // ═══ Very low for smooth cinematic pans
                LookSmoothing = 8f,         // ═══ Heavy smoothing for buttery movement
                MaxPitch = 89f,
                AllowRoll = true,           // ═══ Dutch angles for cinematic shots
                RollReturnRate = 1f
            };

            /// <summary>
            /// Debug/editor motion model for fast level navigation and testing.
            /// Speed: 10 units/sec base, 40 with sprint (quick traversal)
            /// Vertical: Fast up/down (10) for multi-level editing
            /// Acceleration: Very responsive (12) for precise positioning
            /// Drag: Light (0.1) to prevent overshooting targets
            /// Look: Standard sensitivity with very high smoothing (50) for stability
            /// Roll: Disabled for predictable orientation
            /// </summary>
            public static readonly CameraMotionModel Debug = new()
            {
                MaxSpeed = 10f,
                SprintMultiplier = 4f,
                VerticalSpeed = 10f,
                AccelerationRate = 12f,
                DecelerationRate = 6f,
                DragCoefficient = 0f,
                LookSensitivity = 0.1f,
                LookSmoothing = 50,         // ═══ High smoothing for stable editor view
                MaxPitch = 89f,
                AllowRoll = false,
                RollReturnRate = 0f,
                Velocity = Vector3D<float>.Zero,
                VerticalVelocity = 0f,
                SmoothedLookDelta = Vector2D<float>.Zero
            };
        }
    }
}