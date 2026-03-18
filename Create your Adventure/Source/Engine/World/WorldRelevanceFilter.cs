using Create_your_Adventure.Source.Engine.Camera;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.World
{
    public sealed class WorldRelevanceFilter
    {
        // ══════════════════════════════════════════════════
        // DISTANCES (in Chunks)
        // ══════════════════════════════════════════════════
        public int RenderDistance { get; set; } = 16;       // ═══ What will be render
        public int SimulationDistance { get; set; } = 24;   // ═══ What will be simulation
        public int LoadDistance { get; set; } = 32;         // ═══ What will be loaded
        public int UnloadDistance { get; set; } = 40;       // ═══ What will be unloaded

        // ══════════════════════════════════════════════════
        // CACHED VISIBILITY DATA
        // ══════════════════════════════════════════════════
        private CameraVisibilityContext visibility;
        private ChunkCoord cameraChunk;
        private ViewFrustum frustum;

        // ══════════════════════════════════════════════════
        // UPDATE (from CameraManager output)
        // ══════════════════════════════════════════════════
        public void UpdateFromCamera(CameraVisibilityContext visibilityContext)
        {
            visibility = visibilityContext;
            cameraChunk = visibilityContext.CameraChunk;
            frustum = visibility.Frustum;

            // ═══ Override render distance from camera if provided
            if (visibilityContext.RenderDistanceChunks > 0)
            {
                RenderDistance = visibility.RenderDistanceChunks;
            }
        }

        // ══════════════════════════════════════════════════
        // RELEVANZ-CHECKS
        // ══════════════════════════════════════════════════
        public bool ShouldRender(ChunkJob chunk)
        {
            if (!IsWithinDistance(chunk.Coord, RenderDistance))
                return false;

            // ═══ Frustum Culling
            var aabb = GetChunkAABB(chunk.Coord);
            return frustum.Intersects(aabb);
        }

        public bool ShouldSimulate(ChunkJob chunk)
        {
            // ═══ Chunks with active Entitys always simulated
            if (chunk.Metadata?.HasActiveEntities == true)
                return true;

            // ═══ Chunks with Water in Simulationsarea
            if (chunk.Metadata?.HasWater == true &&
                IsWithinDistance(chunk.Coord, SimulationDistance))
                return true;

            return IsWithinDistance(chunk.Coord, SimulationDistance);
        }

        public bool ShouldLoad(ChunkCoord coord)
        {
            return IsWithinDistance(coord, LoadDistance);
        }

        public bool ShouldUnload(ChunkJob chunk)
        {
            // ═══ Never discharge when active entities
            if (chunk.Metadata?.HasActiveEntities == true)
                return false;

            return !IsWithinDistance(chunk.Coord, UnloadDistance);
        }
        // ══════════════════════════════════════════════════
        // PRIORITY CALCULATION
        // ══════════════════════════════════════════════════
        public void UpdateChunkPriority(ChunkJob chunk, bool isPlayerInside)
        {
            // ═══ Update frustum visibility
            var aabb = GetChunkAABB(chunk.Coord);
            chunk.IsInFrustum = frustum.Intersects(aabb);

            // ═══ Delegate to ChunkJob's priority logic
            chunk.UpdatePriority(cameraChunk, isPlayerInside);
        }

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════
        private bool IsWithinDistance(ChunkCoord coord, int distanceChunks)
        {
            long distSq = coord.DistanceSquaredTo(cameraChunk);
            return distSq <= (long)distanceChunks * distanceChunks;
        }

        private Box3D<float> GetChunkAABB(ChunkCoord coord)
        {
            // ═══ Position relative to camera origin for float precision
            var localPos = new Vector3D<float>(
                (coord.X - visibility.CameraChunk.X) * 16f,
                (coord.Y - visibility.CameraChunk.Y) * 16f,
                (coord.Z - visibility.CameraChunk.Z) * 16f
            );

            return new Box3D<float>(
                new Vector3D<float>(coord.X * 16f, coord.Y * 16f, coord.Z * 16f),
                new Vector3D<float>((coord.X + 1) * 16f, (coord.Y + 1) * 16f, (coord.Z + 1) * 16f)
            );
        }
    }
}
