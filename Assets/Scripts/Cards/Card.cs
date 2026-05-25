using System;
using UnityEngine;

[RequireComponent (typeof (RectTransform))]
public abstract class Card : MonoBehaviour, IInteractable, IRadialLayautGroupWeighted
{
    bool isSelected = false;
    Vector2 offset = Vector2.zero; 
    Transform parentTransform;
    protected virtual bool OnUse(RaycastHit hit){Debug.Log("Use Card to: "+hit.collider.name); return true;}

    protected virtual void Start()
    {
        parentTransform= transform.parent;
    }

    void Update()
    {
        if (isSelected)
        {
            ((RectTransform)transform).anchoredPosition = (Vector2)Input.mousePosition + offset;
        }
    }

    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst)
    {
        if(!isFirst) return;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        isSelected = true;
        offset = ((RectTransform)transform).anchoredPosition - (Vector2)Input.mousePosition;
        interactor.onEndInteraction.AddListener(EndInteract);
        interactor.mask |= LayerMask.GetMask("UI");
    }

    public void Interact(Interactor interactor, RaycastHit hit)
    {
    }

    public void EndInteract(Interactor interactor, RaycastHit hit, bool isEnd)
    {
        if (isEnd)
        {
            interactor.onEndInteraction.RemoveListener(EndInteract);
            if(hit.collider && OnUse(hit)) Destroy(gameObject);
            else transform.SetParent(parentTransform);
            isSelected = false;
            interactor.mask &= ~LayerMask.GetMask("UI");
        }
    }

    public void OnPointer(Interactor interactor, RaycastHit hit)
    {
        
    }

    public void OnPointerEnter(Interactor interactor, RaycastHit hit)
    {
        weighted = 5;
    }

    public void OnPointerExit(Interactor interactor, RaycastHit hit)
    {
        weighted = 1;
    }

    int weighted = 1;

    public int Weight
    {
        get => weighted;
    }
}
