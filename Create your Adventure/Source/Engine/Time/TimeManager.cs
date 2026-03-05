using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Time
{
    public sealed class TimeManager
    {
        // ══════════════════════════════════════════════════
        // SINGLETON
        // ══════════════════════════════════════════════════
        private static TimeManager? instance;
        private static readonly Lock instanceLock = new();

        public static TimeManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new TimeManager();
                    }
                }

                return instance;
            }
        }

        // ══════════════════════════════════════════════════
        // FRAME TIME (Rendering, UI, Interpolation)
        // ══════════════════════════════════════════════════
        public double FixedDeltaTime { get; } = 1.0 / 60.0; // ═══ 60 TPS
        public double FrameDeltaTime { get; private set; }
        public double UnscaledFrameDeltaTime { get; private set; }
        public ulong FrameCount { get; private set; }

        public double SimulationTimeScale { get; set; } = 1.0;
        public ulong TickCount { get; private set; }

        private double simulationAccumulator;

        // ══════════════════════════════════════════════════
        // WORLD TIME (Tag/ Nacht, Wetter, Jahreszeiten)
        // ══════════════════════════════════════════════════
        public double WorldTime { get; private set; } // ═══ In-Game Seconds
        public double WorldTimeScale { get; set; } = 1.0; // ═══ 1 real sec = X world sec
        public bool IsWorldPaused { get; set; }

        // ══════════════════════════════════════════════════
        // BACKGROUND TIME (Chunk Loading, unabhängig von Pause)
        // ══════════════════════════════════════════════════
        public double UnscaledTotalTime { get; private set; }

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        private TimeManager()
        {

        }

        // ══════════════════════════════════════════════════
        // UPDATES METHODS
        // ══════════════════════════════════════════════════
        public void BeginFrame(double rawDeltaTime)
        {
            UnscaledFrameDeltaTime = rawDeltaTime;
            FrameDeltaTime = rawDeltaTime * SimulationTimeScale;
            UnscaledTotalTime += rawDeltaTime;
            FrameCount++;

            simulationAccumulator += FrameDeltaTime;
        }

        public int ConsumeFixedTicks()
        {
            int ticksThisFrame = 0;
            const int maxTicksPerFrame = 5; // ═══ Spiral of Death Prevention

            while (simulationAccumulator >= FixedDeltaTime && ticksThisFrame < maxTicksPerFrame)
            {
                simulationAccumulator -= FixedDeltaTime;
                TickCount++;
                ticksThisFrame++;

                if (!IsWorldPaused)
                {
                    WorldTime += FixedDeltaTime * WorldTimeScale;
                }
            }

            return ticksThisFrame;
        }

        public double GetInterpolationAlpha()
            => simulationAccumulator / FixedDeltaTime;
    }
}