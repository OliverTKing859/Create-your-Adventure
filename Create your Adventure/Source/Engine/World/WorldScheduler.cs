using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Create_your_Adventure.Source.Engine.World
{
    internal class WorldScheduler
    {
        private static WorldScheduler? instance;
        public static WorldScheduler Instance => instance ??= new WorldScheduler();

        // ══════════════════════════════════════════════════
        // BUDGET CONFIGURATION (in Milliseconds pro frame)
        // ══════════════════════════════════════════════════
        public double TotalBudgetMs { get; set; } = 8.0; // ═══ ~8ms bei 60 FPS Target

        // ═══ Budget - Distribution (Sum = 1.0)
        private readonly Dictionary<SchedularTask, double> budgetWeights = new()
        {
            { SchedularTask.ChunkLoading, 0.30 },
            { SchedularTask.ChunkMeshing, 0.35 },
            { SchedularTask.BlockUpdates, 0.20 },
            { SchedularTask.Lightning, 0.10 },
            { SchedularTask.EntitySimulation, 0.05 }
        };

        // ══════════════════════════════════════════════════
        // QUEUES
        // ══════════════════════════════════════════════════
        private readonly PriorityQueue<ChunkJob, ChunkPriority> loadQueue = new();
        private readonly PriorityQueue<ChunkJob, ChunkPriority> meshQueue = new();
        private readonly PriorityQueue<ChunkJob, ChunkPriority> simulation = new();

        // ═══ Telemetry
        public double LastFrameBudgetUsedMs { get; private set; }
        public Dictionary<SchedularTask, double> LastTaskTimings { get; } = new();

        private readonly Stopwatch stopwatch = new();

        // ══════════════════════════════════════════════════
        // MAIN SCHEDULAR TICK
        // ══════════════════════════════════════════════════
        public void ProcessFrame()
        {
            stopwatch.Restart();
            double remainingBudgetMs = TotalBudgetMs;

            foreach (var task in budgetWeights.Keys)
            {
                double taskBudgetMs = TotalBudgetMs * budgetWeights[task];
                double taskStartMs = stopwatch.Elapsed.TotalMilliseconds;

                ProcessTask(task, taskBudgetMs);

                double taskUsedMs = stopwatch.Elapsed.TotalMilliseconds - taskStartMs;
                LastTaskTimings[task] = taskBudgetMs;
                remainingBudgetMs -= taskUsedMs;

                // ═══ Early exit when Budget exhausted
                if (remainingBudgetMs <= 0.5) break;
            }

            LastFrameBudgetUsedMs = stopwatch.Elapsed.TotalMilliseconds;
        }

        private void ProcessTask(SchedularTask task, double budgetMs)
        {
            var deadline = stopwatch.Elapsed.TotalMilliseconds + budgetMs;

            switch (task)
            {
                case SchedularTask.ChunkLoading:
                    while (loadQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds < deadline)
                    {
                        var chunk = loadQueue.Dequeue();
                        ProcessChunkLoad(chunk);
                    }
                    break;

                case SchedularTask.ChunkMeshing:
                    while (meshQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds < deadline)
                    {
                        var chunk = meshQueue.Dequeue();
                        ProcessChunkMesh(chunk);
                    }
                    break;

                case SchedularTask.BlockUpdates:
                    ProcessBlockUpdatesWithBudget(deadline);
                    break;

                case SchedularTask.Lightning:
                    ProcessLightningWithBudget(deadline);
                    break;

                case SchedularTask.EntitySimulation:
                    ProcessEntitiesWithBudget(deadline);
                    break;
            }
        }

        // ══════════════════════════════════════════════════
        // CHUNK PROCESSING
        // ══════════════════════════════════════════════════
        private void ProcessChunkLoad(ChunkJob chunk)
        {
            // ═══ Here: (start WorldGen or Disk-Load)
            chunk.State = ChunkState.Loading;
            // ═══ After completion → Add to mesh queue
        }

        private void ProcessChunkMesh(ChunkJob chunk)
        {
            // ═══ Mesh generation (can also be asynchronous)
            chunk.State = ChunkState.Meshing;
        }

        private void ProcessBlockUpdatesWithBudget(double deadlineMs)
        {
            // ═══ Block-Updates with time limit
        }

        private void ProcessLightningWithBudget(double deadlineMs)
        {
            // ═══ Lighting propagation with time limit
        }

        private void ProcessEntitiesWithBudget(double deadlineMs)
        {
            // Entity-Simulation
        }

        // ══════════════════════════════════════════════════
        // PUBLIC API
        // ══════════════════════════════════════════════════
        public void RequestChunkLoad(ChunkJob chunk)
        {
            chunk.State = ChunkState.Requested;
            loadQueue.Enqueue(chunk, chunk.Priority);
        }

        private void RequestChunkMesh(ChunkJob chunk)
        {
            meshQueue.Enqueue(chunk, chunk.Priority);
        }

        public void RequestSimulation(ChunkJob chunk)
        {
            simulationQueue.Enqueue(chunk, chunk.Priority);
        }
    }

    public enum SchedularTask
    {
        ChunkLoading,
        ChunkMeshing,
        BlockUpdates,
        Lightning,
        EntitySimulation
    }
}
