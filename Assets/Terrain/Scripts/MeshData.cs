using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    private int vertexIndex = 0;
    private List<Vector3> vertices = new();
    private List<int> triangles = new();

    private Dictionary<byte, List<int>> TerrainTypeTopology = new()
    {
        { TerrainType.DarkSand, new() },
        { TerrainType.Sand, new() },
        { TerrainType.LowGrass, new() },
        { TerrainType.MidGrass, new() },
        { TerrainType.HighGrass, new() },
        { TerrainType.Water, new() },
    };

    public void Clear()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
    }

    public void Add(Vector3 pos, int face, byte terrainType)
    {
        // Vertices

        for (int i = 0; i < 4; i++)
        {
            if (terrainType == TerrainType.Water)
            {
                vertices.Add(pos + Constants.CubeVerticesWater[Constants.CubeTriangles[face, i]]);
            }
            else
            {
                vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, i]]);
            }
        }

        // Triangles

        List<int> triangles;
        TerrainTypeTopology.TryGetValue(terrainType, out triangles);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);
        vertexIndex += 4;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
            subMeshCount = TerrainTypeTopology.Count
        };
        mesh.SetVertices(vertices);
        for (byte i = 0; i < TerrainTypeTopology.Count; i++)
        {
            TerrainTypeTopology.TryGetValue(i, out List<int> triangles);
            mesh.SetIndices(triangles, MeshTopology.Triangles, i);
        }

        mesh.RecalculateNormals();
        return mesh;
    }
}
