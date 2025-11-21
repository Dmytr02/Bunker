using System;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ClearPlayerPrefs : EditorWindow
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    static void Clear()
    {
        if (EditorUtility.DisplayDialog("Clear PlayerPrefs", "Are you sure you want to delete all data from PlayerPrefs?", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs Cleared.");
        }
    }
    
    [MenuItem("Tools/Clear One PlayerPrefs")]
    static void ClearOne()
    {
        EditorWindow window = GetWindow<ClearPlayerPrefs>();
        window.Show();
    }

    string input = "";
    private void OnGUI()
    {
        GUILayout.Label("Select an object in the hierarchy view");
        
        input = EditorGUILayout.TextField("Object Name: ", input);

        if (GUILayout.Button("Нажми меня"))
        {
            ClierByName(input);
        }
        
        this.Repaint();
    }

    private void ClierByName(string name)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            Debug.Log($"PlayerPrefs - {name}, does not exist");
            return;
        }
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.Save();
        Debug.Log($"PlayerPrefs - {name}, Cleared.");
    }
}