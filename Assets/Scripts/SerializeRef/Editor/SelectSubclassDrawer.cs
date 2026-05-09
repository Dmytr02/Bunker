using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SelectSubclassAttribute))]
public class SelectSubclassDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.LabelField(position, label.text, "Use only with [SerializeReference]");
            return;
        }

        Type baseType = GetPropertyType(property);

        // 2. Рисуем заголовок и кнопку
        Rect buttonRect = EditorGUI.PrefixLabel(position, label);
        buttonRect.height = EditorGUIUtility.singleLineHeight;

        string typeName = GetTypeName(property.managedReferenceFullTypename);

        if (GUI.Button(buttonRect, typeName, EditorStyles.popup))
        {
            ShowTypeMenu(property, baseType);
        }

        EditorGUI.PropertyField(position, property, GUIContent.none, true);
    }

    private void ShowTypeMenu(SerializedProperty property, Type baseType)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Null"), false, () => SetType(property, null));

        var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType)
            .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType);

        foreach (var type in derivedTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => SetType(property, type));
        }
        menu.ShowAsContext();
    }

    private void SetType(SerializedProperty property, Type type)
    {
        property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
        property.serializedObject.ApplyModifiedProperties();
    }

    private Type GetPropertyType(SerializedProperty property)
    {
        string[] typeDetails = property.managedReferenceFieldTypename.Split(' ');
        if (typeDetails.Length < 2) return typeof(object);
        
        var assembly = System.Reflection.Assembly.Load(typeDetails[0]);
        return assembly.GetType(typeDetails[1]);
    }

    private string GetTypeName(string fullTypename)
    {
        if (string.IsNullOrEmpty(fullTypename)) return "Null (Empty)";
        var parts = fullTypename.Split(' ');
        return parts.Last().Split('.').Last();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
