namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// Defines the contract for a mesh that combines vertex and index buffers for efficient rendering.
    /// A mesh represents a 3D model or geometry that can be drawn on screen.
    /// Supports both indexed and non-indexed rendering, as well as instanced rendering for performance.
    /// </summary>
    public interface IMesh : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier name of this mesh.
        /// Used for mesh management and debugging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the vertex buffer containing the mesh's vertex data (positions, colors, UVs, normals, etc.).
        /// </summary>
        IVertexBuffer VertexBuffer { get; }

        /// <summary>
        /// Gets the index buffer containing vertex indices for efficient vertex reuse.
        /// May be null for non-indexed meshes.
        /// </summary>
        IIndexBuffer? IndexBuffer { get; }

        /// <summary>
        /// Gets the total number of vertices in this mesh.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Gets the total number of indices in this mesh.
        /// Returns 0 for non-indexed meshes.
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// Gets a value indicating whether the mesh has been initialized with vertex data.
        /// </summary>
        bool IsInitialized { get; }

        // ══════════════════════════════════════════════════
        // SETUP
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a non-indexed mesh from vertex data only.
        /// Suitable for simple geometries where vertex reuse is not needed.
        /// Vertices are drawn in sequential order.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices defining the mesh geometry.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        void Create<T>(T[] vertices, VertexLayout layout) where T : unmanaged;

        /// <summary>
        /// Creates an indexed mesh from vertex and index data.
        /// More efficient for complex geometries as vertices can be reused through indexing.
        /// Reduces memory usage and improves cache performance.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices defining the mesh geometry.</param>
        /// <param name="indices">The array of indices referencing vertices to form triangles.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        void Create<T>(T[] vertices, uint[] indices, VertexLayout layout) where T : unmanaged;

        // ══════════════════════════════════════════════════
        // RENDERING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Binds the mesh's vertex and index buffers for rendering.
        /// Must be called before Draw() or DrawInstanced().
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the mesh's buffers.
        /// Called to clean up state after rendering operations.
        /// </summary>
        void Unbind();

        /// <summary>
        /// Draws the mesh once using the currently bound shader.
        /// Uses indexed rendering if an index buffer exists, otherwise renders vertices sequentially.
        /// </summary>
        void Draw();

        /// <summary>
        /// Draws multiple instances of the mesh in a single draw call.
        /// Highly efficient for rendering many copies of the same mesh (trees, grass, particles, etc.).
        /// Requires instanced rendering support in the shader.
        /// </summary>
        /// <param name="instanceCount">The number of instances to render.</param>
        void DrawInstanced(int instanceCount);
    }
}