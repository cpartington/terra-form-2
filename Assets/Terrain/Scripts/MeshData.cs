using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    private int vertexIndex = 0;
    private List<Vector3> vertices = new();
    private List<Vector2> uvs = new();
    private List<int> triangles = new();

    private Dictionary<byte, List<int>> TerrainTypeTopology = new()
    {
        { TerrainType.DarkSand, new() },
        { TerrainType.Sand, new() },
        { TerrainType.LowGrass, new() },
        { TerrainType.MidGrass, new() },
        { TerrainType.HighGrass, new() },
        //{ TerrainType.Water, new() },
    };

    public void Clear()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    public void Add(Vector3 pos, int face, byte terrainType)
    {
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 0]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 1]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 2]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 3]]);

        uvs.Add(Constants.CubeUvs[0]);
        uvs.Add(Constants.CubeUvs[1]);
        uvs.Add(Constants.CubeUvs[2]);
        uvs.Add(Constants.CubeUvs[3]);

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
            subMeshCount = 5
        };
        mesh.SetVertices(vertices);
        for (byte terrainType = 0; terrainType < 5; terrainType++)
        {
            TerrainTypeTopology.TryGetValue(terrainType, out List<int> triangles);
            mesh.SetIndices(triangles, MeshTopology.Triangles, terrainType); 
        }
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        return mesh;
    }
}
