using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Vote : MonoBehaviourPunCallbacks
{
    
    [SerializeField] EventTrigger[] voteButtons;
    [SerializeField] TMP_Text[] voteTextButtons;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovmant playerMovmant;
    
    public static List<int> votes = new List<int>();

    [PunRPC]
    public void RPC_SetPlayer(int player)
    {
        playerMovmant = PhotonView.Find(player).GetComponent<PlayerMovmant>();
    }
    
    private void Awake()
    {
        GameManager.Instance.OnStartRound.AddListener(StartRound);
        playerMovmant = PlayerMovmant.player;
    }

    public void StartRound()
    {
        if(!PlayerMovmant.players.Contains(playerMovmant)) return;
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (PlayerMovmant.players.Count > i)
            {
                voteButtons[i].gameObject.SetActive(true);
                voteTextButtons[i].text = PlayerMovmant.players[i].stats.list["Name"].ToString();
                int i0 = i;
                
                EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerDown.callback.AddListener((e) => { photonView.RPC("AddVote", RpcTarget.All, PlayerMovmant.players[i0].photonView.ViewID); animator.SetBool("isShowPanel", false); });
      
                voteButtons[i].triggers.Add(onPointerDown);
            }
            else
            {   
                voteButtons[i].gameObject.SetActive(false);
            }
        }
        animator.SetBool("isShowPanel", true);
    }
    
    [PunRPC]
    public void AddVote(int player)
    {
        animator.SetBool("isShowPanel", false);
        votes.Add(player);
        Debug.Log(votes.Count + " | " + PlayerMovmant.players.Count);
        if(votes.Count == PlayerMovmant.players.Count) GameManager.NextRound = DateTime.Now.AddSeconds(1);
    }
}
