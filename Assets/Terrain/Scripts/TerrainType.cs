public static class TerrainType
{
    public const byte DarkSand = 0;
    public const byte Sand = 1;
    public const byte LowGrass = 2;
    public const byte MidGrass = 3;
    public const byte HighGrass = 4;
    public const byte Water = 5;
    public const byte Air = 6;

    public static bool IsGround(byte terrain)
    {
        switch (terrain)
        {
            case DarkSand:
            case Sand:
            case LowGrass:
            case MidGrass:
            case HighGrass:
                return true;
            default:
                return false;
        }
    }
}