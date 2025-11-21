using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ShowStaticFieldAttribute : PropertyAttribute
{
    public bool isCanChange;

    public ShowStaticFieldAttribute(bool isCanChange = false)
    {
        this.isCanChange = isCanChange;
    }
}