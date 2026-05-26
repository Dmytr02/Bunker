using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] float decayTime = 1;
    
    [SerializeField] AudioSource musicAudioSource;
    private void Start()
    {
        audioSource.clip = audioClip;
        Invoke("PlayAudio", 2);
        StartCoroutine(StartSound());
    }
    
    IEnumerator StartSound()
    {
        float time = decayTime;
        while (time > 0)
        {
            musicAudioSource.volume = time / decayTime;
            yield return null;
            time -= Time.deltaTime;
        }

        musicAudioSource.volume = 0;
    }

    void PlayAudio()
    {
        audioSource.Play();
    }
    
    public void Trigger()
    {
        StartCoroutine(StopSound());
        if(TutorialGameManager.Instance) TutorialGameManager.Instance.Trigger = true;
        
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("EndTutorial", true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    IEnumerator StopSound()
    {
        float time = decayTime;
        while (time > 0)
        {
            audioSource.volume = time / decayTime;
            musicAudioSource.volume = 1 - time / decayTime;
            yield return null;
            time -= Time.deltaTime;
        }

        audioSource.volume = 0;
        musicAudioSource.volume = 1;
    }
}
