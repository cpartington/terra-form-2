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

    private readonly int XLength = Constants.GridXLength;
    private readonly int ZLength = Constants.GridZLength;
    private readonly int YLength = Constants.GridYLength;
    private readonly Vector3 OriginPosition = new Vector3(-(Constants.GridXLength / 2f), 0, -(Constants.GridZLength / 2f));

    private byte[,,] TerrainGrid;
    private int WaterLevel;

    private void Awake()
    {
        Instance = this;
        TerrainMeshData = new();
        WaterLevel = TerrainComputer.Instance.WaterLevel;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        InitTerrainGrid();
        UpdateTerrainMesh();

        stopwatch.Stop();
        Debug.Log($"{ZLength} x {XLength} grid loaded in {stopwatch.Elapsed} seconds.");
    }

    /// <summary>
    /// Initializes the terrain grid.
    /// </summary>
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

    /// <summary>
    /// Updates the terrain mesh data for each x,y,z cell.
    /// </summary>
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
                        AddCellToMesh(ToWorldPosition(x, y, z), TerrainGrid[x, y, z]);
                    }
                }
            }
        }

        Mesh mesh = TerrainMeshData.CreateMesh();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Determines which faces of a single cell should be added to the mesh.
    /// </summary>
    /// <param name="pos">Position of the current cell in the world</param>
    /// <param name="terrainType">Terrain type of the current cell</param>
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
    /// <param name="adjacentPosition">Adjacent position to check against</param>
    /// <param name="terrainType">Terrain type of the original position</param>
    /// <returns>True if the original position is an edge, otherwise False</returns>
    private bool IsEdgeFace(Vector3 adjacentPosition, byte terrainType)
    {
        // Get x,y,z of the adjacent position
        (int x, int y, int z) = FromWorldPosition(adjacentPosition);

        // Cells on the edge of the grid are always edge faces
        if (x < 0 || x >= XLength || y < 0 || y >= YLength || z < 0 || z >= ZLength)
        {
            return true;
        }

        if (TerrainType.IsGround(terrainType))
        {
            // Don't draw edges between solid blocks
            return !TerrainType.IsGround(TerrainGrid[x, y, z]);
        }
        else if (terrainType == TerrainType.Water)
        {
            // Only draw water edges that face air
            return TerrainGrid[x, y, z] == TerrainType.Air;
        }

        return false;
    }

    /// <summary>
    /// Converts an x,y,z coordinate from the terrain grid into its associated world position.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <param name="z">z coordinate</param>
    /// <returns>Vector representing the world position</returns>
    public Vector3 ToWorldPosition(int x, int y, int z)
    {
        return new Vector3(x * Constants.GridCellWidth, y * Constants.GridCellHeight, z * Constants.GridCellWidth) + OriginPosition;
    }

    /// <summary>
    /// Converts a world position into its associated x,y,z coordinate in the terrain grid.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>x,y,z coordinates</returns>
    public (int, int, int) FromWorldPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / Constants.GridCellWidth - OriginPosition.x);
        int y = Mathf.FloorToInt(position.y / Constants.GridCellHeight - OriginPosition.y);
        int z = Mathf.FloorToInt(position.z / Constants.GridCellWidth - OriginPosition.z);
        return (x, y, z);
    }
}
