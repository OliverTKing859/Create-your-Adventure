using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// Defines the contract for an index buffer that stores vertex indices on the GPU.
    /// Index buffers enable efficient vertex reuse by referencing vertices through indices instead of duplicating vertex data.
    /// Abstracts index buffer operations across different graphics APIs (OpenGL, DirectX, Vulkan).
    /// </summary>
    public interface IIndexBuffer : IDisposable
    {
        /// <summary>
        /// Gets the total number of indices stored in this buffer.
        /// Each index references a vertex in the associated vertex buffer.
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// Gets the total size of the index data in bytes.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Uploads index data to the GPU buffer.
        /// Replaces any existing data in the buffer.
        /// Indices are typically organized in groups of 3 for triangle rendering.
        /// </summary>
        /// <param name="indices">The array of vertex indices (uint values referencing vertex positions).</param>
        void SetData(uint[] indices);

        /// <summary>
        /// Binds this index buffer for rendering.
        /// Subsequent indexed draw calls will use indices from this buffer to reference vertices.
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the index buffer.
        /// Called to clean up state after rendering operations.
        /// </summary>
        void Unbind();
    }
}