using Create_your_Adventure.Source.Engine.Assets;
using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Window;
using Create_your_Adventure.Source.Rendering.Texture.OpenGL;

namespace Create_your_Adventure.Source.Engine.Texture
{
    /// <summary>
    /// Manages textures and texture atlases across the application with caching and API abstraction.
    /// Provides a singleton interface for loading, building, and binding textures.
    /// Supports multiple graphics API backends through factory pattern.
    /// </summary>
    public sealed class TextureManager : IDisposable
    {
        // ═══ Singleton instance of the TextureManager
        private static TextureManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the TextureManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
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

        // ═══ Factory function to create textures for the active graphics API
        private Func<string, ITexture>? textureFactory;
        // ═══ Factory function to create texture atlases for the active graphics API
        private Func<string, ITextureAtlas>? atlasFactory;

        // ═══ Cache dictionary storing all loaded textures by name
        private readonly Dictionary<string, ITexture> textureCache = [];
        // ═══ Cache dictionary storing all built texture atlases by name
        private readonly Dictionary<string, ITextureAtlas> atlasCache = [];

        // ═══ Name of the currently bound texture (for state tracking and optimization)
        private string? boundTexture;
        // ═══ Name of the currently bound atlas (for state tracking and optimization)
        private string? boundAtlas;
        // ═══ Flag to track whether this instance has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets a value indicating whether the TextureManager has been initialized with a graphics API backend.
        /// </summary>
        public bool IsInitialized => textureFactory is not null;

        /// <summary>
        /// Gets the total number of individual textures currently cached in memory.
        /// </summary>
        public int CachedTextureCount => textureCache.Count;

        /// <summary>
        /// Gets the total number of texture atlases currently cached in memory.
        /// </summary>
        public int CachedAtlasCount => atlasCache.Count;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private TextureManager()
        {
            // ═══ Intentionally empty - initialization happens in Initialize()
        }

        // ══════════════════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Initializes the TextureManager with the appropriate graphics API backend.
        /// Automatically detects available graphics contexts (currently OpenGL, future: Vulkan, DirectX).
        /// Must be called after WindowManager has been initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no supported graphics API context is found.</exception>
        public void Initialize()
        {
            if (IsInitialized) return;

            var gl = WindowManager.Instance.GlContext;

            if (gl is not null)
            {
                // ═══ OpenGL context is available -> use OpenGL factories
                textureFactory = name => new OpenGLTexture2D(gl, name);
                atlasFactory = name => new OpenGLTextureAtlas(gl, name);
                Logger.Info("[TEXTURE] TextureManager initialized (OpenGL backend)");
            }
            else // Future extension point: else if (vulkanContext is not null) { ... }
            {
                throw new InvalidOperationException("No supported Graphics API context found");
            }
        }

        // ══════════════════════════════════════════════════════════════
        // SINGEL TEXTURES (für UI, Skybox, etc.)
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Loads a texture from a file path.
        /// If a texture with the same name already exists in cache, returns the cached version.
        /// Otherwise, loads and caches a new texture.
        /// </summary>
        /// <param name="name">Unique identifier for this texture.</param>
        /// <param name="path">File path to the texture image.</param>
        /// <param name="settings">Texture configuration settings. If null, uses PixelArt preset.</param>
        /// <returns>The loaded texture.</returns>
        public ITexture LoadTexture(string name, string path, TextureSettings? settings = null)
        {
            EnsureInitialized();

            // ═══ Return cached texture if already loaded
            if (textureCache.TryGetValue(name, out var cached))
                return cached;

            settings ??= TextureSettings.PixelArt;

            // ═══ Create new texture using the factory and load from file
            var texture = textureFactory!(name);
            texture.LoadFromFile(path, settings);

            textureCache[name] = texture;
            Logger.Info($"[TEXTURE] Texture '{name}' loaded ({texture.Width}x{texture.Height})");

            return texture;
        }

        /// <summary>
        /// Loads a texture from the assets folder using a relative path.
        /// Convenience method that automatically resolves the full texture path.
        /// </summary>
        /// <param name="name">Unique identifier for this texture.</param>
        /// <param name="assetPath">Relative path within the assets/textures folder.</param>
        /// <param name="settings">Texture configuration settings. If null, uses PixelArt preset.</param>
        /// <returns>The loaded texture.</returns>
        public ITexture LoadTextureFromAssets(string name, string assetPath, TextureSettings? settings = null)
        {
            var fullPath = AssetLoader.GetTexturePath(assetPath);
            return LoadTexture(name, fullPath, settings);
        }

