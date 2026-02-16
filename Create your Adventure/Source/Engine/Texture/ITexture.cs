using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="settings"></param>
        void LoadFromFile(string path, TextureSettings settings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="settings"></param>
        void LoadFromData(byte[] pixelData, int width, int height, TextureSettings settings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        void Bind(uint unit = 0);

        /// <summary>
        /// 
        /// </summary>
        void Unbind();
    }
}
