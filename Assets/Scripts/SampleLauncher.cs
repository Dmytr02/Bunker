using System;
using Photon.Pun;
using UnityEngine;

public class SampleLauncher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;
	[SerializeField] private Transform[] spawnPoints;
    /*void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }*/

    /*public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
    }*/

    [PunRPC]
    private void getSpawnPoint(PhotonMessageInfo info)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].gameObject.activeSelf)
            {
                photonView.RPC("sendSpawnPoint", info.Sender, i);
                spawnPoints[i].gameObject.SetActive(false);
                return;
            }
        }
    }

    [PunRPC]
    private void sendSpawnPoint(int index)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[index].position, Quaternion.identity, 0);
    }

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            sendSpawnPoint(0);
            spawnPoints[0].gameObject.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        photonView.RPC("getSpawnPoint", RpcTarget.MasterClient);
        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("OnJoinRoomFailed - " + message);
        base.OnJoinRoomFailed(returnCode, message);
    }
}
