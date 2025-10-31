using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerMovmant : MonoBehaviourPunCallbacks
{
    //[SerializeField] CharacterController characterController;
    //[SerializeField] float speed = 2.0f;
    //[SerializeField] float gravity = 9.8f;
    //[SerializeField] float jumpForce = 10;

    [SerializeField] private Massage massage;
    [SerializeField] private float sensivity = 1;
    //public PlayerStats stats = new PlayerStats();
    
    float yForce = 0;
    void Start()
    {
        if (photonView.IsMine)
        {
            CommandManager.Instance.AddInstance(this);
            
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            transform.LookAt(Vector3.zero);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
    }
    
    
    [CommandAtribute("/kick", "take a gameObject if it`s player kick him")]
    public void kick(GameObject player)
    {
        if(player == null) return;
        if (player.TryGetComponent(out PhotonView playerView)) 
            playerView.RPC("RPC_kick", playerView.Owner);
            //PhotonNetwork.CloseConnection(playerView.Owner);
    }

    [PunRPC]
    public void RPC_kick(PhotonMessageInfo info)
    {
        PhotonNetwork.LeaveRoom();
    }
    
    /*
    [CommandAtribute("-getStats", "take a gameObject if it`s player return stats")]
    public static string getStats(GameObject go)
    {
        if (go.TryGetComponent(out PlayerMovmant player))
        {
            return player.stats.ToString();
        }
        return $"{go.name} is not a player";
    }
    */

    public void sendMassage(string msg)
    {
        massage.photonView.RPC("showMassage", RpcTarget.All, msg);
    }
}


/*public class PlayerStats
{
    public int Age = 5;
    public string Name = "name";
    public int Score = 1;

    public PlayerStats()
    {
        Age = Random.Range(1, 100);
        Score = Random.Range(1, 10);
    }

    public override string ToString()
    {
        return $"{Name}, {Age}, {Score}";
    }
}*/