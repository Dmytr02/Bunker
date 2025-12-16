using UnityEngine;

[RequireComponent (typeof (RectTransform))]
public abstract class Card : MonoBehaviour, IInteractable, IRadialLayautGroupWeighted
{
    protected virtual void OnUse(RaycastHit hit){Debug.Log("Use Card to: "+hit.collider.name);}
    void Update()
    {
        int cameraAngel = ((int)Camera.main.transform.rotation.eulerAngles.x + 180) % 360 - 180;
        //(transform as RectTransform).anchoredPosition = new Vector2(0, cameraAngel*7);
    }

    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst)
    {
        if(!isFirst) return;
        Debug.Log(hit.transform.name);
        interactor.onEndInteraction.AddListener(EndInteract);
    }

    public void Interact(Interactor interactor, RaycastHit hit)
    {
    }

    public void EndInteract(Interactor interactor, RaycastHit hit, bool isEnd)
    {
        if (isEnd)
        {
            interactor.onEndInteraction.RemoveListener(EndInteract);
            OnUse(hit);
        }
    }

    public void OnPointer(Interactor interactor, RaycastHit hit)
    {
        
    }

    public void OnPointerEnter(Interactor interactor, RaycastHit hit)
    {
        weighted = 1;
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
