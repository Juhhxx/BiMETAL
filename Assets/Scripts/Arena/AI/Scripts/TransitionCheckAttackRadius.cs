using AI.FSMs.UnityIntegration;
using AI.FSMs.BaseFiles;
using UnityEngine;

[CreateAssetMenu(fileName = "TransitionCheckAttackRadius", menuName = "State Machines/Transitions/TransitionCheckAttackRadius")]
public class TransitionCheckAttackRadius : TransitionAbstract
{
    private GameObject  gameObject;
    private Transform   transform;

    [SerializeField] private float _radius;
    [SerializeField] private bool _checkIfInside;
    private Transform _target;

    protected override void Action()
    {
        Debug.Log($"Transition Attack Radius");
    }
    protected override bool Condition()
    {
        DrawWiredCircle(transform.position,transform.forward,_radius,Color.red);
        return (Vector3.Distance(transform.position, _target.position) < _radius) == _checkIfInside;
    }
    public override void InstantiateTransition()
    {
        gameObject = base.objectReference;
        transform = gameObject.transform;
        _target = FindObjectByType<PlayerMovement>().transform;

        base.transition = new Transition(base.Name, Condition, base.ToState.State, Action);
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
}