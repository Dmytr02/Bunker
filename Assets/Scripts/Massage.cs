using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Massage : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform image;
    [SerializeField] private TMP_Text text;

    [PunRPC]
    public void showMassage(string msg)
    {
        CancelInvoke(nameof(hideMassage));
        gameObject.SetActive(true);
        text.text = msg;
        text.ForceMeshUpdate();
        image.offsetMax = new Vector2(image.offsetMax.x, text.preferredHeight);
        Invoke("hideMassage", msg.Length*0.2f);
    }

    public void hideMassage()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
