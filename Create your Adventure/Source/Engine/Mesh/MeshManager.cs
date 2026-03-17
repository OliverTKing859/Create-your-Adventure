using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Texture.Atlase;
using Create_your_Adventure.Source.Engine.Window;
using Create_your_Adventure.Source.Rendering.Mesh.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// Singleton class responsible for creating, caching, and managing mesh objects.
    /// Supports initialization with a graphics backend and provides convenience methods
    /// for creating common primitives like quads and cubes.
    /// </summary>
    public sealed class MeshManager : IDisposable
    {
        // ═══ Singleton instance
        private static MeshManager? instance;
        // ═══ Lock object for thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the MeshManager.
        /// </summary>
        public static MeshManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new MeshManager();
                    }
                }

                return instance;
            }
        }

        // ═══ Factory function to create new mesh instances
        private Func<string, IMesh>? meshFactory;
        // ═══ Cache storing created meshes by name
        private readonly Dictionary<string, IMesh> meshCache = [];
        // ═══ Disposal stat
        private bool isDisposed;

        /// <summary>
        /// Indicates whether the MeshManager has been initialized with a graphics backend.
        /// </summary>
        public bool IsInitialized => meshFactory is not null;

        /// <summary>
        /// Gets the number of meshes currently stored in the cache.
        /// </summary>
        public int CachedMeshCount => meshCache.Count;

        // ══════════════════════════════════════════════════
        // CONSTRUCTOR 
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private MeshManager()
        {

        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the MeshManager with a supported graphics API context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if no supported graphics API context is found.</exception>
        public void Initialize()
        {
            if (IsInitialized)
            {
                Logger.Warn("[MESH] MeshManager already initialized");
                return;
            }

            var gl = WindowManager.Instance.GlContext;

            if (gl is not null)
            {
                meshFactory = name => new OpenGLMesh(gl, name);
                Logger.Info("[MESH] MeshManager initialized (OpenGL backend)");
            }

            else

            {
                throw new InvalidOperationException("No supported Graphics API context found");
            }
        }

        // ══════════════════════════════════════════════════
        // MESH CREATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a mesh with the specified name or returns a cached one if it already exists.
        /// </summary>
        /// <param name="name">The unique name of the mesh.</param>
        /// <returns>The created or cached mesh instance.</returns>
        public IMesh CreateMesh(string name)
        {
            EnsureInitialized();

            if (meshCache.TryGetValue(name, out var cached))
            {
                Logger.Warn($"[MESH] Mesh '{name}' already exists");
                return cached;
            }

            var mesh = meshFactory!(name);
            meshCache[name] = mesh;

            return mesh;
        }

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a 2D quad mesh of the given size and caches it.
        /// </summary>
        /// <param name="name">The unique name of the quad mesh.</param>
        /// <param name="size">The width/height of the quad.</param>
        /// <returns>The created quad mesh instance.</returns>
        public IMesh CreateQuad(String name, float size = 1.0f)
        {
            var mesh = CreateMesh(name);

            float half = size / 2.0f;

            float[] vertices =
            [
                -half, -half, 0.0f, 0.0f, 0.0f,
                 half, -half, 0.0f, 1.0f, 0.0f,
                 half,  half, 0.0f, 1.0f, 1.0f,
                -half,  half, 0.0f, 0.0f, 1.0f
            ];

            uint[] indices = [0, 1, 2, 2, 3, 0];

            mesh.Create(vertices, indices, VertexLayout.PositionTexCoord());

            Logger.Info($"[MESH] Quad '{name}' created (size: {size})");
            return mesh;
        }

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a cube mesh of the given size and caches it.
        /// </summary>
        /// <param name="name">The unique name of the cube mesh.</param>
        /// <param name="size">The size of the cube.</param>
        /// <returns>The created cube mesh instance.</returns>
        public IMesh CreateCube(string name, float size, AtlasRegion region)
        {
            var mesh = CreateMesh(name);
            float half = size / 2.0f;

            float u0 = region.U0;
            float v0 = region.V0;
            float u1 = region.U1;
            float v1 = region.V1;

            float[] vertices =
            [
                -half, -half,  half, u0, v0,
                 half, -half,  half, u1, v0,
                 half,  half,  half, u1, v1,
                -half,  half,  half, u0, v1,

                 half, -half, -half, u0, v0,
                -half, -half, -half, u1, v0,
                -half,  half, -half, u1, v1,
                 half,  half, -half, u0, v1,

                 half, -half,  half, u0, v0,
                 half, -half, -half, u1, v0,
                 half,  half, -half, u1, v1,
                 half,  half,  half, u0, v1,

                -half, -half, -half, u0, v0,
                -half, -half,  half, u1, v0,
                -half,  half,  half, u1, v1,
                -half,  half, -half, u0, v1,

                -half,  half,  half, u0, v0,
                 half,  half,  half, u1, v0,
                 half,  half, -half, u1, v1,
                -half,  half, -half, u0, v1,

                -half, -half, -half, u0, v0,
                 half, -half, -half, u1, v0,
                 half, -half,  half, u1, v1,
                -half, -half,  half, u0, v1
            ];

            uint[] indices =
            [
                 0,  1,  2,  2,  3,  0,

                 4,  5,  6,  6,  7,  4,

                 8,  9, 10, 10, 11,  8,

                12, 13, 14, 14, 15, 12,

                16, 17, 18, 18, 19, 16,

                20, 21, 22, 22, 23, 20
            ];

            mesh.Create(vertices, indices, VertexLayout.PositionTexCoord());

            Logger.Info($"[MESH] Cube '{name}' created (size: {size})");
            return mesh;
        }

        // ══════════════════════════════════════════════════
        // ACCESS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Retrieves a mesh by name from the cache.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        /// <returns>The cached mesh, or null if not found.</returns>
        public IMesh? GetMesh(string name)
            => meshCache.TryGetValue(name, out var mesh) ? mesh : null;

        /// <summary>
        /// Checks whether a mesh with the given name exists in the cache.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        /// <returns>True if the mesh exists, false otherwise.</returns>
        public bool HasMesh(string name) => meshCache.ContainsKey(name);

        /// <summary>
        /// Ensures that the MeshManager has been initialized before performing operations.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if MeshManager is not initialized.</exception>
        private void EnsureInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MeshManager must be initialized first");
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Disposes all cached meshes and releases resources held by the MeshManager.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            Logger.Info($"[MESH] Disposing {meshCache.Count} mesh(es)...");

            foreach (var mesh in meshCache.Values)
                mesh.Dispose();

            meshCache.Clear();
            isDisposed = true;

            // ═══ Ensure singleton reference cleared in a thread-safe manner
            lock (instanceLock)
            {
                instance = null;
            }

            Logger.Info("[MESH] MeshManager disposed");
        }
    }
}