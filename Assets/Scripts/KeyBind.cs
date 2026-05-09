using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBind : MonoBehaviour
{
    public KeyCode key;
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }
}
