using System.Linq;
using UnityEngine;

/// <summary>
/// Constants for terrain grid
/// </summary>
public static class Constants
{
    public static int GridXLength = 150;
    public static int GridZLength = 270;
    public static int GridCellSize = 1;
    public static float GridCellHeight = 0.5f;
    public static float GridNoiseScale = 0.01f;
    public const float TerrainHeightOffset = 1;

    public static int TerrainLevels = 50;
    //public static readonly int[] TerrainTypeWeights = { 10, 15, 10, 40, 20 };
    public static int[] TerrainTypeWeights = { 1, 1, 4, 5, 4 };

    public const TerrainType MaxWaterType = TerrainType.LowGround;

    public static readonly Vector3[] cubeVertices = new Vector3[8]
    {
        new Vector3(0, 0, 0), // 0
        new Vector3(0, 0, 1), // 1
        new Vector3(0, 1, 0), // 2
        new Vector3(1, 0, 0), // 3
        new Vector3(0, 1, 1), // 4
        new Vector3(1, 0, 1), // 5
        new Vector3(1, 1, 0), // 6
        new Vector3(1, 1, 1), // 7
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0, 0, -1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0),
    };

    public static readonly int[,] cubeTriangles = new int[6, 6]
    {
        { 0, 2, 3, 3, 2, 6 }, // Back face
        { 5, 7, 1, 1, 7, 4 }, // Front face
        { 2, 4, 6, 6, 4, 7 }, // Top face
        { 3, 5, 0, 0, 5, 1 }, // Bottom face
        { 1, 4, 0, 0, 4, 2 }, // Left face
        { 3, 6, 5, 5, 6, 7 }, // Right face
    };

    public static readonly Vector2[] cubeUvs = new Vector2[6]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
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