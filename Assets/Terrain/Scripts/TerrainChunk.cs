using UnityEngine;

public class TerrainChunk
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public GameObject chunkObject;

    public MeshData TerrainMeshData;

    /// <summary>
    /// The chunk's position in the global chunk map
    /// </summary>
    public ChunkCoordinate ChunkPosition;

    /// <summary>
    /// Terrain grid for the chunk
    /// </summary>
    private byte[,,] TerrainGrid;

    /// <summary>
    /// The chunk's X position in the world-level terrain grid
    /// </summary>
    private readonly int BaseXPosition;

    /// <summary>
    /// The chunk's Z position in the world-level terrain grid
    /// </summary>
    private readonly int BaseZPosition;

    public TerrainChunk(ChunkCoordinate position)
    {
        ChunkPosition = position;
        BaseXPosition = position.X * Constants.ChunkSize;
        BaseZPosition = position.Z * Constants.ChunkSize;
        TerrainMeshData = new();

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();

        chunkObject.transform.SetParent(Terrain.Instance.transform);
        chunkObject.transform.position = new Vector3(BaseXPosition, 0f, BaseZPosition);
        chunkObject.name = "Chunk " + ChunkPosition.X + ", " + ChunkPosition.Z;
        meshRenderer.materials = Terrain.Instance.meshRenderer.materials;

        InitTerrainGrid();
        UpdateTerrainMesh();
    }

    /// <summary>
    /// Initializes the chunk's terrain grid.
    /// </summary>
    private void InitTerrainGrid()
    {
        TerrainGrid = new byte[Constants.ChunkSize, Constants.GridYLength, Constants.ChunkSize];

        // Iterate over all (x,z) coordinates
        for (int x = 0; x < Constants.ChunkSize; x++)
        {
            for (int z = 0; z < Constants.ChunkSize; z++)
            {
                // Determine terrain height for this (x,z) coordinate
                int groundHeight = TerrainComputer.Instance.CalculateGroundHeight(x + this.BaseXPosition, z + this.BaseZPosition);

                // Create cells for this (x,z) coordinate
                for (int y = 0; y < Constants.GridYLength; y++)
                {
                    if (y < groundHeight)
                    {
                        TerrainGrid[x, y, z] = TerrainComputer.Instance.LevelToTerrainType(y);
                    }
                    else if (y < TerrainComputer.Instance.WaterLevel)
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
    /// Updates the chunk's terrain mesh data for each x,y,z cell.
    /// </summary>
    private void UpdateTerrainMesh()
    {
        TerrainMeshData.Clear();

        for (int y = 0; y < Constants.GridYLength; y++)
        {
            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int z = 0; z < Constants.ChunkSize; z++)
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
        if (x < 0 || x >= Constants.ChunkSize || y < 0 || y >= Constants.GridYLength || z < 0 || z >= Constants.ChunkSize)
        {
            // Don't draw water edge faces
            return terrainType == TerrainType.Water? false : true;
        }

        if (TerrainType.IsGround(terrainType))
        {
            // Don't draw the edge if the adjacent cell is also ground
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
    /// Converts an x,y,z coordinate from the local chunk grid into its local world position.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <param name="z">z coordinate</param>
    /// <returns>Vector representing the world position</returns>
    public Vector3 ToWorldPosition(int x, int y, int z)
    {
        return new Vector3(
            x: x * Constants.GridCellWidth,
            y: y * Constants.GridCellHeight,
            z: z * Constants.GridCellWidth
        );
    }

    /// <summary>
    /// Converts a local world position into its associated x,y,z coordinate in the local chunk grid.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>x,y,z coordinates</returns>
    public (int, int, int) FromWorldPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / Constants.GridCellWidth);
        int y = Mathf.FloorToInt(position.y / Constants.GridCellHeight);
        int z = Mathf.FloorToInt(position.z / Constants.GridCellWidth);
        return (x, y, z);
    }
}
