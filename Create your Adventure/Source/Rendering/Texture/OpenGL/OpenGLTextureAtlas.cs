using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Texture;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Rendering.Texture.OpenGL
{
    public sealed class OpenGLTextureAtlas : ITextureAtlas
    {
        private readonly GL gl;
        private uint handle;

        // ═══ Pending Texturen (before Build)
        private readonly List<(string Name, string Path)> pendingTextures = [];

        // ═══ Built Region (after Build)
        private readonly Dictionary<string, AtlasRegion> regions = [];

        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TextureCount => regions.Count;
        public bool IsBuilt { get; private set; }

        public OpenGLTextureAtlas(GL glContext, string name)
        {
            gl = glContext;
            Name = name;
        }

        public void AddTexture(string name, string path)
        {
            if (IsBuilt)
                throw new InvalidOperationException("Cannot add textures after Build()");

            pendingTextures.Add((name, path));
        }

        public Build(TextureSettings settings)
        {
            if (IsBuilt) return;
            if (pendingTextures.Count == 0)
                throw new InvalidOperationException("No textures to build atlas from");

            Logger.Info($"[TEXTURE] Building atlas '{Name}' from {pendingTextures.Count} textures...");

            // ═══════════════════════════════════════════════════════════
            // Step 1: Load all Images
            // ═══════════════════════════════════════════════════════════
            var loadedImages = new List<(string Name, byte[] Data, int Width, int Height)>();

            StbImage.stbi_set_flip_vertically_on_load(settings.FlipVertically ? 1 : 0);

            foreach (var(name, path) in pendingTextures)
            {
                using var stream = File.OpenRead(path);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                loadedImages.Add((name, image.Data, image.Width, image.Height));
            }

            // ═══════════════════════════════════════════════════════════
            // Step 2: Calculate Atlas-Size (Power of 2)
            // ═══════════════════════════════════════════════════════════
            int totalPixels = loadedImages.Sum(img => img.Width * img.Height);
            int atlasSize = NextPowerOfTwo((int)Math.Ceiling(Math.Sqrt(totalPixels)));


            atlasSize = Math.Max(atlasSize, 256);   // ═══ Minimum 256x256
            atlasSize = Math.Min(atlasSize, 4096);  // ═══ Maximum 4096x4096

            Width = atlasSize;
            Height = atlasSize;

            // ═══════════════════════════════════════════════════════════
            // Step 3: Packing Textures (Simple Grid for same Size)
            // ═══════════════════════════════════════════════════════════
            byte[] atlasData = new byte[Width * Height * 4]; // ═══ RGBA

            // ═══ Assumption
            int textureSize = loadedImages[0].Width;
            int texturePerRow = Width / textureSize;

            int currentX = 0;
            int currentY = 0;

            foreach (var (name, data, w, h) in loadedImages)
            {
                // ═══ Save Region
                regions[name] = AtlasRegion.Create(name, currentX, currentY, w, h, Width, Height);

                // ═══ Copy Pixel in Atlas
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int srcIndex = (y * w + x) * 4;
                        int dstIndex = ((currentY + y) * Width + (currentX + x)) * 4;

                        atlasData[dstIndex + 0] = data[srcIndex + 0];
                        atlasData[dstIndex + 1] = data[srcIndex + 1];
                        atlasData[dstIndex + 2] = data[srcIndex + 2];
                        atlasData[dstIndex + 3] = data[srcIndex + 3];
                    }
                }

                // ═══ next Position
                currentX += textureSize;
                if (currentX >= Width)
                {
                    currentX = 0;
                    currentY += textureSize;
                }
            }

            // ═══════════════════════════════════════════════════════════
            // Step 4: OpenGL create Textures
            // ═══════════════════════════════════════════════════════════
            handle = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, handle);

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

            // ═══ Texture Parameters
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
            pendingTextures.Clear();

            Logger.Info($"[TEXTURE] Atlas '{Name}' built: {Width}x{Height}, {TextureCount} textures");
        }

        public AtlasRegion GetRegion(string name)
        {
            if (!IsBuilt)
                throw new InvalidOperationException("Atlas must be built first");

            if (regions.TryGetValue(name, out var region))
                return region;

            Logger.Warn($"[TEXTURE] Region '{name}' not found in atlas '{Name}'");
            return default;
        }

        public bool HasTexture(string name) => regions.ContainsKey(name);

        public IEnumerable<string> GetAllTextureNames() => regions.Keys;

        public void Bind(uint unit = 0)
        {
            gl.ActiveTexture(TextureUnit.Texture0 + (int)unit);
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Unbind() => gl.BindTexture(TextureTarget.Texture2D, 0);

        public void Dispose()
        {
            if (handle !=0)
            {
                gl.DeleteTexture(handle);
                Logger.Info($"[TEXTURE] Atlas '{Name}' disposed");
            }
        }

        // ═══ Helper Methods
        private static int NextPowerOfTwo(int n)
        { 
        
        }

        private static GLEnum ConvertWrapMode(TextureWrapMode mode)
        {

        }

        private static GLEnum ConvertMinFilter(TextureMinFilter filter)
        {

        }

        private static GLEnum ConvertMagFilter(TextureMagFilter filter)
        {

        }
    }
}
