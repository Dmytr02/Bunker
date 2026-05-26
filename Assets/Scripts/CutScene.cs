using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] float decayTime = 1;
    private void Start()
    {
        audioSource.clip = audioClip;
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
            yield return null;
            time -= Time.deltaTime;
        }

        audioSource.volume = 0;
    }
}
