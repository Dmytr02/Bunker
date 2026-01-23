using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    private DateTime NextRound = DateTime.MaxValue;
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
        PhotonNetwork.Instantiate(vote.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * votePosition, PlayerMovmant.player.transform.rotation * voteRotation, 0);
        PlayerMovmant.player.SendStat("Name");
        //PlayerMovmant.player.SendStat("Age");

        //StartRound();
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    
    
    
    [PunRPC]
    public void StartRound()
    {
        OnStartRound.Invoke();
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
                    PhotonView.Find(chosenPlayer.Value).RPC("RPC_kick", PhotonView.Find(chosenPlayer.Value).Owner);
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
