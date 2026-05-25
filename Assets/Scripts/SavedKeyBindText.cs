using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SavedKeyBindText : MonoBehaviour
{
    [SerializeField]  private string actionName;
    [SerializeField]  private TMP_Text text;
    private void Start()
    {
        if(text == null) text = GetComponent<TMP_Text>();
        if (text == null) return;
        text.text = ((KeyCode)PlayerPrefs.GetInt(actionName)).KeyCodeString();
        BindKey.OnChanged += OnChanged;
    }

    private void OnDestroy()
    {
        BindKey.OnChanged -= OnChanged;
    }

    private void OnChanged(string s, KeyCode code)
    {
        if(actionName == s) text.text = code.KeyCodeString();
    }
}