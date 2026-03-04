using System.Collections;
using UnityEngine;

public class TutorialGameManager : MonoBehaviour
{
    [SerializeField] Massage[] massages = new Massage[5];
    [SerializeField] TutorialNotepad notepad;
    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(1);
        massages[0].showMassage("I’m 86… well, at least I’ve survived a lot of stuff already.");
        notepad.playersStats[0] = "Name: Bob\nAge: 86";
        yield return new WaitForSeconds(3);
        massages[1].showMassage("Claustrophobia… yeah, I see the irony.");
        notepad.playersStats[1] = "Name: Mike\nPhobias: Claustrophobia";
        yield return new WaitForSeconds(3);
        massages[2].showMassage("I’m a doctor. Just saying.");
        notepad.playersStats[2] = "Name: Chad\nProfession: Doctor";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(3);
        massages[3].showMassage("I hunt. If food runs out, don’t panic.");
        notepad.playersStats[3] = "Name: Dave\nHobby: Hunting";
        yield return new WaitForSeconds(3);
        massages[4].showMassage("I’m 25. Back still works.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25";
        
        massages[0].gameObject.SetActive(false);
        
        massages[1].showMassage("I’m a rescuer. Usually I pull people out of places like this.");
        notepad.playersStats[1] = "Name: Mike\nProfession: Rescuer\nPhobias: Claustrophobia";
        yield return new WaitForSeconds(3);
        massages[2].showMassage("Yeah, I’m a doctor… and yeah, I don’t like blood. It happens.");
        notepad.playersStats[2] = "Name: Chad\nProfession: Doctor\nPhobias: Fear Of Blood";
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(3);
        massages[3].showMassage("Average health. Still holding up.");
        notepad.playersStats[3] = "Name: Dave\nHealth: average\nHobby: Hunting";
        yield return new WaitForSeconds(3);
        massages[4].showMassage("Engineer. If something breaks, that’s my problem.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer";
        
        massages[2].gameObject.SetActive(false);
        
        massages[1].showMassage("Health’s pretty bad… but I’ve got experience.");
        notepad.playersStats[1] = "Name: Mike\nProfession: Rescuer\nHealth: Critical\nPhobias: Claustrophobia";
        yield return new WaitForSeconds(3);
        massages[3].showMassage("36. Old enough to be smart, not old enough to be useless.");
        notepad.playersStats[3] = "Name: Dave\nAge: 36\nHealth: average\nHobby: Hunting";
        yield return new WaitForSeconds(3);
        massages[4].showMassage("I’m logical. Arguing with me won’t be easy.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer\nPersonality: Logical";
        
        massages[1].gameObject.SetActive(false);
        
        massages[3].showMassage("I’m a leader. Someone has to say ‘we’re doing it this way.");
        notepad.playersStats[3] = "Name: Dave\nAge: 36\nHealth: average\nHobby: Hunting\nPersonality: Leader";
        yield return new WaitForSeconds(3);
        massages[4].showMassage("Excellent health. I can work longer than most.");
        notepad.playersStats[4] = "Name: Kevin\nAge: 25\nProfession: Engineer\nHealth: Excellent\nPersonality: Logical";
    }
}
