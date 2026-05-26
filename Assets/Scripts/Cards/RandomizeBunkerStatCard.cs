using TMPro;
using UnityEngine;

public class RandomizeBunkerStatCard : Card
{

    [SerializeField] TMP_Text cardName;
    public string Stat = "";
    private static readonly string[] ststsList = {"Size", "TimeInside", "Supplies"};
    
    protected override void Start()
    {
        base.Start();
        Stat = ststsList[Random.Range(0, ststsList.Length)];
        cardName.text = $"Randomize {Stat}, for bunker";
    }
    
    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider.transform.parent == null) return false;
        //if (hit.collider.transform.parent.parent == null) return false;
        //if (hit.collider.transform.parent.parent.parent == null) return false;
        if (!hit.collider.transform.parent.TryGetComponent(out Notepad notepad)) return false;
        if(BunkerStats.Instance.SetStat(Stat, BunkerStats.GetRandomStat(Stat))) return true;
        return false;
    }

}
