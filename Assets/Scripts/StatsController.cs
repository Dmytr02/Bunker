using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatsController : MonoBehaviour
{
    [SerializeField] private EventTrigger statPrefab;
    [SerializeField] private Color showedColor;
    
    public static StatsController Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start_()
    {
        foreach (var i in PlayerMovmant.player.stats.list)
        {
            EventTrigger trigger = Instantiate(statPrefab, transform);
            TMP_Text text = trigger.GetComponent<TMP_Text>();
            text.text = $"{i.Key}: {i.Value}";
            
            EventTrigger.Entry e = new EventTrigger.Entry();
            e.eventID = EventTriggerType.PointerClick;
            e.callback.AddListener((data) => { if(!PlayerMovmant.player.stats.isShowed[i.Key]) PlayerMovmant.player.SendStat(i.Key); text.color = showedColor; text.fontStyle =  FontStyles.Underline; });
            
            trigger.triggers.Add(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
