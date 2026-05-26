using System.Linq;
using Photon.Pun;
using UnityEngine;

public class CursoreCard : Card
{
    protected override bool OnUse(RaycastHit hit)
    {
        if(hit.collider.tag != "bookLeft" && hit.collider.tag != "bookRight" ) return false;
        if (hit.collider.transform.parent == null) return false;
        //if (hit.collider.transform.parent.parent == null) return false;
        //if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.TryGetComponent(out Notepad notepad))
        {
            PlayerMovmant player = PlayerMovmant.players.FirstOrDefault(p => p.index == notepad.index + (hit.collider.CompareTag("bookLeft")?0:1));
            if (player != null)
            {
                int playerID = player.stats.playerID;
                PhotonView.Find(playerID).RPC("RPC_CursoreEffect", PhotonView.Find(playerID).Owner);
                    
                return true;
            }
            /*if (hit.collider.tag == "bookLeft")
            {
                if (notepad.playersStats.Count - 1 > notepad.index)
                {
                    int playerID = notepad.playersStats[notepad.index].playerID;
                    PhotonView.Find(playerID).RPC("RPC_CursoreEffect", PhotonView.Find(playerID).Owner);
                    
                    return true;
                }
            }
            else if (hit.collider.tag == "bookRight")
            {
                if (notepad.playersStats.Count - 1 > notepad.index + 1)
                {
                    int playerID = notepad.playersStats[notepad.index+1].playerID;
                    PhotonView.Find(playerID).RPC("RPC_CursoreEffect", PhotonView.Find(playerID).Owner);
                    
                    return true;
                }
            }*/
        }
        return false;
    }
}
