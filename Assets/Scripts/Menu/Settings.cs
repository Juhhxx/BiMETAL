using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsObject;
    [SerializeField] private Slider _brightness;
    [SerializeField] private TMP_Text _brightnessText;
    [SerializeField] private Slider _sensitivity;
    [SerializeField] private TMP_Text _sensitivityText;
    [SerializeField] private Slider _volume;
    [SerializeField] private TMP_Text _volumeText;


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
        if ( _settingsObject != null )
            _settingsObject.SetActive(true);
    }

    public void TurnOffSettings()
    {
        if ( _settingsObject != null )
            _settingsObject.SetActive(false);
    }

    public bool GetActive()
    {
        return _settingsObject.activeSelf;
    }

    public void ChangeBrightness(float value)
    {
        if ( _postExposure == null ) return;
        
        float final =  _clampBrightness * (value - _brightness.minValue)
            / (_brightness.maxValue - _brightness.minValue);

        _postExposure.postExposure.value =  final;
        
        _brightnessText.text = FormatShort(final);
    }

    public void ChangeSensitivity(float value)
    {
        float final = _maxSensitivity * (value - _sensitivity.minValue)
            / (_sensitivity.maxValue - _sensitivity.minValue);

        final = Mathf.Max(1f, final);

        InputManager.MouseSensitivity = final;

        _sensitivityText.text = FormatShort(final);
    }

    public void ChangeVolume(float value)
    {
        float final = _maxVolume * (value - _volume.minValue)
            / (_volume.maxValue - _volume.minValue);
            
        AudioListener.volume = final;
        
        _volumeText.text = FormatShort(final);
    }

    private string FormatShort(float value)
    {
        if (value >= 10f)
            return Mathf.RoundToInt(value).ToString();
        else
            return value.ToString("0.0");
    }

    public void OnDestroy()
    {
        _brightness.onValueChanged.RemoveListener(ChangeBrightness);
        _sensitivity.onValueChanged.RemoveListener(ChangeSensitivity);
        _volume.onValueChanged.RemoveListener(ChangeVolume);
    }
}
