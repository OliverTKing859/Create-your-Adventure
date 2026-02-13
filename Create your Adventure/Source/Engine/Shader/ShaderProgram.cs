using Create_your_Adventure.Source.Engine.DevDebug;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Shader
{
    public sealed class ShaderProgram : IDisposable
    {
        private readonly GL gl;
        private readonly Dictionary<string, int> uniformCache = [];
        private bool isDisposed;

        public uint Handle { get; private set; }

        public string Name { get; }

        public bool IsCompiled { get; private set; }

        public ShaderProgram(GL glContext, string name)
        {
            gl = glContext ?? throw new ArgumentNullException(nameof(glContext));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Compile(string vertexSource, string fragmentSource)
        {
            if (IsCompiled)
            {
                Logger.Warn($"[SHADER] Program '{Name}' is already compiled");
                return;
            }

            Logger.Info($"[SHADER] Compiling program '{Name}'...");

            // VERTEX SHADER ----------------------------------------------------------------
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexSource);
            gl.CompileShader(vertexShader);

            if (!CheckShaderCompileStatus(vertexShader, "VERTEX"))
            {
                gl.DeleteShader(vertexShader);
                throw new InvalidOperationException($"Vertex shader compilation failed for '{Name}'");
            }

            // FRAGMENT SHADER ----------------------------------------------------------------
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSource);
            gl.CompileShader(fragmentShader);

            if (!CheckShaderCompileStatus(fragmentShader, "FRAGMENT"))
            {
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                throw new InvalidOperationException($"Shader program linking failed for '{Name}'");
            }

            Handle = gl.CreateProgram();
            gl.AttachShader(Handle, vertexShader);
            gl.AttachShader(Handle, fragmentShader);
            gl.LinkProgram(Handle);

            if (!CheckProgramLinkStatus())
            {
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                gl.DeleteProgram(Handle);
                Handle = 0;
                throw new InvalidOperationException($"Shader program linking failed for '{Name}'");
            }

            gl.DetachShader(Handle, vertexShader);
            gl.DetachShader(Handle, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            IsCompiled = true;
            Logger.Info($"[SHADER] Program '{Name}' compiled successfully (Handle: {Handle})");

        }
        public void Use()
        {
            if (!IsCompiled)
            {
                Logger.Warn($"[SHADER] Cannot use uncompiled program '{Name}'");
                return;
            }

            gl.UseProgram(Handle);
        }

        public int GetUniformLocation(string name)
        {
            if (uniformCache.TryGetValue(name, out int location))
            {
                return location;
            }

            location = gl.GetUniformLocation(Handle, name);

            if (location == -1)
            {
                Logger.Warn($"[SHADER] Uniform '{name}' not found in program '{Name}'");
            }

            uniformCache[name] = location;
            return location;
        }

        public void SetUniform(string name, int value)
        {
            gl.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUniform(string name, float value)
        {
            gl.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUniform(string name, Vector2D<float> value)
        {
            gl.Uniform2(GetUniformLocation(name), value.X, value.Y);
        }

        public void SetUniform(string name, Vector3D<float> value)
        {
            gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }

        public void SetUniform(string name, Vector4D<float> value)
        {
            gl.Uniform4(GetUniformLocation(name), value.X, value.Y, value.Z, value.W);
        }

        public unsafe void SetUniform(string name, Matrix4X4<float> value)
        {
            gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);
        }

        private bool CheckShaderCompileStatus(uint shader, string type)
        {
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);

            if (success == 0)
            {
                string infoLog = gl.GetShaderInfoLog(shader);
                Logger.Error($"[SHADER] {type} compilation failed for '{Name}':\n{infoLog}");
                return false;
            }

            Logger.Info($"[SHADER] {type} shader compiled successfully");
            return true;
        }

        private bool CheckProgramLinkStatus()
        {
            gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int success);

            if (success == 0)
            {
                string infoLog = gl.GetProgramInfoLog(Handle);
                Logger.Error($"[SHADER] Program linking failed for '{Name}':\n{infoLog}");
                return false;
            }

            Logger.Info($"[SHADER] Program '{Name}' linked successfully");
            return true;
        }

        public void Dispose()
        {
            if (isDisposed) return;

            if (Handle != 0)
            {
                gl.DeleteProgram(Handle);
                Logger.Info($"[SHADER] Program '{Name}' disposed");
            }

            uniformCache.Clear();
            isDisposed = true;
        }
    }
}
