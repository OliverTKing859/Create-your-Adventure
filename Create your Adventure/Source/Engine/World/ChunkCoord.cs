using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.World
{
    public readonly struct ChunkCoord : IEquatable<ChunkCoord>
    {
        public readonly long X;
        public readonly long Y;
        public readonly long Z;

        public ChunkCoord(long x, long y, long z) => (X, Y, Z) = (x, y, z);

        // ═══ Squared Distance für Performance (kein sqrt)
        public long DistanceSquaredTo(ChunkCoord other)
        {
            long dx = X - other.X;
            long dy = Y - other.Y;
            long dz = Z - other.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        public bool Equals(ChunkCoord other) => X == other.X && Y == other.Y && Z == other.Z;
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    }

    public enum ChunkState : byte
    {
        Requested,
        Loading,
        Meshing,
        Active,
        Sleeping,
        Serializing,
        Unloaded
    }

    public enum ChunkPriority : byte
    {
        Critical = 0,   // ═══ Player steht drin
        High = 1,       // ═══ Sichtbar & nah
        Medium = 2,     // ═══ Sichtbar aber fern
        Low = 3,        // ═══ Simulationsbereich
        Background = 4  // ═══ Lazy/Sleeping
    }

    public class ChunkJob
    {
        public ChunkCoord Coord { get; }
        public ChunkState State { get; set; }
        public ChunkPriority Priority { get; private set; }

        // ═══ Simulation
        public int TickRate { get; private set; } = 60; // ═══ Ticks per second
        public ulong LastTickFrame { get; set; }

        // ═══ Visibility
        public bool IsInFrustum { get; set; }
        public bool HasPendingChanges { get; set; }

        // ═══ Metadata for Lazy Simulation
        public ChunkMetadata? Metadata { get; set; }
        public ChunkJob(ChunkCoord coord) => Coord = coord;

        public void UpdatePrivority(ChunkCoord cameraChunk, bool isPlayerInside)
        {
            if (isPlayerInside)
            {
                Priority = ChunkPriority.Critical;
                TickRate = 60;
                return;
            }

            long distSq = Coord.DistanceSquaredTo(cameraChunk);

            Priority = distSq switch
            {
                < 4 when IsInFrustum => ChunkPriority.High,     // ═══ < 2 Chunks
                < 16 when IsInFrustum => ChunkPriority.Medium,  // ═══ < 4 Chunks
                < 64 => ChunkPriority.Low,                      // ═══ < 8 Chunks
                _ => ChunkPriority.Background
            };

            TickRate = Priority switch
            {
                ChunkPriority.Critical => 60,
                ChunkPriority.High => 30,
                ChunkPriority.Medium => 10,
                ChunkPriority.Low => 2,
                ChunkPriority.Background => 1,
                _ => 1
            };
        }
    }

    public class ChunkMetadata
    {
        public bool HasWater { get; set; }
        public bool HasActiveEntities { get; set; }
        public byte BiomeId { get; set; }
        public float AverageTemperature { get; set; }
        public ulong LastModifiedTick { get; set; }
    }
}
