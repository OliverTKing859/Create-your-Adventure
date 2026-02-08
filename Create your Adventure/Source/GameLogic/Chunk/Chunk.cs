using Create_your_Adventure.Source.Engine.DevDebug;
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
    /// - Rendering uses camera-relative coordinates to prevent float precision loss (no Farlands!)
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
        /// Each matrix represents chunk-local transforms (0-15 range).
        /// The chunk offset must be applied during rendering via view matrix.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: These matrices contain LOCAL positions only (0-15 range).
        /// The renderer must apply chunk offset relative to camera to prevent float jittering.
        /// TODO: Only generate matrices for visible, non-air blocks.
        /// TODO: Implement face culling and meshing for performance.
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
        /// Generates LOCAL transformation matrices (0-15 range) to prevent float precision loss.
        /// The chunk's world offset must be applied during rendering via the view matrix.
        /// This approach eliminates Farlands and float jittering at large distances.
        /// TODO: Only generate matrices for visible blocks (not air).
        /// TODO: Implement face culling - don't render hidden faces.
        /// TODO: Implement greedy meshing - combine adjacent faces.
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
                        // Store LOCAL positions only (0-15 range) to maintain float precision
                        Vector3D<float> localPosition = new(x, y, z);

                        InstanceMatrices[index] = Matrix4X4.CreateTranslation(localPosition);
                        index++;
                    }
                }
            }

            Logger.Info($"[CHUNK] Instance data rebuilt successfully ({InstanceCount} instances with local transforms)");
        }

        // COORDINATE CONVERSION ----------------------------------------------------------------

        /// <summary>
        /// Converts a local block position within this chunk to absolute world coordinates (64-bit precision).
        /// </summary>
        /// <param name="localPosition">
        /// The position relative to the chunk origin (0-15 per axis).
        /// </param>
        /// <returns>
        /// The corresponding position in world-space coordinates using 64-bit integers.
        /// </returns>
        /// <remarks>
        /// Returns `long` coordinates to maintain precision at any distance.
        /// Use <see cref="LocalToWorldRelative"/> for rendering to avoid float precision loss.
        /// </remarks>
        /// <example>
        /// For a chunk at (1, 0, 0), LocalToWorldAbsolute((5, 3, 2)) returns (21, 3, 2).
        /// </example>
        public Vector3D<long> LocalToWorldAbsolute(Vector3D<int> localPosition)
        {
            return new Vector3D<long>(
                ChunkPosition.X * ChunkSize + localPosition.X,
                ChunkPosition.Y * ChunkSize + localPosition.Y,
                ChunkPosition.Z * ChunkSize + localPosition.Z
            );
        }

        /// <summary>
        /// Converts a local block position to world coordinates relative to a reference chunk.
        /// This prevents float precision loss when rendering at large distances.
        /// </summary>
        /// <param name="localPosition">
        /// The position relative to the chunk origin (0-15 per axis).
        /// </param>
        /// <param name="referenceChunkPosition">
        /// The reference chunk position (typically the camera's chunk) to calculate relative offset.
        /// </param>
        /// <returns>
        /// The position relative to the reference chunk as float, safe for rendering.
        /// </returns>
        /// <remarks>
        /// This method prevents Farlands and float jittering by keeping coordinates camera-relative.
        /// Even at extreme distances (billions of blocks), float precision remains sufficient
        /// because we only represent the small distance between chunks, not absolute positions.
        /// 
        /// RENDERING APPROACH:
        /// 1. Determine camera's current chunk position
        /// 2. For each chunk, calculate its offset relative to camera chunk
        /// 3. Apply this offset to the chunk's local instance matrices
        /// 4. Result: All rendering happens in camera-relative space with high precision
        /// </remarks>
        /// <example>
        /// Camera at chunk (1000000, 0, 0), rendering chunk (1000002, 0, 0):
        /// - Relative offset = (32, 0, 0) - perfectly precise as float
        /// - Without this: absolute position (16000032, 0, 0) would lose precision
        /// </example>
        public Vector3D<float> LocalToWorldRelative(Vector3D<int> localPosition, Vector3D<long> referenceChunkPosition)
        {
            // Calculate chunk offset relative to reference (stays small even at huge distances)
            long relativeChunkX = ChunkPosition.X - referenceChunkPosition.X;
            long relativeChunkY = ChunkPosition.Y - referenceChunkPosition.Y;
            long relativeChunkZ = ChunkPosition.Z - referenceChunkPosition.Z;

            // Combine relative chunk offset with local block position
            return new Vector3D<float>(
                relativeChunkX * ChunkSize + localPosition.X,
                relativeChunkY * ChunkSize + localPosition.Y,
                relativeChunkZ * ChunkSize + localPosition.Z
            );
        }

        /// <summary>
        /// Gets the world offset of this chunk relative to a reference chunk position.
        /// Use this to offset the chunk's local instance matrices during rendering.
        /// </summary>
        /// <param name="referenceChunkPosition">
        /// The reference chunk position (typically the camera's chunk).
        /// </param>
        /// <returns>
        /// The translation offset to apply to this chunk's instance matrices.
        /// </returns>
        /// <remarks>
        /// USAGE IN RENDERER:
        /// <code>
        /// var cameraChunk = Chunk.WorldToChunkPosition(cameraPosition);
        /// var chunkOffset = chunk.GetRenderOffset(cameraChunk);
        /// 
        /// // Apply offset to model matrix or view matrix before rendering
        /// var chunkTransform = Matrix4X4.CreateTranslation(chunkOffset);
        /// </code>
        /// </remarks>
        public Vector3D<float> GetRenderOffset(Vector3D<long> referenceChunkPosition)
        {
            long relativeChunkX = ChunkPosition.X - referenceChunkPosition.X;
            long relativeChunkY = ChunkPosition.Y - referenceChunkPosition.Y;
            long relativeChunkZ = ChunkPosition.Z - referenceChunkPosition.Z;

            return new Vector3D<float>(
                relativeChunkX * ChunkSize,
                relativeChunkY * ChunkSize,
                relativeChunkZ * ChunkSize
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
        /// Overload for 64-bit world positions (e.g., from block coordinates).
        /// </summary>
        public static Vector3D<long> WorldToChunkPosition(Vector3D<long> worldPosition)
        {
            return new Vector3D<long>(
                worldPosition.X >= 0 ? worldPosition.X / ChunkSize : (worldPosition.X - ChunkSize + 1) / ChunkSize,
                worldPosition.Y >= 0 ? worldPosition.Y / ChunkSize : (worldPosition.Y - ChunkSize + 1) / ChunkSize,
                worldPosition.Z >= 0 ? worldPosition.Z / ChunkSize : (worldPosition.Z - ChunkSize + 1) / ChunkSize
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

        /// <summary>
        /// Overload for 64-bit world positions.
        /// </summary>
        public static Vector3D<int> WorldToLocal(Vector3D<long> worldPosition)
        {
            return new Vector3D<int>(
                (int)((worldPosition.X % ChunkSize + ChunkSize) % ChunkSize),
                (int)((worldPosition.Y % ChunkSize + ChunkSize) % ChunkSize),
                (int)((worldPosition.Z % ChunkSize + ChunkSize) % ChunkSize)
            );
        }
    }
}
