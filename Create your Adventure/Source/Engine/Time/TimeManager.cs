namespace Create_your_Adventure.Source.Engine.Time
{
    /// <summary>
    /// Central time management system for the game engine.
    /// Manages three independent timelines: frame time (rendering), simulation time (fixed timestep physics),
    /// and world time (day/night cycles, weather). Supports time scaling and pausing for each timeline.
    /// Uses fixed timestep accumulator pattern for deterministic physics simulation.
    /// </summary>
    public sealed class TimeManager
    {
        // ══════════════════════════════════════════════════
        // SINGLETON
        // ══════════════════════════════════════════════════

        // ═══ Singleton instance of the TimeManager
        private static TimeManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the TimeManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
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
        /// <summary>
        /// Gets the fixed timestep for simulation updates in seconds.
        /// Set to 1/60 (0.0166s) for 60 ticks per second.
        /// Used for deterministic physics and gameplay logic.
        /// </summary>
        public double FixedDeltaTime { get; } = 1.0 / 60.0;

        /// <summary>
        /// Gets the scaled time elapsed since last frame in seconds.
        /// Affected by SimulationTimeScale (slow motion, fast forward).
        /// Use this for frame-based updates like animation and interpolation.
        /// </summary>
        public double FrameDeltaTime { get; private set; }

        /// <summary>
        /// Gets the raw unscaled time elapsed since last frame in seconds.
        /// Not affected by time scale - always represents real-world time.
        /// Use this for UI, background tasks, and frame rate calculations.
        /// </summary>
        public double UnscaledFrameDeltaTime { get; private set; }

        /// <summary>
        /// Gets the total number of frames rendered since engine start.
        /// Increments once per frame regardless of time scale.
        /// </summary>
        public ulong FrameCount { get; private set; }

        // ═══ Internal time scale multiplier for simulation
        private double simulationTimeScale = 1.0;

        /// <summary>
        /// Gets or sets the simulation time scale multiplier.
        /// 1.0 = normal speed, 0.5 = half speed (slow motion), 2.0 = double speed.
        /// Clamped to [0.0, 10.0] to prevent extreme values.
        /// Affects FrameDeltaTime but not UnscaledFrameDeltaTime.
        /// </summary>
        public double SimulationTimeScale
        {
            get => simulationTimeScale;
            set => simulationTimeScale = Math.Clamp(value, 0.0, 10.0);
        }

        /// <summary>
        /// Gets the total number of fixed simulation ticks processed.
        /// Increments by FixedDeltaTime intervals, not every frame.
        /// Use this to count simulation steps for deterministic gameplay.
        /// </summary>
        public ulong TickCount { get; private set; }

        // ═══ Accumulates frame time until enough for a fixed timestep
        private double simulationAccumulator;

        // ══════════════════════════════════════════════════
        // WORLD TIME (Tag/ Nacht, Wetter, Jahreszeiten)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the current in-game world time in seconds.
        /// Advances at WorldTimeScale rate (can be faster/slower than real time).
        /// Use this for day/night cycles, weather progression, and seasonal effects.
        /// Pauses when IsWorldPaused is true.
        /// </summary>
        public double WorldTime { get; private set; }

        // ═══ Internal time scale multiplier for world time
        private double worldTimeScale = 1.0;

        /// <summary>
        /// Gets or sets the world time progression rate.
        /// 1.0 = real-time, 60.0 = 1 real second = 1 in-game minute (Minecraft-style).
        /// Clamped to [0.0, 100.0] to prevent extreme values.
        /// Allows speeding up day/night cycles without affecting physics.
        /// </summary>
        public double WorldTimeScale
        {
            get => worldTimeScale;
            set => worldTimeScale = Math.Clamp(value, 0.0, 100.0);
        }

        /// <summary>
        /// Gets or sets whether world time is paused.
        /// When true, WorldTime stops advancing (freeze day/night, weather).
        /// Simulation and rendering continue normally - only world progression pauses.
        /// </summary>
        public bool IsWorldPaused { get; set; }

        // ══════════════════════════════════════════════════
        // BACKGROUND TIME (Chunk Loading, unabhängig von Pause)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the total unscaled time since engine start in seconds.
        /// Never affected by time scale or pausing - pure real-world time.
        /// Use for background tasks like chunk loading, profiling, and real-time systems.
        /// </summary>
        public double UnscaledTotalTime { get; private set; }

        // ═══ Maximum fixed ticks to process in a single frame to prevent spiral of death
        private const int MaxTicksPerFrame = 5;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// Use TimeManager.Instance to access the time manager.
        /// </summary>
        private TimeManager()
        {

        }

        // ══════════════════════════════════════════════════
        // UPDATES METHODS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Called at the start of each frame to update time values.
        /// Records raw delta time, applies time scale, and accumulates for fixed timesteps.
        /// Must be called before ConsumeFixedTicks() in the game loop.
        /// </summary>
        /// <param name="rawDeltaTime">Raw time elapsed since last frame in seconds (from system timer).</param>
        public void BeginFrame(double rawDeltaTime)
        {
            // ═══ Store raw unscaled time for frame-rate-independent operations
            UnscaledFrameDeltaTime = rawDeltaTime;

            // ═══ Apply time scale for simulation (slow motion, fast forward)
            FrameDeltaTime = rawDeltaTime * SimulationTimeScale;

            // ═══ Track total real-world elapsed time
            UnscaledTotalTime += rawDeltaTime;

            // ═══ Increment frame counter
            FrameCount++;

            // ═══ Accumulate scaled time for fixed timestep processing
            // ═══ This will be consumed by ConsumeFixedTicks()
            simulationAccumulator += FrameDeltaTime;
        }

        /// <summary>
        /// Processes accumulated time into fixed timestep ticks.
        /// Returns the number of simulation ticks that should be executed this frame.
        /// Each tick represents exactly FixedDeltaTime seconds of simulation.
        /// Prevents spiral of death by capping at MaxTicksPerFrame (5 ticks).
        /// Call this after BeginFrame() to drive fixed-timestep physics and gameplay.
        /// </summary>
        /// <returns>Number of fixed ticks to process this frame (0-5).</returns>
        public int ConsumeFixedTicks()
        {
            int ticksThisFrame = 0;

            // ═══ Process fixed timesteps while we have enough accumulated time
            // ═══ Cap at MaxTicksPerFrame to prevent death spiral on slow frames
            while (simulationAccumulator >= FixedDeltaTime && ticksThisFrame < MaxTicksPerFrame)
            {
                // ═══ Consume one fixed timestep from accumulator
                simulationAccumulator -= FixedDeltaTime;

                // ═══ Increment tick counter
                TickCount++;
                ticksThisFrame++;

                // ═══ Advance world time if not paused
                // ═══ World time is separate from simulation time (can have different scale)
                if (!IsWorldPaused)
                {
                    WorldTime += FixedDeltaTime * WorldTimeScale;
                }
            }

            return ticksThisFrame;
        }

        /// <summary>
        /// Resets all time values to their initial state.
        /// Use when restarting the game or loading a new level.
        /// Does not reset singleton instance - only time values.
        /// </summary>
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

        /// <summary>
        /// Gets the interpolation factor for smooth frame rendering between fixed ticks.
        /// Returns a value between 0.0 and 1.0 indicating progress toward next tick.
        /// Use this to interpolate object positions for smooth visuals despite fixed physics timestep.
        /// Formula: accumulated_time / fixed_timestep
        /// Example: if halfway to next tick, returns 0.5 for 50% interpolation.
        /// </summary>
        /// <returns>Interpolation alpha between 0.0 (current tick) and 1.0 (next tick).</returns>
        public double GetInterpolationAlpha()
            => simulationAccumulator / FixedDeltaTime;
    }
}