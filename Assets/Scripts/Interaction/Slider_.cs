using UnityEngine;
using UnityEngine.Events;

public class Slider_ : MonoBehaviour,  IInteractable
{
    [SerializeField] UnityEngine.UI.Slider slider;
    
    public float value => slider.value;
    public float maxValue => slider.maxValue;
    
    public UnityEngine.UI.Slider.SliderEvent onValueChange => slider.onValueChanged;

    public void StartInteract(Interactor interactor, RaycastHit hit, bool isFirst)
    {
        
    }

    public void Interact(Interactor interactor, RaycastHit hit)
    {
        if (slider == null) return;
        
        slider.value = (transform.worldToLocalMatrix.MultiplyPoint(hit.point).x + ((RectTransform)transform).rect.width/2)/((RectTransform)transform).rect.width*maxValue;
        onValueChange.Invoke(slider.value);
    }

    public void EndInteract(Interactor interactor, RaycastHit hit)
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
