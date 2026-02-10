using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract =  new UnityEvent(); 

    public void StartInteract(Interactor interactor, RaycastHit hit,  bool isFirst)
    {
        if(!isFirst) return;
        Debug.Log("Button Interacting...");
        OnInteract?.Invoke();
    }

    public void Interact(Interactor interactor, RaycastHit hit) { }

    public void EndInteract(Interactor interactor, RaycastHit hit) { }
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
