using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkControler : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    public static string roomName;
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
            Debug.Log("OnSceneLoadedTarget - " + targetSceneName);
            if(!string.IsNullOrEmpty(roomName)) PhotonNetwork.JoinRoom(roomName); 
        }
    }
}
