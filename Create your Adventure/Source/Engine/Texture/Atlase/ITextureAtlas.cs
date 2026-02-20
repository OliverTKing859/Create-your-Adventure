using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture.Atlase
{
    /// <summary>
    /// Defines the contract for a texture atlas that combines multiple textures into a single larger texture.
    /// Reduces draw calls and improves rendering performance by batching multiple sprites/textures together.
    /// Provides region-based access to individual textures within the atlas.
    /// </summary>
    public interface ITextureAtlas : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier name of this texture atlas.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the total width of the atlas texture in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the total height of the atlas texture in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the number of individual textures packed into this atlas.
        /// </summary>
        int TextureCount { get; }

        /// <summary>
        /// Gets a value indicating whether the atlas has been built and is ready for use.
        /// Textures must be added before building, and the atlas must be built before binding.
        /// </summary>
        bool IsBuilt { get; }

        // ══════════════════════════════════════════════════
        // ATLAS BUILDING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Adds a texture to the atlas by loading it from a file.
        /// Must be called before Build(). The texture will be packed into the atlas during the build process.
        /// </summary>
        /// <param name="name">The unique identifier name for this texture within the atlas.</param>
        /// <param name="path">The file path to the texture image.</param>
        void AddTexture(string name, string path);

        /// <summary>
        /// Builds the atlas by packing all added textures into a single large texture.
        /// Calculates optimal placement, generates UV coordinates, and uploads to GPU memory.
        /// Must be called after adding all textures and before using the atlas for rendering.
        /// </summary>
        /// <param name="settings">Configuration settings for texture filtering, wrapping, and mipmaps.</param>
        void Build(TextureSettings settings);

        // ══════════════════════════════════════════════════
        // REGION ACCES (for Rendering)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Retrieves the atlas region for a specific texture by name.
        /// Returns the pixel coordinates and normalized UV coordinates needed for rendering.
        /// </summary>
        /// <param name="name">The name of the texture to retrieve.</param>
        /// <returns>The atlas region containing coordinates and UV data for the requested texture.</returns>
        AtlasRegion GetRegion(string name);

        /// <summary>
        /// Checks whether a texture with the specified name exists in the atlas.
        /// </summary>
        /// <param name="name">The name of the texture to check.</param>
        /// <returns>True if the texture exists in the atlas, false otherwise.</returns>
        bool HasTexture(string name);

        /// <summary>
        /// Gets all texture names contained in this atlas.
        /// Useful for debugging or iterating over all available textures.
        /// </summary>
        /// <returns>An enumerable collection of all texture names in the atlas.</returns>
        IEnumerable<string> GetAllTextureNames();

        // ══════════════════════════════════════════════════
        // BINDING
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Binds the atlas texture to the specified texture unit for rendering.
        /// The atlas must be built before it can be bound.
        /// </summary>
        /// <param name="unit">The texture unit to bind to (0-31 on most hardware). Default is 0.</param>
        void Bind(uint unit = 0);

        /// <summary>
        /// Unbinds the atlas texture from its current texture unit.
        /// Called to clean up state after rendering operations.
        /// </summary>
        void Unbind();

    }
}
