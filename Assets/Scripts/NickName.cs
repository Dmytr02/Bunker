using System;
using TMPro;
using UnityEngine;

public class NickName : MonoBehaviour
{
    [SerializeField] PlayerMovmant player;
    [SerializeField] TMP_Text text;

    private void Awake()
    {
        PlayerMovmant.onPlayersAdded.AddListener((() => text.text = player.stats.list["Name"].ToString()));
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
