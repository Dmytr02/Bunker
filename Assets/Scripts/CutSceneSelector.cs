using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class CutSceneSelector : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] cutscenes;
    void Start()
    {
        photonView.RPC("RPC_CutScene", RpcTarget.AllBuffered, Random.Range(0, cutscenes.Length));
    }

    [PunRPC]
    public void RPC_CutScene(int i)
    {
        cutscenes[i].SetActive(true);
    }
}
