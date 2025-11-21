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
    [SerializeField] Animator animator;
    
    public static List<int> votes = new List<int>();


    private void Awake()
    {
        GameManager.Instance.OnStartRound.AddListener(StartRound);
    }

    public void StartRound()
    {
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (PlayerMovmant.players.Count > i)
            {
                voteButtons[i].gameObject.SetActive(true);
                voteTextButtons[i].text = PlayerMovmant.players[i].stats.list["Name"].ToString();
                int i0 = i;
                voteButtons[i].OnInteract.AddListener(() => { photonView.RPC("AddVote", RpcTarget.All, PlayerMovmant.players[i0].photonView.ViewID); animator.SetBool("isShowPanel", false); });
            }
            else
            {   
                voteButtons[i].gameObject.SetActive(false);
            }
        }
        votes = new List<int>();
        animator.SetBool("isShowPanel", true);
    }
    
    [PunRPC]
    public void AddVote(int player)
    {
        votes.Add(player);
    }
}
