using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerMovmant : MonoBehaviourPunCallbacks
{
    //[SerializeField] CharacterController characterController;
    //[SerializeField] float speed = 2.0f;
    //[SerializeField] float gravity = 9.8f;
    //[SerializeField] float jumpForce = 10;

    [SerializeField] private Massage massage;
    [SerializeField] private float sensivity = 1;
    [SerializeField] private Outline playerMash;
    public Vector2 lookAngelRangeX;
    public Vector2 lookAngelRangeY;
    public PlayerStats stats;
    
    public VoiceController voiceController;
    
    [ShowStaticField] public static List<PlayerMovmant> players = new List<PlayerMovmant>();
    public static PlayerMovmant player;
    
    public static UnityEvent onPlayersAdded =  new UnityEvent();
    public static UnityEvent onPlayersRemoved =  new UnityEvent();
    
    public static UnityEvent<PlayerMovmant> onStatOpened =  new UnityEvent<PlayerMovmant>();
    
    
    float yForce = 0;
    void Start()
    {
        stats = new PlayerStats(this, photonView.IsMine);
        players.Add(this);
        if (photonView.IsMine)
        {
            GetComponent<Interactor>().enabled = true;
            player = this;
            CommandManager.Instance.AddInstance(this);
            
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            
            playerMash.enabled = false;
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
        onPlayersRemoved?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.instance.CurrentState is UIGameState)
        {
            //Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lookAngelRangeY.x, lookAngelRangeY.y, 1 - Mathf.Clamp01(Input.mousePosition.y / Screen.height)), Mathf.Lerp(lookAngelRangeX.x, lookAngelRangeX.y, Mathf.Clamp01(Input.mousePosition.x / Screen.width)), 0));
            photonView.RPC("RPC_SendRotation", RpcTarget.All, Mathf.Lerp(lookAngelRangeX.x, lookAngelRangeX.y, Mathf.Clamp01(Input.mousePosition.x / Screen.width)));
        }
    }

    [PunRPC]
    public void RPC_SendRotation(float rotationy)
    {
        playerMash.transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, rotationy, transform.localRotation.eulerAngles.z);
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
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    public void RPC_expel(PhotonMessageInfo info)
    {
        players.Remove(this);
        onPlayersRemoved?.Invoke();
        
        GetComponent<Renderer>().enabled = false;
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

    public void SendStat(string stat)
    {
        photonView.RPC("RPC_Stat", RpcTarget.All, stat, stats.list[stat]);
        stats.isShowed[stat] = true;
        Debug.Log(stat + " - Sended");
    }

    [PunRPC]
    private void RPC_Stat(string stat, object value)
    {
        stats.list[stat] = value;
        onStatOpened?.Invoke(this);
    }
    
    public void SendAllStats()
    {
            foreach (var i in stats.list.ToDictionary(k => k.Key, v => v.Value))
            {
                photonView.RPC("RPC_Stat", RpcTarget.All, i.Key, i.Value);
                stats.isShowed[i.Key] = true;
                Debug.Log(i.Key + " - Sended");
            }
    }

    public void sendMassage(string msg)
    {
        massage.photonView.RPC("showMassage", RpcTarget.All, msg);
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

    public Dictionary<string, bool> isShowed = new Dictionary<string, bool>();
    public Dictionary<Professions, string> professionDescription = new Dictionary<Professions, string>()
    {
        {Professions.Doctor, "Maintaining health and preventing critical conditions"},
        {Professions.enginee, ""}
    };
    
    public PlayerStats(PlayerMovmant player, bool init = false)
    {
        if (init)
        {
            list["Name"] = PlayerPrefs.GetString("name");
            list["Profession"] =  (Professions)Random.Range(1, 16);
            list["Experience"] =  Random.Range(1, 30);
            list["Age"] = Random.Range(18, 100);
            list["Healthe"] =  (Healthe)Random.Range(1, 5);
            list["Phobias"] =  (Phobias)Random.Range(1, 7);
            list["Hobby"] =  (Hobby)Random.Range(1, 10);
            list["Personality"] =  (Personality)Random.Range(1, 14);
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
}


public enum Professions
{
    unknown,
    Doctor,
    enginee,
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
    critical
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