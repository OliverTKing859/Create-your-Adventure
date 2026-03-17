using Create_your_Adventure.Source.Engine.World;
using Silk.NET.Maths;

namespace Create_your_Adventure.Source.Engine.Camera
{
    /// <summary>
    /// Manages camera position in a chunked infinite world using floating-point origin shifting.
    /// Solves floating-point precision issues in large worlds by keeping the camera near (0,0,0).
    /// Automatically shifts the coordinate origin when the camera moves too far from the current origin.
    /// Translates between absolute world coordinates (long integers) and local rendering coordinates (floats).
    /// Essential for infinite worlds where coordinates exceed float precision limits (~16 million units).
    /// </summary>
    public class CameraWorldBinding
    {
        // ══════════════════════════════════════════════════
        // CONSTANTS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// The size of a chunk in blocks (16x16x16).
        /// Used for chunk coordinate calculations and world-to-local conversions.
        /// </summary>
        public const int ChunkSize = 16;

        /// <summary>
        /// Distance threshold (in blocks) at which the origin is shifted to maintain float precision.
        /// Set to 256 blocks (16 chunks) - keeps camera within ±256 blocks of origin.
        /// Beyond this distance, float precision degrades enough to cause rendering jitter.
        /// </summary>
        public const float OriginShiftThreshold = 256f; // ═══ 16 Chunks

        // ══════════════════════════════════════════════════
        // WORLD ANCHOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Gets the current coordinate origin chunk.
        /// All local positions are relative to this chunk's corner.
        /// Shifts periodically to keep the camera near (0,0,0) in local space.
        /// </summary>
        public ChunkCoord OriginChunk { get; private set; }

        /// <summary>
        /// Gets the chunk coordinate where the camera is currently located.
        /// Updated every frame based on camera position.
        /// Used for chunk loading/unloading decisions.
        /// </summary>
        public ChunkCoord CurrentChunk { get; private set; }

        /// <summary>
        /// Gets the camera's position within its current chunk (0-16 block units).
        /// Combined with CurrentChunk to determine absolute world position.
        /// </summary>
        public Vector3D<float> LocalOffset { get; private set; }

        // ══════════════════════════════════════════════════
        // EVENTS (for systems that need to react to origin shifts)
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Event raised when the coordinate origin shifts to a new chunk.
        /// Provides old and new origin chunks - systems must update their coordinates accordingly.
        /// Critical for renderers, chunk managers, and physics systems to recalculate positions.
        /// </summary>
        public event Action<ChunkCoord, ChunkCoord>? OriginShifted;

        /// <summary>
        /// Event raised when the camera moves into a different chunk.
        /// Used by chunk managers to trigger chunk loading/unloading around the new position.
        /// </summary>
        public event Action<ChunkCoord>? ChunkChanged;

        // ══════════════════════════════════════════════════
        // CONTRUCTOR
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes a new camera world binding at world origin (0,0,0).
        /// Call Initialize() with actual spawn position before first use.
        /// </summary>
        public CameraWorldBinding()
        {
            OriginChunk = new ChunkCoord(0, 0, 0);
            CurrentChunk = new ChunkCoord(0, 0, 0);
            LocalOffset = Vector3D<float>.Zero;
        }

        // ══════════════════════════════════════════════════
        // INITIALIZATION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Initializes the camera position from absolute world coordinates.
        /// Sets both origin and current chunk based on spawn position.
        /// Should be called once before the camera starts moving.
        /// </summary>
        /// <param name="worldX">Absolute world X coordinate in blocks.</param>
        /// <param name="worldY">Absolute world Y coordinate in blocks.</param>
        /// <param name="worldZ">Absolute world Z coordinate in blocks.</param>
        public void Initialize(long worldX, long worldY, long worldZ)
        {
            // ═══ Convert absolute world position to chunk coordinate and local offset
            var (chunk, offset) = WorldToChunkLocal(worldX, worldY, worldZ);
            OriginChunk = chunk;
            CurrentChunk = chunk;
            LocalOffset = offset;
        }

