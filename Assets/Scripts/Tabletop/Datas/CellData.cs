using System;

public class CellData : IComparable<CellData>
{
    public float G { get; set; }
    public float H { get; set; }
    public float F => G + H;
    public HexagonCell Connection { get; set; }
    public HexagonCell Cell  { get; private set; }

    public CellData(HexagonCell cell, float g, float h, HexagonCell connection)
    {
        G = g;
        H = h;
        Cell = cell;
        Connection = connection;
    }
    public override string ToString()
    {
        return $"GHF: ({G}, {H}, {F}) connected to :{(Connection? Connection : "null")}";
    }

    public int CompareTo(CellData other)
    {
        if (other == null)
            return 0;

        if (F < other.F) return -1;
        if (F > other.F) return 1;

        if (H < other.H) return -1;
        if (H > other.H) return 1;

        return GetHashCode().CompareTo(other.GetHashCode());
    }
}