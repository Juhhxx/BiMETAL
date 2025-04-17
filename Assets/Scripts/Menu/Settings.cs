using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsObject;
    [SerializeField] private Slider _brightness;
    [SerializeField] private Slider _sensitivity;
    [SerializeField] private Slider _volume;


    [SerializeField] private float _maxSensitivity = 10f;
    [SerializeField] private float _maxVolume = 3f;
    [SerializeField] private float _clampBrightness = 2.5f;

    [SerializeField] private Volume volume;
    private ColorAdjustments _postExposure;

    private void Awake()
    {
        volume.profile.TryGet(out _postExposure);

        _brightness.onValueChanged.AddListener(ChangeBrightness);
        _sensitivity.onValueChanged.AddListener(ChangeSensitivity);
        _volume.onValueChanged.AddListener(ChangeVolume);
    }

    private void Start()
    {
        ChangeBrightness(_brightness.value);
        ChangeSensitivity(_sensitivity.value);
        ChangeVolume(_volume.value);

        TurnOffSettings();
    }

    public void TurnOnSettings()
    {
        _settingsObject.SetActive(true);
    }

    public void TurnOffSettings()
    {
        _settingsObject.SetActive(false);
    }

    public void ChangeBrightness(float value)
    {
        if ( _postExposure == null ) return;
        
        _postExposure.postExposure.value =  _clampBrightness * (value - _brightness.minValue)
            / (_brightness.maxValue - _brightness.minValue);
    }

    public void ChangeSensitivity(float value)
    {
        float final = _maxSensitivity * (value - _sensitivity.minValue)
            / (_sensitivity.maxValue - _sensitivity.minValue);
        InputManager.MouseSensitivity = Mathf.Max(0.1f, final);
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = _maxVolume * (value - _volume.minValue)
            / (_volume.maxValue - _volume.minValue);
    }

    public void OnDestroy()
    {
        _brightness.onValueChanged.RemoveListener(ChangeBrightness);
        _sensitivity.onValueChanged.RemoveListener(ChangeSensitivity);
        _volume.onValueChanged.RemoveListener(ChangeVolume);
    }
}