        // ══════════════════════════════════════════════════════════════
        // TEXTURE ATLAS (for Blocks, Items, etc.)
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Creates an empty texture atlas that can be populated with textures.
        /// Textures must be added using AddTexture() before calling Build().
        /// </summary>
        /// <param name="name">Unique identifier for this atlas.</param>
        /// <returns>The created texture atlas.</returns>
        public ITextureAtlas CreateAtlas(string name)
        {
            EnsureInitialized();

            if (atlasCache.TryGetValue(name, out var cached))
            {
                Logger.Warn($"[TEXTURE] Atlas '{name}' already exists");
                return cached;
            }

            // ═══ Create new atlas using the factory
            var atlas = atlasFactory!(name);
            atlasCache[name] = atlas;

            Logger.Info($"[TEXTURE] Atlas '{name}' created");
            return atlas;
        }

        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Creates and builds a texture atlas from all PNG files in a folder.
        /// Automatically scans the folder recursively, adds all textures, and builds the atlas.
        /// Texture names are derived from filenames without extension.
        /// </summary>
        /// <param name="name">Unique identifier for this atlas.</param>
        /// <param name="folderPath">Path to the folder containing texture images.</param>
        /// <param name="settings">Texture configuration settings. If null, uses Atlas preset.</param>
        /// <returns>The built texture atlas.</returns>
        public ITextureAtlas BuildAtlasFromFolder(string name, string folderPath, TextureSettings? settings = null)
        {
            EnsureInitialized();

            var atlas = CreateAtlas(name);
            settings ??= TextureSettings.Atlas;

            // ═══ Scan folder for all PNG files recursively
            var textureFiles = Directory.EnumerateFiles(folderPath, "*.png", SearchOption.AllDirectories);

            foreach (var file in textureFiles)
            {
                // ═══ Use filename without extension as texture name
                var textureName = Path.GetFileNameWithoutExtension(file);
                atlas.AddTexture(textureName, file);

                Logger.Info($"[TEXTURE] Added '{textureName}' to atlas '{name}'");
            }

            // ═══ Build the atlas (pack all textures and upload to GPU)
            atlas.Build(settings);

            Logger.Info($"[TEXTURE] Atlas '{name}' built ({atlas.Width}x{atlas.Height}, {atlas.TextureCount} textures)");

            return atlas;
        }

        // ══════════════════════════════════════════════════════════════
        // ACCESS & BINDING
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Builds the block texture atlas from the default blocks folder.
        /// Convenience method for loading all block textures into a single atlas.
        /// </summary>
        /// <returns>The built block atlas.</returns>
        public ITextureAtlas BuildBlockAtlas()
        {
            var blocksPath = Path.Combine("assets", "base", "textures", "blocks");
            return BuildAtlasFromFolder("blocks", blocksPath, TextureSettings.Atlas);
        }

        /// <summary>
        /// Retrieves a texture from the cache by name.
        /// </summary>
        /// <param name="name">The name of the texture to retrieve.</param>
        /// <returns>The texture if found, null otherwise.</returns>
        public ITexture? GetTexture(string name)
            => textureCache.TryGetValue(name, out var tex) ? tex : null;

        /// <summary>
        /// Retrieves a texture atlas from the cache by name.
        /// </summary>
        /// <param name="name">The name of the atlas to retrieve.</param>
        /// <returns>The atlas if found, null otherwise.</returns>
        public ITextureAtlas? GetAtlas(string name)
            => atlasCache.TryGetValue(name, out var atlas) ? atlas : null;

        /// <summary>
        /// Binds a texture to the specified texture unit for rendering.
        /// Only binds if the texture is not already active (state optimization).
        /// </summary>
        /// <param name="name">The name of the texture to bind.</param>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware). Default is 0.</param>
        public void BindTexture(string name, uint unit = 0)
        {
            // ═══ Skip if already bound (optimization)
            if (boundTexture == name) return;

            if (textureCache.TryGetValue(name, out var texture))
            {
                texture.Bind(unit);
                boundAtlas = name;
                boundTexture = null; // ═══ Clear atlas binding
            }
        }

        /// <summary>
        /// Binds a texture atlas to the specified texture unit for rendering.
        /// Only binds if the atlas is not already active (state optimization).
        /// </summary>
        /// <param name="name">The name of the atlas to bind.</param>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware).
        public void BindAtlas(string name, uint unit = 0)
        {
            // ═══ Skip if already bound (optimization)
            if (boundAtlas == name) return;

            if (atlasCache.TryGetValue(name, out var atlas))
            {
                atlas.Bind(unit);
                boundAtlas = name;
                boundTexture = null; // ═══ Clear texture binding
            }
        }

        /// <summary>
        /// Retrieves the atlas region for a specific block texture.
        /// Convenience method for accessing block UVs from the block atlas.
        /// </summary>
        /// <param name="blockName">The name of the block texture.</param>
        /// <returns>The atlas region containing UV coordinates for the block.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the block atlas has not been loaded.</exception>
        public AtlasRegion GetBlockUV(string blockName)
        {
            var atlas = GetAtlas("blocks");
            if (atlas is null)
                throw new InvalidOperationException("Block atlas not loaded");

            return atlas.GetRegion(blockName);
        }

        // ══════════════════════════════════════════════════════════════
        // ENSURE INITIALIZED
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Internal validation method to ensure the TextureManager is initialized before use.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if TextureManager has not been initialized.</exception>
        private void EnsureInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("TextureManager must be initialized first");
        }

        // ══════════════════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════════════════
        /// <summary>
        /// Disposes of all cached textures and atlases, releasing GPU resources.
        /// Clears both caches and resets binding state.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            // ═══ Dispose all cached textures
            foreach (var texture in textureCache.Values)
                texture.Dispose();

            // ═══ Dispose all cached atlases
            foreach (var atlas in atlasCache.Values)
                atlas.Dispose();

            textureCache.Clear();
            atlasCache.Clear();
            isDisposed = true;

            Logger.Info("[TEXTURE] TextureManager disposed");
        }
    }
}