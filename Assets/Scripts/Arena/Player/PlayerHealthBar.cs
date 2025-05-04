using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private CharController     _playerCharacter;
    [SerializeField] private TextMeshProUGUI    _tmp;
    [SerializeField] private Image              _hpBar;
    [SerializeField] private float              _updateSpeed = 2.0f;
    private Character _character;
    private float _fullHP;
    private YieldInstruction _wait = new WaitForEndOfFrame();
    private Coroutine _currentChange;

    private void Start()
    {
        _character = _playerCharacter.Character;

        _tmp.text = $"HP : {_character.HP} / {_character.HP}";
        _fullHP = _character.HP;
    }

    public void UpdateBar()
    {
        float currentHPPercent = _hpBar.fillAmount;

        currentHPPercent = _character.HP / _fullHP;

        if (_currentChange != null)
            StopCoroutine(_currentChange);
        
        _currentChange = StartCoroutine(ChangeScale(_hpBar.fillAmount,currentHPPercent));

        // _hpBar.localScale = currentHPPercent;
    }
    private IEnumerator ChangeScale(float currentHP, float hp)
    {
        float newHP = currentHP;
        float i = 0;

        while (newHP != hp)
        {
            newHP = Mathf.Lerp(currentHP,hp,i);

            _hpBar.fillAmount = newHP;

            _tmp.text = $"HP : {newHP * _fullHP:f0} / {_fullHP}";

            i += _updateSpeed * Time.deltaTime;

            yield return _wait;
        }
    }
}
