using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Mesh;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Mesh.OpenGL
{
    /// <summary>
    /// OpenGL-specific implementation of a mesh using Vertex Array Objects (VAO).
    /// Combines vertex buffers, index buffers, and vertex attribute configuration into a single bindable object.
    /// VAO stores all vertex attribute state, allowing efficient binding with a single call.
    /// </summary>
    public sealed class OpenGLMesh : IMesh
    {
        // ═══ The OpenGL context used for all mesh operations
        private readonly GL gl;
        // ═══ The Vertex Array Object (VAO) handle that stores vertex attribute state
        private uint vao;
        // ═══ The vertex buffer containing vertex data
        private OpenGLVertexBuffer? vertexBuffer;
        // ═══ The index buffer containing vertex indices (optional for non-indexed meshes)
        private OpenGLIndexBuffer? indexBuffer;
        // ═══ Flag to track whether this mesh has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets the unique identifier name of this mesh.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the vertex buffer containing vertex data.
        /// </summary>
        public IVertexBuffer VertexBuffer => vertexBuffer!;

        /// <summary>
        /// Gets the index buffer containing vertex indices.
        /// Returns null for non-indexed meshes.
        /// </summary>
        public IIndexBuffer? IndexBuffer => indexBuffer;

        /// <summary>
        /// Gets the total number of vertices in this mesh.
        /// </summary>
        public int VertexCount => vertexBuffer?.VertexCount ?? 0;

        /// <summary>
        /// Gets the total number of indices in this mesh.
        /// Returns 0 for non-indexed meshes.
        /// </summary>
        public int IndexCount => indexBuffer?.IndexCount ?? 0;

        /// <summary>
        /// Gets a value indicating whether the mesh has been initialized with a VAO.
        /// </summary>
        public bool IsInitialized => vao != 0;

        /// <summary>
        /// Initializes a new instance of the OpenGLMesh class.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for mesh operations.</param>
        /// <param name="name">The unique name identifier for this mesh.</param>
        /// <exception cref="ArgumentNullException">Thrown when glContext or name is null.</exception>
        public OpenGLMesh(GL glContext, string name)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        // ══════════════════════════════════════════════════
        // CREATE (No Indices)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a non-indexed mesh from vertex data only.
        /// Sets up VAO, VBO, and vertex attributes for sequential rendering.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices defining the mesh geometry.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        public void Create<T>(T[] vertices, VertexLayout layout) where T : unmanaged
        {
            if (IsInitialized)
            {
                Logger.Warn($"[MESH] Mesh '{Name}' already initialized");
                return;
            }

            Logger.Info($"[MESH] Creating mesh '{Name}' ({vertices.Length} vertices)...");

            // ═══ Step 1. Create VAO to store all vertex state
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // ═══ Step 2. Create VBO and upload vertex data
            vertexBuffer = new OpenGLVertexBuffer(gl);
            vertexBuffer.SetData(vertices, layout);

            // ═══ Step 3. Configure vertex attributes (position, texcoords, normals, etc.)
            SetupVertexAttributes(layout);

            // ═══ Step 4. Unbind VAO (state is now saved in the VAO)
            gl.BindVertexArray(0);

            Logger.Info($"[MESH] Mesh '{Name}' created (VAO: {vao}, VBO: {vertexBuffer.Handle})");
        }

        // ══════════════════════════════════════════════════
        // CREATE (With Indices)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates an indexed mesh from vertex and index data.
        /// Sets up VAO, VBO, EBO, and vertex attributes for efficient indexed rendering.
        /// </summary>
        /// <typeparam name="T">The vertex struct type (must be unmanaged for GPU compatibility).</typeparam>
        /// <param name="vertices">The array of vertices defining the mesh geometry.</param>
        /// <param name="indices">The array of indices referencing vertices to form triangles.</param>
        /// <param name="layout">The vertex layout describing the structure of each vertex.</param>
        public void Create<T>(T[] vertices, uint[] indices, VertexLayout layout) where T : unmanaged
        {
            if (IsInitialized)
            {
                Logger.Warn($"[MESH] Mesh '{Name}' already initialized");
                return;
            }

            Logger.Info($"[MESH] Creating mesh '{Name}' ({vertices.Length} vertices, {indices.Length} indices)...");

            // ═══ Step 1. Create VAO to store all vertex and index state
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // ═══ Step 2. Create VBO and upload vertex data
            vertexBuffer = new OpenGLVertexBuffer(gl);
            vertexBuffer.SetData(vertices, layout);

            // ═══ Step 3. Configure vertex attributes (position, texcoords, normals, etc.)
            SetupVertexAttributes(layout);

            // ═══ Step 4. Create EBO and upload index data
            // ═══ IMPORTANT: EBO is saved in the VAO state when bound while VAO is active
            indexBuffer = new OpenGLIndexBuffer(gl);
            indexBuffer.SetData(indices);

            // ═══ Step 5. Unbind VAO (EBO binding remains associated with VAO)
            gl.BindVertexArray(0);

            Logger.Info($"[MESH] Mesh '{Name}' created (VAO: {vao}, VBO: {vertexBuffer.Handle}, EBO: {indexBuffer.Handle})");
        }

        // ══════════════════════════════════════════════════
        // VERTEX ATTRIBUTES SETUP
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Configures vertex attributes based on the vertex layout.
        /// Enables and sets up each attribute (position, texcoords, normals, etc.) for the shader.
        /// </summary>
        /// <param name="layout">The vertex layout defining all attributes.</param>
        private void SetupVertexAttributes(VertexLayout layout)
        {
            foreach (var attribute in layout.Attributes)
            {
                // Enable the vertex attribute at this location
                gl.EnableVertexAttribArray(attribute.Location);

                // Configure how the GPU should interpret this attribute's data
                unsafe
                {
                    gl.VertexAttribPointer(
                        attribute.Location,                     // ═══ Shader attribute location
                        attribute.ComponentCount,               // ═══ Number of components (e.g., 3 for vec3)
                        ConvertAttributeType(attribute.Type),   // ═══ Data type (float, int, etc.)
                        attribute.Normalized,                   // ═══ Normalize integer types to [0,1] or [-1,1]
                        (uint)layout.Stride,                    // ═══ Bytes between consecutive vertices
                        (void*)attribute.Offset                 // ═══ Byte offset of this attribute in the vertex
                        );
                }

                Logger.Info($"[MESH] Attribute '{attribute.Name}' configured (location: {attribute.Location}, components: {attribute.ComponentCount})");
            }
        }

        // ══════════════════════════════════════════════════
        // BINDING & DRAWING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Binds the VAO, restoring all vertex attribute and buffer state.
        /// This is the key advantage of VAO: one bind instead of rebinding VBO, EBO, and all attributes.
        /// </summary>
        public void Bind()
        {
            if (!IsInitialized)
            {
                Logger.Warn($"[MESH] Cannot bind uninitialized mesh '{Name}'");
                return;
            }

            // ═══ Bind VAO - all state (VBO, EBO, attributes) is restored automatically
            gl.BindVertexArray(vao);
        }

        /// <summary>
        /// Unbinds the VAO.
        /// </summary>
        public void Unbind()
        {
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Draws the mesh once using the currently bound shader.
        /// Automatically chooses indexed or non-indexed rendering based on whether an index buffer exists.
        /// </summary>
        public void Draw()
        {
            if (!IsInitialized) return;

            Bind();

            if (indexBuffer is not null)
            {
                // ═══ Indexed drawing using EBO (more efficient for complex geometry)
                unsafe
                {
                    gl.DrawElements(
                        PrimitiveType.Triangles,        // ═══ Draw triangles
                        (uint)IndexCount,               // ═══ Number of indices to draw
                        DrawElementsType.UnsignedInt,   // ═══ Index data type
                        null                            // ═══ Offset (0 = start from beginning)
                    );
                }
            }

            else

            {
                // ═══ Non-indexed drawing (vertices rendered sequentially)
                gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)VertexCount);
            }
        }

        /// <summary>
        /// Draws multiple instances of the mesh in a single draw call.
        /// Highly efficient for rendering many copies (trees, grass, particles, etc.).
        /// Requires instanced rendering support in the shader (gl_InstanceID).
        /// </summary>
        /// <param name="instanceCount">The number of instances to render.</param>
        public void DrawInstanced(int instanceCount)
        {
            if (!IsInitialized) return;

            Bind();

            if (indexBuffer is not null)
            {
                // ═══ Indexed instanced drawing
                unsafe
                {
                    gl.DrawElementsInstanced(
                        PrimitiveType.Triangles,
                        (uint)IndexCount,
                        DrawElementsType.UnsignedInt,
                        null,
                        (uint)instanceCount             // ═══ Number of instances to draw
                    );
                }
            }

            else

            {
                // ═══ Non-indexed instanced drawing
                gl.DrawArraysInstanced(
                    PrimitiveType.Triangles,
                    0,
                    (uint)VertexCount,
                    (uint)instanceCount
                );
            }
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases all GPU resources associated with this mesh.
        /// Disposes buffers and deletes the VAO.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            // ═══ Dispose buffers first
            indexBuffer?.Dispose();
            vertexBuffer?.Dispose();

            // ═══ Delete the VAO
            if (vao != 0)
            {
                gl.DeleteVertexArray(vao);
                Logger.Info($"[MESH] Mesh '{Name}' disposed (VAO: {vao})");
                vao = 0;
            }

            isDisposed = true;
        }

        // ══════════════════════════════════════════════════
        // HELPER
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Converts custom vertex attribute type enum to OpenGL enum.
        /// </summary>
        private static VertexAttribPointerType ConvertAttributeType(VertexAttributeType type) => type switch
        {
            VertexAttributeType.Float => VertexAttribPointerType.Float,
            VertexAttributeType.Int => VertexAttribPointerType.Int,
            VertexAttributeType.UInt => VertexAttribPointerType.UnsignedInt,
            VertexAttributeType.Byte => VertexAttribPointerType.UnsignedByte,
            _ => VertexAttribPointerType.Float
        };
    }
}