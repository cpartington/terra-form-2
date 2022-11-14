using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TerrainComputer
{
    public static TerrainComputer Instance { 
        get
        {
            _Instance ??= new TerrainComputer();
            return _Instance;
        }
    }
    public static TerrainComputer _Instance;

    public float[] TerrainTypePercentiles { get; private set; }
    public int WaterLevel { get; private set; }

    private Dictionary<int, byte> LevelToTerrainTypeDict;
    public static int[] TerrainTypeWeights = { 1, 1, 6, 9, 8 };

    private float[] xOffsets;
    private float[] zOffsets;

    private TerrainComputer()
    {
        // Get scaled indices
        float scale = (float)(Constants.TerrainLevels - 1) / (TerrainTypeWeights.Length - 1);
        float[] scaledIndices = new float[TerrainTypeWeights.Length];
        for (int i = 0; i < TerrainTypeWeights.Length; i++)
        {
            scaledIndices[i] = i * scale;
        }

        // Calculate weight for each terrain level
        float[] terrainLevelWeights = new float[Constants.TerrainLevels];
        terrainLevelWeights[0] = TerrainTypeWeights[0];
        for (int i = 1; i < Constants.TerrainLevels; i++)
        {
            for (int j = 0; j < scaledIndices.Length; j++)
            {
                if (i <= scaledIndices[j])
                {
                    float biggerIndex = scaledIndices[j];
                    float smallerIndex = scaledIndices[j - 1];

                    int bigWeight = TerrainTypeWeights[j];
                    int smallWeight = TerrainTypeWeights[j - 1];

                    float bigDist = Math.Abs(i - biggerIndex);
                    float smallDist = Math.Abs(i - smallerIndex);

                    float interpolatedWeight = ((bigWeight * bigDist) + (smallWeight * smallDist)) / (bigDist + smallDist);
                    terrainLevelWeights[i] = interpolatedWeight;
                }
            }
        }

        // Calculate percentiles
        float terrainTypeWeightTotal = TerrainTypeWeights.Sum();
        TerrainTypePercentiles = TerrainTypeWeights.Select((weight, index) =>
        { return (weight + TerrainTypeWeights.Take(index).Sum()) / terrainTypeWeightTotal; }).ToArray();

        // Create mapping from y level to terrain type
        LevelToTerrainTypeDict = new Dictionary<int, byte>();
        byte lastType = TerrainType.DarkSand;
        for (int level = 0; level < Constants.TerrainLevels; level++)
        {
            float percentile = level / (float)Constants.TerrainLevels;
            for (byte type = 0; type < TerrainTypePercentiles.Length; type++)
            {
                if (percentile <= TerrainTypePercentiles[type])
                {
                    LevelToTerrainTypeDict.Add(level, type);
                    
                    if (lastType == TerrainType.Sand && type == TerrainType.LowGrass)
                    {
                        WaterLevel = level;
                    }
                    lastType = type;
                    
                    break;
                }
            }
        }

        // Calculate noise value offsets
        this.xOffsets = new float[Constants.Octaves];
        this.zOffsets = new float[Constants.Octaves];
        for (int i = 0; i < Constants.Octaves; i++)
        {
            (xOffsets[i], zOffsets[i]) = (UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
        }
    }

    /// <summary>
    /// Determines the ground height for a given x,z coordinate.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="z">z coordinate</param>
    /// <returns>Ground height</returns>
    /// <exception cref="Exception">The calculated noise value should always fit into a percentile. Something is wrong if it does not.</exception>
    public int CalculateGroundHeight(int x, int z)
    {
        float noiseValue = Mathf.Abs(GetNoiseValue(x, z));
        return Mathf.FloorToInt(noiseValue * Constants.TerrainLevels);
    }

    private float GetNoiseValue(int x, int z)
    {
        float totalNoiseValue = 0f;

        for (int i = 0; i < Constants.Octaves; i++)
        {
            float frequency = Constants.Scale * Mathf.Pow(Constants.Lacunarity, i);
            float perlinValue = Mathf.PerlinNoise(x * frequency + xOffsets[i], z * frequency + zOffsets[i]) * 2 - 1;
            totalNoiseValue += perlinValue * Mathf.Pow(Constants.Persistence, i);
        }

        return Mathf.Clamp(totalNoiseValue, -1, 1);
    }

    /// <summary>
    /// Converts a y coordinate into its expected terrain type.
    /// </summary>
    /// <param name="y"></param>
    /// <returns>Terrain type</returns>
    public byte LevelToTerrainType(int y)
    {
        byte terrainType;
        LevelToTerrainTypeDict.TryGetValue(y, out terrainType);
        return terrainType;
    }
}
