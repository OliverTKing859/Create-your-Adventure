using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Defines the projection parameters for a perspective camera.
    /// Controls field of view, clipping planes, and aspect ratio for 3D rendering.
    /// Supports dynamic FOV adjustments for effects like sprinting, zooming, or damage feedback.
    /// </summary>
    public struct CameraProjection
    {
        // ══════════════════════════════════════════════════
        // PROJECTION PARAMETERS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the field of view in degrees.
        /// Typical values: 60-90 for first-person games, 40-60 for third-person.
        /// </summary>
        public float FieldOfView;

        /// <summary>
        /// Gets or sets the near clipping plane distance.
        /// Objects closer than this are not rendered.
        /// Very small values (0.01-0.1) allow close-up viewing but may cause depth precision issues.
        /// </summary>
        public float NearPlane;

        /// <summary>
        /// Gets or sets the far clipping plane distance.
        /// Objects farther than this are not rendered.
        /// Balance between view distance and depth buffer precision (typical: 1000-5000).
        /// </summary>
        public float FarPlane;

        /// <summary>
        /// Gets or sets the aspect ratio (width / height).
        /// Typically 16:9 (1.778), 21:9 (2.333), or 4:3 (1.333).
        /// Updated automatically when window is resized.
        /// </summary>
        public float AspectRatio;

        // ══════════════════════════════════════════════════
        // DYNAMIC FOV MODIFIERS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets or sets the base field of view without any modifiers applied.
        /// This is the default FOV when no effects are active.
        /// </summary>
        public float BaseFov;

        /// <summary>
        /// Gets or sets the additional FOV modifier applied dynamically.
        /// Used for effects like sprint FOV (+10), zoom FOV (-20), or damage feedback.
        /// Added to BaseFov to calculate EffectiveFov.
        /// </summary>
        public float FovModifier;

        // ══════════════════════════════════════════════════
        // COMPUTED PROPERTIES
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the effective field of view by combining base FOV and modifiers.
        /// This is the actual FOV used for rendering after all effects are applied.
        /// </summary>
        public readonly float EffectiveFov => BaseFov + FovModifier;

        /// <summary>
        /// Gets the effective field of view converted to radians.
        /// Used for projection matrix calculations (OpenGL/graphics APIs use radians).
        /// </summary>
        public readonly float EffectiveFovRadians => EffectiveFov * MathHelper.Deg2Rad;

        // ══════════════════════════════════════════════════
        // MATRIX GENERATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Generates a perspective projection matrix based on current parameters.
        /// Used to transform 3D world coordinates to 2D screen coordinates.
        /// Must be updated when FOV, aspect ratio, or clipping planes change.
        /// </summary>
        /// <returns>A 4x4 perspective projection matrix for rendering.</returns>
        public readonly Matrix4X4<float> GetProjectionMatrix()
        {
            // ═══ Convert effective FOV to radians for matrix calculation
            float fovRad = EffectiveFovRadians;

            // ═══ Prevent division by zero with default aspect ratio
            float aspect = AspectRatio > 0f ? AspectRatio : 1f;

            // ═══ Create OpenGL-style perspective projection matrix
            // ═══ Maps 3D frustum to normalized device coordinates [-1, 1]
            return Matrix4X4.CreatePerspectiveFieldOfView(
                fovRad,     // ═══ Vertical field of view in radians
                aspect,     // ═══ Width/height ratio
                NearPlane,  // ═══ Near clipping distance
                FarPlane    // ═══ Far clipping distance
            );
        }

        /// <summary>
        /// Updates the aspect ratio based on window dimensions.
        /// Should be called when the window is resized to maintain correct proportions.
        /// </summary>
        /// <param name="width">The window width in pixels.</param>
        /// <param name="height">The window height in pixels.</param>
        public void UpdateAspect(int width, int height)
        {
            AspectRatio = height > 0 ? (float)width / height : 1f;
        }

        // ══════════════════════════════════════════════════
        // FACTORY
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a default camera projection with balanced settings for general gameplay.
        /// FOV: 70° (comfortable for first-person without distortion)
        /// Near: 0.05 (allows very close viewing for first-person interactions)
        /// Far: 2000 (good view distance for open worlds)
        /// Aspect: 16:9 (most common monitor ratio)
        /// </summary>
        public static CameraProjection Default => new()
        {
            BaseFov = 70f,
            FieldOfView = 70f,
            FovModifier = 0f,
            NearPlane = 0.05f,      // ═══ Very close for first-person
            FarPlane = 2000f,       // ═══ Far enough for large view distances
            AspectRatio = 16f / 9f  // ═══ Standard widescreen ratio
        };

        // ══════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Provides pre-configured projection presets for common camera scenarios.
        /// Use these as starting points for different game modes or visual styles.
        /// </summary>
        public static class Presets
        {
            /// <summary>
            /// Standard preset - same as Default.
            /// Balanced settings for general first-person and third-person gameplay.
            /// </summary>
            public static readonly CameraProjection Standard = Default;

            /// <summary>
            /// Cinematic preset for dramatic camera work and cutscenes.
            /// FOV: 50° (narrower FOV creates compression, more dramatic perspective)
            /// Near: 0.1 (slightly farther to reduce near-plane clipping in cutscenes)
            /// Far: 5000 (extended view distance for sweeping vistas)
            /// Aspect: 21:9 (ultra-wide cinematic ratio)
            /// </summary>
            public static readonly CameraProjection Cinematic = new()
            {
                BaseFov = 50f,
                FieldOfView = 50f,
                NearPlane = 0.1f,
                FarPlane = 5000f,
                AspectRatio = 21f / 9f  // ═══ Ultra-wide cinematic aspect ratio
            };

            /// <summary>
            /// First-person preset optimized for immersive FPS gameplay.
            /// FOV: 90° (wider FOV for better peripheral vision and fast movement)
            /// Near: 0.01 (extremely close for weapon models and hand interactions)
            /// Far: 1500 (moderate distance, prioritizes performance over distant views)
            /// Aspect: 16:9 (standard gaming ratio)
            /// </summary>
            public static readonly CameraProjection FirstPerson = new()
            {
                BaseFov = 90f,
                FieldOfView = 90f,
                NearPlane = 0.01f,  // ═══ Very close for first-person weapon rendering
                FarPlane = 1500f,
                AspectRatio = 16f / 9f
            };
        }
    }
}