using UnityEngine;

/// <summary>
/// Constants for terrain grid
/// </summary>
public static class Constants
{
    /// Grid Properties
    /// Values coordinate with the x,y,z TerrainGrid values.

    /// <summary>
    /// Grid size in Z dimension.
    /// </summary>
    public static int GridZLength = 400;
    /// <summary>
    /// Grid size in X dimension.
    /// </summary>
    public static int GridXLength = 225;
    /// <summary>
    /// Number of extra cells to include at the bottom of the grid.
    /// </summary>
    public const int TerrainHeightOffset = 1;
    /// <summary>
    /// Maximum y value (excluding the TerrainHeightOffset).
    /// </summary>
    public static int TerrainLevels = 25;
    /// <summary>
    /// Grid size in the Y dimension.
    /// Calculated as TerrainLevels + TerrainHeightOffset.
    /// </summary>
    public static int GridYLength = TerrainLevels + TerrainHeightOffset;
    /// <summary>
    /// Chunk size in the X/Z dimensions.
    /// </summary>
    public const int ChunkSize = 16;
    /// <summary>
    /// Number of chunks to create during initial world generation.
    /// </summary>
    public const int WorldSizeInChunks = 5;

    /// World Properties
    /// Values represent positions in the game world.

    /// <summary>
    /// Single cell size in x and z dimensions.
    /// </summary>
    public static float GridCellWidth = 1f;
    /// <summary>
    /// Single cell size in y dimension.
    /// </summary>
    public static float GridCellHeight = 0.5f;
    /// <summary>
    /// How much the water line sits below the grass
    /// </summary>
    public const float WaterPosOffset = -0.1f;

    /// Mesh Properties

    public static readonly Vector3[] CubeVertices = new Vector3[8]
    {
        new Vector3(0, 0, 0),                                      // 0
        new Vector3(GridCellWidth, 0, 0),                          // 1
        new Vector3(GridCellWidth, GridCellHeight, 0),             // 2
        new Vector3(0, GridCellHeight, 0),                         // 3
        new Vector3(0, 0, GridCellWidth),                          // 4
        new Vector3(GridCellWidth, 0, GridCellWidth),              // 5
        new Vector3(GridCellWidth, GridCellHeight, GridCellWidth), // 6
        new Vector3(0, GridCellHeight, GridCellWidth),             // 7
    };

    public static readonly Vector3[] CubeVerticesWater = new Vector3[8]
    {
        new Vector3(0, 0, 0),                                                       // 0
        new Vector3(GridCellWidth, 0, 0),                                           // 1
        new Vector3(GridCellWidth, GridCellHeight + WaterPosOffset, 0),             // 2
        new Vector3(0, GridCellHeight + WaterPosOffset, 0),                         // 3
        new Vector3(0, 0, GridCellWidth),                                           // 4
        new Vector3(GridCellWidth, 0, GridCellWidth),                               // 5
        new Vector3(GridCellWidth, GridCellHeight + WaterPosOffset, GridCellWidth), // 6
        new Vector3(0, GridCellHeight + WaterPosOffset, GridCellWidth),             // 7
    };

    public static readonly Vector3[] FaceCheckDirections = new Vector3[6]
    {
        new Vector3(0, 0, -GridCellWidth),  // Back face
        new Vector3(0, 0, GridCellWidth),   // Front face
        new Vector3(0, GridCellHeight, 0),  // Top face
        new Vector3(0, -GridCellHeight, 0), // Bottom face
        new Vector3(-GridCellWidth, 0, 0),  // Left face
        new Vector3(GridCellWidth, 0, 0),   // Right face
    };

    public static readonly int[,] CubeTriangles = new int[6, 4]
    {
        {0, 3, 1, 2}, // Back face
		{5, 6, 4, 7}, // Front face
		{3, 7, 2, 6}, // Top face
		{1, 5, 0, 4}, // Bottom face
		{4, 7, 0, 3}, // Left face
		{1, 2, 5, 6}, // Right face
    };
}
