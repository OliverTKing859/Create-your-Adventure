using Create_your_Adventure.Source.Debug;
using Create_your_Adventure.Source.Engine.Texture;
using Create_your_Adventure.Source.Engine.Texture.Atlase;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Create_your_Adventure.Source.Rendering.Texture.OpenGL
{
    /// <summary>
    /// OpenGL-specific implementation of a texture atlas.
    /// Packs multiple textures into a single large texture to reduce draw calls and improve rendering performance.
    /// Uses a simple grid-based packing algorithm optimized for same-sized textures.
    /// </summary>
    public sealed class OpenGLTextureAtlas : ITextureAtlas
    {
        // ═══ The OpenGL context used for all texture operations
        private readonly GL gl;
        // ═══ The OpenGL handle (ID) for the atlas texture
        private uint handle;

        // ═══ List of textures waiting to be packed (before Build() is called)
        private readonly List<(string Name, string Path)> pendingTextures = [];

        // ═══ Dictionary of atlas regions after the atlas has been built
        private readonly Dictionary<string, AtlasRegion> regions = [];

        /// <summary>
        /// Gets the unique identifier name of this texture atlas.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the total width of the built atlas texture in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the total height of the built atlas texture in pixels.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the number of textures successfully packed into this atlas.
        /// </summary>
        public int TextureCount => regions.Count;

        /// <summary>
        /// Gets a value indicating whether the atlas has been built and is ready for use.
        /// </summary>
        public bool IsBuilt { get; private set; }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the OpenGLTextureAtlas class.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for texture operations.</param>
        /// <param name="name">The unique name identifier for this atlas.</param>
        public OpenGLTextureAtlas(GL glContext, string name)
        {
            gl = glContext;
            Name = name;
        }

        // ═══════════════════════════════════════════════════════════════
        // ADD TEXTURE
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Adds a texture to the atlas by specifying its file path.
        /// Must be called before Build(). Duplicate names are ignored with a warning.
        /// </summary>
        /// <param name="name">The unique identifier for this texture within the atlas.</param>
        /// <param name="path">The file path to the texture image.</param>
        /// <exception cref="InvalidOperationException">Thrown when trying to add textures after Build() has been called.</exception>
        public void AddTexture(string name, string path)
        {
            if (IsBuilt)
                throw new InvalidOperationException("Cannot add textures after Build()");

            if (pendingTextures.Any(t => t.Name == name))
            {
                Logger.Warn($"[TEXTURE] Texture '{name}' already added to atlas '{Name}'");
                return;
            }

            pendingTextures.Add((name, path));
        }

        // ═══════════════════════════════════════════════════════════════
        // THE BUILD
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Builds the atlas by loading, packing, and uploading all pending textures to GPU memory.
        /// This is a four-step process:
        /// 1. Load all images from disk
        /// 2. Calculate optimal atlas size (power-of-two)
        /// 3. Pack textures using grid-based layout
        /// 4. Upload to OpenGL and configure texture parameters
        /// </summary>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        /// <exception cref="InvalidOperationException">Thrown when no textures have been added or when already built.</exception>
        public void Build(TextureSettings settings)
        {
            if (IsBuilt)
            {
                Logger.Warn($"[TEXTURE] Atlas '{Name}' already built");
                return;
            }

            if (pendingTextures.Count == 0)
                throw new InvalidOperationException("No textures to build atlas from");

            Logger.Info($"[TEXTURE] Building atlas '{Name}' from {pendingTextures.Count} textures...");

            // ═══════════════════════════════════════════════════════════
            // STEP 1: Load all Images
            // ═══════════════════════════════════════════════════════════
            var loadedImages = new List<(string Name, byte[] Data, int Width, int Height)>();

            // ═══ Configure image loading to match OpenGL coordinate system if needed
            StbImage.stbi_set_flip_vertically_on_load(settings.FlipVertically ? 1 : 0);

            foreach (var(name, path) in pendingTextures)
            {
                try
                {
                    using var stream = File.OpenRead(path);
                    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    loadedImages.Add((name, image.Data, image.Width, image.Height));
                    Logger.Info($"[TEXTURE] Loaded image '{name}' ({image.Width}x{image.Height})");
                }
                catch (Exception ex)
                {
                    Logger.Error($"[TEXTURE] Failed to load '{name}': {ex.Message}");
                }
            }

            if (loadedImages.Count == 0)
                throw new InvalidOperationException("No images could be loaded for atlas");

            // ═══════════════════════════════════════════════════════════
            // STEP 2: Calculate Atlas-Size (Power of 2)
            // ═══════════════════════════════════════════════════════════

            // ═══ Calculate total pixel count to estimate required atlas size
            int totalPixels = loadedImages.Sum(img => img.Width * img.Height);
            int atlasSize = NextPowerOfTwo((int)Math.Ceiling(Math.Sqrt(totalPixels)));

            // ═══ Clamp atlas size to reasonable bounds
            atlasSize = Math.Max(atlasSize, 256);   // ═══ Minimum 256x256
            atlasSize = Math.Min(atlasSize, 4096);  // ═══ Maximum 4096x4096

            Width = atlasSize;
            Height = atlasSize;

            // ═══════════════════════════════════════════════════════════
            // STEP 3: Packing Textures (Simple Grid for same Size)
            // ═══════════════════════════════════════════════════════════

            // ═══ Allocate memory for the full atlas (RGBA format)
            byte[] atlasData = new byte[Width * Height * 4]; // ═══ RGBA

            // ═══ Assume all textures have the same size (optimized for block textures)
            int textureSize = loadedImages[0].Width;

            int currentX = 0;
            int currentY = 0;

            foreach (var (name, data, w, h) in loadedImages)
            {
                // ═══ Check if we need to wrap to the next row
                if (currentX + w > Width)
                {
                    currentX = 0;
                    currentY += textureSize;
                }

                // ═══ Check if atlas is full
                if (currentY + h > Height)
                {
                    Logger.Error($"[TEXTURE] Atlas '{Name}' is full, cannot fit '{name}'");
                    continue;
                }

                // ═══ Save the region with calculated UV coordinates
                regions[name] = AtlasRegion.Create(name, currentX, currentY, w, h, Width, Height);

                // ═══ Copy pixel data into the atlas buffer
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int srcIndex = (y * w + x) * 4;
                        int dstIndex = ((currentY + y) * Width + (currentX + x)) * 4;

                        // ═══ Bounds check to prevent buffer overflows
                        if (srcIndex + 3 < data.Length && dstIndex + 3 < atlasData.Length)
                        {
                            atlasData[dstIndex + 0] = data[srcIndex + 0]; // ═══ R
                            atlasData[dstIndex + 1] = data[srcIndex + 1]; // ═══ G
                            atlasData[dstIndex + 2] = data[srcIndex + 2]; // ═══ B
                            atlasData[dstIndex + 3] = data[srcIndex + 3]; // ═══ A
                        }
                    }
                }

                // ═══ Move to the next position in the grid
                currentX += w;
            }

            // ═══════════════════════════════════════════════════════════
            // STEP 4: OpenGL create Textures
            // ═══════════════════════════════════════════════════════════

            // ═══ Generate and bind the texture object
            handle = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, handle);

            // ═══ Upload atlas data to GPU
            unsafe
            {
                fixed (byte* ptr = atlasData)
                {
                    gl.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        InternalFormat.Rgba,
                        (uint)Width,
                        (uint)Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        ptr
                    );
                }
            }

            // ═══ Configure texture parameters
            if (settings.GenerateMipmaps)
                gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)ConvertWrapMode(settings.WrapS));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)ConvertWrapMode(settings.WrapT));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)ConvertMinFilter(settings.MinFilter));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)ConvertMagFilter(settings.MagFilter));

            IsBuilt = true;
            pendingTextures.Clear(); // ═══ Clear pending list as they're now in the atlas

            Logger.Info($"[TEXTURE] Atlas '{Name}' built: {Width}x{Height}, {TextureCount} textures");
        }

        // ═══════════════════════════════════════════════════════════
        // GET REGIONS
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Retrieves the atlas region for a specific texture by name.
        /// Returns pixel coordinates and normalized UV coordinates for rendering.
        /// </summary>
        /// <param name="name">The name of the texture to retrieve.</param>
        /// <returns>The atlas region containing coordinates and UV data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the atlas has not been built yet.</exception>
        public AtlasRegion GetRegion(string name)
        {
            if (!IsBuilt)
                throw new InvalidOperationException("Atlas must be built first");

            if (regions.TryGetValue(name, out var region))
                return region;

            Logger.Warn($"[TEXTURE] Region '{name}' not found in atlas '{Name}'");
            return default;
        }

        // ═══════════════════════════════════════════════════════════
        // HAS TEXTURE
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Checks whether a texture with the specified name exists in the atlas.
        /// </summary>
        /// <param name="name">The name of the texture to check.</param>
        /// <returns>True if the texture exists in the atlas, false otherwise.</returns>
        public bool HasTexture(string name) => regions.ContainsKey(name);

        // ═══════════════════════════════════════════════════════════
        // GET ALL TEXTURE NAMES
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Gets all texture names contained in this atlas.
        /// Useful for debugging or iterating over all available textures.
        /// </summary>
        /// <returns>An enumerable collection of all texture names in the atlas.</returns>
        public IEnumerable<string> GetAllTextureNames() => regions.Keys;

        // ═══════════════════════════════════════════════════════════
        // BIND
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Binds the atlas texture to the specified texture unit for rendering.
        /// The atlas must be built before it can be bound.
        /// </summary>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware). Default is 0.</param>
        public void Bind(uint unit = 0)
        {
            if (!IsBuilt)
            {
                Logger.Warn($"[TEXTURE] Cannot bind unbuilt atlas '{Name}'");
                return;
            }

            gl.ActiveTexture(TextureUnit.Texture0 + (int)unit);
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }

        // ═══════════════════════════════════════════════════════════
        // UNBIND
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Unbinds the atlas texture from the current texture unit.
        /// </summary>
        public void Unbind() => gl.BindTexture(TextureTarget.Texture2D, 0);

        // ═══════════════════════════════════════════════════════════
        // DISPOSE
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Releases all GPU resources associated with this atlas.
        /// Deletes the OpenGL texture object and clears region data.
        /// </summary>
        public void Dispose()
        {
            if (handle !=0)
            {
                gl.DeleteTexture(handle);
                handle = 0;
                Logger.Info($"[TEXTURE] Atlas '{Name}' disposed");
            }

            regions.Clear();
        }

        // ═══════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Calculates the next power of two greater than or equal to the given number.
        /// Used to ensure atlas dimensions are compatible with OpenGL texture requirements.
        /// </summary>
        /// <param name="n">The input number.</param>
        /// <returns>The next power of two.</returns>
        private static int NextPowerOfTwo(int n)
        {
            if (n <= 0) return 1;

            // ═══ Bit manipulation trick to find next power of two
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;

            return n + 1;
        }

        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Converts custom wrap mode enum to OpenGL enum.
        /// </summary>
        private static GLEnum ConvertWrapMode(SamplerWrapMode mode) => mode switch
        {
            SamplerWrapMode.Repeat => GLEnum.Repeat,
            SamplerWrapMode.ClampToEdge => GLEnum.ClampToEdge,
            SamplerWrapMode.MirroredRepeat => GLEnum.MirroredRepeat,
            _ => GLEnum.ClampToEdge
        };

        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Converts custom minification filter enum to OpenGL enum.
        /// </summary>
        private static GLEnum ConvertMinFilter(SamplerMinFilter filter) => filter switch
        {
            SamplerMinFilter.Nearest => GLEnum.Nearest,
            SamplerMinFilter.Linear => GLEnum.Linear,
            SamplerMinFilter.NearestMipmapNearest => GLEnum.NearestMipmapNearest,
            SamplerMinFilter.NearestMipmapLinear => GLEnum.NearestMipmapLinear,
            SamplerMinFilter.LinearMipmapNearest => GLEnum.LinearMipmapNearest,
            SamplerMinFilter.LinearMipmapLinear => GLEnum.LinearMipmapLinear,
            _ => GLEnum.NearestMipmapLinear
        };

        // ═══════════════════════════════════════════════════════════
        /// <summary>
        /// Converts custom magnification filter enum to OpenGL enum.
        /// </summary>
        private static GLEnum ConvertMagFilter(SamplerMagFilter filter) => filter switch
        {
            SamplerMagFilter.Nearest => GLEnum.Nearest,
            SamplerMagFilter.Linear => GLEnum.Linear,
            _ => GLEnum.Nearest
        };
    }
}