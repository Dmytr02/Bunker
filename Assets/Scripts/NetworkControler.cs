using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkControler : MonoBehaviourPunCallbacks
{
    [SerializeField] private string targetSceneName;
    public static string roomName;
    
    public static NetworkControler instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем, совпадает ли имя загруженной сцены с целевой
        Debug.Log("OnSceneLoaded - " + scene.name);
        if (scene.name == targetSceneName)
        {
            if(PhotonNetwork.IsMasterClient) return;
            Debug.Log("OnSceneLoadedTarget - " + targetSceneName);
            if (!string.IsNullOrEmpty(roomName)) PhotonNetwork.JoinRoom(roomName);
            else PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        AsyncOperation menuLoad = SceneManager.LoadSceneAsync(0);
        base.OnJoinRoomFailed(returnCode, message);
    }
}
