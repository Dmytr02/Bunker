using TMPro;
using UnityEngine;

public class BunkerStatCard : Card
{
    [SerializeField] TMP_Text cardName;
    public string Stat = "";
    public object value = null;
    private static readonly string[] ststsList = {"Size", "TimeInside", "Supplies"};
    
    protected override void Start()
    {
        base.Start();
        Stat = ststsList[Random.Range(0, ststsList.Length)];
        value = BunkerStats.GetRandomStat(Stat);
        cardName.text = $"Set {Stat}, for bunker, to {value}";
    }
    
    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider.transform.parent == null) return false;
        //if (hit.collider.transform.parent.parent == null) return false;
        //if (hit.collider.transform.parent.parent.parent == null) return false;
        if (!hit.collider.transform.parent.TryGetComponent(out Notepad notepad)) return false;
        if(BunkerStats.Instance.SetStat(Stat, value)) return true;
        return false;
    }
}
