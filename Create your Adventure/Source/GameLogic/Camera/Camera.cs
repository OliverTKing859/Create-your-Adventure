using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Input;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;

namespace Create_your_Adventure.Source.Gamelogic.Camera
{
    public class Camera
    {
        // ══════════════════════════════════════════════════
        // CONFIGURATION
        // ══════════════════════════════════════════════════
        private float mouseSensitivity = 0.1f;
        private float mouseSmoothingFactor = 15.0f;

        // ═══ Movement
        private float movementSpeed = 6.0f;
        private float verticalSpeed = 5.0f;
        private float sprintMultiplier = 2.5f;

        // ═══ accel & decel rate
        private float accelerationRate = 8.0f;
        private float decelerationRate = 6.0f;

        // ══════════════════════════════════════════════════
        // STATE
        // ══════════════════════════════════════════════════
        private Vector3D<float> position = new(0f, 0f, 5f);
        private float yaw = -90f;
        private float pitch = 0f;
        
        private Vector3D<float> velocity = Vector3D<float>.Zero;
        private float verticalVelocity = 0f;
        private Vector2 smoothedMouseDelta = Vector2.Zero;

        // ══════════════════════════════════════════════════
        // PROPERTIES
        // ══════════════════════════════════════════════════
        public Vector3D<float> Position => position;
        public float Yaw => yaw;
        public float Pitch => pitch;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        public Camera()
        {
            Logger.Info($"[CAMERA] Initialized at {position}");
        }

        // ══════════════════════════════════════════════════
        // UPDATE (Direct queries only!)
        // ══════════════════════════════════════════════════

        public void Update(float deltaTime)
        {
            var input = InputManager.Instance;

            // ═══ Cursor toggle (only action use permitted)
            if (input.IsActionTriggered("ToggleCursorLock"))
                input.ToggleCursorLock();

            // ═══ Movement (Direct Queries!)
            ProcessMovement(deltaTime);

            ProcessRotation(deltaTime);
        }

        private void ProcessMovement(float dt)
        {
            var input = InputManager.Instance;

            // ═══ Direction-Vectors
            var forward = GetViewDirection();
            var right = Vector3D.Normalize(Vector3D.Cross(forward, Vector3D<float>.UnitY));
            var forwardFlat = Vector3D.Normalize(new Vector3D<float>(forward.X, 0, forward.Z));

            // ═══ Input (Direct Queries!)
            Vector3D<float> inputDir = Vector3D<float>.Zero;

            if (input.IsKeyDown(KeyCode.W)) inputDir += forwardFlat;
            if (input.IsKeyDown(KeyCode.S)) inputDir -= forwardFlat;
            if (input.IsKeyDown(KeyCode.A)) inputDir -= right;
            if (input.IsKeyDown(KeyCode.D)) inputDir += right;

            if (inputDir.LengthSquared > 0)
                inputDir = Vector3D.Normalize(inputDir);

            // ═══ Sprint
            float speed = movementSpeed;
            if (input.IsKeyDown(KeyCode.LeftShift))
                speed *= sprintMultiplier;

            // ═══ Horizontal Velocity (Smooth)
            var targetVel = inputDir * speed;
            float rate = inputDir.LengthSquared > 0 ? accelerationRate : decelerationRate;
            velocity = Vector3D.Lerp(velocity, targetVel, 1f - MathF.Exp(-rate * dt));

            // ═══ Vertical
            float targetVert = 0f;
            if (input.IsKeyDown(KeyCode.Space)) targetVert += verticalSpeed;
            if (input.IsKeyDown(KeyCode.LeftControl)) targetVert -= verticalSpeed;

            verticalVelocity = MathHelper.Lerp(verticalVelocity, targetVert, 1f - MathF.Exp(-accelerationRate * dt));

            // ═══ Apply
            position += velocity * dt;
            position.Y += verticalVelocity * dt;
        }

        private void ProcessRotation(float dt)
        {
            var input = InputManager.Instance;

            // ═══ Mouse Delta (Direct Query!)
            var rawDelta = input.GetLookVector();

            // ═══ Smoothing
            float smooth = 1f - MathF.Exp(-mouseSmoothingFactor * dt);
            smoothedMouseDelta = Vector2.Lerp(smoothedMouseDelta, rawDelta, smooth);

            // ═══ Apply Rotation
            yaw += smoothedMouseDelta.X * mouseSensitivity;
            pitch -= smoothedMouseDelta.Y * mouseSensitivity;
            pitch = Math.Clamp(pitch, -89f, 89f);
        }

        // ══════════════════════════════════════════════════
        // MATRICES
        // ══════════════════════════════════════════════════
        public Matrix4X4<float> GetViewMatrix()
        {
            var front = GetViewDirection();
            var right = Vector3D.Normalize(Vector3D.Cross(front, Vector3D<float>.UnitY));
            var up = Vector3D.Cross(right, front);
            return Matrix4X4.CreateLookAt(position, position + front, up);
        }

        public Matrix4X4<float> GetProjectionMatrix(int width, int height, float fovDegrees = 60f, float near = 0.1f, float far = 1000f)
        {
            float aspect = height > 0 ? (float)width / height : 1f;
            float fovRad = fovDegrees * (MathF.PI / 180f);
            return Matrix4X4.CreatePerspectiveFieldOfView(fovRad, aspect, near, far);
        }

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════
        private Vector3D<float> GetViewDirection()
        {
            float yawRad = yaw * (MathF.PI / 180);
            float pitchRad = pitch * (MathF.PI / 180f);

            return Vector3D.Normalize(new Vector3D<float>(
                MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                MathF.Sin(pitchRad),
                MathF.Sin(yawRad) * MathF.Cos(pitchRad)
                ));
        }

        internal static class MathHelper
        {
            public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        }
    }
}