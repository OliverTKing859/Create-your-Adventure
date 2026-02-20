using Create_your_Adventure.Source.Rendering.Shader.OpenGL;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using Create_your_Adventure.Source.Debug;

namespace Create_your_Adventure.Source.Engine.Shader
{
    /// <summary>
    /// Manages shader programs across the application with caching and API abstraction.
    /// Provides a singleton interface for loading, compiling, and accessing shader programs.
    /// Supports multiple graphics API backends through factory pattern.
    /// </summary>
    public sealed class ShaderManager : IDisposable
    {
        // ═══ Singleton instance of the ShaderManager
        private static ShaderManager? instance;
        // ═══ Lock object to ensure thread-safe singleton initialization
        private static readonly Lock instanceLock = new();

        /// <summary>
        /// Gets the singleton instance of the ShaderManager.
        /// Creates a new instance if one doesn't exist yet (thread-safe).
        /// </summary>
        public static ShaderManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (instanceLock)
                    {
                        instance ??= new ShaderManager();
                    }
                }

                return instance;
            }
        }

        // ═══ Cache dictionary storing all loaded shader programs by name
        private readonly Dictionary<string, IShaderProgram> shaderCache = [];
        // ═══ Factory function to create shader programs for the active graphics API
        private Func<string, IShaderProgram>? shaderFactory;
        // ═══ Name of the currently active shader program (for state tracking)
        private string? activeProgram;
        // ═══ Flag to track whether this instance has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets a value indicating whether the ShaderManager has been initialized with a graphics API backend.
        /// </summary>
        public bool IsInitialized => shaderFactory is not null;

        /// <summary>
        /// Gets the total number of shader programs currently cached in memory.
        /// </summary>
        public int CachedProgramCount => shaderCache.Count;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private ShaderManager()
        {
            // ═══ Intentionally empty - initialization happens in Initialize()
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the ShaderManager with the appropriate graphics API backend.
        /// Automatically detects available graphics contexts (currently OpenGL, future: Vulkan, DirectX).
        /// Must be called after WindowManager has been initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no supported graphics API context is found.</exception>
        public void Initialize()
        {
            if (IsInitialized)
            {
                Logger.Warn("[SHADER] ShaderManager already initialized");
                return;
            }

            var gl = WindowManager.Instance.GlContext;

            if (gl is not null)
            {
                // ═══ OpenGL context is available -> use OpenGL factory
                shaderFactory = name => new OpenGLShaderProgram(gl, name);
                Logger.Info("[SHADER] ShaderManager initialized (OpenGL backend)");
            }
            // ═══ Future extension point: else if (vulkanContext is not null) { ... }
            else
            {
                throw new InvalidOperationException("No supported Graphics API context found");
            }
        }

        // ══════════════════════════════════════════════════
        // LOAD / CREATE (API-agnostic)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Loads or creates a shader program from source code.
        /// If a program with the same name already exists in cache, returns the cached version.
        /// Otherwise, compiles a new program and adds it to the cache.
        /// </summary>
        /// <param name="name">Unique identifier for this shader program.</param>
        /// <param name="vertexSource">The vertex shader source code (GLSL format).</param>
        /// <param name="fragmentSource">The fragment shader source code (GLSL format).</param>
        /// <returns>The compiled shader program.</returns>
        public IShaderProgram LoadProgram(string name, string vertexSource, string fragmentSource)
        {
            EnsureInitialized();

            // ═══ Check if program is already cached
            if (shaderCache.TryGetValue(name, out var cached))
            {
                Logger.Info($"[SHADER] Program '{name}' loaded from cache");
                return cached;
            }

            // ═══ Create new shader program using the factory
            var program = shaderFactory(name);
            program.Compile(vertexSource, fragmentSource);

            shaderCache[name] = program;
            Logger.Info($"[SHADER] Program '{name}' created and cached (Total: {shaderCache.Count})");

            return program;
        }

        // ══════════════════════════════════════════════════
        // LOAD FROM FILE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Loads a shader program from vertex and fragment shader files.
        /// Returns cached version if the program was previously loaded.
        /// </summary>
        /// <param name="name">Unique identifier for this shader program.</param>
        /// <param name="vertexPath">File path to the vertex shader (.vert or .glsl).</param>
        /// <param name="fragmentPath">File path to the fragment shader (.frag or .glsl).</param>
        /// <returns>The compiled shader program.</returns>
        public IShaderProgram LoadFromFiles(string name, string vertexPath, string fragmentPath)
        {
            if (shaderCache.TryGetValue(name, out var cached))
            {
                return cached;
            }

            Logger.Info($"[SHADER] Loading program '{name}' from files...");
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            return LoadProgram(name, vertexSource, fragmentSource);
        }

        // ══════════════════════════════════════════════════
        // ACCESS / USE (API-agnostic)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Retrieves a shader program from the cache by name.
        /// </summary>
        /// <param name="name">The name of the shader program to retrieve.</param>
        /// <returns>The shader program if found, null otherwise.</returns>
        public IShaderProgram? GetProgram(string name)
            => shaderCache.TryGetValue(name, out var program) ? program : null;

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Activates a shader program for rendering.
        /// Only binds the shader if it's not already active (optimization).
        /// </summary>
        /// <param name="name">The name of the shader program to activate.</param>
        /// <returns>The activated shader program, or null if not found.</returns>
        public IShaderProgram? UseProgram(string name)
        {
            if (!shaderCache.TryGetValue(name, out var program))
            {
                Logger.Warn($"[SHADER] Program '{name}' not found");
                return null;
            }

            // ═══ Only bind if this program is not already active (state optimization)
            if (activeProgram != name)
            {
                program.Use();
                activeProgram = name;
            }

            return program;
        }

        // ══════════════════════════════════════════════════ 
        /// <summary>
        /// Checks whether a shader program with the specified name exists in the cache.
        /// </summary>
        /// <param name="name">The name of the shader program to check.</param>
        /// <returns>True if the program exists in cache, false otherwise.</returns>
        public bool HasProgram(string name) => shaderCache.ContainsKey(name);

        // ══════════════════════════════════════════════════
        // ENSURE INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Internal validation method to ensure the ShaderManager is initialized before use.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if ShaderManager has not been initialized.</exception>
        private void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("ShaderManager must be initialized first");
            }
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Disposes of all cached shader programs and releases GPU resources.
        /// Clears the shader cache and resets the active program state.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            Logger.Info($"[SHADER] Disposing {shaderCache.Count} shader program(s)...");

            // Dispose all cached shader programs
            foreach (var program in shaderCache.Values)
            {
                program.Dispose();
            }

            shaderCache.Clear();
            activeProgram = null;
            isDisposed = true;

            Logger.Info("[SHADER] ShaderManager disposed");
        }
    }
}