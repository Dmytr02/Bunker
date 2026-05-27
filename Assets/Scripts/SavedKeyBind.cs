using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SavedKeyBind : MonoBehaviour
{
    [SerializeField] private SavedKey key;
    [SerializeField] private bool isOnlyGame;

    private void Start()
    {
        key.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(key.key))
        {
            if(!isOnlyGame || (UIController.instance.CurrentState is UIGameState)) ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }
    
    private void OnDestroy()
    {
        key.OnDestroy();
    }
}

[Serializable]
public class SavedKey
{
    [HideInInspector] public KeyCode key;
    [SerializeField] private string actionName;

    public void Init()
    {
        Debug.Log(actionName);
        key = (KeyCode)PlayerPrefs.GetInt(actionName);
        BindKey.OnChanged += OnChanged;
    }

    public void OnDestroy()
    {
        BindKey.OnChanged -= OnChanged;
    }

    private void OnChanged(string s, KeyCode code)
    {
        if(actionName == s) key = code;
    }
}