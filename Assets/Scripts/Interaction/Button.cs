using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract =  new UnityEvent(); 
    public UnityEvent OnPointerEnterEvent =  new UnityEvent(); 
    public UnityEvent OnPointerExitEvent =  new UnityEvent(); 

    public void StartInteract(Interactor interactor, RaycastHit hit,  bool isFirst)
    {
        if(!enabled) return;
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
        if(!enabled) return;
        OnPointerEnterEvent?.Invoke();
        Debug.Log("Button Pointer Enter");
    }

    public void OnPointerExit(Interactor interactor, RaycastHit hit)
    {
        if(!enabled) return;
        OnPointerExitEvent?.Invoke();
        Debug.Log("Button Pointer Exit");
    }
}
