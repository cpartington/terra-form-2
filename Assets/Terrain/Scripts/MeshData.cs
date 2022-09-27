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
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 0]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 1]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 2]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 3]]);

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
        for (byte terrainType = 0; terrainType < TerrainTypeTopology.Count; terrainType++)
        {
            TerrainTypeTopology.TryGetValue(terrainType, out List<int> triangles);
            mesh.SetIndices(triangles, MeshTopology.Triangles, terrainType); 
        }

        mesh.RecalculateNormals();
        return mesh;
    }
}
