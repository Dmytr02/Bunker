using System;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ColorRandoizer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] Outline outline;
    
    public static Color NumberToColor(int n)
    {
        double phi = 0.61803398875; 

        long index = n >= 0 ? n : -n;

        float h = (float)((index * phi) % 1.0); 

        return Color.HSVToRGB(h, 0.8f, 0.9f);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        outline.OutlineColor = NumberToColor((int)info.photonView.InstantiationData[0]);
    }
}
