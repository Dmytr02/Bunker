using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static DateTime NextRound = DateTime.MaxValue;
    [SerializeField] Notepad notepad;
    [SerializeField] Vector3 notepadPosition;
    [SerializeField] Quaternion notepadRotation =  Quaternion.identity;
    [SerializeField] Vote vote;
    [SerializeField] Vector3 votePosition;
    [SerializeField] Quaternion voteRotation = Quaternion.identity;
    [SerializeField] TMP_Text timerText;

    public static GameManager Instance;
    
    public UnityEvent OnStartRound;
    public UnityEvent OnEndRound;
    
    private static readonly Dictionary<(string, object), int> costs = new Dictionary<(string, object), int>()
    {
        {("profession", Professions.Actor), 2},
        {("profession", Professions.Artist), 1},
        {("Healthe", Healthe.excellent), 4},
        {("Healthe", Healthe.critical), 0},
        {("phobias", Phobias.Claustrophobia), 1},
        {("hobby", Hobby.Drawing), 1}
    };
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        CommandManager.Instance.AddInstance(this);
    }

    [CommandAtribute("/TimeToEndRound", "description")]
    public void TimeToEndRound(float seconds)
    {
        NextRound = DateTime.Now.AddSeconds(seconds);
    }

    [CommandAtribute("/Start", "description")]
    public void StartGameForAll()
    {
        Debug.Log("Starting game local");
        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    public void StartGame()
    {
        Debug.Log("Starting game");
        NextRound = DateTime.Now.AddSeconds(5);
        
        PhotonNetwork.Instantiate(notepad.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * notepadPosition, PlayerMovmant.player.transform.rotation * notepadRotation, 0);
        PhotonNetwork.Instantiate(vote.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * votePosition, PlayerMovmant.player.transform.rotation * voteRotation, 0).GetComponent<Vote>().photonView.RPC("RPC_SetPlayer", RpcTarget.All, PlayerMovmant.player.photonView.ViewID);
        PlayerMovmant.player.SendStat("Name");
        //PlayerMovmant.player.SendStat("Age");

        //StartRound();
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    
    
    
    [PunRPC]
    public void StartRound()
    {
        if (PlayerMovmant.players.Count <= 2)
        {
            PlayerMovmant.player.SendAllStats();
            Invoke("CalculatePoints", 5);
        }
        else OnStartRound.Invoke();
    }
    
    void CalculatePoints()
    {
        int points = 0;
        foreach (var player in PlayerMovmant.players)
        {
            points += costs[("profession", player.stats.list["profession"])];
            points += (int)player.stats.list["Experience"] > 20 ? 4 : (int)player.stats.list["Experience"] > 15 ? 3 : (int)player.stats.list["Experience"] > 10 ? 2 : (int)player.stats.list["Experience"] > 3 ? 1 : 0;
            points += (int)player.stats.list["Age"] > 80 ? 0 : (int)player.stats.list["Age"] > 50 ? 1 : (int)player.stats.list["Age"] > 40 ? 2 : (int)player.stats.list["Age"] > 30 ? 3 : 4;
            points += costs[("Healthe", player.stats.list["Healthe"])];
            points += costs[("phobias", player.stats.list["phobias"])];
            points += costs[("hobby", player.stats.list["hobby"])];
        }
        Debug.Log(points);
    }
    
    [PunRPC]
    public void EndRound()
    {
        OnEndRound.Invoke();
    }

    [PunRPC]
    public void UpdateTimer(string message)
    {
        timerText.text = message;
    }
    void Update()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("UpdateTimer", RpcTarget.All, new object[] { (NextRound-DateTime.Now).ToString(@"mm\:ss") + " to end of voting" });
        if (NextRound < DateTime.Now)
        {
            if (Vote.votes.Count > 0)
            {
                var groups = Vote.votes
                    .GroupBy(n => n)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                var chosenPlayer = groups.First();

                if (groups.Count < 2 || chosenPlayer.Count != groups[1].Count)
                    PhotonView.Find(chosenPlayer.Value).RPC("RPC_expel", /*PhotonView.Find(chosenPlayer.Value).Owner*/ RpcTarget.All);
                Vote.votes = new List<int>();
                photonView.RPC("EndRound", RpcTarget.All);
                NextRound = DateTime.Now.AddSeconds(5);
            }
            else
            {
                NextRound = DateTime.Now.AddSeconds(60);
                photonView.RPC("StartRound", RpcTarget.All);
            }
        }
    }
}
