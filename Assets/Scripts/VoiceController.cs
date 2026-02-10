using System;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour
{
    [SerializeField] Button muteButton;
    [SerializeField] Recorder recorder;
    [SerializeField] Image microphoneImg;
    [SerializeField] Sprite microphoneOn;
    [SerializeField] Sprite microphoneOff;
    public AudioSource audioSource;
    private void Start()
    {
        microphoneImg = GameObject.Find("MicroImg").GetComponent<Image>();
        muteButton.OnInteract.AddListener((() =>
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled; 
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }
    }
}
