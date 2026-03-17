namespace Create_your_Adventure.Source.Engine.Texture
{
    /// <summary>
    /// Defines the contract for a texture that can be loaded, bound, and used in rendering operations.
    /// Abstracts texture management across different graphics APIs (OpenGL, DirectX, Vulkan).
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier name of this texture.
        /// Used for texture management and debugging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets a value indicating whether the texture has been successfully loaded into GPU memory.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Loads texture data from an image file on disk.
        /// Supports common formats like PNG, JPG, BMP, TGA.
        /// </summary>
        /// <param name="path">The file path to the image file.</param>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        void LoadFromFile(string path, TextureSettings settings);

        /// <summary>
        /// Loads texture data from a raw pixel array in memory.
        /// Useful for procedurally generated textures or custom image data.
        /// </summary>
        /// <param name="pixelData">The raw pixel data in RGBA format (4 bytes per pixel).</param>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        void LoadFromData(byte[] pixelData, int width, int height, TextureSettings settings);

        /// <summary>
        /// Binds this texture to the specified texture unit for rendering.
        /// The texture must be bound before it can be used in shader programs.
        /// </summary>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware). Default is 0.</param>
        void Bind(uint unit = 0);

        /// <summary>
        /// Unbinds the texture from its current texture unit.
        /// Called to clean up state after rendering operations.
        /// </summary>
        void Unbind();
    }
}
