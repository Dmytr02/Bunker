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
    
    public static List<int> votes = new List<int>();

    public static Vote Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        GameManager.Instance.OnStartRound.AddListener(StartRound);
    }

    public void StartRound()
    {
        Debug.Log("Starting round2");
        if (!PlayerMovmant.players.Contains(PlayerMovmant.player))
        {
            Debug.Log(PlayerMovmant.players.Count + " players");
            return;
        }
        Debug.Log("Starting round3");
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (PlayerMovmant.players.Count > i)
            {
                voteButtons[i].gameObject.SetActive(true);
                voteTextButtons[i].text = PlayerMovmant.players[i].stats.list["Name"].ToString();
                int i0 = i;
                
                voteButtons[i].triggers.Clear(); 
                EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerDown.callback.AddListener((e) => { photonView.RPC("AddVote", RpcTarget.All, PlayerMovmant.players[i0].photonView.ViewID); animator.SetBool("isShowPanel", false); Debug.Log("vote");});
      
                voteButtons[i].triggers.Add(onPointerDown);
            }
            else
            {   
                voteButtons[i].gameObject.SetActive(false);
            }
        }
        animator.SetBool("isShowPanel", true);
        Debug.Log("Starting round4");
    }
    
    [PunRPC]
    public void AddVote(int player)
    {
        votes.Add(player);
        Debug.Log(votes.Count + " | " + PlayerMovmant.players.Count);
        if(votes.Count == PlayerMovmant.players.Count) GameManager.NextRound = DateTime.Now.AddSeconds(1);
    }
}
