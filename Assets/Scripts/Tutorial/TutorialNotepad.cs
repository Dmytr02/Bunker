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

    public TutorialPlayerStats playerStats;
    public List<TutorialPlayerStats> playersStats = new List<TutorialPlayerStats>(5) { new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats(), new TutorialPlayerStats() };
    
    //public PlayerStats SelectedPlayerStats => playersStats[index];


    private void Start()
    {
        playerStats =  new TutorialPlayerStats();
        playerStats.list =  new Dictionary<EStats, object>()
        {
            { EStats.Name, PlayerPrefs.GetString("name") },
            { EStats.Age, 73 },
            { EStats.Profession, Professions.Student },
            { EStats.Experience, 5},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
        playersStats[0].list = new Dictionary<EStats, object>()
        {
            { EStats.Name, "Bob" },
            { EStats.Age, -1 },
            { EStats.Profession, Professions.unknown },
            { EStats.Experience, -1},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
        playersStats[1].list = new Dictionary<EStats, object>()
        {
            { EStats.Name, "Mike" },
            { EStats.Age, -1 },
            { EStats.Profession, Professions.unknown },
            { EStats.Experience, -1},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
        playersStats[2].list = new Dictionary<EStats, object>()
        {
            { EStats.Name, "Chad" },
            { EStats.Age, -1 },
            { EStats.Profession, Professions.unknown },
            { EStats.Experience, -1},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
        playersStats[3].list = new Dictionary<EStats, object>()
        {
            { EStats.Name, "Dave" },
            { EStats.Age, -1 },
            { EStats.Profession, Professions.unknown },
            { EStats.Experience, -1},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
        playersStats[4].list = new Dictionary<EStats, object>()
        {
            { EStats.Name, "Kevin" },
            { EStats.Age, -1 },
            { EStats.Profession, Professions.unknown },
            { EStats.Experience, -1},
            { EStats.Healthe, Healthe.unknown},
            { EStats.Phobias, Phobias.unknown},
            { EStats.Hobby, Hobby.unknown },
            { EStats.Personality, Personality.unknown}
        };
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
        gameObject.SetActive(false);
        
        /*PlayerMovmant.onStatOpened.AddListener((p) =>
        {
            if (p.stats == playersStats[index]) SetIndex(index);
        });*/
    }

    public void SetIndex(int index)
    {
        this.index = (index+playersStats.Count)%playersStats.Count;
        if (this.index == 0) text.text = playerStats.ToString(); 
        else text.text = playersStats[this.index-1].ToString();
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
    public Dictionary<EStats, object> list = new Dictionary<EStats, object>()
    {
        { EStats.Name, "" },
        { EStats.Age, -1 },
        { EStats.Profession, Professions.unknown },
        { EStats.Experience, -1},
        { EStats.Healthe, Healthe.unknown},
        { EStats.Phobias, Phobias.unknown},
        { EStats.Hobby, Hobby.unknown },
        { EStats.Personality, Personality.unknown}
    };

    public Dictionary<EStats, bool> showed = new Dictionary<EStats, bool>()
    {
        { EStats.Name, true },
        { EStats.Age, false },
        { EStats.Profession, false },
        { EStats.Experience, false },
        { EStats.Healthe, false },
        { EStats.Phobias, false },
        { EStats.Hobby, false },
        { EStats.Personality, false }
    };
    
    
    public override string ToString()
    {
        string Name = list[EStats.Name].ToString();
        int Age = list[EStats.Age] is int ? (int)list[EStats.Age] : -1;
        Professions Profession = (Professions)list[EStats.Profession];
        int experience = list[EStats.Experience] is int ? (int)list[EStats.Experience] : -1;
        Healthe Healthe = (Healthe)list[EStats.Healthe];
        Phobias Phobia = (Phobias)list[EStats.Phobias];
        Hobby Hobby = (Hobby)list[EStats.Hobby];
        Personality personality = (Personality)list[EStats.Personality];
        return (string.IsNullOrEmpty(Name)||!showed[EStats.Name]? "" : $"Name - {Name}\n\n") + 
               (Age==-1||!showed[EStats.Age]?"": $"Age - {Age}\n") + 
               (Profession==Professions.unknown|| !showed[EStats.Profession] ?"": $"Profession - {Profession}\n") +
               (experience==-1 || !showed[EStats.Experience]?"":$"Experience - {experience} years\n") + 
               (Healthe==Healthe.unknown|| !showed[EStats.Healthe]?"":$"Healthe - {Healthe}\n") + 
               (Phobia==Phobias.unknown|| !showed[EStats.Phobias]?"":$"Phobia - {Phobia}\n") +
               (Hobby==Hobby.unknown|| !showed[EStats.Hobby]?"":$"Hobby - {Hobby}\n")+
               (personality==Personality.unknown|| !showed[EStats.Personality]?"":$"Personality - {personality}\n");
    }
}

public enum EStats
{
    Name,
    Age,
    Profession,
    Experience,
    Healthe,
    Phobias,
    Hobby,
    Personality
}
