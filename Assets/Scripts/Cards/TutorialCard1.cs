using UnityEngine;

public class TutorialCard1 : Card
{
    [SerializeField] TutorialHints hints;
    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent.TryGetComponent(out TutorialNotepad notepad))
        {
            if (hit.collider.tag == "bookLeft")
            {
                if (notepad.playersStats.Count - 1 > notepad.index)
                {
                    TutorialGameManager.Instance.Trigger = true;
                    if (notepad.index == 0)
                    {
                        notepad.playerStats.list[EStats.Experience] = 3;
                        notepad.SetIndex(notepad.index);
                        TutorialGameManager.assistentLast = 0;
                        
                    }
                    else
                    {
                        notepad.playersStats[notepad.index + 1].list[EStats.Experience] = 3;
                        notepad.SetIndex(notepad.index);
                        TutorialGameManager.assistentLast = 1;
                        if(notepad.index == 1) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Hey, less experience now, but the knowledge is still there.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                        if(notepad.index == 2) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Didn’t really change much.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                        if(notepad.index == 3) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Well… that’s fine.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                    }
                    
                    return true;
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    TutorialGameManager.Instance.Trigger = true;
                    notepad.playersStats[notepad.index+2].list[EStats.Experience] = 3;
                    notepad.SetIndex(notepad.index);
                    TutorialGameManager.assistentLast = 1;
                    if(notepad.index+1 == 1) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Hey, less experience now, but the knowledge is still there.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                    if(notepad.index+1 == 2) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Didn’t really change much.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                    if(notepad.index+1 == 3) TutorialGameManager.Instance.tutorialCommandManager.SendMassage("Well… that’s fine.", notepad.playersStats[notepad.index].list[EStats.Name].ToString());
                    return true;
                    
                }
            }
        }
        return false;
    }
}