        // ══════════════════════════════════════════════════
        // UPDATE
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Updates camera position and handles chunk changes and origin shifts.
        /// Call this every frame with the camera's local position (relative to OriginChunk).
        /// Returns potentially corrected position if origin shift occurred.
        /// </summary>
        /// <param name="localPosition">Camera position relative to OriginChunk (in blocks).</param>
        /// <returns>Corrected local position if origin shifted, otherwise original position.</returns>
        public Vector3D<float> UpdateFromLocalPosition(Vector3D<float> localPosition)
        {
            // ═══ Convert local position to absolute world coordinates
            var worldPos = LocalToWorld(localPosition);

            // ═══ Determine which chunk the camera is currently in
            // ═══ Use Math.Floor on double to maintain precision before chunk division
            long floorX = (long)Math.Floor(worldPos.X);
            long floorY = (long)Math.Floor(worldPos.Y);
            long floorZ = (long)Math.Floor(worldPos.Z);

            // ═══ Calculate chunk coordinate using floor division (handles negative coords correctly)
            var newChunk = new ChunkCoord(
                FloorDivide(floorX, ChunkSize),
                FloorDivide(floorY, ChunkSize),
                FloorDivide(floorZ, ChunkSize)
            );

            // ═══ Detect chunk boundary crossing
            if (!newChunk.Equals(CurrentChunk))
            {
                CurrentChunk = newChunk;

                // ═══ Notify chunk manager to update loaded chunks
                ChunkChanged?.Invoke(newChunk);
            }

            // ═══ Calculate position within current chunk (0-16 range)
            // ═══ Using modulo ensures proper wrapping for negative coordinates
            LocalOffset = new Vector3D<float>(
                Mod((float)worldPos.X, ChunkSize),
                Mod((float)worldPos.Y, ChunkSize),
                Mod((float)worldPos.Z, ChunkSize)
            );

            // ═══ Check if origin shift is needed and return corrected position
            var correctedPosition = CheckOriginShift(localPosition, worldPos);

            return correctedPosition;
        }

        /// <summary>
        /// Checks if the camera has moved too far from origin and shifts if necessary.
        /// Origin shifts prevent float precision loss by keeping camera coordinates small.
        /// All rendering positions must be recalculated when origin shifts.
        /// </summary>
        /// <param name="localPosition">Current local position before shift.</param>
        /// <param name="worldPos">Absolute world position in double precision.</param>
        /// <returns>Corrected local position if shift occurred, original position otherwise.</returns>
        private Vector3D<float> CheckOriginShift(Vector3D<float> localPosition, Vector3D<double> worldPos)
        {
            // ═══ Measure distance from current origin
            float distanceFromOrigin = localPosition.Length;

            // ═══ Only shift if camera exceeds threshold distance (256 blocks)
            // ═══ Prevents frequent shifts which are expensive (all objects must be repositioned)
            if (distanceFromOrigin > OriginShiftThreshold)
            {
                var oldOrigin = OriginChunk;

                // ═══ Shift origin to camera's current chunk
                // ═══ This makes the camera near (0,0,0) again in local coordinates
                OriginChunk = CurrentChunk;

                // ═══════════════════════════════════════════════════════════
                // CRITICAL: Calculate new LocalPosition relative to new Origin!
                // ═══════════════════════════════════════════════════════════
                // ═══ All rendered objects must also be updated by this delta
                var correctedPosition = new Vector3D<float>(
                    (float)(worldPos.X - OriginChunk.X * ChunkSize),
                    (float)(worldPos.Y - OriginChunk.Y * ChunkSize),
                    (float)(worldPos.Z - OriginChunk.Z * ChunkSize)
                );

                // ═══ Notify all systems (renderer, chunk manager, physics) to update positions
                // ═══ They must subtract (oldOrigin - newOrigin) * ChunkSize from all positions
                OriginShifted?.Invoke(oldOrigin, OriginChunk);

                return correctedPosition;
            }

            // ═══ No shift needed, return original position
            return localPosition;
        }

        // ══════════════════════════════════════════════════
        // COORDINATE CONVERSION
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Converts local rendering position to absolute world coordinates.
        /// Returns double precision to maintain accuracy for large world coordinates.
        /// </summary>
        /// <param name="localPosition">Position relative to OriginChunk (float precision).</param>
        /// <returns>Absolute world position (double precision for large coordinates).</returns>
        public Vector3D<double> LocalToWorld(Vector3D<float> localPosition)
        {
            // ═══ Add origin chunk offset (in blocks) to local position
            return new Vector3D<double>(
                OriginChunk.X * ChunkSize + localPosition.X,
                OriginChunk.Y * ChunkSize + localPosition.Y,
                OriginChunk.Z * ChunkSize + localPosition.Z
            );
        }

