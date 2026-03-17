using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Shader
{
    /// <summary>
    /// Defines the contract for a shader program that can compile and execute vertex and fragment shaders.
    /// Provides API-agnostic uniform setters for passing data to shaders.
    /// </summary>
    public interface IShaderProgram : IDisposable
    {
        /// <summary>
        /// Gets the name identifier of this shader program.
        /// Used for debugging and shader management.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the shader program has been successfully compiled and linked.
        /// </summary>
        bool IsCompiled { get; }

        /// <summary>
        /// Compiles and links the shader program from vertex and fragment shader source code.
        /// </summary>
        /// <param name="vertexSource">The GLSL source code for the vertex shader.</param>
        /// <param name="fragmentSource">The GLSL source code for the fragment shader.</param>
        void Compile(string vertexSource, string fragmentSource);

        /// <summary>
        /// Activates this shader program for subsequent rendering operations.
        /// All draw calls after this will use this shader until another shader is activated.
        /// </summary>
        void Use();

        // ══════════════════════════════════════════════════
        // UNIFORM SETTERS (API-Agnostic)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets an integer uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The integer value to set.</param>
        void SetUniform(string name, int value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a floating-point uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The float value to set.</param>
        void SetUniform(string name, float value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 2D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The 2D vector value to set.</param>
        void SetUniform(string name, Vector2D<float> value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 3D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The 3D vector value to set.</param>
        void SetUniform(string name, Vector3D<float> value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 4D vector uniform variable in the shader.
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The 4D vector value to set.</param>
        void SetUniform(string name, Vector4D<float> value);

        // ══════════════════════════════════════════════════
        /// <summary>
        /// Sets a 4x4 matrix uniform variable in the shader.
        /// Commonly used for transformation matrices (model, view, projection).
        /// </summary>
        /// <param name="name">The name of the uniform variable in the shader.</param>
        /// <param name="value">The 4x4 matrix value to set.</param>
        void SetUniform(string name, Matrix4X4<float> value);
    }
}