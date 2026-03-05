using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialNotepad : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] public TMP_Text text;
    [SerializeField] public ManualCameraRender cameraRender;
    [SerializeField] public TMP_Text text2;
    [SerializeField] public ManualCameraRender cameraRender2;
    [SerializeField] public int index = 0;
    [SerializeField] public Animator animator;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip audioClip;

    
    public List<string> playersStats = new List<string>(6){"Name: Bob", "Name: Mike", "Name: Chad", "Name: Dave", "Name: Kevin", "6"};
    
    //public PlayerStats SelectedPlayerStats => playersStats[index];


    private void Start()
    {
        cameraRender = GameObject.Find("LeftSideRender").GetComponent<ManualCameraRender>();
        text = cameraRender.GetComponentInChildren<TMP_Text>();
        cameraRender2 = GameObject.Find("RightSideRender").GetComponent<ManualCameraRender>();
        text2 = cameraRender2.GetComponentInChildren<TMP_Text>();
        /*foreach (var i in PlayerMovmant.players)
        {
            playersStats.Add(i.stats);
            Debug.Log(i.stats);
        }*/
        SetIndex(0);
        
        /*PlayerMovmant.onStatOpened.AddListener((p) =>
        {
            if (p.stats == playersStats[index]) SetIndex(index);
        });*/
    }

    public void SetIndex(int index)
    {
        this.index = (index+playersStats.Count)%playersStats.Count;
        text.text = playersStats[this.index].ToString();
        cameraRender.RenderCameraNow();

        if (playersStats.Count > this.index+1)
        {
            text2.text = playersStats[this.index + 1].ToString();
            cameraRender2.RenderCameraNow();
        }
        else
        {
            text2.text = "";
            cameraRender2.RenderCameraNow();
        }
    }

    public void NextIndex()
    {
        SetIndex(index + 2);
        animator.SetTrigger("next");
        audioSource.PlayOneShot(audioClip);
    }

    public void PreviousIndex()
    {
        SetIndex(index - 2);
        animator.SetTrigger("previus");
        audioSource.PlayOneShot(audioClip);
    }

    private object[] data;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        data = info.photonView.InstantiationData;
        PlayerMovmant.onPlayersSelected.AddListener(tryDestroy);
    }

    private void OnDestroy()
    {
        PlayerMovmant.onPlayersSelected.RemoveListener(tryDestroy);
    }

    void tryDestroy()
    {
        if(!PlayerMovmant.players.Contains(PhotonView.Find((int)data[0]).GetComponent<PlayerMovmant>()))
        {
            Destroy(gameObject);
        }
    }
    
}
