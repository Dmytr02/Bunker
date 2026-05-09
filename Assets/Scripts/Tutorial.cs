using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    //[SerializeField] List<Sprite> images = new();
    [SerializeField] Image image1;
    [SerializeField] Image image2;
    [SerializeField] Image image3;
    [SerializeField] Animator animator;

    public List<Sprite> Images;
    
    public bool inProgress = false;
    public bool isOpened = true;

    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            endTutorial();
        }
    }
    #endif

    private int index = 0;

    private void Start()
    {
        SwichImages();
        endTutorial();
    }

    public void ShowList(List<Sprite> images)
    {
        Images = images;
        index = 0;
        SwichImages();
        animator.SetBool("end", false);
        isOpened = true;
    }

    public void SwichImages()
    {
        if (index - 1 >= 0)
        {
            image1.sprite = Images[index - 1];
            image1.color = Color.white;
        }
        else image1.color = Color.clear;
        image2.sprite = Images[index];
        if (index + 1 < Images.Count)
        {
            image3.sprite = Images[index+1];
            image3.color = Color.white;
        }
        else image3.color = Color.clear;
    }
    
    public void NextImage()
    {
        if(inProgress) return;
        index++;
        if (index >= Images.Count)
        {
            endTutorial();
            return;
        }
        animator.SetTrigger("change");
        //SetImage(++index);
    }
    
    public void PreviusImage()
    {
        if(inProgress) return;
        index = index - 1;
        if (index < 0) index = 0;
        else animator.SetTrigger("changeBack");
        
        //SetImage(index);
    }
    
    public void endTutorial()
    {
        animator.SetBool("end", true);
        isOpened = false;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("EndTutorial", true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
