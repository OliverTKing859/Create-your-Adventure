using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.World
{
    public sealed class WorldRelevanceFilter
    {
        // ══════════════════════════════════════════════════
        // PUBLIC API
        // ══════════════════════════════════════════════════
        public int renderDistance { get; set; } = 16;       // ═══ What will be render
        public int SimulationDistance { get; set; } = 24;   // ═══ What will be simulation
        public int LoadDistance { get; set; } = 32;         // ═══ What will be loaded
        public int UnloadDistance { get; set; } = 40;       // ═══ What will be unloaded

        // ══════════════════════════════════════════════════
        // ═══ Frustum
        private Frustum frustum;
        private ChunkCoord cameraChunk;

        // ══════════════════════════════════════════════════
        // UPDATE (One time pro frame)
        // ══════════════════════════════════════════════════
        public void UpdateFromCamera(
            Vector3D<float> cameraPosition,
            Matrix4X4<float> viewProjection)
        {
            cameraChunk = new ChunkCoord(
                (long)MathF.Floor(cameraPosition.X / 16f),
                (long)MathF.Floor(cameraPosition.Y / 16f),
                (long)MathF.Floor(cameraPosition.Z / 16f)
            );

            // ═══ Extract Frustum
            frustum = Frustum.ExtractFromMatrix(viewProjection);
        }

        // ══════════════════════════════════════════════════
        // RELEVANZ-CHECKS
        // ══════════════════════════════════════════════════
        public bool ShouldRender(ChunkJob chunk)
        {
            if (!IsWithinDistance(chunk.Coord, renderDistance))
                return false;

            // ═══ Frustum Culling
            var aabb = GetChunkAABB(ChunkCoord);
            return frustum.Intersects(aabb);
        }

        public bool ShouldSimulate(ChunkJob chunk)
        {
            // ═══ Chunks with active Entitys always simulated
            if (chunk.Metadata?.HasWater == true &&
                IsWithinDistance(chunk.Coord, SimulationDistance))
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
        // HELPERS
        // ══════════════════════════════════════════════════
        private bool IsWithinDistance(ChunkCoord coord, int distance)
        {
            long distSq = coord.DistanceSquaredTo(cameraChunk);
            return distSq <= (long)distance * distance;
        }

        private static Box3D<float> GetChunkAABB(ChunkCoord coord)
        {
            return new Box3D<float>(
                new Vector3D<float>(coord.X * 16f, coord.Y * 16f, coord.Z * 16f),
                new Vector3D<float>((coord.X + 1) * 16f, (coord.Y + 1) * 16f, (coord.Z + 1) * 16f)
            );
        }
    }

    public readonly struct Frustum
    {
        // ═══ 6 Planes Near, Far, Left, Right, Top, Bottom
        private readonly Plane3D<float>[] planes;

        private Frustum(Plane3D<float>[] planes) => planes = planes;

        public static Frustum ExtractFromMatrix(Matrix4X4<float> vp)
        {
            // ═══ Frustum-Plane Extraction out View Projection Matrix
            // ═══ (Standard-Algorithmus)
            var planes = new Plane3D<float>[6];
            // ═══ Implementation
            return new Frustum(planes);
        }

        public bool Intersects(Box3D<float> aabb)
        {
            foreach (var plane in planes)
            {
                // ═══ P-Vertex Test
                // ═══ Implementation
            }

            return true;
        }
    }
}
