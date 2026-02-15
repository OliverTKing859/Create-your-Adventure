using Create_your_Adventure.Source.Engine.DevDebug;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Shader.OpenGL
{
    /// <summary>
    /// OpenGL-specific implementation of a shader program.
    /// Handles compilation, linking, and uniform management for vertex and fragment shaders.
    /// Implements caching for uniform locations to optimize performance.
    /// </summary>
    public sealed class OpenGLShaderProgram : IShaderProgram
    {
        // ═══ The OpenGL context used for all shader operations
        private readonly GL gl;
        // ═══ Cache for uniform variable locations to avoid repeated OpenGL queries
        private readonly Dictionary<string, int> uniformCache = [];
        // ═══ Flag to track whether this shader program has been disposed
        private bool isDisposed;

        /// <summary>
        /// Gets the OpenGL handle (ID) for this shader program.
        /// Used internally for OpenGL operations.
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// Gets the unique name identifier of this shader program.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this shader program has been successfully compiled and linked.
        /// </summary>
        public bool IsCompiled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the OpenGLShaderProgram class.
        /// </summary>
        /// <param name="glContext">The OpenGL context to use for shader operations.</param>
        /// <param name="name">The unique name identifier for this shader program.</param>
        /// <exception cref="ArgumentNullException">Thrown when glContext or name is null.</exception>
        public OpenGLShaderProgram(GL glContext, string name)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        // ══════════════════════════════════════════════════
        // COMPILER
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Compiles and links the vertex and fragment shaders into a complete shader program.
        /// Creates individual shader objects, compiles them, links them into a program,
        /// and performs cleanup of intermediate shader objects.
        /// </summary>
        /// <param name="vertexSource">The GLSL source code for the vertex shader.</param>
        /// <param name="fragmentSource">The GLSL source code for the fragment shader.</param>
        /// <exception cref="InvalidOperationException">Thrown when shader compilation or program linking fails.</exception>
        public void Compile(string vertexSource, string fragmentSource)
        {
            if (IsCompiled)
            {
                Logger.Warn($"[SHADER] Program '{Name}' is already compiled");
                return;
            }

            Logger.Info($"[SHADER] Compiling program '{Name}'...");

            // ══════════════════════════════════════════════════
            // VERTEX SHADER
            // ══════════════════════════════════════════════════
            // ═══ Create and compile the vertex shader
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexSource);
            gl.CompileShader(vertexShader);

            // ═══ Check for compilation errors
            if (!CheckShaderCompileStatus(vertexShader, "VERTEX"))
            {
                gl.DeleteShader(vertexShader);
                throw new InvalidOperationException($"Vertex shader compilation failed for '{Name}'");
            }

            // ══════════════════════════════════════════════════
            // FRAGMENT SHADER
            // ══════════════════════════════════════════════════
            // ═══ Create and compile the fragment shader
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSource);
            gl.CompileShader(fragmentShader);

            // ═══ Check for compilation errors
            if (!CheckShaderCompileStatus(fragmentShader, "FRAGMENT"))
            {
                // ═══ Clean up vertex shader before throwing
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                throw new InvalidOperationException($"Shader program linking failed for '{Name}'");
            }

            // ══════════════════════════════════════════════════
            // PROGRAM LINKING
            // ══════════════════════════════════════════════════
            // ═══ Create the shader program and attach both shaders
            Handle = gl.CreateProgram();
            gl.AttachShader(Handle, vertexShader);
            gl.AttachShader(Handle, fragmentShader);
            gl.LinkProgram(Handle);

            // ═══ Check if linking was successful
            if (!CheckProgramLinkStatus())
            {
                // ═══ Clean up all resources on failure
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                gl.DeleteProgram(Handle);
                Handle = 0;
                throw new InvalidOperationException($"Shader program linking failed for '{Name}'");
            }

            // ═══ Detach and delete individual shaders as they're no longer needed after linking
            gl.DetachShader(Handle, vertexShader);
            gl.DetachShader(Handle, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            IsCompiled = true;
            Logger.Info($"[SHADER] Program '{Name}' compiled successfully (Handle: {Handle})");

        }

        // ══════════════════════════════════════════════════
        // USING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Activates this shader program for subsequent rendering operations.
        /// All draw calls after this will use this shader until another shader is activated.
        /// </summary>
        public void Use()
        {
            if (!IsCompiled)
            {
                Logger.Warn($"[SHADER] Cannot use uncompiled program '{Name}'");
                return;
            }

            // ═══ Bind this shader program to the OpenGL context
            gl.UseProgram(Handle);
        }

        // ══════════════════════════════════════════════════
        // UNIFORMS IMPLEMENTATION (with Caching)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Retrieves the location of a uniform variable in the shader program.
        /// Uses caching to avoid repeated OpenGL queries for the same uniform.
        /// </summary>
        /// <param name="name">The name of the uniform variable as declared in the shader.</param>
        /// <returns>The uniform location, or -1 if the uniform was not found.</returns>
        public int GetUniformLocation(string name)
        {
            // ═══ Check if the location is already cached
            if (uniformCache.TryGetValue(name, out int location))
            {
                return location;
            }

            // ═══ Query OpenGL for the uniform location
            location = gl.GetUniformLocation(Handle, name);

            if (location == -1)
            {
                Logger.Warn($"[SHADER] Uniform '{name}' not found in program '{Name}'");
            }

            // ═══ Cache the location for future use (optimization)
            uniformCache[name] = location;
            return location;
        }

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets an integer uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The integer value to set.</param>
        public void SetUniform(string name, int value)
            => gl.Uniform1(GetUniformLocation(name), value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a floating-point uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The float value to set.</param>
        public void SetUniform(string name, float value)
            => gl.Uniform1(GetUniformLocation(name), value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 2D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 2D vector value to set.</param>
        public void SetUniform(string name, Vector2D<float> value)
            => gl.Uniform2(GetUniformLocation(name), value.X, value.Y);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 3D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 3D vector value to set.</param>
        public void SetUniform(string name, Vector3D<float> value)
            => gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 4D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 4D vector value to set.</param>
        public void SetUniform(string name, Vector4D<float> value)
            => gl.Uniform4(GetUniformLocation(name), value.X, value.Y, value.Z, value.W);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 4x4 matrix uniform variable in the shader.
        /// Commonly used for transformation matrices (model, view, projection).
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 4x4 matrix value to set.</param>
        public unsafe void SetUniform(string name, Matrix4X4<float> value)
            // ═══ Pass the matrix data directly to OpenGL (transpose = false means column-major order)
            => gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);

        // ══════════════════════════════════════════════════
        // ERROR CHECKING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks whether a shader compiled successfully and logs any compilation errors.
        /// </summary>
        /// <param name="shader">The OpenGL shader handle to check.</param>
        /// <param name="type">The type of shader (for logging purposes, e.g., "VERTEX" or "FRAGMENT").</param>
        /// <returns>True if compilation succeeded, false otherwise.</returns>
        private bool CheckShaderCompileStatus(uint shader, string type)
        {
            // ═══ Query the compilation status
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);

            if (success == 0)
            {
                // ═══ Get and log the detailed error message
                string infoLog = gl.GetShaderInfoLog(shader);
                Logger.Error($"[SHADER] {type} compilation failed for '{Name}':\n{infoLog}");
                return false;
            }

            Logger.Info($"[SHADER] {type} shader compiled successfully");
            return true;
        }

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Checks whether the shader program linked successfully and logs any linking errors.
        /// </summary>
        /// <returns>True if linking succeeded, false otherwise.</returns>
        private bool CheckProgramLinkStatus()
        {
            // ═══ Query the link status
            gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int success);

            if (success == 0)
            {
                // ═══ Get and log the detailed error message
                string infoLog = gl.GetProgramInfoLog(Handle);
                Logger.Error($"[SHADER] Program linking failed for '{Name}':\n{infoLog}");
                return false;
            }

            Logger.Info($"[SHADER] Program '{Name}' linked successfully");
            return true;
        }

        // ══════════════════════════════════════════════════
        // DISPOSE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Releases all GPU resources associated with this shader program.
        /// Deletes the OpenGL program object and clears the uniform cache.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            if (Handle != 0)
            {
                // ═══ Delete the shader program from GPU memory
                gl.DeleteProgram(Handle);
                Logger.Info($"[SHADER] Program '{Name}' disposed");
            }

            // ═══ Clear the uniform location cach
            uniformCache.Clear();
            isDisposed = true;
        }
    }
}