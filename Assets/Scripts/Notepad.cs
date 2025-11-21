using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Notepad : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] int index = 0;

    
    private List<PlayerStats> playersStats = new List<PlayerStats>();


    private void Start()
    {
        foreach (var i in PlayerMovmant.players)
        {
            playersStats.Add(i.stats);
            Debug.Log(i.stats);
        }
        Debug.Log(playersStats.Count);
        SetIndex(0);
    }

    public void SetIndex(int index)
    {
        Debug.Log("Interact2");
        this.index = (index+playersStats.Count)%playersStats.Count;
        text.text = playersStats[this.index].ToString();
    }

    public void NextIndex()
    {
        Debug.Log("Interact");
        SetIndex(index + 1);
    }

    public void PreviousIndex()
    {
        SetIndex(index - 1);
    }
}
