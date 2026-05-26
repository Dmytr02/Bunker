using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Vote : MonoBehaviourPunCallbacks
{
    
    [SerializeField] EventTrigger[] voteButtons;
    [SerializeField] TMP_Text[] voteTextButtons;
    public Image[] voteImageButtons;
    [SerializeField] Animator animator;
    
    public static List<int> votes = new List<int>();

    public static Vote Instance;
    
    private int selectedIndex = -1;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        GameManager.Instance.OnStartRound.AddListener(StartRound);
    }
    
    private void SetSelected(int index)
    {
        //if(votes.Count >= count) return;
        foreach (var image in voteImageButtons) image.gameObject.SetActive(false);
        voteImageButtons[index].gameObject.SetActive(true);
        selectedIndex = index;
    }

    public void StartRound()
    {
        if (!PlayerMovmant.players.Contains(PlayerMovmant.player))
        {
            return;
        }
        
        selectedIndex = -1;
        foreach (var image in voteImageButtons) image.gameObject.SetActive(false);
        for (int i = 0; i < voteButtons.Length; i++) voteButtons[i].gameObject.SetActive(false);
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (PlayerMovmant.players.Count > i)
            {
                voteButtons[PlayerMovmant.players[i].index].gameObject.SetActive(true);
                voteTextButtons[PlayerMovmant.players[i].index].text = PlayerMovmant.players[i].stats.list["Name"].ToString();
                int i0 = i;
                
                voteButtons[PlayerMovmant.players[i].index].triggers.Clear(); 
                EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerDown.callback.AddListener((e) => { SetSelected(i0); });
      
                voteButtons[PlayerMovmant.players[i].index].triggers.Add(onPointerDown);
            }
        }
        animator.SetBool("isShowPanel", true);
    }

    public void Submit()
    {
        //bbif(votes.Count == count) return;
        if (selectedIndex != -1)
        {
            photonView.RPC("AddVote", RpcTarget.All, PlayerMovmant.players[selectedIndex].photonView.ViewID); animator.SetBool("isShowPanel", false); Debug.Log("vote");
        }
    }
    
    [PunRPC]
    public void AddVote(int player)
    {
        votes.Add(player);
        if(votes.Count == PlayerMovmant.players.Count) GameManager.NextRound = DateTime.Now.AddSeconds(1);
    }
}
