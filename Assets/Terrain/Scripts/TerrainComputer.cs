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
            if (_Instance == null)
            {
                _Instance = new TerrainComputer();
            }
            return _Instance;
        }
    }
    public static TerrainComputer _Instance;

    public float[] TerrainTypePercentiles { get; private set; }
    public float[] TerrainLevelPercentiles { get; private set; }
    public int WaterLevel { get; private set; }

    private Dictionary<int, byte> LevelToTerrainTypeDict;

    private readonly int TerrainLevels = Constants.TerrainLevels;
    private readonly int[] TerrainTypeWeights = Constants.TerrainTypeWeights;

    private float xOffset;
    private float zOffset;

    private TerrainComputer()
    {
        float scale = (float)(TerrainLevels - 1) / (TerrainTypeWeights.Length - 1);
        float[] scaledIndices = new float[TerrainTypeWeights.Length];
        for (int i = 0; i < TerrainTypeWeights.Length; i++)
        {
            scaledIndices[i] = i * scale;
        }

        float[] terrainLevelWeights = new float[TerrainLevels];
        terrainLevelWeights[0] = TerrainTypeWeights[0];
        for (int i = 1; i < TerrainLevels; i++)
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
        float terrainlevelWeightTotal = terrainLevelWeights.Sum();
        TerrainLevelPercentiles = terrainLevelWeights.Select((weight, index) =>
        { return (weight + terrainLevelWeights.Take(index).Sum()) / terrainlevelWeightTotal; }).ToArray();

        float terrainTypeWeightTotal = TerrainTypeWeights.Sum();
        TerrainTypePercentiles = TerrainTypeWeights.Select((weight, index) =>
        { return (weight + TerrainTypeWeights.Take(index).Sum()) / terrainTypeWeightTotal; }).ToArray();

        // Create mapping from y level to terrain type
        LevelToTerrainTypeDict = new Dictionary<int, byte>();
        // Bottom layer is always dark sand
        LevelToTerrainTypeDict.Add(0, TerrainType.DarkSand);
        byte lastType = TerrainType.DarkSand;
        for (int level = 1; level <= TerrainLevels; level++)
        {
            float percentile = level / ((float)TerrainLevels - 1);
            for (byte type = 0; type < TerrainTypePercentiles.Length; type++)
            {
                if (percentile <= TerrainTypePercentiles[type])
                {
                    LevelToTerrainTypeDict.Add(level, type);
                    
                    if (lastType == TerrainType.Sand && type == TerrainType.LowGrass)
                    {
                        WaterLevel = level + Constants.TerrainHeightOffset;
                    }
                    lastType = type;
                    
                    break;
                }
            }
        }

        // Calculate noise value offsets
        (xOffset, zOffset) = (UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
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
        float noiseValue = Mathf.Abs(Mathf.Clamp(Mathf.PerlinNoise(x * Constants.GridNoiseScale + xOffset, z * Constants.GridNoiseScale + zOffset), 0, 1) * 2 - 1);
        for (int i = 0; i < TerrainLevelPercentiles.Length; i++)
        {
            if (noiseValue <= TerrainLevelPercentiles[i])
            {
                return i + Constants.TerrainHeightOffset;
            }
        }
        throw new Exception("noiseValue should be less than or equal to the max TerrainLevelPercentiles but is not.");
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
