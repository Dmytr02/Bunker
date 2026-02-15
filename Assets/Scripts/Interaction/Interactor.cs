using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    IInteractable interactable;
    public UnityEvent<Interactor, RaycastHit, bool> onEndInteraction;
    void Update()
    {
        if(!(UIController.instance.CurrentState is UIGameState)) return;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnPointer(this, hit);
                if (interactable != this.interactable)
                {
                    this.interactable?.OnPointerExit(this, hit);
                    interactable.OnPointerEnter(this, hit);
                    if (Input.GetMouseButton(0))
                    {
                        onEndInteraction?.Invoke(this, hit, false); 
                        interactable.StartInteract(this, hit, false);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    interactable.StartInteract(this, hit, true);
                }

                if (Input.GetMouseButton(0))
                {
                    if (interactable == this.interactable)
                        interactable.Interact(this, hit);
                }
                
                this.interactable = interactable;
            }

            if (Input.GetMouseButtonUp(0))
            {
                onEndInteraction?.Invoke(this, hit, true);
                this.interactable = null;
            }

            
        }
    }
}

public interface IInteractable
{
    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst);
    public void Interact(Interactor interactor, RaycastHit hit);
    public void OnPointer(Interactor interactor, RaycastHit hit);
    public void OnPointerEnter(Interactor interactor, RaycastHit hit);
    public void OnPointerExit(Interactor interactor, RaycastHit hit);
}