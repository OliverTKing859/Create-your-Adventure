using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Input;
using Silk.NET.Maths;
using System.Numerics;

namespace Create_your_Adventure.Source.Gamelogic.Camera
{
    // Future me: When you do first-person and third-person later, you'll do it with an ->interface<- "ICamera".


    /// <summary>
    /// A free-flying debug camera with smooth WASD movement and mouse look.
    /// Supports sprinting, vertical movement (Space/Ctrl), and configurable acceleration.
    /// </summary>
    /// <remarks>
    /// Future: Implement <c>ICamera</c> interface for first-person and third-person variants.
    /// </remarks>
    public class Camera
    {
        // -------- Configurations --------
        // --- Mouse
        private float mouseSensitivity = 50.0f;
        private float mouseSmoothingFactor = 60.0f;

        // --- Camera Movement
        private float movementSpeed = 6.0f;
        private float movementVerticalSpeed = 5.0f;
        private float sprintMovementSpeedMultiplier = 2.5f;

        // --- Acceleration & Deceleration
        private float accelerationHorizontalRate = 4.0f;
        private float decelerationHorizontalRate = 2.0f;
        private float accelerationVerticalRate = 12.0f;
        private float decelerationVerticalRate = 8.0f;

        // --- Velocity
        private float velocityVertical = 0.0f;
        private Vector3D<float> velocityHorizontal = Vector3D<float>.Zero;

        // -------- Camera State --------
        private Vector3D<float> cameraPosition = new(0f, 0f, 3f);
        private float yaw = -90f;
        private float pitch = 0f;

        // -------- Mouse State ---------
        private Vector2 mouseDeltaSmoothed = Vector2.Zero;
        private Vector2 rawMouseDelta = Vector2.Zero;
        private bool firstMouse = true;
        private Vector2 lastMousePosition;

        // -------- Cache Matrices --------
        private Matrix4X4<float> view;
        private Matrix4X4<float> projection;

        // CONSTRUCTION ----------------------------------------------------------------

        /// <summary>
        /// Initializes a new camera with default position (0, 0, 3) and rotation (yaw: -90°, pitch: 0°).
        /// </summary>
        public Camera()
        {
            Logger.Info($"[CAMERA] Initialized at position: {cameraPosition}");
            Logger.Info($"[CAMERA] Initial rotation - Yaw: {yaw}°, Pitch: {pitch}°");
        }

        // UPDATE ----------------------------------------------------------------------

        /// <summary>
        /// Updates the camera position and rotation based on input and delta time.
        /// Applies smooth acceleration/deceleration for fluid movement.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame in seconds.</param>
        /// <param name="keyW">Forward movement input.</param>
        /// <param name="keyA">Left strafe input.</param>
        /// <param name="keyS">Backward movement input.</param>
        /// <param name="keyD">Right strafe input.</param>
        /// <param name="keySpace">Upward movement input.</param>
        /// <param name="keyLeftCtrl">Downward movement input.</param>
        /// <param name="keyLeftShift">Sprint modifier input.</param>
        public void Update(double deltaTime)
        {
            // ═══ Delta Time
            float dt = (float)deltaTime;

            // ═══ Input of InputManager to get (Not more on Silk.NET directly)
            var input = InputManager.Instance;

            // ═══ Movement Vector (Keyboard or Gamepad)
            Vector2 movement = input.GetMovementVector();
            bool isSprinting = input.IsKeyDown(KeyCode.LeftShift) ||
                               input.IsGamepadButtonDown(GamepadButton.LeftBumper);

            // ═══ Vertical Movement
            float verticalInput = 0f;
            if (input.IsKeyDown(KeyCode.Space)) verticalInput += 1f;
            if (input.IsKeyDown(KeyCode.LeftControl)) verticalInput -= 1f;

            // ═══ Look Vector
            Vector2 lookDelta = input.GetLookVector();

            // ═══ Toggle Cursor Lock (ESC)
            if (input.IsActionTriggered("ToggleCursorLock"))
            {
                input.ToggleCursorLock();
            }

            /*
            // -------- View --------
            Vector3D<float> viewForward = GetViewDirection(yaw, pitch);
            Vector3D<float> viewRight = Vector3D.Normalize(Vector3D.Cross(viewForward, Vector3D<float>.UnitY));
            //Code fossil: Vector3D<float> viewUp = Vector3D<float>.UnitY;

            // -------- Input Direction --------
            Vector3D<float> viewForwardHorizontal = Vector3D.Normalize(new Vector3D<float>(viewForward.X, 0, viewForward.Z));
            Vector3D<float> inputDirection = Vector3D<float>.Zero;

            // -------- WASD --------
            if (keyW)
            {
                inputDirection += viewForwardHorizontal;
            }
            if (keyA)
            {
                inputDirection -= viewRight;
            }
            if (keyS)
            {
                inputDirection -= viewForwardHorizontal;
            }
            if (keyD)
            {
                inputDirection += viewRight;
            }

            // --- Normalize input direction to prevent faster diagonal movement
            if (inputDirection.LengthSquared > 0)
            {
                inputDirection = Vector3D.Normalize(inputDirection);
            }

            // -------- Sprint Input --------
            float speed = movementSpeed;
            if (keyLeftShift)
            {
                speed *= sprintMovementSpeedMultiplier;
            }

            // -------- Horizontal Velocity --------
            Vector3D<float> targetVelocity = inputDirection * speed;
            float rate = (inputDirection.LengthSquared > 0) ? accelerationHorizontalRate : decelerationHorizontalRate;
            velocityHorizontal = Vector3D.Lerp(
                velocityHorizontal,
                targetVelocity,
                1.0f - MathF.Exp(-rate * dt)
                );

            // -------- Vertical Input (Space/Ctrl) --------
            float targetVertical = 0f;

            if (keySpace)
            {
                targetVertical += movementVerticalSpeed;
            }
            if (keyLeftCtrl)
            {
                targetVertical -= movementVerticalSpeed;
            }

            // -------- Vertical Velocity --------
            float verticalRate = (MathF.Abs(targetVertical) > 0.001f)
                ? accelerationVerticalRate
                : decelerationVerticalRate;

            float verticalLerpFactor = 1.0f - MathF.Exp(-verticalRate * dt);
            velocityVertical = velocityVertical + (targetVertical - velocityVertical) * verticalLerpFactor;

            // -------- Apply Movement --------
            cameraPosition += velocityHorizontal * dt;
            cameraPosition.Y += velocityVertical * dt;

            // -------- Mouse Smoothing --------
            float smoothFactor = 1.0f - MathF.Exp(-mouseSmoothingFactor * dt);
            mouseDeltaSmoothed = Vector2.Lerp(mouseDeltaSmoothed, rawMouseDelta, smoothFactor);

            // -------- Apply Rotation --------
            yaw += mouseDeltaSmoothed.X * mouseSensitivity * dt;
            pitch -= mouseDeltaSmoothed.Y * mouseSensitivity * dt;
            // --- Clamp Pitch
            pitch = Math.Clamp(pitch, -89f, 89f);

            // --- Reset raw delta for next frame
            rawMouseDelta = Vector2.Zero;

            */
            // ═══ Apply
            ApplyMovement(movement, verticalInput, isSprinting, dt);
            ApplyRotation(lookDelta, dt);
        }

