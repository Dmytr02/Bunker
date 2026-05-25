using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interactor : MonoBehaviour
{
    IInteractable interactable;
    public UnityEvent<Interactor, RaycastHit, bool> onEndInteraction;
    public int mask = 0;
    public AsyncRTFilter  asyncRTFilter;
    void Update()
    {
        if(TutorialUIController.instance && !(TutorialUIController.instance.CurrentState is TutorialUIGameState)) return;
        if(UIController.instance && !(UIController.instance.CurrentState is UIGameState)) return;
        //if(asyncRTFilter != null && asyncRTFilter.IsRaycastLocationValid(Input.mousePosition, Camera.main)) return;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, ~mask))
        {
            IInteractable[] interactables = hit.collider.gameObject.GetComponents<IInteractable>().Where(n =>
            {
                if (n is MonoBehaviour monoBehaviour) return monoBehaviour.enabled;
                return false;
            }).ToArray();
            foreach(var interactable in interactables)
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
                Debug.Log("End IIIIIIIIIIIIIIIInteraction - " + interactable);
                onEndInteraction?.Invoke(this, hit, true);
                this.interactable = null;
            }
        }
        else
        {
            interactable?.OnPointerExit(this, hit);
            interactable = null;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("End IIIIIIIIIIIIIIIInteraction - " + interactable);
            onEndInteraction?.Invoke(this, default, true);
            this.interactable = null;
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