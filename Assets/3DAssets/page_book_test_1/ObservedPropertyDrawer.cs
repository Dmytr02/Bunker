using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Observed<>), true)]
public class ObservedPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        EditorGUI.BeginChangeCheck();
        
        EditorGUI.PropertyField(position, valueProp, label);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();

            object targetObject = property.serializedObject.targetObject;
            var fieldInfo = targetObject.GetType().GetField(property.name, 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            
            if (fieldInfo != null)
            {
                object observedStruct = fieldInfo.GetValue(targetObject);
                
                
                //if(valueProp.propertyType == SerializedPropertyType.Integer)
                //{
                    var propInfo = observedStruct.GetType().GetProperty("Value");
                    propInfo?.SetValue(observedStruct, valueProp.GetUnderlyingValue());
                //}
            }
        }
    }
}