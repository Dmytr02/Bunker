using Photon.Pun;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    public void Trigger()
    {
        if(TutorialGameManager.Instance) TutorialGameManager.Instance.Trigger = true;
        
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("EndTutorial", true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
