using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Mesh;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Mesh.OpenGL
{
    /// <summary>
    /// OpenGL-specific implementation of a vertex buffer.
    /// Stores vertex data (positions, colors, normals, UVs, etc.) in GPU memory for efficient rendering.
    /// Supports both static data (uploaded once) and dynamic updates (partial modifications).
    /// </summary>
    public sealed class OpenGLVertexBuffer : IVertexBuffer
    {
        // ═══ The OpenGL context used for all buffer operations
        private readonly GL gl;
        // ═══ Flag to track whether this buffer has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets the OpenGL handle (ID) for this vertex buffer.
        /// Used internally for OpenGL operations.
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// Gets the total number of vertices stored in this buffer.
        /// </summary>
        public int VertexCount { get; private set; }

        /// <summary>
        /// Gets the total size of the vertex data in bytes.
        /// </summary>
        public int SizeInBytes { get; private set; }

        /// <summary>
        /// Gets the vertex layout that describes the structure and format of vertex data.
        /// Defines attributes like position, color, texture coordinates, and normals.
        /// </summary>
        public VertexLayout Layout { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the OpenGLVertexBuffer class.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for buffer operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when glContext is null.</exception>
        public OpenGLVertexBuffer(GL glContext)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
        }

        // ══════════════════════════════════════════════════
        // SET DATA
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates the buffer and uploads vertex data to GPU memory.
        /// Replaces any existing data in the buffer.
        /// Uses StaticDraw hint, indicating the data will be modified rarely (optimal for static meshes).
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices to upload.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        public unsafe void SetData<T>(T[] vertices, VertexLayout layout) where T : unmanaged
        {
            Layout = layout;
            VertexCount = vertices.Length;
            VertexCount = vertices.Length * sizeof(T);

            // ═══ Create and bind the OpenGL buffer object
            Handle = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);

            // ═══ Upload vertex data to GPU memory
            fixed (T* ptr = vertices)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,    // ═══ Target: vertex buffer
                    (nuint)SizeInBytes,             // ═══ Size of data in bytes
                    ptr,                            // ═══ Pointer to vertex data
                    BufferUsageARB.StaticDraw       // ═══ Usage hint: data won't change often
                );
            }

            Logger.Info($"[VBO] Created ({VertexCount} vertices, {SizeInBytes} bytes)");
        }

        // ══════════════════════════════════════════════════
        // UPDATE DATA
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Updates a portion of the vertex data in the GPU buffer without recreating it.
        /// More efficient than SetData() for partial updates of dynamic meshes.
        /// Useful for animations, procedural generation, or streaming data.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices to upload.</param>
        /// <param name="offsetInBytes">The byte offset in the buffer where the update should start. Default is 0.</param>
        public unsafe void UpdateData<T>(T[] vertices, int offsetInBytes = 0) where T : unmanaged
        {
            // ═══ Bind the existing buffer
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);

            // ═══ Update a sub-region of the buffer without reallocating
            fixed (T* ptr = vertices)
            {
                gl.BufferSubData(
                    BufferTargetARB.ArrayBuffer,            // ═══ Target: vertex buffer
                    offsetInBytes,                          // ═══ Byte offset where to start writing
                    (nuint)(vertices.Length * sizeof(T)),   // ═══ Size of data to update
                    ptr                                     // ═══ Pointer to new vertex data
                );
            }
        }

        /// <summary>
        /// Binds this vertex buffer for use in rendering operations.
        /// </summary>
        public void Bind() => gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);

        /// <summary>
        /// Unbinds the vertex buffer.
        /// </summary>
        public void Unbind() => gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        /// <summary>
        /// Releases all GPU resources associated with this vertex buffer.
        /// Deletes the OpenGL buffer object from GPU memory.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            if (Handle != 0)
            {
                // ═══ Delete the buffer from GPU memory
                gl.DeleteBuffer(Handle);
                Handle = 0;
            }

            isDisposed = true;
        }
    }
}