using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BindKey : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public TMP_Text buttonText;
    public static Action<string, KeyCode> OnChanged;
    
    [Header("Key Settings")]
    public string actionName = "Jump";
    public KeyCode currentKey;

    // Черный список клавиш, которые нельзя назначать
    private readonly HashSet<KeyCode> forbiddenKeys = new HashSet<KeyCode>()
    {
        KeyCode.Escape,  
        KeyCode.Return,   
        KeyCode.KeypadEnter, 
    };

    private bool isListening = false;
    
    void Start()
    {
        currentKey = (KeyCode)PlayerPrefs.GetInt(actionName, (int)currentKey);

        UpdateButtonText(currentKey.ToString());
    }

    void StartListening()
    {
        isListening = true;
        UpdateButtonText("Click Any Key...");
    }

    void OnGUI()
    {
        if (!isListening) return;

        Event currentEvent = Event.current;

        if (currentEvent.isKey && currentEvent.type == EventType.KeyDown)
        {
            if (currentEvent.keyCode != KeyCode.None)
            {
                // ПРОВЕРКА: Если клавиша в черном списке, игнорируем её
                if (forbiddenKeys.Contains(currentEvent.keyCode))
                {
                    Debug.LogWarning($"Клавишу {currentEvent.keyCode} нельзя назначить!");
                    return; 
                }

                SaveNewKey(currentEvent.keyCode);
            }
        }
        else if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown)
        {
            KeyCode mouseKey = KeyCode.Mouse0 + currentEvent.button;
            SaveNewKey(mouseKey);
        }
    }

    void SaveNewKey(KeyCode newKey)
    {
        currentKey = newKey;
        isListening = false;

        PlayerPrefs.SetInt(actionName, (int)newKey);
        OnChanged.Invoke(actionName, newKey);
        PlayerPrefs.Save();

        UpdateButtonText(currentKey.ToString());
    }

    void UpdateButtonText(string text)
    {
        if (buttonText != null) buttonText.text = text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartListening();
    }
}
