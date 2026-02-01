using Silk.NET.Maths;
using System.Numerics;

namespace Create_your_Adventure
{
    // Future me: When you do first-person and third-person later, you'll do it with an ->interface<- "ICamera".
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

        // -------- Camera --------
        private Vector3D<float> cameraPosition = new(0f, 0f, 3f);
        private float yaw = -90f;
        private float pitch = 0f;

        private Vector2 mouseDeltaSmoothed = Vector2.Zero;
        private Vector2 rawMouseDelta = Vector2.Zero;

        private bool firstMouse = true;
        private Vector2 lastMousePosition;

        private Matrix4X4<float> view;
        private Matrix4X4<float> projection;

        // CONSTRUCTION ----------------------------------------------------------------

        /// <summary>
        /// 
        /// Constructor für die Camera-Klasse.
        /// 
        /// Aktuell leer, da alle Felder bereits bei der Deklaration initialisiert werden
        /// (z.B. yaw = -90f, pitch = 0f, cameraPosition = new(0f, 0f, 3f)).
        /// 
        /// ZUKÜNFTIGE ERWEITERUNGEN:
        /// - Parameter für Start-Position: Camera(Vector3D<float> startPos)
        /// - Parameter für Start-Rotation: Camera(float startYaw, float startPitch)
        /// - Initialisierungs-Logik die Berechnungen benötigt
        /// - Setup-Code der nur einmal beim Erstellen ausgeführt werden soll
        /// 
        /// Beispiel für späteren erweiterten Constructor:
        /// public Camera(Vector3D<float> startPos, float startYaw = -90f, float startPitch = 0f)
        /// {
        ///     this.cameraPosition = startPos;
        ///     this.yaw = startYaw;
        ///     this.pitch = startPitch;
        ///     Console.WriteLine($"Camera erstellt an Position {startPos}");
        /// }
        /// </summary>
        public Camera()
        {
        }

        // UPDATE ----------------------------------------------------------------------
        public void Update(
            double deltaTime,
            bool keyW,
            bool keyA,
            bool keyS,
            bool keyD,
            bool keySpace,
            bool keyLeftCtrl,
            bool keyLeftShift
            )
        {
            // -------- Delta Time --------
            float dt = (float)deltaTime;

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

            // -------- Target Velocity --------
            Vector3D<float> targetVelocity = inputDirection * speed;

            // -------- Horizontal smoothing --------
            float rate = (inputDirection.LengthSquared > 0) ? accelerationHorizontalRate : decelerationHorizontalRate;
            velocityHorizontal = Vector3D.Lerp(
                velocityHorizontal,
                targetVelocity,
                1.0f - MathF.Exp(-rate * dt)
                );

            // -------- Space/Shift Input --------
            float targetVertical = 0f;

            if (keySpace)
            {
                targetVertical += movementVerticalSpeed;
            }
            if (keyLeftCtrl)
            {
                targetVertical -= movementVerticalSpeed;
            }

            // -------- Acceleration & Deleceration ---------
            float verticalRate = (MathF.Abs(targetVertical) > 0.001f)
                ? accelerationVerticalRate
                : decelerationVerticalRate;

            // -------- Vertical smoothing --------
            float verticalLerpFactor = 1.0f - MathF.Exp(-verticalRate * dt);
            velocityVertical = velocityVertical + (targetVertical - velocityVertical) * verticalLerpFactor;
            cameraPosition += velocityHorizontal * dt;
            cameraPosition.Y += velocityVertical * dt;

            // -------- Smoothing --------
            float smoothFactor = 1.0f - MathF.Exp(-mouseSmoothingFactor * dt);
            mouseDeltaSmoothed = Vector2.Lerp(mouseDeltaSmoothed, rawMouseDelta, smoothFactor);

            // -------- Yaw & Pitch --------
            yaw += mouseDeltaSmoothed.X * mouseSensitivity * dt;
            pitch -= mouseDeltaSmoothed.Y * mouseSensitivity * dt;
            // --- Clamp Pitch
            pitch = Math.Clamp(pitch, -89f, 89f);
            rawMouseDelta = Vector2.Zero;
        }

        // VIEW & PROJECTION MATRICES ---------------------------------------------------
        private static Vector3D<float> GetViewDirection(float yawDegrees, float pitchDegrees)
        {
            // -------- Degrees to Radians --------
            float yaw = DegreesToRadians(yawDegrees);
            float pitch = DegreesToRadians(pitchDegrees);

            // -------- Spherical to Cartesian --------
            // Calculate view direction from yaw and pitch
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

        // VIEW & PROJECTION MATRICES ---------------------------------------------------
        public Matrix4X4<float> GetViewMatrix()
        {
            // -------- Calculate camera axes --------
            Vector3D<float> cameraFront = GetViewDirection(yaw, pitch);
            Vector3D<float> cameraRight = Vector3D.Normalize(Vector3D.Cross(cameraFront, Vector3D<float>.UnitY));
            Vector3D<float> cameraUp = Vector3D.Cross(cameraRight, cameraFront);
            return Matrix4X4.CreateLookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
        }

        // PROJECTION MATRIX ---------------------------------------------------
        public Matrix4X4<float> CreatePerspective(int width, int height, float fovDegrees, float near, float far)
        {
            // -------- Aspect Ratio --------
            float aspect = width <= 0 || height <= 0 ? 1.0f : (float)width / height;
            float fov = DegreesToRadians(fovDegrees);
            return Matrix4X4.CreatePerspectiveFieldOfView(fov, aspect, near, far);
        }

        // HELPER METHODS ---------------------------------------------------
        private static float DegreesToRadians(float degrees) => degrees * (MathF.PI / 180.0f);

        // MOUSE INPUT ---------------------------------------------------
        public void OnMouseMove(Vector2 currentPosition)
        {
            // -------- (First) Mouse movement initialization --------
            if (firstMouse)
            {
                lastMousePosition = currentPosition;
                firstMouse = false;
                return;
            }

            // -------- Calculate raw delta --------
            rawMouseDelta = currentPosition - lastMousePosition;
            lastMousePosition = currentPosition;
        }
    }
}