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

    
    public List<TutorialPlayerStats> playersStats = new List<TutorialPlayerStats>(5) { new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats() };
    
    //public PlayerStats SelectedPlayerStats => playersStats[index];


    private void Start()
    {
        playersStats[0].list["Name"] = "Bob";
        playersStats[1].list["Name"] = "Mike";
        playersStats[2].list["Name"] = "Chad";
        playersStats[3].list["Name"] = "Dave";
        playersStats[4].list["Name"] = "Kevin";
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

public class TutorialPlayerStats
{
    public Dictionary<string, object> list = new Dictionary<string, object>()
    {
        { "Name", "" },
        { "Age", -1 },
        { "Profession", Professions.unknown },
        { "Experience", -1},
        { "Healthe", Healthe.unknown},
        { "Phobias", Phobias.unknown},
        { "Hobby", Hobby.unknown },
        { "Personality", Personality.unknown}
    };
    
    public override string ToString()
    {
        string Name = list["Name"].ToString();
        int Age = list["Age"] is int ? (int)list["Age"] : -1;
        Professions Profession = (Professions)list["Profession"];
        int experience = list["Experience"] is int ? (int)list["Experience"] : -1;
        Healthe Healthe = (Healthe)list["Healthe"];
        Phobias Phobia = (Phobias)list["Phobias"];
        Hobby Hobby = (Hobby)list["Hobby"];
        Personality personality = (Personality)list["Personality"];
        return (string.IsNullOrEmpty(Name)? "" : $"Name - {Name}\n\n") + (Age==-1?"": $"Age - {Age}\n") + (Profession==Professions.unknown?"": $"Profession - {Profession}\n") +
               (experience==-1?"":$"Experience - {experience} years\n") + (Healthe==Healthe.unknown?"":$"Healthe - {Healthe}\n") + (Phobia==Phobias.unknown?"":$"Phobia - {Phobia}\n") +
               (Hobby==Hobby.unknown?"":$"Hobby - {Hobby}\n")+(personality==Personality.unknown?"":$"Personality - {personality}\n");
    }
}
