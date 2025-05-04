using NaughtyAttributes;
using UnityEngine;

public class KeepPosition : MonoBehaviour
{
    [SerializeField] private bool       _keepX;
    [ShowIf(EConditionOperator.And, "KeepX", "NotChangeAll")][SerializeField] private float _xValue;
    [SerializeField] private bool       _keepY;
    [ShowIf(EConditionOperator.And, "KeepY", "NotChangeAll")][SerializeField] private float _yValue;
    [SerializeField] private bool       _keepZ;
    [ShowIf(EConditionOperator.And, "KeepZ", "NotChangeAll")][SerializeField] private float _zValue;
    private bool KeepX => _keepX;
    private bool KeepY => _keepY;
    private bool KeepZ => _keepZ;

    [HideIf("NotChangeAll")][SerializeField] private Vector3    _position;
    private bool NotChangeAll => !(_keepX && _keepY && _keepZ);
    
    [SerializeField] private bool _useLocalPosition;

    private void Start()
    {
        if (NotChangeAll) _position = new Vector3(_xValue, _yValue, _zValue);
    }
    private void Update()
    {
        Vector3 newPosition;
        
        if (_useLocalPosition) newPosition = transform.localPosition;
        else newPosition = transform.position;

        if (_keepX) newPosition.x = _position.x;
        if (_keepY) newPosition.y = _position.y;
        if (_keepZ) newPosition.z = _position.z;

        if (_useLocalPosition) transform.localPosition = newPosition;
        else transform.position = newPosition;
    }
}
