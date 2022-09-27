using UnityEngine;

/// <summary>
/// Constants for terrain grid
/// </summary>
public static class Constants
{
    // 16x9 proportions: 270x150, 400x225, 800x450
    public static int GridZLength = 400;
    public static int GridXLength = 225;
    public static float GridCellHeight = 0.5f;
    public static float GridNoiseScale = 0.01f;
    public const int TerrainHeightOffset = 1;
    public static int TerrainLevels = 50;
    public static int GridYLength = TerrainLevels + TerrainHeightOffset;
    public static int[] TerrainTypeWeights = { 1, 1, 3, 6, 5 };

    public static readonly Vector3[] CubeVertices = new Vector3[8]
    {
        new Vector3(0, 0, 0), // 0
        new Vector3(1, 0, 0), // 1
        new Vector3(1, GridCellHeight, 0), // 2
        new Vector3(0, GridCellHeight, 0), // 3
        new Vector3(0, 0, 1), // 4
        new Vector3(1, 0, 1), // 5
        new Vector3(1, GridCellHeight, 1), // 6
        new Vector3(0, GridCellHeight, 1), // 7
    };

    public static readonly Vector3[] FaceCheckDirections = new Vector3[6]
    {
        new Vector3(0, 0, -1), // Back face
        new Vector3(0, 0, 1),  // Front face
        new Vector3(0, GridCellHeight, 0),  // Top face
        new Vector3(0, -GridCellHeight, 0), // Bottom face
        new Vector3(-1, 0, 0), // Left face
        new Vector3(1, 0, 0),  // Right face
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
