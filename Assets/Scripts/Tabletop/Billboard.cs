using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform _transformToFollow;
    [SerializeField] private float _hoverOffset = 0.07f;
    private Vector3 _upDis;

    private void Start()
    {
        _upDis = transform.position - _transformToFollow.position;
        _upDis.x = 0;
        _upDis.z = 0;
    }

    private void Update()
    {
        transform.position = _transformToFollow.position + _upDis;
        transform.position += _hoverOffset * Mathf.Sin(Time.time *2f) * Vector3.up;
        
        if ( Camera.main != null )
            transform.forward = Camera.main.transform.forward;
    }
}