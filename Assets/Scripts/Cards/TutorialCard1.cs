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
                    hints._triger2 = true;
                    notepad.playersStats[notepad.index].list["Age"] = 1;
                    notepad.SetIndex(notepad.index);
                    return true;
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    hints._triger2 = true;
                    notepad.playersStats[notepad.index+1].list["Age"] = 1;
                    notepad.SetIndex(notepad.index);
                    return true;
                }
            }
        }
        return false;
    }
}
