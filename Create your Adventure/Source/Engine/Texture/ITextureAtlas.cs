using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Texture
{
    public interface ITextureAtlas : IDisposable
    {
        string Name { get; }

        int Width { get; }

        int Height { get; }

        int TextureCount { get; }

        bool IsBuilt { get; }

        // ══════════════════════════════════════════════════
        // ATLAS BUILDING
        // ══════════════════════════════════════════════════

        void AddTexture(string name, string path);

        void Build(TextureSettings settings);

        // ══════════════════════════════════════════════════
        // REGION ACCES (for Rendering)
        // ══════════════════════════════════════════════════

        AtlasRegion GetRegion(string name);

        bool HasTexture(string name);

        IEnumerable<string> GetAllTextureNames();

        // ══════════════════════════════════════════════════
        // BINDING
        // ══════════════════════════════════════════════════

        void Bind(uint unit = 0);

        void Unbind();

    }
}
