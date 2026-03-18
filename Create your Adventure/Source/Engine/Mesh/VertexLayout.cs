namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// Describes a single attribute within a vertex structure (position, texture coordinates, normals, etc.).
    /// Defines the name, data type, component count, and memory layout for shader attribute binding.
    /// Immutable value type for safe sharing across rendering operations.
    /// </summary>
    public readonly struct VertexAttribute
    {
        /// <summary>
        /// Gets the name of this attribute as used in the shader (e.g., "aPosition", "aTexCoord").
        /// Must match the attribute name declared in the vertex shader.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the attribute location/index in the shader.
        /// Corresponds to the layout location in GLSL (e.g., layout(location = 0)).
        /// </summary>
        public uint Location { get; init; }

        /// <summary>
        /// Gets the number of components in this attribute.
        /// For example: 3 for vec3 (x, y, z), 2 for vec2 (u, v), 4 for vec4 (r, g, b, a).
        /// </summary>
        public int ComponentCount { get; init; }

        /// <summary>
        /// Gets the data type of each component (Float, Int, UInt, Byte).
        /// </summary>
        public VertexAttributeType Type { get; init; }

        /// <summary>
        /// Gets a value indicating whether the attribute data should be normalized.
        /// When true, integer types are mapped to [0,1] (unsigned) or [-1,1] (signed).
        /// Typically false for positions/normals, true for color bytes.
        /// </summary>
        public bool Normalized { get; init; }

        /// <summary>
        /// Gets the byte offset of this attribute within the vertex structure.
        /// Used to calculate the starting position of this attribute in vertex memory.
        /// </summary>
        public int Offset { get; init; }

        // ══════════════════════════════════════════════════
        // FACTORY METHODS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Creates a standard 3D position attribute (vec3).
        /// Corresponds to shader attribute "aPosition" at location 0.
        /// </summary>
        /// <param name="offset">The byte offset within the vertex structure. Default is 0.</param>
        /// <returns>A configured position attribute.</returns>
        public static VertexAttribute Position(int offset = 0) => new()
        {
            Name = "aPosition",
            Location = 0,
            ComponentCount = 3,
            Type = VertexAttributeType.Float,
            Normalized = false,
            Offset = offset
        };

        /// <summary>
        /// Creates a standard 2D texture coordinate attribute (vec2).
        /// Corresponds to shader attribute "aTexCoord" at location 1.
        /// </summary>
        /// <param name="offset">The byte offset within the vertex structure.</param>
        /// <returns>A configured texture coordinate attribute.</returns>
        public static VertexAttribute TexCoord(int offset) => new()
        {
            Name = "aTexCoord",
            Location = 1,
            ComponentCount = 2,
            Type = VertexAttributeType.Float,
            Normalized = false,
            Offset = offset
        };

        /// <summary>
        /// Creates a standard 3D normal vector attribute (vec3).
        /// Corresponds to shader attribute "aNormal" at location 2.
        /// </summary>
        /// <param name="offset">The byte offset within the vertex structure.</param>
        /// <returns>A configured normal attribute.</returns>
        public static VertexAttribute Normal(int offset) => new()
        {
            Name = "aNormal",
            Location = 2,
            ComponentCount = 3,
            Type = VertexAttributeType.Float,
            Normalized = false,
            Offset = offset
        };
    }

    /// <summary>
    /// Defines the complete layout of a vertex structure by combining multiple vertex attributes.
    /// Calculates the total stride (size) of a vertex and manages attribute ordering.
    /// Used to describe vertex format to the GPU for proper data interpretation.
    /// </summary>
    public class VertexLayout
    {
        // ═══ List of attributes that make up this vertex layout
        private readonly List<VertexAttribute> attributes = [];

        /// <summary>
        /// Gets the stride (total size in bytes) of a single vertex.
        /// Calculated automatically as attributes are added.
        /// </summary>
        public int Stride { get; private set; }

        /// <summary>
        /// Gets the read-only collection of all attributes in this layout.
        /// </summary>
        public IReadOnlyList<VertexAttribute> Attributes => attributes;

        /// <summary>
        /// Adds a vertex attribute to the layout and updates the stride.
        /// Returns this instance for method chaining (fluent API).
        /// </summary>
        /// <param name="attribute">The attribute to add to the layout.</param>
        /// <returns>This VertexLayout instance for chaining.</returns>
        public VertexLayout Add(VertexAttribute attribute)
        {
            attributes.Add(attribute);
            // ═══ Update stride by adding the size of this attribute
            Stride += attribute.ComponentCount * GetTypeSize(attribute.Type);
            return this;
        }

        /// <summary>
        /// Creates a standard vertex layout with position and texture coordinates.
        /// Common format for 2D sprites and textured quads.
        /// Total stride: 20 bytes (3 floats position + 2 floats texcoord).
        /// </summary>
        /// <returns>A configured VertexLayout.</returns>
        public static VertexLayout PositionTexCoord()
        {
            return new VertexLayout()
                .Add(VertexAttribute.Position(0))
                .Add(VertexAttribute.TexCoord(3 * sizeof(float)));
        }

        /// <summary>
        /// Creates a standard vertex layout with position, texture coordinates, and normals.
        /// Common format for 3D meshes with lighting.
        /// Total stride: 32 bytes (3 floats position + 2 floats texcoord + 3 floats normal).
        /// </summary>
        /// <returns>A configured VertexLayout.</returns>
        public static VertexLayout PositionTexCoordNormal()
        {
            return new VertexLayout()
                .Add(VertexAttribute.Position(0))
                .Add(VertexAttribute.TexCoord(3 * sizeof(float)))
                .Add(VertexAttribute.Normal(5 * sizeof(float)));
        }

        /// <summary>
        /// Gets the size in bytes of a single component of the specified type.
        /// </summary>
        /// <param name="type">The vertex attribute type.</param>
        /// <returns>The size in bytes.</returns>
        private static int GetTypeSize(VertexAttributeType type) => type switch
        {
            VertexAttributeType.Float => sizeof(float), // ═══ 4 bytes
            VertexAttributeType.Int => sizeof(int),     // ═══ 4 bytes
            VertexAttributeType.UInt => sizeof(uint),   // ═══ 4 bytes
            VertexAttributeType.Byte => sizeof(byte),   // ═══ 1 byte
            _ => sizeof(float)
        };
    }

    /// <summary>
    /// Defines the data type of vertex attribute components.
    /// Determines how the GPU interprets the raw bytes in vertex memory.
    /// </summary>
    public enum VertexAttributeType
    {
        /// <summary>
        /// 32-bit floating-point number (4 bytes).
        /// Most common type for positions, normals, and texture coordinates.
        /// </summary>
        Float,

        /// <summary>
        /// 32-bit signed integer (4 bytes).
        /// Used for integer vertex attributes.
        /// </summary>
        Int,

        /// <summary>
        /// 32-bit unsigned integer (4 bytes).
        /// Used for indices or unsigned integer attributes.
        /// </summary>
        UInt,

        /// <summary>
        /// 8-bit unsigned byte (1 byte).
        /// Often used for colors (0-255 range, can be normalized to 0.0-1.0).
        /// </summary>
        Byte
    }
}