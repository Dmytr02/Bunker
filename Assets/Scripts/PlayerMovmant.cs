using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerMovmant : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    //[SerializeField] CharacterController characterController;
    //[SerializeField] float speed = 2.0f;
    //[SerializeField] float gravity = 9.8f;
    //[SerializeField] float jumpForce = 10;

    [SerializeField] private Massage massage;
    [SerializeField] private float sensivity = 1;
    [SerializeField] Animator playerMashAnimator;
    [FormerlySerializedAs("playerMash")] public Outline playerMashOutline;
    public Vector2 lookAngelRangeX;
    public Vector2 lookAngelRangeY;
    public PlayerStats stats;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClip;
    [SerializeField] AudioClip psihosisClip;
    public int index;
    [SerializeField] CursoreMuves cursoreMuves;
    [SerializeField] private Renderer bini;
    [SerializeField] private Material[] biniMaterials;
    [SerializeField] public Color[] colors;
    [SerializeField] private Canvas Name; 
    
    public VoiceController voiceController;
    
    public static List<PlayerMovmant> players = new List<PlayerMovmant>();
    public static PlayerMovmant player;
    
    public static UnityEvent onPlayersAdded =  new UnityEvent();
    public static UnityEvent onPlayersRemoved =  new UnityEvent();
    
    public static UnityEvent onPlayersSelected =  new UnityEvent();
    
    public static UnityEvent<PlayerMovmant> onStatOpened =  new UnityEvent<PlayerMovmant>();
    
    public UnityEvent<string> onStatChanged =  new UnityEvent<string>();
    
    
    
    float yForce = 0;

    [PunRPC]
    public void PlaySound(int index)
    {
        audioSource.PlayOneShot(audioClip[index]);
    }
    
    private void Awake()
    {
        stats = new PlayerStats(this, photonView.IsMine, photonView.ViewID);
    }

    [PunRPC]
    public void RPC_ChangeColorsEffect()
    {
        ChangeColors();
        Invoke("ChangeColors", 120);
        stats.SetStat("Healthe", Healthe.colorBlindness);
    }
    
    [PunRPC]
    public void RPC_CursoreEffect()
    {
        CursoreMuves();
        Invoke("CursoreMuves", 120);
        stats.SetStat("Healthe", Healthe.withdrawal);
    }
    
    [PunRPC]
    public void RPC_PsichosisEffect()
    {
        audioSource.PlayOneShot(psihosisClip);
        stats.SetStat("Healthe", Healthe.psychosis);
    }
    
    [Button]
    public void ChangeColors()
    {
        Volume volume = FindAnyObjectByType<Volume>();
        if (volume.profile.TryGet(out ChannelMixer channelMixer))
        {
            channelMixer.active = !channelMixer.active;
        }   
    }
    
    [Button]
    public void CursoreMuves()
    {
        cursoreMuves.enabled = !cursoreMuves.enabled;
    }

    void Start()
    {
        players.Add(this);
        if (photonView.IsMine)
        {
            GetComponent<Interactor>().enabled = true;
            player = this;
            CommandManager.Instance.AddInstance(this);
            
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            
            playerMashOutline.enabled = false;
            foreach (var renderer in playerMashAnimator.GetComponentsInChildren<Renderer>()) renderer.enabled = false;

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            
            SendStat("Name");
            StatsController.Instance.Start_();
        }
        else
        {
            GetComponent<Interactor>().enabled = false;
            enabled = false;
        }
        //transform.rotation = Quaternion.LookRotation(Vector3.zero - new Vector3(transform.position.x, 0, transform.position.z), Vector3.up);
        
        StartCoroutine(onPlayersAddedInvoke());
    }

    IEnumerator onPlayersAddedInvoke()
    {
        while (string.IsNullOrEmpty(stats.list["Name"].ToString()))
        {
            yield return null;
        }
        onPlayersAdded?.Invoke();
    }

    private void OnDestroy()
    {
        players.Remove(this);
        if (!photonView.IsMine) SampleLauncher.Instance.spawnPoints[index].gameObject.SetActive(true);
        onPlayersRemoved?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.instance.isFollowCursore && UIController.instance.CurrentState is UIGameState && photonView.IsMine)
        {
            //Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lookAngelRangeY.x, lookAngelRangeY.y, 1 - Mathf.Clamp01(Input.mousePosition.y / Screen.height)), Mathf.Lerp(lookAngelRangeX.x, lookAngelRangeX.y, Mathf.Clamp01(Input.mousePosition.x / Screen.width)), 0));
            photonView.RPC("RPC_SendRotation", RpcTarget.All, Mathf.Lerp(-1, 1, Mathf.Clamp01(Input.mousePosition.x / Screen.width)), Mathf.Lerp(-1, 1, Mathf.Clamp01(Input.mousePosition.y / Screen.height)));
        }
    }

    [PunRPC]
    public void RPC_SendRotation(float rotationy, float rotationx)
    {
        //playerMash.transform.localRotation = Quaternion.Euler(playerMash.transform.localRotation.eulerAngles.x, rotationy, playerMash.transform.localRotation.eulerAngles.z);
        playerMashAnimator.SetFloat("horyzontal", rotationy);
        playerMashAnimator.SetFloat("vertycal", rotationx);
    }

    [CommandAtribute("/kick", "take a gameObject if it`s player kick him")]
    public void kick(GameObject player)
    {
        if(player == null) return;
        if (player.TryGetComponent(out PhotonView playerView)) 
            playerView.RPC("RPC_kick", playerView.Owner);
            //PhotonNetwork.CloseConnection(playerView.Owner);
    }

    [PunRPC]
    public void RPC_kick(PhotonMessageInfo info)
    {
        PhotonNetwork.LeaveRoom();
        UIController.instance.loadingScreenAnimator.SetTrigger("loadScene"); 
    }

    [PunRPC]
    public void RPC_expel(PhotonMessageInfo info)
    {
        players.Remove(this);
        onPlayersSelected?.Invoke();
        onPlayersRemoved?.Invoke();
        
        playerMashOutline.gameObject.SetActive(false);
        foreach (var i in playerMashAnimator.GetComponentsInChildren<Renderer>())
        {
            i.enabled = false;
        }
        Name.gameObject.SetActive(false);

        
        //GetComponent<Renderer>().enabled = false;
    }
    
    /*
    [CommandAtribute("-getStats", "take a gameObject if it`s player return stats")]
    public static string getStats(GameObject go)
    {
        if (go.TryGetComponent(out PlayerMovmant player))
        {
            return player.stats.ToString();
        }
        return $"{go.name} is not a player";
    }
    */

    public bool SendStat(string stat)
    {
        Debug.Log(stat + " - TrySend");
        if(stats.isShowed[stat] || stats.isShowed.Count(pair => pair.Value) >= GameManager.roundNumber) return false;
        GameManager.Instance.StopCoroutine("ShowAssistentToShowStat");
        photonView.RPC("RPC_Stat", RpcTarget.AllBuffered, stat, stats.list[stat], true);
        stats.isShowed[stat] = true;
        
        Debug.Log(stat + " - Sended");
        return true;
    }


    [PunRPC]
    private void RPC_Stat(string stat, object value, bool isShowed = true)
    {
        stats.list[stat] = Convert.ChangeType(value, PlayerStats.statType[stat]);
        onStatChanged?.Invoke(stat);
        if(isShowed) {onStatOpened?.Invoke(this); stats.isShowed[stat] = true; }
        if(photonView.IsMine && stats.isShowed[stat]) photonView.RPC("RPC_Stat", RpcTarget.Others, stat, stats.list[stat], true);
    }
    
    public void SendAllStats()
    {
        foreach (var i in stats.list.ToDictionary(k => k.Key, v => v.Value))
        {
            photonView.RPC("RPC_Stat", RpcTarget.All, i.Key, i.Value, true);
            stats.isShowed[i.Key] = true;
            Debug.Log(i.Key + " - Sended");
        }
    }

    public void sendMassage(string msg)
    {
        massage.photonView.RPC("showMassage", RpcTarget.All, msg);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        index = (int)data[0];
        bini.material = biniMaterials[index];
    }
}


