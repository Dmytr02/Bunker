using System;
using UnityEngine;

public class Bookmark : MonoBehaviour, IInteractable
{
    [SerializeField] private int index;
    [SerializeField] private Notepad notepad;
    private void Start()
    {
        if(PlayerMovmant.players.Count <= index) gameObject.SetActive(false); 
    }

    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst)
    {
        notepad.SetIndex(index);
    }

    public void Interact(Interactor interactor, RaycastHit hit)
    {
        
    }

    public void OnPointer(Interactor interactor, RaycastHit hit)
    {
        
    }

    public void OnPointerEnter(Interactor interactor, RaycastHit hit)
    {
        
    }

    public void OnPointerExit(Interactor interactor, RaycastHit hit)
    {
        
    }
}
