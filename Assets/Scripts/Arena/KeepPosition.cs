using UnityEngine;

public class KeepPosition : MonoBehaviour
{
    [SerializeField] private Vector3    _position;
    [SerializeField] private bool       _keepX;
    [SerializeField] private bool       _keepY;
    [SerializeField] private bool       _keepZ;
    [SerializeField] private bool       _localPosition;

    private void Update()
    {
        Vector3 newPosition;
        
        if (_localPosition) newPosition = transform.localPosition;
        else newPosition = transform.position;

        if (_keepX) newPosition.x = _position.x;
        if (_keepY) newPosition.y = _position.y;
        if (_keepZ) newPosition.z = _position.z;

        if (_localPosition) transform.localPosition = newPosition;
        else transform.position = newPosition;
    }
}
