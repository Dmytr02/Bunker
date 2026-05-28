using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatCard : Card
{
    [SerializeField] TMP_Text cardName;
    public string Stat = "";
    public object value = null;
    private static readonly string[] ststsList = {"Profession", "Age", "Experience", "Health", "Phobias", "Hobby", "Personality"};
    protected override void Start()
    {
        base.Start();
        Stat = ststsList[Random.Range(0, ststsList.Length)];
        value = PlayerStats.RandomizeStat(Stat);
        cardName.text = $"Set {Stat}, for selected player, to {value.StatToString()}";
    }

    protected override bool OnUse(RaycastHit hit)
    {
        if(hit.collider.tag != "bookLeft" && hit.collider.tag != "bookRight" ) return false;
        if (hit.collider.transform.parent == null) return false;
        //if (hit.collider.transform.parent.parent == null) return false;
        //if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.TryGetComponent(out Notepad notepad))
        {
            PlayerMovmant player = PlayerMovmant.players.FirstOrDefault(p => p.index == notepad.index + (hit.collider.CompareTag("bookLeft") ? 0 : 1));
            if (player != null)
            {
                if (player.stats.SetStat(Stat, value))
                {
                    return true;
                }
                notepad.DrawPlayer(player);
            }
        }

        /*if (hit.collider.transform.parent.parent.parent.TryGetComponent(out Notepad notepad))
        {
            if (hit.collider.tag == "bookLeft")
            {
                if (notepad.playersStats.Count - 1 > notepad.index)
                {
                    if (notepad.playersStats[notepad.index].SetStat(Stat, value))
                    {
                        return true;
                    }
                    notepad.SetIndex(notepad.index);
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    if(notepad.playersStats[notepad.index+1].SetStat(Stat, value))
                    {
                        return true;
                    }
                    notepad.SetIndex(notepad.index);
                }
            }
        }*/
        return false;
    }
}

