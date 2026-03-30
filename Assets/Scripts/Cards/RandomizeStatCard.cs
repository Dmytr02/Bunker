using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeStatCard : Card
{
    [SerializeField] TMP_Text cardName;
    public string Stat = "";
    private static readonly string[] ststsList = {"Profession", "Age", "Experience", "Healthe", "Phobias", "Hobby", "Personality"};
    protected override void Start()
    {
        base.Start();
        Stat = ststsList[Random.Range(0, ststsList.Length)];
        cardName.text = $"Randomize {Stat}, for selected player";
    }

    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent.TryGetComponent(out Notepad notepad))
        {
            if (hit.collider.tag == "bookLeft")
            {
                if (notepad.playersStats.Count - 1 > notepad.index)
                {
                    notepad.playersStats[notepad.index].SetRandomStat(Stat);
                    notepad.SetIndex(notepad.index);
                    return true;
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    notepad.playersStats[notepad.index+1].SetRandomStat(Stat);
                    notepad.SetIndex(notepad.index);
                    return true;
                }
            }
        }
        return false;
    }
}
