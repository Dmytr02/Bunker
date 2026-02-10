using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Notepad : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] public TMP_Text text;
    [SerializeField] public int index = 0;

    
    private List<PlayerStats> playersStats = new List<PlayerStats>();
    
    public PlayerStats SelectedPlayerStats => playersStats[index];


    private void Start()
    {
        foreach (var i in PlayerMovmant.players)
        {
            playersStats.Add(i.stats);
            Debug.Log(i.stats);
        }
        Debug.Log(playersStats.Count);
        SetIndex(0);
        
        PlayerMovmant.onStatOpened.AddListener((p) =>
        {
            if (p.stats == playersStats[index]) SetIndex(index);
        });
    }

    public void SetIndex(int index)
    {
        this.index = (index+playersStats.Count)%playersStats.Count;
        text.text = playersStats[this.index].ToString();
    }

    public void NextIndex()
    {
        SetIndex(index + 1);
    }

    public void PreviousIndex()
    {
        SetIndex(index - 1);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        transform.SetParent(PhotonView.Find((int)data[0]).GetComponent<PlayerMovmant>().playerMash.transform);
    }
}
