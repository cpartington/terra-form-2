using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    public static Terrain Instance { get; private set; }

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public MeshData TerrainMeshData;

    public GameObject player;

    private Dictionary<ChunkCoordinate, TerrainChunk> ChunkMap;
    private const int ChunkMapBaseMinValue = Constants.WorldSizeInChunks / -2;
    private const int ChunkMapBaseMaxValue = Constants.WorldSizeInChunks + ChunkMapBaseMinValue;

    private void Start()
    {
        Instance = this;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        InitializeWorld();

        stopwatch.Stop();
        Debug.Log($"{Constants.WorldSizeInChunks * Constants.WorldSizeInChunks} chunks loaded in {stopwatch.Elapsed} seconds.");
    }    

    private void InitializeWorld()
    {
        ChunkMap = new Dictionary<ChunkCoordinate, TerrainChunk>();

        for (int x = ChunkMapBaseMinValue; x < ChunkMapBaseMaxValue; x++)
        {
            for (int z = ChunkMapBaseMinValue; z < ChunkMapBaseMaxValue; z++)
            {
                var coordinate = new ChunkCoordinate(x, z);
                ChunkMap.Add(coordinate, new TerrainChunk(coordinate));
            }
        }
    }

    private void Update()
    {
        var currentCoord = WorldPositionToChunkCoordinate(player.transform.position);

        for (int x = ChunkMapBaseMinValue + currentCoord.X; x < ChunkMapBaseMaxValue + currentCoord.X; x++)
        {
            for (int z = ChunkMapBaseMinValue + currentCoord.Z; z < ChunkMapBaseMaxValue + currentCoord.Z; z++)
            {
                ChunkCoordinate coordinate = new ChunkCoordinate(x, z);
                ChunkMap.TryGetValue(coordinate, out TerrainChunk chunk);
                if (chunk == null)
                {
                    // TODO need to update adjacent mesh to remove redundant edges
                    ChunkMap.Add(coordinate, new TerrainChunk(coordinate));
                }
            }
        }
    }

    private ChunkCoordinate WorldPositionToChunkCoordinate(Vector3 worldPosition)
    {
        return new ChunkCoordinate(
            Mathf.FloorToInt(worldPosition.x / Constants.ChunkSize),
            Mathf.FloorToInt(worldPosition.z / Constants.ChunkSize)
        );
    }
}
