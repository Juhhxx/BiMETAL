using System;

public class CellData : IComparable<CellData>
{
    // Cost until now
    public float G { get; set; }
    public float H { get; set; }
    public float F => G + H;
    public CellData Connection { get; set; }
    public HexagonCell Cell { get; private set; }

    public CellData(HexagonCell cell, float g, float h, CellData connection)
    {
        G = g;
        H = h;
        Cell = cell;
        Connection = connection;
    }

    /*                  cellData.G = costToNeighbor;
                        // Do we need this line?
                        cellData.H = neighbor.GetDistance(objective);
                        cellData.Connection = _data[current];
                        */

    public override string ToString()
    {
        return $"GHF: ({G}, {H}, {F}), Cell:{Cell}";
    }

    public int CompareTo(CellData other)
    {
        if (other == null)
            return 0;

        if (F < other.F) return -1;
        if (F > other.F) return 1;

        if (H < other.H) return -1;
        if (H > other.H) return 1;

        // Use randoms to decide if they are both the same

        return GetHashCode().CompareTo(other.GetHashCode());
    }

    public int GetPathPoints()
    {
        return Cell.Weight + (Connection != null ? Connection.GetPathPoints() : 0);
    }
}