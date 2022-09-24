using System.Linq;
using UnityEngine;

/// <summary>
/// Constants for terrain grid
/// </summary>
public static class Constants
{
    public static int GridXLength = 150;
    public static int GridZLength = 270;
    public static float GridCellHeight = 0.5f;
    public static float GridNoiseScale = 0.01f;
    public const int TerrainHeightOffset = 1;
    public static int TerrainLevels = 50;
    public static int GridYLength = TerrainLevels + TerrainHeightOffset;
    public static int[] TerrainTypeWeights = { 1, 1, 3, 6, 5 };

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

public static class TerrainType
{
    public const byte DarkSand = 0;
    public const byte Sand = 1;
    public const byte LowSoil = 2;
    public const byte MidSoil = 3;
    public const byte HighSoil = 4;
    public const byte Water = 5;
    public const byte Air = 6;

    public static bool IsGround(byte terrain)
    {
        switch (terrain)
        {
            case DarkSand:
            case Sand:
            case LowSoil:
            case MidSoil:
            case HighSoil:
                return true;
            default:
                return false;
        }
    }
}