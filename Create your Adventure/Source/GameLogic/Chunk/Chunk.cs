using Create_your_Adventure.Source.Engine.Debug;
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
    ///   <item>Infinite world generation using 64-bit chunk coordinates</item>
    /// </list>
    /// 
    /// ARCHITECTURE NOTES:
    /// - Chunk positions use `long` for virtually unlimited world size (±9.2 quintillion chunks)
    /// - Local block positions use `int` (0-15 range)
    /// - Rendering uses `float` with world offset to avoid precision loss
    /// 
    /// FUTURE OPTIMIZATIONS (TODO):
    /// - Block data storage (currently all blocks are rendered)
    /// - Visibility culling (only non-air blocks)
    /// - Face culling (don't render faces between solid blocks)
    /// - Greedy meshing (combine adjacent faces into larger quads)
    /// - Separate chunk data from rendering logic (chunk should be pure data container)
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
        /// Gets the position of this chunk in chunk-space coordinates (64-bit for infinite worlds).
        /// Multiply by <see cref="ChunkSize"/> to get the world-space origin.
        /// </summary>
        /// <remarks>
        /// Uses `long` instead of `int` to enable truly infinite world generation.
        /// This allows for approximately ±147 trillion blocks in each direction
        /// (±9.2 quintillion chunks × 16 blocks per chunk).
        /// </remarks>
        /// <example>
        /// ChunkPosition (1, 0, 2) corresponds to world origin (16, 0, 32).
        /// </example>
        public Vector3D<long> ChunkPosition { get; }

        // -------- Instancing Data (for Rendering) --------

        /// <summary>
        /// Gets the transformation matrices for all block instances in this chunk.
        /// Each matrix represents the world-space transform of a single block.
        /// </summary>
        /// <remarks>
        /// CURRENT: All blocks are rendered (temporary for early development).
        /// TODO: Only generate matrices for visible, non-air blocks.
        /// TODO: Implement face culling and meshing for performance.
        /// TODO: Use world offset in rendering to maintain float precision.
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
        /// The position in chunk coordinates (not world coordinates) using 64-bit precision.
        /// For example, (1, 0, 0) represents the chunk starting at world position (16, 0, 0).
        /// </param>
        public Chunk(Vector3D<long> chunkPosition)
        {
            ChunkPosition = chunkPosition;

            Logger.Info($"[CHUNK] Creating chunk at position: {chunkPosition}");

            RebuildInsanceData();

            var worldMin = new Vector3D<long>(
                chunkPosition.X * ChunkSize,
                chunkPosition.Y * ChunkSize,
                chunkPosition.Z * ChunkSize
            );

            var worldMax = new Vector3D<long>(
                (chunkPosition.X + 1) * ChunkSize - 1,
                (chunkPosition.Y + 1) * ChunkSize - 1,
                (chunkPosition.Z + 1) * ChunkSize - 1
            );

            Logger.Info($"[CHUNK] World area: ({worldMin.X}, {worldMin.Y}, {worldMin.Z}) to ({worldMax.X}, {worldMax.Y}, {worldMax.Z})");
            Logger.Info($"[CHUNK] Generated {InstanceCount} block instances");

        }

        // INSTANCE DATA GENERATION ----------------------------------------------------------------

        /// <summary>
        /// Rebuilds the instance matrices for all blocks in this chunk.
        /// Call this method after modifying block data to update the GPU buffer.
        /// </summary>
        /// <remarks>
        /// CURRENT: Iterates through all block positions and creates matrices.
        /// TODO: Only generate matrices for visible blocks (not air).
        /// TODO: Implement face culling - don't render hidden faces.
        /// TODO: Implement greedy meshing - combine adjacent faces.
        /// TODO: Apply world offset for rendering to maintain float precision at far distances.
        /// </remarks>
        public void RebuildInsanceData()
        {
            Logger.Info($"[CHUNK] Rebuilding instance data for chunk at {ChunkPosition}");

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

            Logger.Info($"[CHUNK] Instance data rebuilt successfully ({InstanceCount} instances)");
        }

        // COORDINATE CONVERSION ----------------------------------------------------------------

        /// <summary>
        /// Converts a local block position within this chunk to world coordinates.
        /// </summary>
        /// <param name="localPosition">
        /// The position relative to the chunk origin (0-15 per axis).
        /// </param>
        /// <returns>
        /// The corresponding position in world-space coordinates as float for rendering.
        /// </returns>
        /// <remarks>
        /// Converts from internal integer coordinates to float for rendering.
        /// TODO: Apply camera-relative offset to avoid floating-point precision issues at far distances.
        /// </remarks>
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
        /// The chunk-space coordinates (64-bit) of the chunk containing this position.
        /// </returns>
        /// <remarks>
        /// Uses floor division to correctly handle negative coordinates.
        /// Converts from rendering float coordinates to internal long chunk coordinates.
        /// </remarks>
        public static Vector3D<long> WorldToChunkPosition(Vector3D<float> worldPosition)
        {
            return new Vector3D<long>(
                (long)MathF.Floor(worldPosition.X / ChunkSize),
                (long)MathF.Floor(worldPosition.Y / ChunkSize),
                (long)MathF.Floor(worldPosition.Z / ChunkSize)
            );
        }

        /// <summary>
        /// Converts a world position to local chunk coordinates (0-15 per axis).
        /// </summary>
        /// <param name="worldPosition">A position in world-space coordinates.</param>
        /// <returns>
        /// The local position within the containing chunk (0-15 range per axis).
        /// </returns>
        /// <remarks>
        /// Uses modulo with offset to correctly handle negative world positions.
        /// Formula: ((floor(pos) % size) + size) % size ensures positive results.
        /// Converts from rendering float coordinates to internal int local coordinates.
        /// </remarks>
        public static Vector3D<int> WorldToLocal(Vector3D<float> worldPosition)
        {
            return new Vector3D<int>(
                ((int)MathF.Floor(worldPosition.X) % ChunkSize + ChunkSize) % ChunkSize,
                ((int)MathF.Floor(worldPosition.Y) % ChunkSize + ChunkSize) % ChunkSize,
                ((int)MathF.Floor(worldPosition.Z) % ChunkSize + ChunkSize) % ChunkSize
            );
        }

        // TODO: Future method for rendering with world offset
        // public Vector3D<float> LocalToWorldRelative(Vector3D<int> localPosition, Vector3D<long> cameraChunkPosition)
        // {
        //     // Calculate position relative to camera chunk to avoid float precision loss
        //     long relativeChunkX = ChunkPosition.X - cameraChunkPosition.X;
        //     long relativeChunkY = ChunkPosition.Y - cameraChunkPosition.Y;
        //     long relativeChunkZ = ChunkPosition.Z - cameraChunkPosition.Z;
        //     
        //     return new Vector3D<float>(
        //         relativeChunkX * ChunkSize + localPosition.X,
        //         relativeChunkY * ChunkSize + localPosition.Y,
        //         relativeChunkZ * ChunkSize + localPosition.Z
        //     );
        // }
    }
}
