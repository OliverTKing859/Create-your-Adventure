using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Mesh;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Mesh.OpenGL
{
    public sealed class OpenGLMesh : IMesh
    {
        private readonly GL gl;
        private uint vao;
        private OpenGLVertexBuffer? vertexBuffer;
        private OpenGLIndexBuffer? indexBuffer;
        private bool isDisposed;

        public string Name { get; }
        public IVertexBuffer VertexBuffer => vertexBuffer!;
        public IIndexBuffer? IndexBuffer => indexBuffer;
        public int VertexCount => vertexBuffer?.VertexCount ?? 0;
        public int IndexCount => indexBuffer?.IndexCount ?? 0;
        public bool IsInitialized => vao != 0;

        public OpenGLMesh(GL glContext, string name)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        // ══════════════════════════════════════════════════
        // CREATE (No Indices)
        // ══════════════════════════════════════════════════

        public void Create<T>(T[] vertices, VertexLayout layout) where T : unmanaged
        {
            if (IsInitialized)
            {
                Logger.Warn($"[MESH] Mesh '{Name}' already initialized");
                return;
            }

            Logger.Info($"[MESH] Creating mesh '{Name}' ({vertices.Length} vertices)...");

            // ═══ 1. Create VAO (Save the State)
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // ═══ 2. Create VBO and upload Data
            vertexBuffer = new OpenGLVertexBuffer(gl);
            vertexBuffer.SetData(vertices, layout);

            // ═══ 3. Configuration Vertex Attributes
            SetupVertexAttributes(layout);

            // ═══ 4. Unbind
            gl.BindVertexArray(0);

            Logger.Info($"[MESH] Mesh '{Name}' created (VAO: {vao}, VBO: {vertexBuffer.Handle})");
        }

        // ══════════════════════════════════════════════════
        // CREATE (With Indices)
        // ══════════════════════════════════════════════════

        public void Create<T>(T[] vertices, uint[] indices, VertexLayout layout) where T : unmanaged
        {
            if (IsInitialized)
            {
                Logger.Warn($"[MESH] Mesh '{Name}' already initialized");
                return;
            }

            Logger.Info($"[MESH] Creating mesh '{Name}' ({vertices.Length} vertices, {indices.Length} indices)...");

            // ═══ 1. Create VAO
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // ═══ 2. Create VBO
            vertexBuffer = new OpenGLVertexBuffer(gl);
            vertexBuffer.SetData(vertices, layout);

            // ═══ 3. Configuration Vertex Attributes
            SetupVertexAttributes(layout);

            // ═══ 4. Create EBO (Important: EBO saves on the VAO!)
            indexBuffer = new OpenGLIndexBuffer(gl);
            indexBuffer.SetData(indices);

            // ═══ 5. Unbind VAO (EBO remains connect to VAO)
            gl.BindVertexArray(0);

            Logger.Info($"[MESH] Mesh '{Name}' created (VAO: {vao}, VBO: {vertexBuffer.Handle}, EBO: {indexBuffer.Handle})");
        }

        // ══════════════════════════════════════════════════
        // VERTEX ATTRIBUTES SETUP
        // ══════════════════════════════════════════════════

        private void SetupVertexAttributes(VertexLayout layout)
        {
            foreach (var attribute in layout.Attributes)
            {
                gl.EnableVertexAttribArray(attribute.Location);

                unsafe
                {
                    gl.VertexAttribPointer(
                        attribute.Location,
                        attribute.ComponentCount,
                        ConvertAttributeType(attribute.Type),
                        attribute.Normalized,
                        (uint)layout.Stride,
                        (void*)attribute.Offset
                        );
                }

                Logger.Info($"[MESH] Attribute '{attribute.Name}' configured (location: {attribute.Location}, components: {attribute.ComponentCount})");
            }
        }

        // ══════════════════════════════════════════════════
        // BINDING & DRAWING
        // ══════════════════════════════════════════════════

        public void Bind()
        {
            if (!IsInitialized)
            {
                Logger.Warn($"[MESH] Cannot bind uninitialized mesh '{Name}'");
                return;
            }

            // ═══ Connect VAO = All State will be restored
            // ═══ That's the advantage of VAO: 1 bind instead of VBO + EBO + attributes.
            gl.BindVertexArray(vao);
        }

        public void Unbind()
        {
            gl.BindVertexArray(0);
        }

        public void Draw()
        {
            if (!IsInitialized) return;

            Bind();

            if (indexBuffer is not null)
            {
                // ═══ Indexed Drawing (with EBO)
                unsafe
                {
                    gl.DrawElements(
                        PrimitiveType.Triangles,
                        (uint)IndexCount,
                        DrawElementsType.UnsignedInt,
                        null
                    );
                }
            }

            else

            {
                // ═══ Non-Indexed Drawing
                gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)VertexCount);
            }
        }

        public void DrawInstanced(int instanceCount)
        {
            if (!IsInitialized) return;

            Bind();

            if (indexBuffer is not null)
            {
                unsafe
                {
                    gl.DrawElementsInstanced(
                        PrimitiveType.Triangles,
                        (uint)IndexCount,
                        DrawElementsType.UnsignedInt,
                        null,
                        (uint)instanceCount
                    );
                }
            }

            else

            {
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

        public void Dispose()
        {
            if (isDisposed) return;

            indexBuffer?.Dispose();
            vertexBuffer?.Dispose();

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

        private static VertexAttribPointerType ConvertAttribType(VertexAttributeType type) => type switch
        {
            VertexAttributeType.Float => VertexAttribPointerType.Float,
            VertexAttributeType.Int => VertexAttribPointerType.Int,
            VertexAttributeType.UInt => VertexAttribPointerType.UnsignedInt,
            VertexAttributeType.Byte => VertexAttribPointerType.UnsignedByte,
            _ => VertexAttribPointerType.Float
        };

    }
}
