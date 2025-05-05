using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatePatrolling", menuName = "State Machines/StatePatrolling")]
public class StatePatrolling : StateAbstract
{
    private GameObject  gameObject;
    private Transform   transform;

    [SerializeField] private float _numberOfWaypoints;
    [SerializeField] private float _maximumDistance;
    [SerializeField] private LayerMask _layersToRaycast;

    private EnemyMovement   _movement;
    private Vector3         _target;
    private List<Vector3>   _waypoints;
    private int             _currentWaypoint = 0;
    private bool            _increase = true;

    protected override void EntryAction()
    {
        Debug.Log("Entering Patrol");

        GenerateWaypoints(); 
        ChooseWaypoint();
        _movement.SetSpeed(_movement.MaxSpeed);
        _movement.SetAcceptanceRadius(0.5f);
        _movement.SetDestination(_target);
    }
    protected override void StateAction()
    {
        Debug.Log("Patrolling");
        DrawWaypoints();
        NavigateWaypoints();
    }
    protected override void ExitAction()
    {
        Debug.Log("Exit Patrol");
    }
    public override void InstantiateState()
    {
        gameObject  = base.objectReference;
        transform   = gameObject.transform;
        _movement   = GetComponent<EnemyMovement>(gameObject);
        _waypoints  = new List<Vector3>();

        base.state = new State(base.Name, EntryAction, StateAction, ExitAction);
    }

    private void NavigateWaypoints()
    {
        Debug.Log($"Has reached target? {Vector3.Distance(transform.position, _target) < 2.0f}");
        if (Vector3.Distance(transform.position, _target) < 0.6f)
        {
            ChooseWaypoint();
            _movement.SetDestination(_target);
        }
    }
    private void ChooseWaypoint()
    {
        if (_increase) _currentWaypoint += 1;
        else           _currentWaypoint -= 1;

        if (_currentWaypoint == _numberOfWaypoints - 1) _increase = false;
        if (_currentWaypoint == 0) _increase = true;

        _target = _waypoints[_currentWaypoint];
    }
    private void GenerateWaypoints()
    {
        _waypoints.Clear();

        for (int i = 0; i < _numberOfWaypoints; i++)
        {
            NavMeshHit hit;
            Vector3 newWP = Vector3.zero;

            if (i == 0) newWP = transform.position;
            else
            {
                Vector2 rotation;
                Vector3 direction;
                do
                {
                    newWP = _waypoints[i - 1];
                    
                    rotation    = Random.insideUnitCircle;
                    direction   = new Vector3(rotation.x, 0f, rotation.y);
                    direction   = direction.normalized;

                    newWP += direction * _maximumDistance;

                    Debug.Log($"Testing Point at {newWP}");
                    Debug.Log($"New point Raycast Test: {Physics.Raycast(_waypoints[i - 1], direction, _maximumDistance, _layersToRaycast)}");
                    Debug.Log($"New point NavMesh Test: {NavMesh.SamplePosition(newWP, out hit, 1.1f, 1)}");
                }
                while (Physics.Raycast(_waypoints[i - 1], direction, _maximumDistance, _layersToRaycast) || 
                      !NavMesh.SamplePosition(newWP, out hit, 1.1f, 1));

            }

            _waypoints.Add(newWP);
        }
    }
    
    private void DrawWaypoints()
    {
        for (int i = 0; i < _numberOfWaypoints; i++)
        {
            Vector3 wp = _waypoints[i];

            DrawCross(wp, 1, Color.green);

            if (i > 0) Debug.DrawLine(wp, _waypoints[i - 1], Color.green);
        }
    }
    private void DrawCross(Vector3 position, float size, Color color)
    {
        Vector3 bgnX = position + (Vector3.right * (size/2));
        Vector3 endX = position + (Vector3.left * (size/2));

        Debug.DrawLine(bgnX, endX, color);

        Vector3 bgnY = position + (Vector3.up * (size/2));
        Vector3 endY = position + (Vector3.down * (size/2));

        Debug.DrawLine(bgnY, endY, color);

        Vector3 bgnZ = position + (Vector3.forward * (size/2));
        Vector3 endZ = position + (Vector3.back * (size/2));

        Debug.DrawLine(bgnZ, endZ, color);
    }
}
