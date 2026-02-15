using Create_your_Adventure.Source.Engine.DevDebug;
using Create_your_Adventure.Source.Engine.Window;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Shader
{
    public sealed class ShaderManager : IDisposable
    {
        private static ShaderManager? instance;
        private static readonly Lock instanceLock = new();

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

        private GL? gl;
        private readonly Dictionary<string, ShaderProgram> shaderCache = [];
        private string? activeProgram;
        private bool isDisposed;

        public bool IsInitialized => gl is not null;

        private ShaderManager()
        {

        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                Logger.Warn("[SHADER] ShaderManager already initialized");
                return;
            }

            gl = WindowManager.Instance.GlContext ?? throw new InvalidOperationException(
                "WindowManager must be loaded before initializing ShaderManager"
                );

            Logger.Info("[SHADER] ShaderManager initialized");
        }

        public ShaderProgram LoadProgram(string name, string vertexSource, string fragmentSource)
        {
            EnsureInitialized();

            if (shaderCache.TryGetValue(name, out var cached))
            {
                Logger.Info($"[SHADER] Program '{name}' loaded from cache");
                return cached;
            }

            var program = new ShaderProgram(gl!, name);
            program.Compile(vertexSource, fragmentSource);

            shaderCache[name] = program;
            Logger.Info($"[SHADER] Program '{name}' created and cached (Total: {shaderCache.Count})");

            return program;
        }

        public ShaderProgram LoadFromFiles(string name, string vertexPath, string fragmentPath)
        {
            EnsureInitialized();

            if (shaderCache.TryGetValue(name, out var cached))
            {
                Logger.Info($"[SHADER] Program '{name}' loaded from cache");
                return cached;
            }

            Logger.Info($"[SHADER] Loading program '{name}' from files...");

            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            return LoadProgram(name, vertexSource, fragmentSource);
        }

        public ShaderProgram? GetProgram(string name)
        {
            return shaderCache.TryGetValue(name, out var program) ? program : null;
        }

        public ShaderProgram? UseProgram(string name)
        {
            if (!shaderCache.TryGetValue(name, out var program))
            {
                Logger.Warn($"[SHADER] Program '{name}' not found");
                return null;
            }

            if (activeProgram != name)
            {
                program.Use();
                activeProgram = name;
            }

            return program;
        }

        public void UnbindProgram()
        {
            gl?.UseProgram(0);
            activeProgram = null;
        }

        public bool HasProgram(string name) => shaderCache.ContainsKey(name);

        public int CachedProgramCount => shaderCache.Count;

        private void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("ShaderManager must be initialized first");
            }
        }

        public void Dispose()
        {
            if (isDisposed) return;

            Logger.Info($"[SHADER] Disposing {shaderCache.Count} shader program(s)...");

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
