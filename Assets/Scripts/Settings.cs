using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] List<Slider> Sliders = new List<Slider>();
    [SerializeField] AudioMixer AudioMixer;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToggle;

    private List<Resolution> filteredResolutions;
    private void Start()
    {
        GenerateResolutionDropDown();
        LoadSettings();
        AudioMixer.SetFloat("VoiceVolume", Mathf.Log10(PlayerPrefs.GetFloat("VoiceVolume", 1)) * 20);
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20);
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1)) * 20);
        AudioMixer.SetFloat("GlobalVolume", Mathf.Log10(PlayerPrefs.GetFloat("GlobalVolume", 1)) * 20);
    }

    private void LoadSettings()
    {
        foreach (Slider slider in Sliders)
        {
            slider.value = PlayerPrefs.GetFloat($"{slider.name}", 1);
        }
        fullscreenToggle.isOn = PlayerPrefs.GetInt("isFullScreen", 0) == 1;
        Screen.fullScreen = fullscreenToggle.isOn;
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("isFullScreen", isFullscreen ? 1 : 0);
    }

    void GenerateResolutionDropDown()
    {
        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        filteredResolutions = allResolutions
            .Where(res => Mathf.Approximately((float)res.width / res.height, 16f / 9f))
            .GroupBy(res => new { res.width, res.height })
            .Select(group => group.OrderByDescending(res => 
            res.refreshRateRatio.value
            ).First())
            .ToList();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            // Проверяем, является ли это разрешение текущим
            if (currentResolutionIndex == -1 && filteredResolutions[i].width == Screen.currentResolution.width && 
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        if(currentResolutionIndex == -1) currentResolutionIndex = 0;

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(currentResolutionIndex);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
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
