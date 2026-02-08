using ImGuiNET;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Create_your_Adventure.Source.Engine.Debug
{
    public class DebugDisplay
    {
        // -------- FPS Tracking --------
        // --- Tracking
        private static double frameTimeAccumulator = 0.0;
        private static int frameCount = 0;
        private static double updateInterval = 1.0;

        // --- FPS
        private static double currentFPS = 0.0;
        private static double minFPS = double.MaxValue;
        private static double maxFPS = 0.0;
        private static double frameTime = 0.0;

        // --- Low FPS Tracking (1% and 0.1%)
        private static readonly List<double> frameTimeSamples = new(1000);
        private static double fpsOnePercentLow = 0.0;
        private static double fpsZeroOnePercentLow = 0.0;

        // --- Display
        private static bool showDebugWindow = true;
        private static Vector4 fpsColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);

        // --- Position Colors (X=Red, Y=Green, Z=Blue)
        private static readonly Vector4 colorX = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        private static readonly Vector4 colorY = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        private static readonly Vector4 colorZ = new Vector4(0.0f, 0.5f, 1.0f, 1.0f);

        // --- Camera Position (Update externally)
        private static Vector3D<float> cameraPosition = Vector3D<float>.Zero;
        private static float cameraYaw = 0.0f;
        private static float cameraPitch = 0.0f;


        // -------- Properties --------
        // --- ShowDebugWindow
        public static bool ShowDebugWindow
        {
            get => showDebugWindow;
            set => showDebugWindow = value;
        }

        // --- Current FPS
        public static double CurrentFPS => currentFPS;

        // --- Minimum FPS
        public static double MinFPS => minFPS == double.MaxValue ? 0.0 : minFPS;

        // --- Maximal FPS
        public static double MaxFPS => maxFPS;

        // --- Frametime ms
        public static double FrameTimeMs => frameTime * 1000.0;

        // --- 1% low FPS
        public static double FPSOnePercentLow => fpsOnePercentLow;

        // --- 0.1% low FPS
        public static double FPSZeroOnePercetLow => fpsZeroOnePercentLow;

        // UPDATE ---------------------------------------------------
        public static void Update(double deltaTime)
        {
            frameTime = deltaTime;
            frameTimeAccumulator += deltaTime;
            frameCount++;

            if (frameTimeSamples.Count >= 1000)
            {
                frameTimeSamples.RemoveAt(0);
            }
            frameTimeSamples.Add(deltaTime);

            if (frameTimeAccumulator >= updateInterval)
            {
                currentFPS = frameCount / frameTimeAccumulator;

                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;

                CalculateLowFPS();

                UpdateFPSColor();

                frameTimeAccumulator = 0.0;
                frameCount = 0;
            }
        }

        // UPDATE CAMERA POSITION ---------------------------------------------------
        public static void UpdateCameraPosition(Vector3D<float> position, float yaw, float pitch)
        {
            cameraPosition = position;
            cameraYaw = yaw;
            cameraPitch = pitch;
        }

        // RENDER IMGUI ---------------------------------------------------
        public static void RenderImGui()
        {
            if (!showDebugWindow) return;

            try
            {
                RenderLeftWing();
                RenderRightWing();
            }
            catch (Exception ex)
            {
                Logger.Error($"[DEBUGDISPLAY] Render error: {ex.Message}");
            }
        }

        // LEFT WING (GAME DEBUG INFO) ---------------------------------------------------
        private static void RenderLeftWing()
        {
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration |
                                           ImGuiWindowFlags.AlwaysAutoResize |
                                           ImGuiWindowFlags.NoSavedSettings |
                                           ImGuiWindowFlags.NoFocusOnAppearing |
                                           ImGuiWindowFlags.NoNav;

            ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Always);
            ImGui.SetNextWindowBgAlpha(0.35f);

            if (ImGui.Begin("Game Debug Info", windowFlags))
            {
                ImGui.Text("Camera Position");
                ImGui.Separator();

                ImGui.Text("Position:");
                ImGui.SameLine();
                ImGui.TextColored(colorX, $"X: {cameraPosition.X:F2}");
                ImGui.SameLine();
                ImGui.Text("|");
                ImGui.SameLine();
                ImGui.TextColored(colorY, $"Y: {cameraPosition.Y:F2}");
                ImGui.SameLine();
                ImGui.Text("|");
                ImGui.SameLine();
                ImGui.TextColored(colorZ, $"Z: {cameraPosition.Z:F2}");

                ImGui.Spacing();

                ImGui.Text($"Yaw: {cameraYaw:F1}°");
                ImGui.Text($"Pitch: {cameraPitch:F1}°");
            }

            ImGui.End();
        }

        // RIGHT WING (PERFORMANCE INFO) ---------------------------------------------------
        private static void RenderRightWing()
        {
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration |
                                           ImGuiWindowFlags.AlwaysAutoResize |
                                           ImGuiWindowFlags.NoSavedSettings |
                                           ImGuiWindowFlags.NoFocusOnAppearing |
                                           ImGuiWindowFlags.NoNav;

            var viewport = ImGui.GetMainViewport();

            float windowWidth = 380;
            ImGui.SetNextWindowPos(new Vector2(viewport.Size.X - windowWidth - 10, 10), ImGuiCond.Always);
            ImGui.SetNextWindowBgAlpha(0.35f);

            if (ImGui.Begin("Performance Info", windowFlags))
            {
                ImGui.Text("Performance");
                ImGui.Separator();

                ImGui.BeginGroup();
                {
                    ImGui.TextColored(fpsColor, $"FPS: {currentFPS:F1}");
                    ImGui.SameLine();
                    ImGui.Text("|");
                    ImGui.SameLine();
                    ImGui.Text($"MS: {FrameTimeMs:F2}");
                    ImGui.SameLine();
                    ImGui.Text("|");
                    ImGui.SameLine();
                    ImGui.Text($"1% Low: {fpsOnePercentLow:F1}");
                    ImGui.SameLine();
                    ImGui.Text("|");
                    ImGui.SameLine();
                    ImGui.Text($"0.1% Low: {fpsZeroOnePercentLow:F1}");
                }
                ImGui.EndGroup();

                ImGui.Spacing();

                ImGui.Text($"Min: {MinFPS:F1} FPS | Max:  {MaxFPS:F1} FPS");

                ImGui.Spacing();

                if (ImGui.Button("Reset Stats"))
                {
                    ResetStats();
                }
            }

            ImGui.End();
        }

        // HELPER METHOD ---------------------------------------------------
        // CALCULATE LOW PERCENNT FPS ---------------------------------------------------
        private static void CalculateLowFPS()
        {
            if (frameTimeSamples.Count < 10) return;

            var sortedFrameTimes = new List<double>(frameTimeSamples);
            sortedFrameTimes.Sort((a, b) => b.CompareTo(a));

            int countOnePercent = Math.Max(1, sortedFrameTimes.Count / 100);
            double sumOnePercent = 0.0;
            for (int i = 0; i < countOnePercent; i++)
            {
                sumOnePercent += sortedFrameTimes[i];
            }
            double avgOnePercent = sumOnePercent / countOnePercent;
            fpsOnePercentLow = avgOnePercent > 0.0001 ? 1.0 / avgOnePercent : 0.0;

            int countZeroOnePercent = Math.Max(1, sortedFrameTimes.Count / 1000);
            double sumZeroOnePercent = 0.0;
            for (int i = 0; i < countZeroOnePercent; i++)
            {
                sumZeroOnePercent += sortedFrameTimes[i];
            }
            double avgZeroOnePercent = sumZeroOnePercent / countZeroOnePercent;
            fpsZeroOnePercentLow = avgZeroOnePercent > 0.0001 ? 1.0 / avgZeroOnePercent : 0.0;
        }

        // FPS COLOR ---------------------------------------------------
        private static void UpdateFPSColor()
        {
            if (currentFPS >= 60.0)
                fpsColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            else if (currentFPS >= 30.0)
                fpsColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
            else
                fpsColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
        public static void ResetStats()
        {
            minFPS = double.MaxValue;
            maxFPS = 0.0;
            frameTimeAccumulator = 0.0;
            frameCount = 0;
            frameTimeSamples.Clear();
            fpsOnePercentLow = 0.0;
            fpsZeroOnePercentLow = 0.0;
        }
    }
}
