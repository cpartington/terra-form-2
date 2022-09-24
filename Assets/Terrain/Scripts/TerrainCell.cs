using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCell
{
    //public int x;
    //public int y;
    //public int z;

    //public TerrainType type;
    public TerrainCellType cellType;

    public TerrainCell(TerrainCellType cellType)
    {
        this.cellType = cellType;
    }
}