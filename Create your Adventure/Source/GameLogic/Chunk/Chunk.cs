using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Create_your_Adventure.Source.GameLogic.Chunk
{
    public class Chunk
    {
        // -------- Chunk Constants --------
        public const int ChunkSize = 16;
        public const int BlockCount = ChunkSize * ChunkSize * ChunkSize;

        // -------- Chunk Position --------
        public Vector3D<int> ChunkPosition { get; }

        // -------- Instancing Data (for Rendering) --------
        public Matrix4X4<float>[] InstanceMatrices { get; private set; }
        public int InstanceCount { get; private set; }

        // CONSTRUCTION ----------------------------------------------------------------
        public Chunk(Vector3D<int> chunkPosition)
        {
            ChunkPosition = chunkPosition;
            RebuildInsanceData();


            Console.WriteLine($"[INFO] Create Chunk on ChunkPos: {chunkPosition}");
            Console.WriteLine($"[INFO] World-Area: ({chunkPosition.X * ChunkSize}, {chunkPosition.Y * ChunkSize}, {chunkPosition.Z * ChunkSize}) bis ({(chunkPosition.X + 1) * ChunkSize - 1}, {(chunkPosition.Y + 1) * ChunkSize - 1}, {(chunkPosition.Z + 1) * ChunkSize - 1})");

        }

        // INSTANCE DATA GENERATION ----------------------------------------------------------------
        public void RebuildInsanceData()
        {
            InstanceCount = BlockCount;
            InstanceMatrices = new Matrix4X4<float>[InstanceCount];

            int index = 0;
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        Vector3D<int> localPosition = new(x, y, z);
                        Vector3D<float> worldPosition = LocalToWorld(localPosition);

                        InstanceMatrices[index] = Matrix4X4.CreateTranslation<float>(worldPosition);
                        index++;
                    }
                }
            }
        }

        // COORDINATE CONVERSION ----------------------------------------------------------------
        public Vector3D<float> LocalToWorld(Vector3D<int> localPosition)
        {
            return new Vector3D<float>(
                ChunkPosition.X * ChunkSize + localPosition.X,
                ChunkPosition.Y * ChunkSize + localPosition.Y,
                ChunkPosition.Z * ChunkSize + localPosition.Z
            );
        }

        private static Vector3D<int> WorldToChunkPosition(Vector3D<float> worldPosition)
        {
            return new Vector3D<int>(
                (int)MathF.Floor(worldPosition.X / ChunkSize),
                (int)MathF.Floor(worldPosition.Y / ChunkSize),
                (int)MathF.Floor(worldPosition.Z / ChunkSize)
            );
        }

        private static Vector3D<int> WorldTolocal(Vector3D<float> worldPosition)
        {
            return new Vector3D<int>(
                ((int)MathF.Floor(worldPosition.X) % ChunkSize + ChunkSize) % ChunkSize,
                ((int)MathF.Floor(worldPosition.Y) % ChunkSize + ChunkSize) % ChunkSize,
                ((int)MathF.Floor(worldPosition.Z) % ChunkSize + ChunkSize) % ChunkSize
            );
        }
    }
}
