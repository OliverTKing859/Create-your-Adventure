using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Camera
{
    public class CameraWorldBinding
    {
        // ══════════════════════════════════════════════════
        // CONSTANTS
        // ══════════════════════════════════════════════════
        public const int ChunkSize = 16;
        public const float OriginShiftThreshold = 256f; // ═══ 16 Chunks

        // ══════════════════════════════════════════════════
        // WORLD ANCHOR
        // ══════════════════════════════════════════════════
        public ChunkCoord OriginChunk { get; private set; }
        public ChunkCoord CurrentChunk { get; private set; }
        public Vector3D<float> LocalOffset { get; private set; }

        // ══════════════════════════════════════════════════
        // EVENTS (for systems that need to react to origin shifts)
        // ══════════════════════════════════════════════════
        public event Action<ChunkCoord, ChunkCoord>? OriginShifted;
        public event Action<ChunkCoord>? ChunkChanged;

        // ══════════════════════════════════════════════════
        // CONTRUCTOR
        // ══════════════════════════════════════════════════
        public CameraWorldBinding()
        {
            OriginChunk = new ChunkCoord(0, 0, 0);
            CurrentChunk = new ChunkCoord(0, 0, 0);
            LocalOffset = Vector3D<float>.Zero;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        public void Initialize(long worldX, long worldY, long worldZ)
        {
            var (chunk, offset) = WorldToChunkLocal(worldX, worldY, worldZ);
            OriginChunk = chunk;
            CurrentChunk = chunk;
            LocalOffset = offset;
        }

        // ══════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════
        public Vector3D<float> UpdateFromLocalPosition(Vector3D<float> localPosition)
        {
            // ═══ Compute absolute world position
            var worldPos = LocalToWorld(localPosition);

            // ═══ Determine which chunk we're in
            var newChunk = new ChunkCoord(
                FloorDivide((long)MathF.Floor((float)worldPos.X), ChunkSize),
                FloorDivide((long)MathF.Floor((float)worldPos.Y), ChunkSize),
                FloorDivide((long)MathF.Floor((float)worldPos.Z), ChunkSize)
            );

            // ═══ Check for chunk change
            if (!newChunk.Equals(CurrentChunk))
            {
                CurrentChunk = newChunk;
                ChunkChanged?.Invoke(newChunk);
            }

            // ═══ Compute local offset within chunk
            LocalOffset = new Vector3D<float>(
                Mod((float)worldPos.X, ChunkSize),
                Mod((float)worldPos.Y, ChunkSize),
                Mod((float)worldPos.Z, ChunkSize)
            );

            var correctedPosition = CheckOriginShift(localPosition, worldPos);

            return correctedPosition;
        }

        private Vector3D<float> CheckOriginShift(Vector3D<float> localPosition, Vector3D<double> worldPos)
        {
            float distanceFromOrigin = localPosition.Length;

            if (distanceFromOrigin > OriginShiftThreshold)
            {
                var oldOrigin = OriginChunk;

                // ═══ Shift origin to current chunk
                OriginChunk = CurrentChunk;

                // ═══════════════════════════════════════════════════════════
                // CRITICAL: Calculate new LocalPosition relative to new Origin!
                // ═══════════════════════════════════════════════════════════
                var correctedPosition = new Vector3D<float>(
                    (float)(worldPos.X - OriginChunk.X * ChunkSize),
                    (float)(worldPos.Y - OriginChunk.Y * ChunkSize),
                    (float)(worldPos.Z - OriginChunk.Z * ChunkSize)
                );

                // ═══ Notify listeners about the shift
                OriginShifted?.Invoke(oldOrigin, OriginChunk);

                return correctedPosition;
            }

            return localPosition;
        }

        // ══════════════════════════════════════════════════
        // COORDINATE CONVERSION
        // ══════════════════════════════════════════════════
        public Vector3D<double> LocalToWorld(Vector3D<float> localPosition)
        {
            return new Vector3D<double>(
                OriginChunk.X * ChunkSize + localPosition.X,
                OriginChunk.Y * ChunkSize + localPosition.Y,
                OriginChunk.Z * ChunkSize + localPosition.Z
            );
        }

        public Vector3D<float> WorldToLocal(long worldX, long worldY, long worldZ)
        {
            return new Vector3D<float>(
                (float)(worldX - OriginChunk.X * ChunkSize),
                (float)(worldY - OriginChunk.Y * ChunkSize),
                (float)(worldZ - OriginChunk.Z * ChunkSize)
            );
        }

        public static (ChunkCoord chunk, Vector3D<float> offset) WorldToChunkLocal(long x, long y, long z)
        {
            var chunk = new ChunkCoord(
                FloorDivide(x, ChunkSize),
                FloorDivide(y, ChunkSize),
                FloorDivide(z, ChunkSize)
            );

            var offset = new Vector3D<float>(
                Mod(x, ChunkSize),
                Mod(y, ChunkSize),
                Mod(z, ChunkSize)
            );

            return (chunk, offset);
        }

        public Vector3D<float> GetChunkLocalPosition(ChunkCoord chunk)
        {
            return new Vector3D<float>(
                (chunk.X - OriginChunk.X) * ChunkSize,
                (chunk.Y - OriginChunk.Y) * ChunkSize,
                (chunk.Z - OriginChunk.Z) * ChunkSize
            );
        }

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════
        private static long FloorDivide(long a, int b)
        {
            return a >= 0 ? a / b : (a - b + 1) / b;
        }

        private static float Mod(float a, int b)
        {
            float result = a % b;
            return result < 0 ? result + b : result;
        }

        private static float Mod(long a, int b)
        {
            long result = a % b;
            return result < 0 ? result + b : result;
        }
    }
}