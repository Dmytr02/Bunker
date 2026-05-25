using System;
using UnityEngine;

public class TutorialBookmark : MonoBehaviour, IInteractable
{
    [SerializeField] private int index;
    [SerializeField] private TutorialNotepad notepad;
    

    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst)
    {
        notepad.SetIndex(index/2);
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
