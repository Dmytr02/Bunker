using System;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour
{
    [SerializeField] Button muteButton;
    [SerializeField] Recorder recorder;
    [SerializeField] Image microphoneImg;
    [SerializeField] Sprite microphoneOn;
    [SerializeField] Sprite microphoneOff;
    public AudioSource audioSource;
    public static UnityEvent onVolumeChange = new();
    [SerializeField] SavedKey key;
    private void Start()
    {
        PlayerMovmant.onPlayersRemoved.AddListener((() =>
        {
            if(PlayerMovmant.players.Contains(GetComponent<PlayerMovmant>())) audioSource.volume = PlayerPrefs.GetFloat($"VoiceVolume{GetComponent<PlayerMovmant>().index + (PlayerMovmant.player.index <= GetComponent<PlayerMovmant>().index ? 0 : -1)}", 1f);
            else audioSource.volume = 0f;
        }));
        key.Init();
        setVolume();
        onVolumeChange.AddListener(setVolume);
        microphoneImg = GameObject.Find("MicroImg").GetComponent<Image>();
        muteButton.OnInteract.AddListener((() =>
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled; 
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }));
    }

    private void OnDestroy()
    {
        onVolumeChange.RemoveListener(setVolume);
    }

    private void Update()
    {
        if(!GetComponent<PhotonView>().IsMine) return;
        if (Input.GetKeyDown(key.key) && UIController.instance.CurrentState is UIGameState)
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            microphoneImg.sprite = recorder.TransmitEnabled ? microphoneOn : microphoneOff;
        }
    }

    async void setVolume()
    {
        while (PlayerMovmant.player == null) await Task.Delay(500);
        if (PlayerPrefs.HasKey($"VoiceVolume{GetComponent<PlayerMovmant>().index}")) //  + (PlayerMovmant.player.index <= GetComponent<PlayerMovmant>().index ? 0 : -1)
        {
            if(PlayerMovmant.players.Contains(GetComponent<PlayerMovmant>())) audioSource.volume = PlayerPrefs.GetFloat($"VoiceVolume{GetComponent<PlayerMovmant>().index}", 1f); //  + (PlayerMovmant.player.index <= GetComponent<PlayerMovmant>().index ? 0 : -1)
            else audioSource.volume = 0f;
        }
    }
}
