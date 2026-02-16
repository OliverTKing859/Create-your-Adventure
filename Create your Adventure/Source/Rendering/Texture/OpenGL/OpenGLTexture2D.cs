using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Texture;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Create_your_Adventure.Source.Rendering.Texture.OpenGL
{
    /// <summary>
    /// OpenGL-specific implementation of a 2D texture.
    /// Handles loading textures from files or raw data, uploading to GPU, and configuring texture parameters.
    /// Supports various filtering modes, wrapping modes, and automatic mipmap generation.
    /// </summary>
    public sealed class OpenGLTexture2D : ITexture
    {
        // ═══ The OpenGL context used for all texture operations
        private readonly GL gl;
        // ═══ The OpenGL handle (ID) for this texture
        private uint handle;
        // ═══ Flag to track whether this texture has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets the unique identifier name of this texture.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the texture has been successfully loaded into GPU memory.
        /// </summary>
        public bool IsLoaded { get; private set; }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new instance of the OpenGLTexture2D class.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for texture operations.</param>
        /// <param name="name">The unique name identifier for this texture.</param>
        /// <exception cref="ArgumentNullException">Thrown when glContext or name is null.</exception>
        public OpenGLTexture2D(GL glContext, string name)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD FROM FILE
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Loads texture data from an image file on disk.
        /// Supports common formats like PNG, JPG, BMP, TGA through StbImage.
        /// Automatically decodes the image and uploads it to GPU memory.
        /// </summary>
        /// <param name="path">The file path to the image file.</param>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        public void LoadFromFile(string path, TextureSettings settings)
        {
            if (IsLoaded)
            {
                Logger.Warn($"[TEXTURE] Texture '{Name}' already loaded");
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                Logger.Error($"[TEXTURE] Invalid path for texture '{Name}'");
                return;
            }

            Logger.Info($"[TEXTURE] Loading texture '{Name}' from: {path}");

            try
            {
                // ═══ Configure image loading to match OpenGL coordinate system if needed
                StbImage.stbi_set_flip_vertically_on_load(settings.FlipVertically ? 1 : 0);

                // ═══ Load and decode the image file
                using var stream = File.OpenRead(path);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                // ═══ Upload the decoded image data to GPU
                LoadFromData(image.Data, image.Width, image.Height, settings);
            }
            catch (FileNotFoundException)
            {
                Logger.Error($"[TEXTURE] File not found: {path}");
            }
            catch (Exception ex)
            {
                Logger.Error($"[TEXTURE] Failed to load '{Name}': {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD FROM DATA
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Loads texture data from a raw pixel array in memory.
        /// Creates an OpenGL texture object and uploads the pixel data to GPU.
        /// Useful for procedurally generated textures or custom image data.
        /// </summary>
        /// <param name="pixelData">The raw pixel data in RGBA format (4 bytes per pixel).</param>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        public unsafe void LoadFromData(byte[] pixelData, int width, int height, TextureSettings settings)
        {
            if (IsLoaded)
            {
                Logger.Warn($"[TEXTURE] Texture '{Name}' already loaded");
                return;
            }

            Width = width;
            Height = height;

            // ═══ Create and bind the OpenGL texture object
            handle = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, handle);

            // ═══ Upload pixel data to GPU memory
            fixed (byte* ptr = pixelData)
            {
                gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    (uint)width,
                    (uint)height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    ptr
                );
            }

            // ═══ Generate mipmaps for improved quality at different distances
            if (settings.GenerateMipmaps)
                gl.GenerateMipmap(TextureTarget.Texture2D);

            // ═══ Configure texture sampling parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)ConvertWrapMode(settings.WrapS));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)ConvertWrapMode(settings.WrapT));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)ConvertMinFilter(settings.MinFilter));
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)ConvertMagFilter(settings.MagFilter));

            IsLoaded = true;
            Logger.Info($"[TEXTURE] Texture '{Name}' loaded ({Width}x{Height})");
        }

        // ═══════════════════════════════════════════════════════════════
        // BIND
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Binds this texture to the specified texture unit for rendering.
        /// The texture must be loaded before it can be bound.
        /// </summary>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware). Default is 0.</param>
        public void Bind(uint unit = 0)
        {
            if (!IsLoaded)
            {
                Logger.Warn($"[TEXTURE] Cannot bind unloaded texture '{Name}'");
                return;
            }

            // ═══ Activate the specified texture unit and bind this texture
            gl.ActiveTexture(TextureUnit.Texture0 + (int)unit);
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }

        // ═══════════════════════════════════════════════════════════════
        // UNBIND
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Unbinds the texture from the current texture unit.
        /// Called to clean up state after rendering operations.
        /// </summary>
        public void Unbind() => gl.BindTexture(TextureTarget.Texture2D, 0);

        // ═══════════════════════════════════════════════════════════════
        // DISPOSE
        // ═══════════════════════════════════════════════════════════════
        /// <summary>
        /// Releases all GPU resources associated with this texture.
        /// Deletes the OpenGL texture object from GPU memory.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            if (handle != 0)
            {
                // ═══ Delete the texture from GPU memory
                gl.DeleteTexture(handle);
                handle = 0;
                Logger.Info($"[TEXTURE] Texture '{Name}' disposed");
            }

            isDisposed = true;
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════
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