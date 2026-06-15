using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Settings/SettingsData")]
public class SettingsSO : ScriptableObject
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;

    public event Action<float> OnMasterVolumeChanged;
    public event Action<float> OnBGMVolumeChanged;
    public event Action<float> OnSFXVolumeChanged;

    private const string MASTER_PARAM = "Master";
    private const string BGM_PARAM = "BGM";
    private const string SFX_PARAM = "SFX";

    public float MasterVolume
    {
        get => _masterVolume;
        set { 
            _masterVolume = Mathf.Clamp01(value);
            SetMixerVolume(MASTER_PARAM, _masterVolume);
            OnMasterVolumeChanged?.Invoke(value); 
        }
    }
    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            SetMixerVolume(BGM_PARAM, _bgmVolume);
            OnBGMVolumeChanged?.Invoke(value); 
        }
    }
    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            SetMixerVolume(SFX_PARAM, _sfxVolume);
            OnSFXVolumeChanged?.Invoke(value); 
        }
    }

    private void SetMixerVolume(string param, float linear)
    {
        float db = linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
        _mixer.SetFloat(param, db);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}