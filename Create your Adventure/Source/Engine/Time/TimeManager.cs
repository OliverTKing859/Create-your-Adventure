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

        private double simulationTimeScale = 1.0;
        public double SimulationTimeScale
        {
            get => simulationTimeScale;
            set => simulationTimeScale = Math.Clamp(value, 0.0, 10.0);
        }

        public ulong TickCount { get; private set; }

        private double simulationAccumulator;

        // ══════════════════════════════════════════════════
        // WORLD TIME (Tag/ Nacht, Wetter, Jahreszeiten)
        // ══════════════════════════════════════════════════
        public double WorldTime { get; private set; } // ═══ In-Game Seconds

        private double worldTimeScale = 1.0;
        public double WorldTimeScale
        {
            get => worldTimeScale;
            set => worldTimeScale = Math.Clamp(value, 0.0, 100.0);
        }

        public bool IsWorldPaused { get; set; }

        // ══════════════════════════════════════════════════
        // BACKGROUND TIME (Chunk Loading, unabhängig von Pause)
        // ══════════════════════════════════════════════════
        public double UnscaledTotalTime { get; private set; }

        private const int MaxTicksPerFrame = 5;

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

            while (simulationAccumulator >= FixedDeltaTime && ticksThisFrame < MaxTicksPerFrame)
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

        public void Reset()
        {
            FrameDeltaTime = 0.0;
            UnscaledFrameDeltaTime = 0.0;
            FrameCount = 0;
            TickCount = 0;
            simulationAccumulator = 0.0;
            WorldTime = 0.0;
            UnscaledTotalTime = 0.0;
            SimulationTimeScale = 1.0;
            WorldTimeScale = 1.0;
            IsWorldPaused = false;
        }

        public double GetInterpolationAlpha()
            => simulationAccumulator / FixedDeltaTime;
    }
}