using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Massage : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Transform canvas;
    

    [PunRPC]
    public void showMassage(string msg)
    {
        CancelInvoke(nameof(hideMassage));
        image.gameObject.SetActive(true);
        text.text = msg;
        text.ForceMeshUpdate();
        image.offsetMax = new Vector2(image.offsetMax.x, text.preferredHeight);
        Invoke("hideMassage", msg.Length*0.5f);
    }

    public void hideMassage()
    {
        image.gameObject.SetActive(false);
    }

    private void Update()
    {
        canvas.transform.LookAt(Camera.main.transform);
    }
}
