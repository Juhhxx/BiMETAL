using TMPro;
using UnityEngine;
using System.Collections;

public class HeallthBar : MonoBehaviour
{
    [SerializeField] private TextMeshPro    _tmp;
    [SerializeField] private Transform      _hpBar;
    [SerializeField] private float          _updateSpeed = 2.0f;
    private Transform      _lookTarget;
    private Character _character;
    private float _fullHP;
    private YieldInstruction _wait = new WaitForEndOfFrame();
    private Coroutine _currentChange;

    private void Start()
    {
        CharController _charContoller = GetComponentInParent<CharController>();
        _character = _charContoller.Character;
        _lookTarget = Camera.main.transform;

        _tmp.text = _character.Name;
        _fullHP = _character.HP;
    }
    private void Update()
    {
        // UpdateBar();

        if ( Camera.main != null )
            _lookTarget = Camera.main.transform;
        
        transform.LookAt(_lookTarget);
    }
    public void UpdateBar()
    {
        Vector3 currentHPPercent = _hpBar.localScale;

        currentHPPercent.x = _character.HP / _fullHP;

        if (_currentChange != null)
            StopCoroutine(_currentChange);
        
        _currentChange = StartCoroutine(ChangeScale(_hpBar.localScale,currentHPPercent.x));

        // _hpBar.localScale = currentHPPercent;
    }
    private IEnumerator ChangeScale(Vector3 currentScale, float hp)
    {
        Vector3 newScale = currentScale;
        float i = 0;

        while (newScale.x != hp)
        {
            newScale.x = Mathf.Lerp(currentScale.x,hp,i);

            // Debug.Log($"COLOR {newColor} = {color} ? {newColor == color}");

            _hpBar.localScale = newScale;

            i += _updateSpeed * Time.deltaTime;

            yield return _wait;
        }
    }
}
