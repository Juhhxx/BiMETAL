using System.Linq;
using UnityEngine;

public class EnvironmentInteractive : ModifierInteractive
{
    public bool Modified { get; protected set; } = true;

    protected override void Start()
    {
        base.Start();
        Modified = false;

        if ( HasModifier )
        {
            _modPathfinder = PathfinderChooser.ChooseRange(this, _modRangeType);

            if ( _modPathfinder != null )
                _modPathfinder.Path.CollectionChanged += DemonstratePath;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " environment Interactive does not have modifier, aborting game object. ");
            Destroy(gameObject);
        }
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

        // Debug.Log("modifier? setting path as fr fr hopefully, is path null or count 0?  " + (_modPathfinder.Path.Count == 0 || _modPathfinder.Path == null));

        // we just clear the current path to save the current cells settings and move on

        foreach ( HexagonCell cell in _modPathfinder.Path)
            cell.SetEnvironment();
       _modPathfinder.Path.Clear();
    }

    public override void Path(ObservableStack<HexagonCell> other = null)
    {
        Debug.Log("Pathing: " + ( other != null && other.Count > 0 ) + " and modified is: " + Modified);

        if ( Modified ) return;

        
        // Debug.Log("modifier? modifying? " + (other == null));
        // Debug.Log("modifier " + gameObject.name + " trying to path cell: " + Cell + " and other " + other?.Contains(Cell) + " contains it");
        
        if ( other == null || other.Count <= 0 )
        {
            Debug.Log("Un-Pathing modifier? stopping");
            _modPathfinder.Stop();
            return;
        }

        ObservableStack<HexagonCell> clone = new(other);

        HexagonCell last = clone.Peek();

        while ( clone.Count > 0 && last != Cell)
        {
            last = clone.Pop();
            // Debug.Log("modifier clone at: " + clone.Count);

            if ( !clone.Any() || clone.Peek() == Cell)
                break;
        }

        if ( clone.Count < 1 )
            return;

        // Debug.Log("modifier? starting");
        // only supposed to do this once
        _modPathfinder.FindPath(Cell, last, _reach);
    }
}
