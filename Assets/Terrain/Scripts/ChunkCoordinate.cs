using System;
/// <summary>
/// Corresponds to a chunk's position within the chunk map.
/// </summary>
public struct ChunkCoordinate
{
    public int X { get; }
    public int Z { get; }

    public ChunkCoordinate(int x, int z)
    {
        X = x;
        Z = z;
    }

    public override bool Equals(object obj)
    {
        return obj is ChunkCoordinate coordinate &&
               X == coordinate.X &&
               Z == coordinate.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public static bool operator ==(ChunkCoordinate a, ChunkCoordinate b)
    {
        return a.X == b.X && a.Z == b.Z;
    }

    public static bool operator !=(ChunkCoordinate a, ChunkCoordinate b)
    {
        return a.X != b.X || a.Z != b.Z;
    }
}
