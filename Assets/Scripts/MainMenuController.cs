using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField createRoomName;
    [SerializeField] TMP_InputField joinRoomName;
    [SerializeField] GameObject newGamePanel;
    [SerializeField] GameObject joinGamePanel;
    [SerializeField] GameObject mainMenuPanel;

    [SerializeField] LobbyAsset prefabLobbyAsset;
    [SerializeField] Transform lobbyList;

    Dictionary<string, LobbyAsset> lobbys = new Dictionary<string, LobbyAsset>();

    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            MainMenu();
            PhotonNetwork.JoinLobby(customLobby);
        }
        else PhotonNetwork.ConnectUsingSettings(); // Connect to Photon
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        MainMenu();
        PhotonNetwork.JoinLobby(customLobby);
    }

    public void NewGame()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);
    }

    public void JoinGame()
    {
        mainMenuPanel.SetActive(false);
        joinGamePanel.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuPanel.SetActive(true);
        newGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
    }


    public void OnCreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomName.text);
    }

    public void OnJoinRoom()
    {
        NetworkControler.roomName = joinRoomName.text;
        SceneManager.LoadScene("GameScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                Destroy(lobbys[info.Name]);
            }
            else
            {
                lobbys[info.Name] = Instantiate(prefabLobbyAsset, lobbyList);
                lobbys[info.Name].roomName.text = info.Name;

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) =>
                {
                    NetworkControler.roomName = info.Name; 
                    SceneManager.LoadScene("GameScene");
                } );
                    lobbys[info.Name].eventTrigger.triggers.Add(entry);
            }
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        SceneManager.LoadScene("GameScene");
    }
}