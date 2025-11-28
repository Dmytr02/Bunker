using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract; 
    public void Interact()
    {
        Debug.Log("Button Interacting...");
        OnInteract.Invoke();
    }
}