        // VIEW MAXTRIX ---------------------------------------------------

        /// <summary>
        /// Computes the view matrix based on current camera position and orientation.
        /// </summary>
        /// <returns>A look-at view matrix for rendering.</returns>
        public Matrix4X4<float> GetViewMatrix()
        {
            // -------- Calculate camera axes --------
            Vector3D<float> cameraFront = GetViewDirection(yaw, pitch);
            Vector3D<float> cameraRight = Vector3D.Normalize(Vector3D.Cross(cameraFront, Vector3D<float>.UnitY));
            Vector3D<float> cameraUp = Vector3D.Cross(cameraRight, cameraFront);
            return Matrix4X4.CreateLookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
        }

        // PROJECTION MATRIX ---------------------------------------------------

        /// <summary>
        /// Creates a perspective projection matrix for the given viewport dimensions.
        /// </summary>
        /// <param name="width">Viewport width in pixels.</param>
        /// <param name="height">Viewport height in pixels.</param>
        /// <param name="fovDegrees">Vertical field of view in degrees.</param>
        /// <param name="near">Near clipping plane distance.</param>
        /// <param name="far">Far clipping plane distance.</param>
        /// <returns>A perspective projection matrix.</returns>
        public Matrix4X4<float> CreatePerspective(int width, int height, float fovDegrees, float near, float far)
        {
            if (width <= 0 || height <= 0)
            {
                Logger.Warn($"[CAMERA] Invalid viewport dimensions: {width}x{height} - using fallback aspect ratio 1.0");
            }

            float aspect = width <= 0 || height <= 0 ? 1.0f : (float)width / height;
            float fov = DegreesToRadians(fovDegrees);

            Logger.Info($"[CAMERA] Projection created - FOV: {fovDegrees}°, Aspect: {aspect:F2}, Near: {near}, Far: {far}");

            return Matrix4X4.CreatePerspectiveFieldOfView(fov, aspect, near, far);
        }

        // MOUSE INPUT ---------------------------------------------------

        /// <summary>
        /// Processes mouse movement input for camera rotation.
        /// Call this from your mouse move event handler.
        /// </summary>
        /// <param name="currentPosition">Current mouse position in screen coordinates.</param>
        public void OnMouseMove(Vector2 currentPosition)
        {
            // -------- First Mouse Initialization --------
            if (firstMouse)
            {
                lastMousePosition = currentPosition;
                firstMouse = false;
                Logger.Info("[CAMERA] Mouse input initialized");
                return;
            }

            // -------- Calculate Delta --------
            rawMouseDelta = currentPosition - lastMousePosition;
            lastMousePosition = currentPosition;
        }

        // HELPER METHODS ---------------------------------------------------
        private static Vector3D<float> GetViewDirection(float yawDegrees, float pitchDegrees)
        {
            // -------- Degrees to Radians --------
            float yaw = DegreesToRadians(yawDegrees);
            float pitch = DegreesToRadians(pitchDegrees);

            // -------- Spherical to Cartesian --------
            float sinPitch = MathF.Sin(pitch);
            float cosPitch = MathF.Cos(pitch);
            float sinYaw = MathF.Sin(yaw);
            float cosYaw = MathF.Cos(yaw);

            return Vector3D.Normalize(new Vector3D<float>(
                x: cosYaw * cosPitch,
                y: sinPitch,
                z: sinYaw * cosPitch
                )
                );
        }
        private static float DegreesToRadians(float degrees) => degrees * (MathF.PI / 180.0f);
    }
}