using TMPro;
using UnityEngine;

public class TutorialCard1 : Card
{
    [SerializeField] TutorialHints hints;
    [SerializeField] TMP_Text statText;
    public bool isTirigger {get; set;}
    protected override bool OnUse(RaycastHit hit)
    {
        
        if (hit.collider == null) return false;
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.TryGetComponent(out TutorialNotepad notepad))
        {
            if (hit.collider.tag == "bookLeft")
            {
                //if (notepad.playersStats.Count - 1 > notepad.index)
                //{
                    if(isTirigger) TutorialGameManager.Instance.Trigger = true;
                    if (notepad.index == 0)
                    {
                        notepad.playerStats.list[EStats.Experience] = 3;
                        statText.text = "Experience: 3";
                        notepad.DrawAll();
                        TutorialGameManager.assistentLast = 0;
                        
                    }
                    else
                    {
                        Debug.Log(notepad.index);
                        notepad.playersStats[notepad.index-1].list[EStats.Experience] = 3;
                        notepad.DrawAll();
                        TutorialGameManager.assistentLast = 1;
                        if(notepad.index == 1) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Hey, less experience now, but the knowledge is still there.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[2]);
                        if(notepad.index == 2) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Didn’t really change much.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[3]);
                        if(notepad.index == 3) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Well… that’s fine.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[4]);
                    }
                    
                    return true;
                //}
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    if(isTirigger) TutorialGameManager.Instance.Trigger = true;
                    notepad.playersStats[notepad.index].list[EStats.Experience] = 3;
                    notepad.DrawAll();
                    TutorialGameManager.assistentLast = 1;
                    if(notepad.index+1 == 1) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Hey, less experience now, but the knowledge is still there.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[2]);
                    if(notepad.index+1 == 2) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Didn’t really change much.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[3]);
                    if(notepad.index+1 == 3) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Well… that’s fine.", notepad.playersStats[notepad.index].list[EStats.Name].ToString(), TutorialGameManager.Instance.notepad.botColors[4]);
                    return true;
                    
                }
            }
        }
        return false;
    }
}
