using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Create_your_Adventure.Source.GameLogic.Chunk
{
    /// <summary>
    /// Represents a cubic chunk of blocks in the voxel world.
    /// Each chunk contains a fixed number of blocks (16x16x16) and manages
    /// its own instance matrices for GPU-instanced rendering.
    /// </summary>
    /// <remarks>
    /// Chunks are the fundamental unit of world subdivision. They enable:
    /// <list type="bullet">
    ///   <item>Efficient memory management by loading/unloading regions</item>
    ///   <item>Instanced rendering for performance optimization</item>
    ///   <item>Spatial partitioning for physics and culling</item>
    /// </list>
    /// </remarks>
    public class Chunk
    {
        // -------- Chunk Constants --------

        /// <summary>
        /// The size of a chunk along each axis (16 blocks per side).
        /// </summary>
        public const int ChunkSize = 16;

        /// <summary>
        /// The total number of blocks in a chunk (16 × 16 × 16 = 4096).
        /// </summary>
        public const int BlockCount = ChunkSize * ChunkSize * ChunkSize;

        // -------- Chunk Position --------

        /// <summary>
        /// Gets the position of this chunk in chunk-space coordinates.
        /// Multiply by <see cref="ChunkSize"/> to get the world-space origin.
        /// </summary>
        /// <example>
        /// ChunkPosition (1, 0, 2) corresponds to world origin (16, 0, 32).
        /// </example>
        public Vector3D<int> ChunkPosition { get; }

        // -------- Instancing Data (for Rendering) --------

        /// <summary>
        /// Gets the transformation matrices for all block instances in this chunk.
        /// Each matrix represents the world-space transform of a single block.
        /// </summary>
        /// <remarks>
        /// Used for GPU instanced rendering to draw all blocks in a single draw call.
        /// </remarks>
        public Matrix4X4<float>[] InstanceMatrices { get; private set; }

        /// <summary>
        /// Gets the number of block instances to render.
        /// </summary>
        public int InstanceCount { get; private set; }

        // CONSTRUCTION ----------------------------------------------------------------

        /// <summary>
        /// Initializes a new chunk at the specified chunk-space position.
        /// </summary>
        /// <param name="chunkPosition">
        /// The position in chunk coordinates (not world coordinates).
        /// For example, (1, 0, 0) represents the chunk starting at world position (16, 0, 0).
        /// </param>
        public Chunk(Vector3D<int> chunkPosition)
        {
            ChunkPosition = chunkPosition;
            RebuildInsanceData();


            Console.WriteLine($"[INFO] Create Chunk on ChunkPos: {chunkPosition}");
            Console.WriteLine($"[INFO] World-Area: ({chunkPosition.X * ChunkSize}, {chunkPosition.Y * ChunkSize}, {chunkPosition.Z * ChunkSize}) bis ({(chunkPosition.X + 1) * ChunkSize - 1}, {(chunkPosition.Y + 1) * ChunkSize - 1}, {(chunkPosition.Z + 1) * ChunkSize - 1})");

        }

        // INSTANCE DATA GENERATION ----------------------------------------------------------------

        /// <summary>
        /// Rebuilds the instance matrices for all blocks in this chunk.
        /// Call this method after modifying block data to update the GPU buffer.
        /// </summary>
        /// <remarks>
        /// Iterates through all block positions (x, y, z) and creates a translation
        /// matrix for each block's world position. Future optimization: skip air blocks.
        /// </remarks>
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

        /// <summary>
        /// Converts a local block position within this chunk to world coordinates.
        /// </summary>
        /// <param name="localPosition">
        /// The position relative to the chunk origin (0-15 per axis).
        /// </param>
        /// <returns>
        /// The corresponding position in world-space coordinates.
        /// </returns>
        /// <example>
        /// For a chunk at (1, 0, 0), LocalToWorld((5, 3, 2)) returns (21, 3, 2).
        /// </example>
        public Vector3D<float> LocalToWorld(Vector3D<int> localPosition)
        {
            return new Vector3D<float>(
                ChunkPosition.X * ChunkSize + localPosition.X,
                ChunkPosition.Y * ChunkSize + localPosition.Y,
                ChunkPosition.Z * ChunkSize + localPosition.Z
            );
        }

        /// <summary>
        /// Determines which chunk contains the given world position.
        /// </summary>
        /// <param name="worldPosition">A position in world-space coordinates.</param>
        /// <returns>
        /// The chunk-space coordinates of the chunk containing this position.
        /// </returns>
        /// <remarks>
        /// Uses floor division to correctly handle negative coordinates.
        /// </remarks>
        private static Vector3D<int> WorldToChunkPosition(Vector3D<float> worldPosition)
        {
            return new Vector3D<int>(
                (int)MathF.Floor(worldPosition.X / ChunkSize),
                (int)MathF.Floor(worldPosition.Y / ChunkSize),
                (int)MathF.Floor(worldPosition.Z / ChunkSize)
            );
        }

        /// <summary>
        /// Converts a world position to local chunk coordinates (0-15 per axis).
        /// </summary>
        /// <param name="worldPosition">A position in world-space coordinates.</param>
        /// <returns>
        /// The local position within the containing chunk.
        /// </returns>
        /// <remarks>
        /// Uses modulo with offset to correctly handle negative world positions.
        /// Formula: ((floor(pos) % size) + size) % size ensures positive results.
        /// </remarks>
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
