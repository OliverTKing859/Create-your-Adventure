namespace Create_your_Adventure.Source.Engine.Texture
{
    /// <summary>
    /// Configuration settings for texture filtering, wrapping, and mipmap generation.
    /// Provides predefined presets for common use cases like pixel art, smooth textures, and atlases.
    /// Immutable record type for safe sharing across texture operations.
    /// </summary>
    public record TextureSettings
    {
        // ══════════════════════════════════════════════════════════════
        // FILTERING (How to see the Texture on scaling?)
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Gets or initializes the minification filter used when the texture is scaled down.
        /// Determines how pixels are sampled when the texture appears smaller than its native resolution.
        /// Default: NearestMipmapLinear (good for pixel art with smooth mipmap transitions)
        /// </summary>
        public SamplerMinFilter MinFilter { get; init; } = SamplerMinFilter.NearestMipmapLinear;

        /// <summary>
        /// Gets or initializes the magnification filter used when the texture is scaled up.
        /// Determines how pixels are sampled when the texture appears larger than its native resolution.
        /// Default: Nearest (sharp, blocky appearance - ideal for pixel art)
        /// </summary>
        public SamplerMagFilter MagFilter { get; init; } = SamplerMagFilter.Nearest;

        // ══════════════════════════════════════════════════════════════
        // WRAPPING (What happens outside von 0-1 UV?)
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Gets or initializes the texture wrapping mode for the horizontal (S/U) axis.
        /// Determines how texture coordinates outside the [0,1] range are handled.
        /// Default: Repeat (texture tiles infinitely)
        /// </summary>
        public SamplerWrapMode WrapS { get; init; } = SamplerWrapMode.Repeat;

        /// <summary>
        /// Gets or initializes the texture wrapping mode for the vertical (T/V) axis.
        /// Determines how texture coordinates outside the [0,1] range are handled.
        /// Default: Repeat (texture tiles infinitely)
        /// </summary>
        public SamplerWrapMode WrapT { get; init; } = SamplerWrapMode.Repeat;

        // ══════════════════════════════════════════════════════════════
        // MIPMAPS
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Gets or initializes whether to automatically generate mipmaps for this texture.
        /// Mipmaps improve rendering quality and performance for textures viewed at varying distances.
        /// Default: true
        /// </summary>
        public bool GenerateMipmaps { get; init; } = true;

        // ══════════════════════════════════════════════════════════════
        // LOADING
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Gets or initializes whether to flip the texture vertically during loading.
        /// Needed because OpenGL's coordinate system has (0,0) at the bottom-left, while most image formats have it at top-left.
        /// Default: true
        /// </summary>
        public bool FlipVertically { get; init; } = true;

        // ══════════════════════════════════════════════════════════════
        // PRESETS
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Gets preset settings optimized for pixel art and retro-style graphics.
        /// Uses nearest-neighbor filtering to preserve sharp, crisp pixels without blurring.
        /// </summary>
        public static TextureSettings PixelArt => new()
        {
            MinFilter = SamplerMinFilter.NearestMipmapLinear,
            MagFilter = SamplerMagFilter.Nearest,
            GenerateMipmaps = true
        };

        /// <summary>
        /// Gets preset settings optimized for smooth, realistic textures and photographs.
        /// Uses linear filtering for smooth blending between pixels.
        /// </summary>
        public static TextureSettings Smooth => new()
        {
            MinFilter = SamplerMinFilter.LinearMipmapLinear,
            MagFilter = SamplerMagFilter.Linear,
            GenerateMipmaps = true
        };

        /// <summary>
        /// Gets preset settings optimized for texture atlases and sprite sheets.
        /// Uses nearest filtering with clamped edges to prevent bleeding between adjacent regions.
        /// </summary>
        public static TextureSettings Atlas => new()
        {
            MinFilter = SamplerMinFilter.NearestMipmapLinear,
            MagFilter = SamplerMagFilter.Nearest,
            WrapS = SamplerWrapMode.ClampToEdge,
            WrapT = SamplerWrapMode.ClampToEdge,
            GenerateMipmaps = true
        };
    }

    /// <summary>
    /// Defines minification filter modes for texture sampling when the texture is scaled down.
    /// Determines how multiple texels are combined into a single pixel.
    /// </summary>
    public enum SamplerMinFilter
    {
        /// <summary>
        /// Nearest-neighbor filtering. Samples the closest texel (sharp, blocky).
        /// </summary>
        Nearest,

        /// <summary>
        /// Linear filtering. Blends the four nearest texels (smooth, blurred).
        /// </summary>
        Linear,

        /// <summary>
        /// Nearest-neighbor filtering with nearest mipmap selection.
        /// </summary>
        NearestMipmapNearest,

        /// <summary>
        /// Nearest-neighbor filtering with linear interpolation between mipmaps (recommended for pixel art).
        /// </summary>
        NearestMipmapLinear,

        /// <summary>
        /// Linear filtering with nearest mipmap selection.
        /// </summary>
        LinearMipmapNearest,

        /// <summary>
        /// Trilinear filtering. Linear filtering with linear interpolation between mipmaps (highest quality).
        /// </summary>
        LinearMipmapLinear
    }

    /// <summary>
    /// Defines magnification filter modes for texture sampling when the texture is scaled up.
    /// Simpler than minification as mipmaps are not used for upscaling.
    /// </summary>
    public enum SamplerMagFilter
    {
        /// <summary>
        /// Nearest-neighbor filtering. Samples the closest texel (sharp, blocky - ideal for pixel art).
        /// </summary>
        Nearest,

        /// <summary>
        /// Linear filtering. Blends the four nearest texels (smooth, blurred).
        /// </summary>
        Linear
    }

    /// <summary>
    /// Defines wrapping modes that determine how texture coordinates outside [0,1] are handled.
    /// </summary>
    public enum SamplerWrapMode
    {
        /// <summary>
        /// Texture repeats infinitely (tiles). UV coordinates wrap around (e.g., 1.5 becomes 0.5).
        /// </summary>
        Repeat,

        /// <summary>
        /// Texture clamps to edge pixels. UV coordinates outside [0,1] use the nearest edge color.
        /// Prevents bleeding and artifacts in texture atlases.
        /// </summary>
        ClampToEdge,

        /// <summary>
        /// Texture repeats with mirroring. Each repetition is flipped horizontally/vertically.
        /// </summary>
        MirroredRepeat
    }
}
