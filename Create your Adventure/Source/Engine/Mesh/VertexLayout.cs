using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Mesh
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct VertexAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public uint Location { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int ComponentCount { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeType Type { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool Normalized { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int Offset { get; init; }

        // ══════════════════════════════════════════════════
        // FACTORY METHODS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
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
    /// 
    /// </summary>
    public class VertexLayout
    {
        private readonly List<VertexAttribute> attributes = [];

        /// <summary>
        /// 
        /// </summary>
        public int Stride { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<VertexAttribute> Attributes => attributes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public VertexLayout Add(VertexAttribute attribute)
        {
            attributes.Add(attribute);
            Stride += attribute.ComponentCount * GetTypeSize(attribute.Type);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static VertexLayout PositionTexCoord()
        {
            return new VertexLayout()
                .Add(VertexAttribute.Position(0))
                .Add(VertexAttribute.TexCoord(3 * sizeof(float)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static VertexLayout PositionTexCoordNormal()
        {
            return new VertexLayout()
                .Add(VertexAttribute.Position(0))
                .Add(VertexAttribute.TexCoord(3 * sizeof(float)))
                .Add(VertexAttribute.Normal(5 * sizeof(float)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int GetTypeSize(VertexAttributeType type) => type switch
        {
            VertexAttributeType.Float => sizeof(float),
            VertexAttributeType.Int => sizeof(int),
            VertexAttributeType.UInt => sizeof(uint),
            VertexAttributeType.Byte => sizeof(byte),
            _ => sizeof(float)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public enum VertexAttributeType
    {
        Float,
        Int,
        UInt,
        Byte
    }
}
