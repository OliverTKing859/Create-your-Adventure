using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct AtlasRegion
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int X { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int Y { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; init; }

        // ══════════════════════════════════════════════════
        // UV-COORDINATION (0.0 - 1.0, normalization)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// 
        /// </summary>
        public float U0 { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public float V0 { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public float U1 { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public float V1 { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="atlasWidth"></param>
        /// <param name="atlasHeight"></param>
        /// <returns></returns>
        public static AtlasRegion Create(
            string name,
            int x,
            int y,
            int width,
            int height,
            int atlasWidth,
            int atlasHeight
            )
        {
            return new AtlasRegion
            {
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,

                // ═══ UV normalization
                U0 = (float)x / atlasWidth,
                V0 =(float)y / atlasHeight,
                U1 =(float)(x + width) / atlasWidth,
                V1 =(float)(y + height) / atlasHeight
            };
        }
    }
}
