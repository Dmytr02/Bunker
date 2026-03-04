using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField createRoomName;
    [SerializeField] TMP_InputField joinRoomName;
    [SerializeField] GameObject newGamePanel;
    [SerializeField] GameObject joinGamePanel;
    [SerializeField] GameObject settingsGamePanel;
    [SerializeField] GameObject mainMenuPanel;

    [SerializeField] LobbyAsset prefabLobbyAsset;
    [SerializeField] Transform lobbyList;
    
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] GameObject namePanel;
    [SerializeField] int maxCharsName;
    [SerializeField] int minCharsName;

    Dictionary<string, LobbyAsset> lobbys = new Dictionary<string, LobbyAsset>();

    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);
    [SerializeField] private Animator loadingScreenAnimator;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!PlayerPrefs.HasKey("name"))
        {
            namePanel.SetActive(true);
            nameInput.onSubmit.AddListener(SetName);
            nameInput.onValidateInput += ValidateChar;
        }
        if (PhotonNetwork.IsConnected)
        {
            MainMenu();
            PhotonNetwork.JoinLobby(customLobby);
        }
        else PhotonNetwork.ConnectUsingSettings(); // Connect to Photon
    }
    private char ValidateChar(string text, int charIndex, char addedChar)
    {
        if (text.Length > maxCharsName) return '\0';
        // 1. Проверка на первый символ (нельзя цифру)
        if (charIndex == 0 && char.IsDigit(addedChar))
        {
            return '\0';
        }

        // 2. Разрешаем: a-z, A-Z, 0-9 и _
        if (char.IsLetterOrDigit(addedChar) || addedChar == '_')
        {
            /*if (char.IsLetter(addedChar) && (addedChar < 'A' || addedChar > 'z'))
            {
                return '\0';
            }*/
            return addedChar;
        }

        return '\0'; 
    }
    public void SetName(string name)
    {
        if(name.Length < 3) return;
        PlayerPrefs.SetString("name", name);
        namePanel.SetActive(false);
    }

    public void copyNameToInputField()
    {
        nameInput.text = PlayerPrefs.GetString("name");
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
    
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void JoinGame()
    {
        mainMenuPanel.SetActive(false);
        joinGamePanel.SetActive(true);
    }
    public void SettingsGame()
    {
        mainMenuPanel.SetActive(false);
        settingsGamePanel.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuPanel.SetActive(true);
        newGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        settingsGamePanel.SetActive(false);
    }


    public void OnCreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomName.text);
    }

    public void OnJoinRoom()
    {
        NetworkControler.roomName = joinRoomName.text;
        loadingScreenAnimator.SetTrigger("loadScene"); 
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
                    loadingScreenAnimator.SetTrigger("loadScene"); 
                } );
                lobbys[info.Name].eventTrigger.triggers.Add(entry);
            }
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        loadingScreenAnimator.SetTrigger("loadScene"); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}