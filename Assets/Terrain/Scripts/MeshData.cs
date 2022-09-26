using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    private int vertexIndex = 0;
    private List<Vector3> vertices = new();
    private List<Vector2> uvs = new();
    private List<int> triangles = new();

    public void Clear()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    public void Add(Vector3 pos, int face)
    {
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 0]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 1]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 2]]);
        vertices.Add(pos + Constants.CubeVertices[Constants.CubeTriangles[face, 3]]);

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

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mesh.SetVertices(vertices);
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        return mesh;
    }
}
