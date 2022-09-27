using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    public static Terrain Instance { get; private set; }

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshData TerrainMeshData;

    private readonly int XLength = Constants.GridXLength;
    private readonly int ZLength = Constants.GridZLength;
    private readonly int YLength = Constants.GridYLength;
    //private readonly Vector3 OriginPosition = new Vector3(-(Constants.GridXLength / 2), 0, -(Constants.GridZLength / 2));
    private int WaterLevel;

    private byte[,,] TerrainGrid;

    private void Awake()
    {
        Instance = this;
        TerrainMeshData = new();
        WaterLevel = TerrainComputer.Instance.WaterLevel + Constants.TerrainHeightOffset;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        InitTerrainGrid();
        UpdateTerrainMesh();

        stopwatch.Stop();
        Debug.Log($"{XLength} x {ZLength} cells loaded in {stopwatch.Elapsed} seconds."); // TODO fix this value
    }

    private void InitTerrainGrid()
    {
        TerrainGrid = new byte[XLength, YLength, ZLength];

        // Iterate over all (x,z) coordinates
        for (int x = 0; x < XLength; x++)
        {
            for (int z = 0; z < ZLength; z++)
            {
                // Determine terrain height for this (x,z) coordinate
                int groundHeight = TerrainComputer.Instance.CalculateGroundHeight(x, z);

                // Create cells for this (x,z) coordinate
                for (int y = 0; y < YLength; y++)
                {
                    if (y < groundHeight)
                    {
                        TerrainGrid[x, y, z] = TerrainComputer.Instance.LevelToTerrainType(y);
                    }
                    else if (y < WaterLevel)
                    {
                        TerrainGrid[x, y, z] = TerrainType.Water;
                    }
                    else
                    {
                        TerrainGrid[x, y, z] = TerrainType.Air;
                    }
                }
            }
        }
    }

    private void UpdateTerrainMesh()
    {
        TerrainMeshData.Clear();

        for (int y = 0; y < YLength; y++)
        {
            for (int x = 0; x < XLength; x++)
            {
                for (int z = 0; z < ZLength; z++)
                {
                    if (TerrainGrid[x, y, z] != TerrainType.Air)
                    {
                        AddCellToMesh(new Vector3(x, y, z), TerrainGrid[x, y, z]);
                    }
                }
            }
        }

        Mesh mesh = TerrainMeshData.CreateMesh();
        meshFilter.mesh = mesh;
    }

    private void AddCellToMesh(Vector3 pos, byte terrainType)
    {
        // Iterate over the six faces of the cell
        for (int f = 0; f < 6; f++)
        {
            // If the face is an edge face, add its data to the mesh
            if (IsEdgeFace(pos + Constants.FaceCheckDirections[f], terrainType))
            {
                TerrainMeshData.Add(pos, f, terrainType);
            }
        }
    }

    /// <summary>
    /// Determines if a face is an edge face (and therefore should be added to the mesh).
    /// </summary>
    /// <param name="pos">New position to check against</param>
    /// <param name="terrainType">Terrain type of the original position</param>
    /// <returns></returns>
    private bool IsEdgeFace(Vector3 pos, byte terrainType)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x >= XLength || y < 0 || y >= YLength || z < 0 || z >= ZLength)
        {
            return true;
        }

        if (TerrainType.IsGround(terrainType))
        {
            return !TerrainType.IsGround(TerrainGrid[x, y, z]);
        }
        else if (terrainType == TerrainType.Water)
        {
            return TerrainGrid[x, y, z] == TerrainType.Air;
        }

        return false;
    }

    //public Vector3 GetWorldPosition(int x, int y, int z)
    //{
    //    var v = new Vector3(x, 0, z) + OriginPosition;
    //    v.y = y * Constants.GridCellHeight;
    //    return v;
    //}
}
