using Create_your_Adventure.Source.Engine.Input;
using Create_your_Adventure.Source.Engine.Time;
using Create_your_Adventure.Source.Engine.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Core
{
    public sealed class GameLoop
    {
        private readonly TimeManager time = TimeManager.Instance;
        private readonly WorldScheduler scheduler = WorldScheduler.Instance;
        private readonly WorldRelevanceFilter relevance = new();

        // ══════════════════════════════════════════════════
        // MAIN LOOP ENTRY POINTS (from WindowManager invoke)
        // ══════════════════════════════════════════════════

        public void OnUpdate(double rawDeltaTime)
        {
            // ══════════════════════════════════════════════════
            // PHASE 1: EARLY UPDATE
            // ══════════════════════════════════════════════════
            time.BeginFrame(rawDeltaTime);
            InputManager.Instance.BeginFrame();

            // ══════════════════════════════════════════════════
            // PHASE 2: FIXED SIMULATION TICKS
            // ══════════════════════════════════════════════════
            int ticksThisFrame = time.ConsumeFixedTicks();

            for (int i = 0; i < ticksThisFrame; i++)
            {
                FixedTick();
            }

            // ══════════════════════════════════════════════════
            // PHASE 3: WORLD SCHEDULER (Budget-Based)
            // ══════════════════════════════════════════════════
            scheduler.ProcessFrame();

            // ══════════════════════════════════════════════════
            // PHASE 4: VARIABLE UPDATE
            // ══════════════════════════════════════════════════
            float dt = (float)time.FrameDeltaTime;
            float alpha = (float)time.GetInterpolationAlpha();

            // ═══ Camera Update (Interpolation-ready)
            // ═══ camera.Update(dt, alpha);

            // ══════════════════════════════════════════════════
            // PHASE 5: LATE UPDATE
            // ══════════════════════════════════════════════════
            InputManager.Instance.EndFrame(dt);
        }

        private void FixedTick()
        {
            float fixedDt = (float)time.FixedDeltaTime;

            // ═══ Physics
            // ═══ PhysicsSystem.Tick(fixedDt);

            // ═══ Player Logic
            // ═══ PlayerController.Tick(fixedDt);

            // ═══ Chunk Simulation (nur relevante!)
            // ═══ foreach (var chunk in GetRelevantChunks())
            // ═══ {
            // ═══     if (ShouldTickThisFrame(chunk))
            // ═══         chunk.Simulate(fixedDt);
            // ═══ }
        }

        public void OnRender(double rawDeltaTime)
        {
            // ═══ Interpolation Alpha for smooth rendering
            float alpha = (float)time.GetInterpolationAlpha();

            // ═══ Render only visible Chunks
            // ═══ foreach (var chunk in GetVisibleChunks())
            // ═══ {
            // ═══     chunk.Render(alpha);
            // ═══ }
        }
    }
}
