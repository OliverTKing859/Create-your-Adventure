using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMesh : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        IVertexBuffer VertexBuffer { get; }

        /// <summary>
        /// 
        /// </summary>
        IIndexBuffer IndexBuffer { get; }

        /// <summary>
        /// 
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// 
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsInitialized { get; }

        // ══════════════════════════════════════════════════
        // SETUP
        // ══════════════════════════════════════════════════
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="layout"></param>
        void Create<T>(T[] vertices, VertexLayout layout) where T : unmanaged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="layout"></param>
        void Create<T>(T[] vertices, uint[] indices, VertexLayout layout) where T : unmanaged;

        // ══════════════════════════════════════════════════
        // RENDERING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// 
        /// </summary>
        void Bind();

        /// <summary>
        /// 
        /// </summary>
        void Unbind();

        /// <summary>
        /// 
        /// </summary>
        void Draw();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceCount"></param>
        void DrawInstanced(int instanceCount);
    }
}
