using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyAsset : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text roomName;
    public Action<PointerEventData> eventTrigger;

    public void OnPointerClick(PointerEventData eventData)
    {
        eventTrigger?.Invoke(eventData);
    }
}