public class PlayerStats
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
    
    public static readonly Dictionary<string, Type> statType = new Dictionary<string, Type>()
    {
        { "Name", typeof(string) },
        { "Age", typeof(int) },
        { "Profession", typeof(Professions) },
        { "Experience", typeof(int) },
        { "Healthe", typeof(Healthe) },
        { "Phobias", typeof(Phobias)},
        { "Hobby", typeof(Hobby) },
        { "Personality", typeof(Personality)}
    };

    public Dictionary<string, bool> isShowed = new Dictionary<string, bool>();
    public Dictionary<Professions, string> professionDescription = new Dictionary<Professions, string>()
    {
        {Professions.Doctor, "Maintaining health and preventing critical conditions"},
        {Professions.engineer, ""}
    };

    public int playerID = -1;

    public static object RandomizeStat(string stat)
    {
        switch (stat)
        {
            case "Profession":
                return (Professions)Random.Range(0, 16);
            case "Age":
                return Random.Range(18, 100);
            case "Experience":
                return Random.Range(1, 30);
            case "Healthe":
                return (Healthe)Random.Range(1, 5);
            case "Phobias":
                return (Phobias)Random.Range(1, 7);
            case "Hobby":
                return (Hobby)Random.Range(1, 10);
            case "Personality":
                return (Personality)Random.Range(1, 14);
        }
        return null;
    }

    public bool SetStat(string stat, object value)
    {
        if(stat == "Experience" && (int)value > (int)list["Age"]-18) return false;
        if(stat == "Age" && (int)value-18 < (int)list["Experience"]) return false;
        
        list[stat] = value;
        PhotonView.Find(playerID).RPC("RPC_Stat", PhotonView.Find(playerID).Owner, stat, value, false);
        return true;
    }

    public void SetRandomStat(string stat, bool init = false)
    {
        switch (stat)
        {
            case "Profession":
                list["Profession"] =  (Professions)Random.Range(1, 16);
                break;
            case "Age":
                list["Age"] = Random.Range(18, 100);
                break;
            case "Experience":
                list["Experience"] =  Random.Range(1, Mathf.Min((int)list["Age"]-18, 30));
                break;
            case "Healthe":
                list["Healthe"] =  (Healthe)Random.Range(1, 5);
                break;
            case "Phobias":
                list["Phobias"] =  (Phobias)Random.Range(1, 7);
                break;
            case "Hobby":
                list["Hobby"] =  (Hobby)Random.Range(1, 10);
                break;
            case "Personality":
                list["Personality"] =  (Personality)Random.Range(1, 14);
                break;
        }
        if(!init) PhotonView.Find(playerID).RPC("RPC_Stat", PhotonView.Find(playerID).Owner, stat, list[stat], false);
    }
    public PlayerStats(PlayerMovmant player, bool init = false, int playerID = -1)
    {
        if (init)
        {
            list["Name"] = PlayerPrefs.GetString("name");
            SetRandomStat("Profession", true);
            SetRandomStat("Age", true);
            SetRandomStat("Experience", true);
            SetRandomStat("Healthe", true);
            SetRandomStat("Phobias", true);
            SetRandomStat("Hobby", true);
            SetRandomStat("Personality", true);
        }

        if (playerID != -1)
        {
            this.playerID = playerID;
        }

        foreach (var i in list)
        {
            isShowed.Add(i.Key, false);
        }

    }

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
        return (string.IsNullOrEmpty(Name)? "" : $"Name - {Name}\n") + (Age==-1?"": $"Age - {Age}\n") + (Profession==Professions.unknown?"": $"Profession - {Profession}\n") +
               (experience==-1?"":$"Experience - {experience} years\n") + (Healthe==Healthe.unknown?"":$"Healthe - {Healthe}\n") + (Phobia==Phobias.unknown?"":$"Phobia - {Phobia}\n") +
               (Hobby==Hobby.unknown?"":$"Hobby - {Hobby}\n")+(personality==Personality.unknown?"":$"Personality - {personality}\n");
    }
    public string ToString(HashSet<string> stats)
    {
        string Name = list["Name"].ToString();
        int Age = list["Age"] is int ? (int)list["Age"] : -1;
        Professions Profession = (Professions)list["Profession"];
        int experience = list["Experience"] is int ? (int)list["Experience"] : -1;
        Healthe Healthe = (Healthe)list["Healthe"];
        Phobias Phobia = (Phobias)list["Phobias"];
        Hobby Hobby =  (Hobby)list["Hobby"] ;
        Personality personality = (Personality)list["Personality"];
        return (stats.Contains("Name") ? (string.IsNullOrEmpty(Name)? "Name: - \n" : $"Name: {Name.StatToString()}\n"):"") + 
               (stats.Contains("Age") ? (Age==-1?"Age: - \n": $"Age: {Age.StatToString()}\n"):"") + 
               (stats.Contains("Profession") ? (Profession==Professions.unknown?"Profession: - \n": $"Profession: {Profession.StatToString()}\n"): "") +
               (stats.Contains("Experience") ? (experience==-1?"Experience: - \n":$"Experience: {experience.StatToString()} years\n"): "") + 
               (stats.Contains("Healthe") ? (Healthe==Healthe.unknown?"Health: - \n":$"Health: {Healthe.StatToString()}\n"):"") + 
               (stats.Contains("Phobias") ? (Phobia==Phobias.unknown?"Phobia: - \n":$"Phobia: {Phobia.StatToString()}\n"):"") +
               (stats.Contains("Hobby") ?Hobby==Hobby.unknown?"Hobby: - \n":$"Hobby: {Hobby.StatToString()}\n":"")+
               (stats.Contains("Personality") ? personality==Personality.unknown?"Personality: - \n":$"Personality: {personality.StatToString()}\n":"");
    }
}

public enum Professions
{
    unknown,
    Doctor,
    engineer,
    scientist,
    biologistChemist,
    psychologist,
    Farmer,
    Soldier,
    Electrician,
    RescueWorker,
    Journalist,
    Teacher,
    SocialWorker,
    Actor,
    Artist,
    Student
}

public enum Healthe
{
    unknown,
    excellent,
    average,
    poor,
    critical,
    colorBlindness,
    psychosis,
    withdrawal
}

public enum Phobias
{
    unknown,
    Claustrophobia,
    FearOfBlood,
    FearOfTheDark,
    Anxiety,
    FearOfPublicSpeaking,
    NoPhobias
}

public enum Hobby
{
    unknown,
    Fishing_Hunting,
    Drawing,
    Chemistry,
    Writing,
    Fitness,
    Music,
    Knitting,
    ComputerGames,
    NoHobbies
}

public enum Personality
{
    unknown,
    Leader,
    Logical,
    Stress_resistant,
    Communicator,
    Rational,
    Reliable,
    Adaptable,
    Observant,
    Panicker,
    Unstable,
    Egoist,
    Impulsive,
    Withdrawn
}