using System;
using System.Collections;
using System.Collections.Generic;
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
    public Vector2 lookAngelRangeX;
    public Vector2 lookAngelRangeY;
    public PlayerStats stats;
    
    public VoiceController voiceController;
    
    [ShowStaticField] public static List<PlayerMovmant> players = new List<PlayerMovmant>();
    public static PlayerMovmant player;
    
    public static UnityEvent onPlayersAdded =  new UnityEvent();
    public static UnityEvent onPlayersRemoved =  new UnityEvent();
    
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
        if(UIController.instance.CurrentState is UIGameState)
            //Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lookAngelRangeY.x, lookAngelRangeY.y,1-Mathf.Clamp01(Input.mousePosition.y / Screen.height)) , Mathf.Lerp(lookAngelRangeX.x, lookAngelRangeX.y,Mathf.Clamp01(Input.mousePosition.x / Screen.width)), 0));
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
        { "profession", Professions.unknown },
        { "experience", -1},
        { "Age", -1 },
        { "Healthe", Healthe.unknown},
        { "phobias", Phobias.unknown},
        {"hobby", Hobby.unknown}
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
            list["profession"] =  (Professions)Random.Range(1, 16);
            list["experience"] =  Random.Range(1, 30);
            list["Age"] = Random.Range(18, 100);
            list["Healthe"] =  (Healthe)Random.Range(1, 5);
            list["phobias"] =  (Phobias)Random.Range(1, 7);
            list["hobby"] =  (Hobby)Random.Range(1, 10);
            
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
        Professions Profession = (Professions)list["profession"];
        int experience = list["experience"] is int ? (int)list["experience"] : -1;
        Healthe Healthe = (Healthe)list["Healthe"];
        Phobias Phobia = (Phobias)list["phobias"];
        Hobby Hobby = (Hobby)list["hobby"];
        return (string.IsNullOrEmpty(Name)? "" : $"Name - {Name}\n") + (Age==-1?"": $"Age - {Age}\n") + (Profession==Professions.unknown?"": $"Profession - {Profession}") +
               (experience==-1?"":$"experience - {experience} years") + (Healthe==Healthe.unknown?"":$"Healthe - {Healthe}") + (Phobia==Phobias.unknown?"":$"Phobia - {Phobia}") +
               (Hobby==Hobby.unknown?"":$"Hobby - {Hobby}");
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