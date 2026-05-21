using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SavedKeyBind : MonoBehaviour
{
    private KeyCode key;
    [SerializeField]  private string actionName;

    private void Start()
    {
        key = (KeyCode)PlayerPrefs.GetInt(actionName);
        BindKey.OnChanged += OnChanged;
    }

    private void OnDestroy()
    {
        BindKey.OnChanged -= OnChanged;
    }

    private void OnChanged(string s, KeyCode code)
    {
        if(actionName == s) key = code;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }
}