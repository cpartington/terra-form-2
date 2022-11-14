using System;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public int Seed = -1;
    public int GridXLength = Constants.GridXLength;
    public int GridZLength = Constants.GridZLength;
    public float GridCellHeight = Constants.GridCellHeight;
    public float TerrainNoiseScale = Constants.Scale;
    public int TerrainLevels = Constants.TerrainLevels;
    public int[] TerrainTypeWeights = TerrainComputer.TerrainTypeWeights;

    private void Awake()
    {
        Constants.GridXLength = GridXLength;
        Constants.GridZLength = GridZLength;
        Constants.GridCellHeight = GridCellHeight;
        Constants.Scale = TerrainNoiseScale;
        Constants.TerrainLevels = TerrainLevels;
        TerrainComputer.TerrainTypeWeights = TerrainTypeWeights;

        if (Seed == -1)
        {
            Seed = UnityEngine.Random.Range(0, Int32.MaxValue);
        }
        UnityEngine.Random.InitState(Seed);
    }
}
