using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIndexBuffer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int IndexCount { get; }
        
        /// <summary>
        /// 
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        void SetData(uint[] indices);

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
