using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Debug
{
    public class DebugDisplay
    {
        // -------- FPS Tracking --------
        // --- Tracking
        private static double frameTimeAccumulator = 0.0;
        private static int frameCount = 0;
        private static double updateInterval = 1;

        // --- FPS
        private static double currentFPS = 0.0;
        private static double minFPS = double.MaxValue;
        private static double maxFPS = 0.0;
        private static double frameTime = 0.0;

        // --- Display
        private static bool showDebugWindow = true;
        private static Vector4 fpsColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);


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

        public static void Update(double deltaTime)
        {
            frameTime = deltaTime;
            frameTimeAccumulator += deltaTime;
            frameCount++;

            if (frameTimeAccumulator >= updateInterval)
            {
                currentFPS = frameCount / frameTimeAccumulator;

                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;

                UpdateFPSColor();

                frameTimeAccumulator = 0.0;
                frameCount = 0;
            }
        }

        public static void RenderImGui()
        {
            if (!showDebugWindow) return;

            RenderLeftWing();
            RenderRightWing();
        }

        private static void RenderLeftWing()
        {
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration |
                                            ImGuiWindowFlags.AlwaysAutoResize |
                                            ImGuiWindowFlags.NoSavedSettings |
                                            ImGuiWindowFlags.NoFocusOnAppearing |
                                            ImGuiWindowFlags.NoNav;

            ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Always);
            ImGui.SetNextWindowBgAlpha(0.35f);

            if (ImGui.Begin("GAME DEBUG INFO", windowFlags))
            {
                ImGui.Text("placeholder");
                ImGui.Separator();
            }

        }

        private static void RenderRightWing()
        {
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration |
                                            ImGuiWindowFlags.AlwaysAutoResize |
                                            ImGuiWindowFlags.NoSavedSettings |
                                            ImGuiWindowFlags.NoFocusOnAppearing |
                                            ImGuiWindowFlags.NoNav;

            ImGui.SetNextWindowPos(new Vector2(1750, 10), ImGuiCond.Always);
            ImGui.SetNextWindowBgAlpha(0.35f);

            if (ImGui.Begin("Debug Info", windowFlags))
            {
                ImGui.Text("Performance");
                ImGui.Separator();

                ImGui.TextColored(fpsColor, $"FPS: {currentFPS:F1}");
                ImGui.Text($"Frame Time: {FrameTimeMs:F2} ms");

                ImGui.Spacing();

                ImGui.Text($"Min FPS: {MinFPS:F1}");
                ImGui.Text($"Max FPS: {MaxFPS:F1}");

                ImGui.Spacing();

                if (ImGui.Button("Reset Stats"))
                {
                    ResetStats();
                }
            }

            ImGui.End();
        }

        private static void UpdateFPSColor()
        {
            if (currentFPS >= 60.0)
            {
                fpsColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            }
            else if (currentFPS >= 30.0)
            {
                fpsColor = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
            }
            else
            {
                fpsColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        public static void ResetStats()
        {
            minFPS = double.MaxValue;
            maxFPS = 0.0;
            frameTimeAccumulator = 0.0;
            frameCount = 0;
        }
    }
}