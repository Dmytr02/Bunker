using System;
using UnityEngine;

[Serializable]
public class Observed<T> 
{
    [SerializeField] private T value;
    
    private Func<T, T> onGet;
    private Func<T, T> onSet;

    public Observed() { }

    public void Bind(Func<T, T> getter, Func<T, T> setter)
    {
        onGet = getter;
        onSet = setter;
    }

    public T Value
    {
        get => onGet != null ? onGet(value) : value;
        set
        {
            this.value = onSet != null ? onSet(value) : value;
        }
    }
}
