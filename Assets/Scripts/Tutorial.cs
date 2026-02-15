using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<Sprite> images = new();
    [SerializeField] Image image1;
    [SerializeField] Image image2;
    [SerializeField] Animator animator;
    
    public bool inProgress = false;
    
    private int index = 0;
    void Start()
    {
        image1.sprite = images[0];
        image2.sprite = images[0];
    }

    public void NextImage()
    {
        if(inProgress) return;
        if (index >= images.Count - 1)
        {
            endTutorial();
            return;
        }
        SetImage(++index);
    }
    
    public void PreviusImage()
    {
        if(inProgress) return;
        index = Mathf.Max(index - 1, 0);
        SetImage(index);
    }

    void SetImage(int i)
    {
        image2.sprite = image1.sprite;
        image1.sprite = images[i];
        animator.SetTrigger("change");
        inProgress = true;
    }
    
    void endTutorial()
    {
        animator.SetBool("end", true);
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("EndTutorial", true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
