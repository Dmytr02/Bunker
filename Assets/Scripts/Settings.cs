using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] List<Slider> Sliders = new List<Slider>();
    [SerializeField] AudioMixer AudioMixer;
    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        foreach (Slider slider in Sliders)
        {
            slider.value = PlayerPrefs.GetFloat($"{slider.name}", 1);
        }
    }

    public void setSlider(Slider slider)
    {
        PlayerPrefs.SetFloat($"{slider.name}", slider.value);
    }

    public void SetVoiceSlider(float f)
    {
        VoiceController.onVolumeChange?.Invoke();
    }

    public void SetVoiceVolumeSlider(float f)
    {
        AudioMixer.SetFloat("VoiceVolume", Mathf.Log10(f) * 20);
    }

    public void SetSFXVolumeSlider(float f)
    {
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(f) * 20);
    }

    public void SetMusicVolumeSlider(float f)
    {
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(f) * 20);
    }

    public void SetGlobalVolumeSlider(float f)
    {
        AudioMixer.SetFloat("GlobalVolume", Mathf.Log10(f) * 20);
    }
}
