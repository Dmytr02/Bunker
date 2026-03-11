using System;
using System.Collections;
using UnityEngine;

public class TutorialGameManager : MonoBehaviour
{
    [SerializeField] Massage[] massages = new Massage[5];
    [SerializeField] TutorialNotepad notepad;
    [SerializeField] private GameObject EndPanel;
    
    private int openedCount = 0;
    private bool opened = false;

    public void OpenStat()
    {
        opened = true;
        openedCount++;
    }
    void Start()
    {
        StartCoroutine(GameLoop());
    }


    private void Update()
    {
        if(Input.GetKey(KeyCode.P)) Time.timeScale = 10;
        else Time.timeScale = 1;
    }


    IEnumerator GameLoop()
    {
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[0].showMassage("I’m 86… well, at least I’ve survived a lot of stuff already.");
        notepad.playersStats[0] = "Name: Bob\nAge: 86";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.5f);
        massages[1].showMassage("Claustrophobia… yeah, I see the irony.");
        notepad.playersStats[1] = "Name: Mike\nPhobias: Claustrophobia";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.3f);
        massages[2].showMassage("I’m a doctor. Just saying.");
        notepad.playersStats[2] = "Name: Chad\nProfession: Doctor";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4.1f);
        massages[3].showMassage("I hunt. If food runs out, don’t panic.");
        notepad.playersStats[3] = "Name: Dave\nHobby: Hunting";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.3f);
        massages[4].showMassage("I’m 25. Back still works.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[1].text = $"Bob\nvotes: {5 + (TutorialVote.votes.Contains(1) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {(TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad\nvotes: {(TutorialVote.votes.Contains(3) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";

        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[1].gameObject.SetActive(false);
        massages[0].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[1].showMassage("I’m a rescuer. Usually I pull people out of places like this.");
        notepad.playersStats[1] = "Name: Mike\nProfession: Rescuer\nPhobias: Claustrophobia";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.6f);
        massages[2].showMassage("Yeah, I’m a doctor… and yeah, I don’t like blood. It happens.");
        notepad.playersStats[2] = "Name: Chad\nProfession: Doctor\nPhobias: Fear Of Blood";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.6f);
        massages[3].showMassage("Average health. Still holding up.");
        notepad.playersStats[3] = "Name: Dave\nHealth: average\nHobby: Hunting";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4.8f);
        massages[4].showMassage("Engineer. If something breaks, that’s my problem.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6.4f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {(TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad\nvotes: {4 + (TutorialVote.votes.Contains(3) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[3].gameObject.SetActive(false);
        massages[2].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[1].showMassage("Health’s pretty bad… but I’ve got experience.");
        notepad.playersStats[1] = "Name: Mike\nProfession: Rescuer\nHealth: Critical\nPhobias: Claustrophobia";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6);
        massages[3].showMassage("36. Old enough to be smart, not old enough to be useless.");
        notepad.playersStats[3] = "Name: Dave\nAge: 36\nHealth: average\nHobby: Hunting";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.2f);
        massages[4].showMassage("I’m logical. Arguing with me won’t be easy.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer\nPersonality: Logical";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.8f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {3 + (TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[2].gameObject.SetActive(false);
        massages[1].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[3].showMassage("I’m a leader. Someone has to say ‘we’re doing it this way.");
        notepad.playersStats[3] = "Name: Dave\nAge: 36\nHealth: average\nHobby: Hunting\nPersonality: Leader";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.3f);
        massages[4].showMassage("Excellent health. I can work longer than most.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer\nHealth: Excellent\nPersonality: Logical";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6.1f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {2 + (TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[1].gameObject.SetActive(false);
        //massages[1].gameObject.SetActive(false);
        //игрока выгнали, конец
        EndPanel.SetActive(true);
        
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
    }
}
