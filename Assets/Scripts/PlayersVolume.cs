using System;
using TMPro;
using UnityEngine;

public class PlayersVolume : MonoBehaviour
{
    [SerializeField] Slider_[] sliders;
    [SerializeField] TMP_Text[] names;

    private void Awake()
    {
        PlayerMovmant.onPlayersAdded.AddListener(OnPlayersListChanged);
        PlayerMovmant.onPlayersRemoved.AddListener(OnPlayersListChanged);
    }

    private void Start()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            int i0 = i;
            sliders[i].onValueChange.AddListener((f) =>
            {
                if(PlayerMovmant.players.Count>i0) PlayerMovmant.players[i0].voiceController.audioSource.volume = f;
            });
        }
    }

    public void OnPlayersListChanged()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            names[i].transform.parent.gameObject.SetActive(true);
            if (PlayerMovmant.players.Count > i)
            {
                if(PlayerMovmant.players[i] == PlayerMovmant.player) names[i].transform.parent.gameObject.SetActive(false);
                names[i].text = PlayerMovmant.players[i].stats.list["Name"].ToString();
            }
            else
            {
                names[i].text = "-";
            }
        }
    }
}
