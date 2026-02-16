using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture
{
    public record TextureSettings
    {
        // ══════════════════════════════════════════════════════════════
        // FILTERING (How to see the Texture on scaling?)
        // ══════════════════════════════════════════════════════════════
        public TextureMinFilter MinFilter { get; init; } = TextureMinFilter.NearestMipmapLinear;

        public TextureMagFilter MagFilter { get; init; } = TextureMagFilter.Nearest;

        // ══════════════════════════════════════════════════════════════
        // WRAPPING (What happens outside von 0-1 UV?)
        // ══════════════════════════════════════════════════════════════

        public TextureWrapMode WrapS { get; init; } = TextureWrapMode.Repeat;

        public TextureWrapMode WrapT { get; init; } = TextureWrapMode.Repeat;

        // ══════════════════════════════════════════════════════════════
        // MIPMAPS
        // ══════════════════════════════════════════════════════════════

        public bool GenerateMipmaps { get; init; } = true;

        // ══════════════════════════════════════════════════════════════
        // LOADING
        // ══════════════════════════════════════════════════════════════

        public bool FlipVertically { get; init; } = true;

        // ══════════════════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════════════════

        public static TextureSettings PixelArt => new()
        {
            MinFilter = TextureMinFilter.NearestMipmapLinear,
            MagFilter = TextureMagFilter.Nearest,
            GenerateMipmaps = true
        };

        public static TextureSettings Smooth => new()
        {
            MinFilter = TextureMinFilter.LinearMipmapLinear,
            MagFilter = TextureMagFilter.Linear,
            GenerateMipmaps = true
        };

        public static TextureSettings Atlas => new()
        {
            MinFilter = TextureMinFilter.NearestMipmapLinear,
            MagFilter = TextureMagFilter.Nearest,
            WrapS = TextureWrapMode.ClampToEdge,
            WrapT = TextureWrapMode.ClampToEdge,
            GenerateMipmaps = true
        };
    }

    public enum TextureMinFilter { Nearest, Linear, NearestMipmapNearest, NearestMipmapLinear, LinearMipmapNearest, LinearMipmapLinear }
    public enum TextureMagFilter { Nearest, Linear }
    public enum TextureWrapMode { Repeat, ClampToEdge, MirroredRepeat }
}
