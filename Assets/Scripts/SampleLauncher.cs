using System;
using Photon.Pun;
using UnityEngine;

public class SampleLauncher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;
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

    public void Start()
    {
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("OnJoinRoomFailed - " + message);
        base.OnJoinRoomFailed(returnCode, message);
    }
}
