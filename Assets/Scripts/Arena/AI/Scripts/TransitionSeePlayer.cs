using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "TransitionSeePlayer", menuName = "State Machines/TransitionSeePlayer")]
public class TransitionSeePlayer : TransitionAbstract
{
    private GameObject  gameObject;
    private Transform   transform;
    [SerializeField] private float _visionRange;
    [SerializeField] [Range(0f,180f)] private float _fov;
    [SerializeField] private float _sensingRange;

    private GameObject  _target;
    private Vector3     _directionToTarget;

    protected override void Action()
    {
        Debug.Log("SAW PLAYER!");
    }
    protected override bool Condition()
    {
        DrawWireArc(transform.position, transform.forward, _fov, _visionRange, Color.red);
        DrawWiredCircle(transform.position, transform.forward, _sensingRange, Color.red);
        return CheckForPlayer();
    }
    public override void InstantiateTransition()
    {
        gameObject = base.objectReference;
        transform = gameObject.transform;
        _target = FindAnyObjectByType<PlayerMovement>().gameObject;

        base.transition = new Transition(base.Name, Condition, base.ToState.State, Action);
    }

    private bool CheckForPlayer()
    {
        _directionToTarget = _target.transform.position - transform.position;

        if (Vector3.Distance(transform.position, _target.transform.position) < _visionRange)
        {
            if (Vector3.Dot(transform.forward, _directionToTarget) > 0 &&
            Vector3.Angle(transform.forward, _directionToTarget) < (_fov/2))
            {
                if (Physics.Raycast(transform.position, _directionToTarget, out RaycastHit hit))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.blue);
                    if (hit.collider.gameObject == _target) return true;
                }
            }
        }

        if (Vector3.Distance(transform.position, _target.transform.position) < _sensingRange)
        {
            if (Physics.Raycast(transform.position, _directionToTarget, out RaycastHit hit))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
                if (hit.collider.gameObject == _target) return true;
            }
        }

        return false;
    }
    private void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, Color color, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Debug.DrawLine(posA, posB, color);

            angle += stepAngles;
            posA = posB;
        }
        Debug.DrawLine(posA, initialPos, color);
    }
    private void DrawWiredCircle(Vector3 position, Vector3 forward, float radius, Color color, float maxSteps = 20)
    {
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = 360 / maxSteps;
        var angle = 0f;
        for (var i = 0; i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));
            
            if (i > 0) Debug.DrawLine(posA, posB, color);

            angle += stepAngles;
            posA = posB;
        }
    }
    private float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
    

}