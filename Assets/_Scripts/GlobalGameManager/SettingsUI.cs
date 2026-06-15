using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsSO _settings;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        _settings.Load();

        masterVolumeSlider.SetValueWithoutNotify(_settings.MasterVolume);
        bgmVolumeSlider.SetValueWithoutNotify(_settings.BGMVolume);
        sfxVolumeSlider.SetValueWithoutNotify(_settings.SFXVolume);

        masterVolumeSlider.onValueChanged.AddListener(v => _settings.MasterVolume = v);
        bgmVolumeSlider.onValueChanged.AddListener(v => _settings.BGMVolume = v);
        sfxVolumeSlider.onValueChanged.AddListener(v => _settings.SFXVolume = v);
    }

    public void OnApplyButton() => _settings.Save();
    public void OnCancelButton() => _settings.Load(); // 저장 안 하고 닫으면 롤백
}