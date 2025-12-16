using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxSlider))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            MinMaxSlider myAttribute = (MinMaxSlider)attribute;
            Vector2 val = property.vector2Value;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            float floatFieldWidth = 45f; 
            float spacing = 5f;        
            float sliderWidth = position.width - (floatFieldWidth * 2) - (spacing * 2);

            Rect leftFieldRect = new Rect(position.x, position.y, floatFieldWidth, position.height);
            Rect sliderRect = new Rect(leftFieldRect.xMax + spacing, position.y, sliderWidth, position.height);
            Rect rightFieldRect = new Rect(sliderRect.xMax + spacing, position.y, floatFieldWidth, position.height);

            EditorGUI.BeginChangeCheck();
        
            val.x = EditorGUI.FloatField(leftFieldRect, val.x);
        
            EditorGUI.MinMaxSlider(sliderRect, ref val.x, ref val.y, myAttribute.min, myAttribute.max);
        
            val.y = EditorGUI.FloatField(rightFieldRect, val.y);

            if (EditorGUI.EndChangeCheck())
            {
                val.x = Mathf.Clamp(val.x, myAttribute.min, val.y);
                val.y = Mathf.Clamp(val.y, val.x, myAttribute.max);
                property.vector2Value = val;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }
}