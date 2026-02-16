using Create_your_Adventure.Source.Engine.AssetLoader;
using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Shader;
using Create_your_Adventure.Source.Engine.Window;
using Create_your_Adventure.Source.Rendering.Texture.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture
{
    public sealed class TextureManager : IDisposable
    {
        private static TextureManager? instance;
        private static readonly Lock instanceLock = new();

        public static TextureManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new TextureManager();
                    }
                }

                return instance;
            }
        }

        // ══════════════════════════════════════════════════════════════
        // FIELDS (API-agnostic!)
        // ══════════════════════════════════════════════════════════════

        // ═══ Factory Function for different APIs
        private Func<string, ITexture>? textureFactory;
        private Func<string, ITextureAtlas>? atlasFactory;

        // ═══ Caches
        private readonly Dictionary<string, ITexture> textureCache = [];
        private readonly Dictionary<string, ITextureAtlas> atlasCache = [];

        // ═══ State Tracking
        private string? boundTexture;
        private string? boundAtlas;
        private bool isDisposed;

        public bool IsInitialized => textureFactory is not null;
        public int CachedTextureCount => textureCache.Count;
        public int CachedAtlasCount => atlasCache.Count;

        // ══════════════════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════════════════

        public void Initialize()
        {
            if (IsInitialized) return;

            var gl = WindowManager.Instance.GlContext;

            if (gl is not null)
            {
                // ═══ OpenGL Backend
                textureFactory = name => new OpenGLTexture2D(gl, name);
                atlasFactory = name => new OpenGLTextureAtlas(gl, name);
                Logger.Info("[TEXTURE] TextureManager initialized (OpenGL backend)");
            }
            else // ═══ Zukünftig: else if (vulkanContext is not null) { ... }
            {
                throw new InvalidOperationException("No supported Graphics API context found");
            }
        }

        // ══════════════════════════════════════════════════════════════
        // SINGEL TEXTURES (für UI, Skybox, etc.)
        // ══════════════════════════════════════════════════════════════

        public ITexture LoadTexture(string name, string path, TextureSettings? settings = null)
        {
            EnsureInitialized();

            if (textureCache.TryGetValue(name, out var cached))
                return cached;

            settings ??= TextureSettings.PixelArt;

            var texture = textureFactory!(name);
            texture.LoadFromFile(path, settings);

            textureCache[name] = texture;
            Logger.Info($"[TEXTURE] Texture '{name}' loaded ({texture.Width}x{texture.Height})");

            return texture;
        }

        public ITexture LoadTextureFromAssets(string name, string assetPath, TextureSettings? settings = null)
        {
            var fullPath = AssetLoader.GetTexturePath(assetPath);
            return LoadTexture(name, fullPath, settings);
        }

        // ══════════════════════════════════════════════════════════════
        // TEXTURE ATLAS (for Blocks, Items, etc.)
        // ══════════════════════════════════════════════════════════════

        public ITextureAtlas CreateAtlas(string name)
        {
            EnsureInitialized();

            if (atlasCache.TryGetValue(name, out var cached))
            {
                Logger.Warn($"[TEXTURE] Atlas '{name}' already exists");
                return cached;
            }

            var atlas = atlasFactory!(name);
            atlasCache[name] = atlas;

            Logger.Info($"[TEXTURE] Atlas '{name}' created");
            return atlas;
        }

        public ITextureAtlas BuildAtlasFromFolder(string name, string folderPath, TextureSettings? settings = null)
        {
            EnsureInitialized();

            var atlas = CreateAtlas(name);
            settings ??= TextureSettings.Atlas;

            var textureFiles = Directory.EnumerateFiles(folderPath, "*.png", SearchOption.AllDirectories);

            foreach (var file in textureFiles)
            {
                var textureName = Path.GetFileNameWithoutExtension(file);
                atlas.AddTexture(textureName, file);

                Logger.Info($"[TEXTURE] Added '{textureName}' to atlas '{name}'");
            }

            atlas.Build(settings);

            Logger.Info($"[TEXTURE] Atlas '{name}' built ({atlas.Width}x{atlas.Height}, {atlas.TextureCount} textures)");

            return atlas;
        }

        // ══════════════════════════════════════════════════════════════
        // ACCESS & BINDING
        // ══════════════════════════════════════════════════════════════

        public ITextureAtlas BuildBlockAtlas()
        {
            var blocksPath = Path.Combine("assets", "base", "textures", "blocks");
            return BuildAtlasFromFolder("blocks", blocksPath, TextureSettings.Atlas);
        }
        public ITexture? GetTexture(string name)
            => textureCache.TryGetValue(name, out var tex) ? tex : null;

        public ITextureAtlas? GetAtlas(string name)
            => atlasCache.TryGetValue(name, out var atlas) ? atlas : null;

        public void BindTexture(string name, uint unit = 0)
        {
            // ═══ aready binded
            if (boundTexture == name) return;

            if (textureCache.TryGetValue(name, out var texture))
            {
                texture.Bind(unit);
                boundAtlas = name;
                boundTexture = null;
            }
        }

        public void BindAtlas(string name, uint unit = 0)
        {
            // ═══ aready binded
            if (boundAtlas == name) return;

            if (atlasCache.TryGetValue(name, out var atlas))
            {
                atlas.Bind(unit);
                boundAtlas = name;
                boundTexture = null;
            }
        }

        public AtlasRegion GetBlockUV(string blockName)
        {
            var atlas = GetAtlas("blocks");
            if (atlas is null)
                throw new InvalidOperationException("Block atlas not loaded");

            return atlas.GetRegion(blockName);
        }

        // ══════════════════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════════════════

        public void Dispose()
        {
            if (isDisposed) return;

            foreach (var texture in textureCache.Values)
                texture.Dispose();

            foreach (var atlas in atlasCache.Values)
                atlas.Dispose();

            textureCache.Clear();
            atlasCache.Clear();
            isDisposed = true;

            Logger.Info("[TEXTURE] TextureManager disposed");
        }
    }
}