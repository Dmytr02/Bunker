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
        int line = 0;
        foreach (var i in MyPlayerPrefs.data.stringKeys._hashSet)
        {
            EditorGUI.LabelField(new Rect(0, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), i);
            PlayerPrefs.SetString(i, EditorGUI.TextField(new Rect(EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.5f, EditorGUIUtility.singleLineHeight), PlayerPrefs.GetString(i)));
            
            
            if (GUI.Button(new Rect(EditorGUIUtility.currentViewWidth * 0.75f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), "Clear"))
            {
                MyPlayerPrefs.MyDeleteKey(i);
            }

            line += 4;
        }foreach (var i in MyPlayerPrefs.data.intKeys._hashSet)
        {
            EditorGUI.LabelField(new Rect(0, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), i);
            PlayerPrefs.SetInt(i, EditorGUI.IntField(new Rect(EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.5f, EditorGUIUtility.singleLineHeight), PlayerPrefs.GetInt(i)));
            
            if (GUI.Button(new Rect(EditorGUIUtility.currentViewWidth * 0.75f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), "Clear"))
            {
                MyPlayerPrefs.MyDeleteKey(i);
            }

            line += 4;
        }foreach (var i in MyPlayerPrefs.data.floatKeys._hashSet)
        {
            EditorGUI.LabelField(new Rect(0, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), i);
            PlayerPrefs.SetFloat(i, EditorGUI.FloatField(new Rect(EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.5f, EditorGUIUtility.singleLineHeight), PlayerPrefs.GetFloat(i)));
            
            if (GUI.Button(new Rect(EditorGUIUtility.currentViewWidth * 0.75f, EditorGUIUtility.singleLineHeight*line, EditorGUIUtility.currentViewWidth * 0.25f, EditorGUIUtility.singleLineHeight), "Clear"))
            {
                MyPlayerPrefs.MyDeleteKey(i);
            }

            line += 4;
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