using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialVote : MonoBehaviour
{
    
    public EventTrigger[] voteButtons;
    public TMP_Text[] voteTextButtons;
    public Image[] voteImageButtons;
    public Animator animator;
    public Animator animator2;
    
    public static List<int> votes = new List<int>();

    public static TutorialVote Instance;

    private int selectedIndex = -1;
    private int count = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }


    private void SetSelected(int index)
    {
        if(votes.Count >= count) return;
        foreach (var image in voteImageButtons) image.gameObject.SetActive(false);
        voteImageButtons[index].gameObject.SetActive(true);
        selectedIndex = index;
    }

    public void StartRound()
    {
        StartCoroutine(ShowPanel());
        this.count = 1;
        selectedIndex = -1;
        foreach (var image in voteImageButtons) image.gameObject.SetActive(false);
        for (int i = 0; i < voteButtons.Length; i++)
        {
            int i0 = i;
            
            voteButtons[i].triggers.Clear(); 
            EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
            onPointerDown.eventID = EventTriggerType.PointerDown;
            onPointerDown.callback.AddListener((e) => { SetSelected(i0); });
  
            voteButtons[i].triggers.Add(onPointerDown);
        }
        animator.SetBool("isShowPanel", true);
    }

    IEnumerator ShowPanel()
    {
        animator2.SetBool("isShowed", true);
        yield return new WaitForSeconds(1.5f);
        animator2.SetBool("isShowed", false);
    }
    
    public void Submit()
    {
        if(votes.Count == count) return;
        if (selectedIndex != -1)
        {
            AddVote(selectedIndex);
        }
    }
    
    public void AddVote(int player)
    {
        votes.Add(player);
    }

    public void EndVoting()
    {
        votes.Clear();
        count = 0;
        StartCoroutine(ShowPanel());
        animator.SetBool("isShowPanel", false);
    }
}
