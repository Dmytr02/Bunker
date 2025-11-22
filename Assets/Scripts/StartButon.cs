using Photon.Pun;
using UnityEngine;

public class StartButon : MonoBehaviourPunCallbacks
{
    [SerializeField]Animator animator;

    public void SetTrigger(string trigger)
    {
        photonView.RPC("SetTriggerRPC", RpcTarget.All, trigger);
    }

    [PunRPC]
    private void SetTriggerRPC(string trigger)
    {
        animator.SetTrigger(trigger);
    }
}
