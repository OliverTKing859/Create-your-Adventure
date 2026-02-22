using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Mesh;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Mesh.OpenGL
{
    /// <summary>
    /// Represents an OpenGL index buffer object (EBO) used to store indices for drawing meshes.
    /// Manages GPU memory and provides methods to set data, bind, unbind, and dispose the buffer.
    /// </summary>
    public sealed class OpenGLIndexBuffer : IIndexBuffer
    {
        // ═══ OpenGL context used for buffer operations
        private readonly GL gl;
        // ═══ Flag indicating whether this buffer has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets the OpenGL handle of the buffer.
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// Gets the number of indices stored in the buffer.
        /// </summary>
        public int IndexCount { get; private set; }

        /// <summary>
        /// Gets the size of the buffer in bytes.
        /// </summary>
        public int SizeInBytes { get; private set; }

        /// <summary>
        /// Creates a new instance of the OpenGLIndexBuffer with the given OpenGL context.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for buffer operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if glContext is null.</exception>
        public OpenGLIndexBuffer(GL glContext)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
        }

        // ══════════════════════════════════════════════════
        // SET DATA
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Uploads the given indices to the GPU and creates the OpenGL buffer.
        /// </summary>
        /// <param name="indices">An array of unsigned integers representing the index data.</param>
        public unsafe void SetData(uint[] indices)
        {
            IndexCount = indices.Length;
            SizeInBytes = indices.Length * sizeof(uint);

            Handle = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Handle);

            fixed (uint* ptr = indices)
            {
                gl.BufferData(
                    BufferTargetARB.ElementArrayBuffer,
                    (nuint)SizeInBytes,
                    ptr,
                    BufferUsageARB.StaticDraw
                );
            }

            Logger.Info($"[EBO] Created ({IndexCount} indices, {SizeInBytes} bytes)");
        }

        /// <summary>
        /// Binds this index buffer for use in subsequent draw calls.
        /// </summary>
        public void Bind() => gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Handle);

        /// <summary>
        /// Unbinds any currently bound index buffer.
        /// </summary>
        public void Unbind() => gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        /// <summary>
        /// Releases the GPU resources used by this buffer.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            if (Handle != 0)
            {
                gl.DeleteBuffer(Handle);
                Handle = 0;
            }

            isDisposed = true;
        }
    }
}