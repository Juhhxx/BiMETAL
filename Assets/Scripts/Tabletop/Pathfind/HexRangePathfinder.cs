using System.Collections;
using System.Linq;
using UnityEngine;

public class HexRangePathfinder : Pathfinder
{

    public HexRangePathfinder(MonoBehaviour owner, bool nonAvoid) : base(owner, nonAvoid)
    { }

    protected override IEnumerator GetPath(HexagonCell objective, HexagonCell start, int totalWeight = -1)
    {
        Done = false;

        yield return null;

        Done = true;
    }
}
