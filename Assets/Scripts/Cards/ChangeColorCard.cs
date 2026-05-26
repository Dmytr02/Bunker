using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChangeColorCard : Card
{
    protected override bool OnUse(RaycastHit hit)
    {
        if(hit.collider.tag != "bookLeft" && hit.collider.tag != "bookRight" ) return false;
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent.TryGetComponent(out Notepad notepad))
        {
            PlayerMovmant player = PlayerMovmant.players.FirstOrDefault(p => p.index == notepad.index + (hit.collider.CompareTag("bookLeft")?0:1));
            if (player != null)
            {
                int playerID = player.stats.playerID;
                PhotonView.Find(playerID).RPC("RPC_ChangeColorsEffect", PhotonView.Find(playerID).Owner);

                return true;
            }
        }
        return false;
    }
}
