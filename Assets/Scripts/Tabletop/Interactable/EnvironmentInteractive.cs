using System.Linq;
using UnityEngine;

public class EnvironmentInteractive : ModifierInteractive
{
    public bool LastModified { get; protected set; } = false;
    protected override void Start()
    {
        base.Start();
        Modified = false;

        if ( HasModifier )
        {
            ModPathfinder = PathfinderChooser.ChooseRange(this, _modRangeType);

            if ( ModPathfinder != null )
                ModPathfinder.Path.CollectionChanged += DemonstratePath;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " environment Interactive does not have modifier, aborting game object. ");
            Destroy(gameObject);
        }
    }
    public void SetModified(bool currMod)
    {
        Modified = currMod;
    }

    public void SetLast()
    {
        LastModified = Modified;
    }

    public override void Hover(bool onOrOff = true)
    {
        if ( Modified ) return;

        base.Hover(onOrOff);
    }

    public override void Interact(Interactive other = null)
    {
        if ( Modified ) return;

        Modify();

        Modified = true;
    }

    public override void Select()
    {
        base.Select();
    }

    public override void Modify()
    {
        // some cosmetic way of saying the _modifier now already modified and wont be modified again?

        // Debug.Log("modifier? setting path as fr fr hopefully, is path null or count 0?  " + (ModPathfinder.Path.Count == 0 || ModPathfinder.Path == null));

        // we just clear the current path to save the current cells settings and move on

        foreach ( HexagonCell cell in ModPathfinder.Path)
            cell.SetEnvironment();
       ModPathfinder.Path.Clear();
    }

    public override void Path(ObservableStack<HexagonCell> other = null)
    {
        Debug.Log("envi Pathing: " + ( other != null && other.Count > 0 ) + " and modified is: " + Modified);

        if ( Modified ) return;

        
        // Debug.Log("modifier? modifying? " + (other == null));
        // Debug.Log("modifier " + gameObject.name + " trying to path cell: " + Cell + " and other " + other?.Contains(Cell) + " contains it");
        
        if ( other == null || other.Count <= 0 )
        {
            Debug.Log("envi Un-Pathing modifier? stopping");
            ModPathfinder.Stop();
            return;
        }

        ObservableStack<HexagonCell> clone = new(other);

        HexagonCell last = clone.Peek();

        while ( clone.Count > 0 && last != Cell)
        {
            last = clone.Pop();
            Debug.Log("envi modifier clone at: " + clone.Count);

            if ( !clone.Any() || clone.Peek() == Cell)
                break;
        }

        if ( clone.Count < 1 )
            return;

        Debug.Log("envi modifier? starting");
        // only supposed to do this once
        ModPathfinder.FindPath(Cell, last, _reach);
    }

    public int PathCount()
    {
        return ModPathfinder.Path.Count;
    }
}
