using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
    [SerializeField] private EventTrigger statPrefab;
    //[SerializeField] private Color showedColor;
    [SerializeField] private Sprite showedSprite;
    
    public static StatsController Instance;

    Dictionary<string, (EventTrigger trigger, TMP_Text text)> stats = new ();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start_()
    {
        foreach (var i in PlayerMovmant.player.stats.list)
        {
            if(i.Key == "Name") continue;
            EventTrigger trigger = Instantiate(statPrefab, transform);
            TMP_Text text = trigger.GetComponent<TMP_Text>();
            stats.Add(i.Key, (trigger, text));
            text.text = $"{i.Key}: {i.Value.StatToString()}";
            
            EventTrigger.Entry e = new EventTrigger.Entry();
            e.eventID = EventTriggerType.PointerClick;
            e.callback.AddListener((data) => {
                if(!PlayerMovmant.player.stats.isShowed[i.Key]) if(PlayerMovmant.player.SendStat(i.Key)) text.GetComponentInChildren<Image>().sprite = showedSprite; 
            });
            
            trigger.triggers.Add(e);
        }
        
        PlayerMovmant.player.onStatChanged.AddListener((arg0 =>
        {
            if(arg0 == "Name") return;
            stats[arg0].text.text = $"{arg0}: {PlayerMovmant.player.stats.list[arg0].StatToString()}";
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
