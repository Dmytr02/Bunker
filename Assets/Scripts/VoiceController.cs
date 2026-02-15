using System;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour
{
    [SerializeField] Button muteButton;
    [SerializeField] Recorder recorder;
    [SerializeField] Image microphoneImg;
    [SerializeField] Sprite microphoneOn;
    [SerializeField] Sprite microphoneOff;
    public AudioSource audioSource;
    public static UnityEvent onVolumeChange = new();
    private void Start()
    {
        setVolume();
        onVolumeChange.AddListener(setVolume);
        microphoneImg = GameObject.Find("MicroImg").GetComponent<Image>();
        muteButton.OnInteract.AddListener((() =>
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled; 
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }));
    }

    private void OnDestroy()
    {
        onVolumeChange.RemoveListener(setVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }
    }

    void setVolume()
    {
        audioSource.volume = PlayerPrefs.GetFloat($"VoiceVolume{GetComponent<PlayerMovmant>().index + (PlayerMovmant.player.index<=GetComponent<PlayerMovmant>().index?0:-1)}", 1f);
    }
}
