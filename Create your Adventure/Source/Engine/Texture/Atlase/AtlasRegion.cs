namespace Create_your_Adventure.Source.Engine.Texture.Atlase
{
    /// <summary>
    /// Represents a named region within a texture atlas.
    /// Stores both pixel coordinates and normalized UV coordinates for efficient texture mapping.
    /// Immutable value type for safe sharing across rendering operations.
    /// </summary>
    public readonly struct AtlasRegion
    {
        /// <summary>
        /// Gets the unique identifier name of this atlas region.
        /// Used to reference specific sprites or textures within the atlas.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the X coordinate of the region's top-left corner in pixels.
        /// </summary>
        public int X { get; init; }

        /// <summary>
        /// Gets the Y coordinate of the region's top-left corner in pixels.
        /// </summary>
        public int Y { get; init; }

        /// <summary>
        /// Gets the width of the region in pixels.
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Gets the height of the region in pixels.
        /// </summary>
        public int Height { get; init; }

        // ══════════════════════════════════════════════════
        // UV-COORDINATION (0.0 - 1.0, normalization)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the normalized U coordinate (horizontal) of the region's left edge.
        /// Range: 0.0 to 1.0, where 0.0 is the left edge of the atlas.
        /// </summary>
        public float U0 { get; init; }

        /// <summary>
        /// Gets the normalized V coordinate (vertical) of the region's top edge.
        /// Range: 0.0 to 1.0, where 0.0 is the top edge of the atlas.
        /// </summary>
        public float V0 { get; init; }

        /// <summary>
        /// Gets the normalized U coordinate (horizontal) of the region's right edge.
        /// Range: 0.0 to 1.0, where 1.0 is the right edge of the atlas.
        /// </summary>
        public float U1 { get; init; }

        /// <summary>
        /// Gets the normalized V coordinate (vertical) of the region's bottom edge.
        /// Range: 0.0 to 1.0, where 1.0 is the bottom edge of the atlas.
        /// </summary>
        public float V1 { get; init; }

        /// <summary>
        /// Creates a new atlas region with automatically calculated UV coordinates.
        /// UV coordinates are normalized based on the total atlas dimensions.
        /// </summary>
        /// <param name="name">The unique identifier for this region.</param>
        /// <param name="x">The X coordinate of the region's top-left corner in pixels.</param>
        /// <param name="y">The Y coordinate of the region's top-left corner in pixels.</param>
        /// <param name="width">The width of the region in pixels.</param>
        /// <param name="height">The height of the region in pixels.</param>
        /// <param name="atlasWidth">The total width of the texture atlas in pixels.</param>
        /// <param name="atlasHeight">The total height of the texture atlas in pixels.</param>
        /// <returns>A new AtlasRegion with calculated UV coordinates.</returns>
        public static AtlasRegion Create(
            string name,
            int x,
            int y,
            int width,
            int height,
            int atlasWidth,
            int atlasHeight
            )
        {
            return new AtlasRegion
            {
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,

                // ═══ Calculate normalized UV coordinates (0.0 to 1.0)
                U0 = (float)x / atlasWidth,
                V0 =(float)y / atlasHeight,
                U1 =(float)(x + width) / atlasWidth,
                V1 =(float)(y + height) / atlasHeight
            };
        }
    }
}
