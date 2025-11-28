using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableHashSet<T> : ISerializationCallbackReceiver
{
    public HashSet<T> _hashSet = new HashSet<T>();

    [SerializeField]
    private List<T> _serializedItems = new List<T>();

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _serializedItems.Clear();
        foreach (T item in _hashSet)
        {
            _serializedItems.Add(item);
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _hashSet.Clear();
        foreach (T item in _serializedItems)
        {
            _hashSet.Add(item);
        }
    }
    
    public bool Add(T item) => _hashSet.Add(item);
    public bool Contains(T item) => _hashSet.Contains(item);
    public bool Remove(T item) => _hashSet.Remove(item);
    public void Clear() => _hashSet.Clear();
}