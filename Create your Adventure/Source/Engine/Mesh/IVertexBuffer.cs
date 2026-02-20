using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVertexBuffer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// 
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// 
        /// </summary>
        VertexLayout Layout { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="layout"></param>
        void SetData<T>(T[] vertices, VertexLayout layout) where T : unmanaged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="offsetInBytes"></param>
        void UpdateData<T>(T[] vertices, int offsetInBytes = 0) where T : unmanaged;

        /// <summary>
        /// 
        /// </summary>
        void Bind();

        /// <summary>
        /// 
        /// </summary>
        void Unbind();
    }
}
