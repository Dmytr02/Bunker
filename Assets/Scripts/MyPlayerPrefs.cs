using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyPlayerPrefs : PlayerPrefs
{
    public static PlayerPrefsData data;
    public static void MySetString(string key, string value)
    {
        data.stringKeys.Add(key);
        SetString(key, value);
    }

    public static void MySetInt(string key, int value)
    {
        data.intKeys.Add(key);
        SetInt(key, value);
    }

    public static void MySetFloat(string key, float value)
    {
        data.floatKeys.Add(key);
        SetFloat(key, value);
    }

    public static void MyDeleteKey(string key)
    {
        data.stringKeys.Remove(key);
        data.intKeys.Remove(key);
        data.floatKeys.Remove(key);
        DeleteKey(key);
    }
}
