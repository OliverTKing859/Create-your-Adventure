using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.AssetLoader
{
    /// <summary>
    /// Provides methods for locating game assets (audio, models, shaders, textures)
    /// with support for base and modded asset directories.
    /// </summary>
    /// <remarks>
    /// Assets are searched in the following order:
    /// <list type="number">
    ///   <item>Base directory: assets/base/{subfolder}/</item>
    ///   <item>Modded directory: assets/modded/{subfolder}/</item>
    /// </list>
    /// Results are cached for fast repeated lookups.
    /// </remarks>
    public static class AssetLoader
    {
        // -------- Folder path --------
        private const string AssetsRoot = "assets";
        private const string BaseFolder = "base";

        // --- Cache for quick Lookups
        private static readonly Dictionary<string, string> PathCache = [];

        // PUBLIC ASSET RETRIEVAL METHODS ----------------------------------------------------------------

        /// <summary>
        /// Retrieves the file path of an audio asset.
        /// </summary>
        /// <param name="filename">The audio file name (e.g., "music.ogg").</param>
        /// <returns>The full path if found; otherwise, an empty string.</returns>
        public static string GetAudioPath(string filename)
        {
            return FindAsset("audio", filename);
        }

        /// <summary>
        /// Retrieves the file path of a model asset.
        /// </summary>
        /// <param name="filename">The model file name (e.g., "player.obj").</param>
        /// <returns>The full path if found; otherwise, an empty string.</returns>
        public static string GetModelPath(string filename)
        {
            return FindAsset("models", filename);
        }

        /// <summary>
        /// Retrieves the file path of a shader asset.
        /// </summary>
        /// <param name="filename">The shader file name (e.g., "basic.vert").</param>
        /// <returns>The full path if found; otherwise, an empty string.</returns>
        public static string GetShaderPath(string filename)
        {
            return FindAsset("shaders", filename);
        }

        /// <summary>
        /// Retrieves the file path of a texture asset.
        /// </summary>
        /// <param name="filename">The texture file name (e.g., "dirt.png").</param>
        /// <returns>The full path if found; otherwise, an empty string.</returns>
        public static string GetTexturePath(string filename)
        {
            return FindAsset("textures", filename);
        }

        /// <summary>
        /// Clears the path cache, forcing fresh lookups on next request.
        /// </summary>
        /// <remarks>
        /// Call this after adding or removing asset files at runtime.
        /// </remarks>
        public static void ClearCache() => PathCache.Clear();

        // FIND ASSETS ----------------------------------------------------------------
        private static string FindAsset(string subfolder ,string filename)
        {
            // -------- Cache --------
            // --- Check
            string cacheKey = $"{subfolder}/{filename}";
            if (PathCache.TryGetValue(cacheKey, out string? cachedPath))
                return cachedPath;

            // -------- First in the base --------
            string searchRoot = Path.Combine(AssetsRoot, BaseFolder, subfolder);

            string? result = SearchInFolder(searchRoot, filename);

            // --- If not found, also search in modded
            if (result is null)
            {
                searchRoot = Path.Combine(AssetsRoot, "modded", subfolder);
                result = SearchInFolder(searchRoot, filename);
            }

            // --- If is null
            if (result is null)
            {
                Console.WriteLine($"[AssetLoader] Not found: {filename}");
                return string.Empty;
            }

            // -------- Save on Cache --------
            PathCache[cacheKey] = result;
            Console.WriteLine($"[AssetLoader] Found: {result}");

            return result;
        }

        // SEARCH IN FOLDER ----------------------------------------------------------------
        private static string? SearchInFolder(string searchRoot, string filename)
        {
            // --- If ist null
            if (!Directory.Exists(searchRoot))
            {
                return null;
            }
            // -------- Recursive search in all subfolders --------
            return Directory.EnumerateFiles(searchRoot, filename, SearchOption.AllDirectories)
                            .FirstOrDefault();
        }
    }
}