using System.Linq;
using UnityEngine;

/// <summary>
/// Constants for terrain grid
/// </summary>
public static class Constants
{
    public static int GridXLength = 75;
    public static int GridZLength = 75;
    public static float GridCellHeight = 0.5f;
    public static float GridNoiseScale = 0.01f;
    public const int TerrainHeightOffset = 1;
    public const int AirHeightOffset = 10; // TODO should this be higher?
    public static int TerrainLevels = 25;
    public static int GridYLength = TerrainLevels + TerrainHeightOffset + AirHeightOffset;
    public static int[] TerrainTypeWeights = { 1, 1, 3, 6, 5 };

    public const TerrainType MaxWaterType = TerrainType.LowGround;

    public static readonly Vector3[] CubeVertices = new Vector3[8]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1),
    };

    public static readonly Vector3[] FaceCheckDirections = new Vector3[6]
    {
        new Vector3(0, 0, -1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0),
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

    public static readonly Vector2[] CubeUvs = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1),
    };
}

public enum TerrainType
{
    DeepWater,
    ShallowWater,
    LowGround,
    MidGround,
    HighGround
}

public enum TerrainCellType
{
    Ground,
    Water,
    Air
}