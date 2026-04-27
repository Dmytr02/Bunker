using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Animator animator;
    public Animator[] otherAnimators;
    void Reset()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("isHovered", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("isHovered", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        animator.SetBool("isSelected", !animator.GetBool("isSelected"));
        foreach (Animator animator in otherAnimators)
        {
            if(animator != this.animator) animator.SetBool("isSelected", false);
        }
    }
}
