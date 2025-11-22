using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-5)]
public class PlayerMovmant : MonoBehaviourPunCallbacks
{
    //[SerializeField] CharacterController characterController;
    //[SerializeField] float speed = 2.0f;
    //[SerializeField] float gravity = 9.8f;
    //[SerializeField] float jumpForce = 10;

    [SerializeField] private Massage massage;
    [SerializeField] private float sensivity = 1;
    public PlayerStats stats;
    
    [ShowStaticField] public static List<PlayerMovmant> players = new List<PlayerMovmant>();
    public static PlayerMovmant player;
    
    float yForce = 0;
    void Start()
    {
        stats = new PlayerStats(this, photonView.IsMine);
        players.Add(this);
        if (photonView.IsMine)
        {
            player = this;
            CommandManager.Instance.AddInstance(this);
            
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else enabled = false;
    }

    private void OnDestroy()
    {
        players.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.instance.CurrentState is UIGameState)
            Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
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
        { "Age", -1 },
        { "Name", "" },
        { "Score", -1 }
    };

    public PlayerStats(PlayerMovmant player, bool init = false)
    {
        if (init)
        {
            list["Age"] = Random.Range(1, 100);
            list["Name"] = PlayerPrefs.GetString("name");
            list["Score"] = Random.Range(1, 10);
        }
    }

    public override string ToString()
    {
        string Name = list["Name"].ToString();
        int Age = list["Age"] is int ? (int)list["Age"] : -1;
        int Score = list["Score"] is int ? (int)list["Score"] : -1;
        return (string.IsNullOrEmpty(Name)? "" : $"Name - {Name}\n") +  (Age==-1?"": $"Age - {Age}\n") + (Score==-1?"": $"Score - {Score}");
    }
}