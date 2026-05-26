using System.Linq;
using Photon.Pun;
using UnityEngine;

public class SoundCard : Card
{
    public int idSound;
    protected override bool OnUse(RaycastHit hit)
    {
        
        if (hit.collider.tag != "bookLeft" && hit.collider.tag != "bookRight" ) return false;
        if (hit.collider.transform.parent == null) return false;
        //if (hit.collider.transform.parent.parent == null) return false;
        //if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.TryGetComponent(out Notepad notepad)) return false;
        {
            PlayerMovmant.player.photonView.RPC("PlaySound", RpcTarget.All, idSound);
            return true;
        }

        /*if (hit.collider.transform.parent.parent.parent.TryGetComponent(out Notepad notepad))
        {
            if (hit.collider.tag == "bookLeft")
            {
                if (notepad.playersStats.Count - 1 > notepad.index)
                {
                    int playerID = notepad.playersStats[notepad.index].playerID;
                    PhotonView.Find(playerID).RPC("PlaySound", RpcTarget.All, idSound);

                    return true;
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    int playerID = notepad.playersStats[notepad.index+1].playerID;
                    PhotonView.Find(playerID).RPC("PlaySound", RpcTarget.All, idSound);

                    return true;
                }
            }
        }*/
        return false;
    }
}
