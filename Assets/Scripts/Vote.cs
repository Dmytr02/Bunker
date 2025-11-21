using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Vote : MonoBehaviourPunCallbacks
{
    
    [SerializeField] Button[] voteButtons;
    [SerializeField] TMP_Text[] voteTextButtons;
    
    public static List<int> votes = new List<int>();

    private void Start()
    {
        GameManager.Instance.OnStartRound.AddListener(StartRound);
        StartRound();
    }

    public void StartRound()
    {
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (PlayerMovmant.players.Count > i)
            {
                voteButtons[i].gameObject.SetActive(true);
                voteTextButtons[i].text = PlayerMovmant.players[i].stats.stats["Name"].ToString();
                voteButtons[i].OnInteract.AddListener(() => { photonView.RPC("AddVoteRound", RpcTarget.All, PlayerMovmant.players[i].photonView.ViewID); gameObject.SetActive(false); });
            }
            else
            {   
                voteButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    [PunRPC]
    public void AddVote(int player)
    {
        votes.Add(player);
    }
}
