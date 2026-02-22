using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// Defines the contract for a vertex buffer that stores vertex data on the GPU.
    /// Abstracts vertex buffer operations across different graphics APIs (OpenGL, DirectX, Vulkan).
    /// Supports dynamic updates and custom vertex layouts for flexible rendering.
    /// </summary>
    public interface IVertexBuffer : IDisposable
    {
        /// <summary>
        /// Gets the total number of vertices stored in this buffer.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Gets the total size of the vertex data in bytes.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the vertex layout that describes the structure and format of vertex data.
        /// Defines attributes like position, color, texture coordinates, and normals.
        /// </summary>
        VertexLayout Layout { get; }

        /// <summary>
        /// Uploads vertex data to the GPU and sets the vertex layout.
        /// Replaces any existing data in the buffer.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices to upload.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        void SetData<T>(T[] vertices, VertexLayout layout) where T : unmanaged;

        /// <summary>
        /// Updates a portion of the vertex data in the GPU buffer.
        /// Useful for dynamic meshes or partial updates without recreating the entire buffer.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices to upload.</param>
        /// <param name="offsetInBytes">The byte offset in the buffer where the update should start. Default is 0.</param>
        void UpdateData<T>(T[] vertices, int offsetInBytes = 0) where T : unmanaged;

        /// <summary>
        /// Binds this vertex buffer for rendering.
        /// Subsequent draw calls will use the vertex data from this buffer.
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the vertex buffer.
        /// Called to clean up state after rendering operations.
        /// </summary>
        void Unbind();
    }
}