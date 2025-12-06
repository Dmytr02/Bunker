using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "name", fileName = "name1")]
public class PlayerPrefsData : ScriptableObject
{
    public SerializableHashSet<string> stringKeys = new SerializableHashSet<string>();
    public SerializableHashSet<string> intKeys = new SerializableHashSet<string>();
    public SerializableHashSet<string> floatKeys = new SerializableHashSet<string>();

    private void OnEnable()
    {
        MyPlayerPrefs.data = this;
    }
}
