using System;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    [SerializeField] Button muteButton;
    [SerializeField] Recorder recorder;
    private void Start()
    {
        muteButton.OnInteract.AddListener((() => recorder.TransmitEnabled = !recorder.TransmitEnabled));
    }
}
