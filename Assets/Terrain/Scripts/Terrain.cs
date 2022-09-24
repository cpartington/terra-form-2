using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    public static Terrain Instance { get; private set; }

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private readonly int XLength = Constants.GridXLength;
    private readonly int ZLength = Constants.GridZLength;
    private readonly int YLength = Constants.GridYLength;
    //private readonly Vector3 OriginPosition = new Vector3(-(Constants.GridXLength / 2), 0, -(Constants.GridZLength / 2));
    private int WaterLevel;

    private TerrainCell[,,] TerrainGrid;

    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    private void Awake()
    {
        Instance = this;
        WaterLevel = TerrainComputer.Instance.WaterLevel + Constants.TerrainHeightOffset;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        InitTerrainGrid();
        UpdateTerrainMesh();

        stopwatch.Stop();
        Debug.Log($"{XLength} x {ZLength} cells loaded in {stopwatch.Elapsed} seconds."); // TODO fix this value
    }

    private void InitTerrainGrid()
    {
        TerrainGrid = new TerrainCell[XLength, YLength, ZLength];

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
                        TerrainGrid[x, y, z] = new TerrainCell(TerrainCellType.Ground);
                    }
                    else if (y < WaterLevel)
                    {
                        TerrainGrid[x, y, z] = new TerrainCell(TerrainCellType.Water);
                    }
                    else
                    {
                        TerrainGrid[x, y, z] = new TerrainCell(TerrainCellType.Air);
                    }
                }
            }
        }
    }

    private void UpdateTerrainMesh()
    {
        ClearMeshData();

        for (int y = 0; y < YLength; y++)
        {
            for (int x = 0; x < XLength; x++)
            {
                for (int z = 0; z < ZLength; z++)
                {
                    if (TerrainGrid[x, y, z].cellType == TerrainCellType.Ground)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                    }
                }
            }
        }

        CreateMesh();
    }

    private void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    private void UpdateMeshData(Vector3 pos)
    {
        // Iterate over the six faces of the cell
        for (int f = 0; f < 6; f++)
        {
            // If the face is an edge face, add its data to the mesh
            if (IsEdgeFace(pos + Constants.FaceCheckDirections[f]))
            {
                vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[f, 0]]);
                vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[f, 1]]);
                vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[f, 2]]);
                vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[f, 3]]);

                uvs.Add(Constants.CubeUvs[0]);
                uvs.Add(Constants.CubeUvs[1]);
                uvs.Add(Constants.CubeUvs[2]);
                uvs.Add(Constants.CubeUvs[3]);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    private bool IsEdgeFace(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x >= XLength || y < 0 || y >= YLength || z < 0 || z >= ZLength)
        {
            return true;
        }

        return TerrainGrid[x, y, z].cellType != TerrainCellType.Ground;
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    //public Vector3 GetWorldPosition(int x, int y, int z)
    //{
    //    var v = new Vector3(x, 0, z) + OriginPosition;
    //    v.y = y * Constants.GridCellHeight;
    //    return v;
    //}
}