        /// <summary>
        /// Converts absolute world coordinates to local rendering position.
        /// Result is relative to current OriginChunk.
        /// </summary>
        /// <param name="worldX">Absolute world X in blocks.</param>
        /// <param name="worldY">Absolute world Y in blocks.</param>
        /// <param name="worldZ">Absolute world Z in blocks.</param>
        /// <returns>Position relative to OriginChunk (float precision for rendering).</returns>
        public Vector3D<float> WorldToLocal(long worldX, long worldY, long worldZ)
        {
            // ═══ Subtract origin chunk offset from world position
            return new Vector3D<float>(
                (float)(worldX - OriginChunk.X * ChunkSize),
                (float)(worldY - OriginChunk.Y * ChunkSize),
                (float)(worldZ - OriginChunk.Z * ChunkSize)
            );
        }

        /// <summary>
        /// Decomposes absolute world position into chunk coordinate and local offset.
        /// Static utility method for initial positioning and chunk calculations.
        /// </summary>
        /// <param name="x">Absolute world X coordinate.</param>
        /// <param name="y">Absolute world Y coordinate.</param>
        /// <param name="z">Absolute world Z coordinate.</param>
        /// <returns>Tuple of (chunk coordinate, offset within chunk 0-16).</returns>
        public static (ChunkCoord chunk, Vector3D<float> offset) WorldToChunkLocal(long x, long y, long z)
        {
            // ═══ Use floor division to correctly handle negative coordinates
            var chunk = new ChunkCoord(
                FloorDivide(x, ChunkSize),
                FloorDivide(y, ChunkSize),
                FloorDivide(z, ChunkSize)
            );

            // ═══ Calculate local offset within chunk (0-15.999...)
            // ═══ Modulo handles negative wrapping correctly
            var offset = new Vector3D<float>(
                Mod(x, ChunkSize),
                Mod(y, ChunkSize),
                Mod(z, ChunkSize)
            );

            return (chunk, offset);
        }

        /// <summary>
        /// Calculates the local rendering position for a given chunk coordinate.
        /// Used to position chunk meshes relative to the current origin.
        /// </summary>
        /// <param name="chunk">The chunk coordinate to convert.</param>
        /// <returns>Position where this chunk should be rendered in local space.</returns>
        public Vector3D<float> GetChunkLocalPosition(ChunkCoord chunk)
        {
            // ═══ Calculate offset from origin chunk in chunks, then scale to blocks
            // ═══ Result is where this chunk's (0,0,0) corner is in local rendering space
            return new Vector3D<float>(
                (chunk.X - OriginChunk.X) * ChunkSize,
                (chunk.Y - OriginChunk.Y) * ChunkSize,
                (chunk.Z - OriginChunk.Z) * ChunkSize
            );
        }

        // ══════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════
        /// <summary>
        /// Performs floor division that correctly handles negative numbers.
        /// Standard division truncates toward zero, but we need floor behavior for chunk coords.
        /// Example: -17 / 16 = -1 (wrong), FloorDivide(-17, 16) = -2 (correct for chunks).
        /// </summary>
        /// <param name="a">Dividend (can be negative).</param>
        /// <param name="b">Divisor (chunk size, always positive).</param>
        /// <returns>Floor of a/b (rounds toward negative infinity).</returns>
        private static long FloorDivide(long a, int b)
        {
            // ═══ Positive numbers: normal division
            // ═══ Negative numbers: adjust by (b-1) before division to get floor behavior
            return a >= 0 ? a / b : (a - b + 1) / b;
        }

        /// <summary>
        /// Modulo operation that always returns positive results (mathematical modulo).
        /// C#'s % operator can return negative values, which breaks chunk offset calculations.
        /// </summary>
        /// <param name="a">Value to mod (can be negative).</param>
        /// <param name="b">Modulus (chunk size, always positive).</param>
        /// <returns>Result in range [0, b), always positive.</returns>
        private static float Mod(float a, int b)
        {
            float result = a % b;

            // ═══ If result is negative, add modulus to make it positive
            return result < 0 ? result + b : result;
        }

        /// <summary>
        /// Modulo operation for long integers with positive result guarantee.
        /// Overload for processing world coordinates before conversion to float.
        /// </summary>
        /// <param name="a">Value to mod (can be negative).</param>
        /// <param name="b">Modulus (chunk size, always positive).</param>
        /// <returns>Result in range [0, b), always positive.</returns>
        private static float Mod(long a, int b)
        {
            long result = a % b;
            return result < 0 ? result + b : result;
        }
    }
}