using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Card), true)]
public class StaticFieldEditor : Editor
{
    private List<FieldInfo> staticFields;
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
    
    private object _pendingKeyChangeOldKey = null;
    private object _pendingKeyChangeNewKey = null;
    private IDictionary _pendingKeyChangeDictionary = null;
    private FieldInfo _pendingKeyChangeFieldInfo = null;
    private object _pendingKeyChangeParentObject = null;
    void OnEnable()
    {
        staticFields = this.target.GetType()
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => Attribute.IsDefined(field, typeof(ShowStaticFieldAttribute)))
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        ApplyPendingKeyChange();
        
        DrawDefaultInspector();

        if (staticFields.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Static Fields:", EditorStyles.boldLabel);

            foreach (var field in staticFields)
            {
                object currentValue = field.GetValue(null);
                object newValue = DrawFieldRecursive(field.Name, field.FieldType, currentValue, null, field);
                
                if (newValue != currentValue && field.GetCustomAttribute<ShowStaticFieldAttribute>().isCanChange)
                {
                    field.SetValue(null, newValue);
                }
            }
        }
    }

    private object DrawFieldRecursive(string label, Type fieldType, object value, object parentObject, FieldInfo fieldInfo)
    {
        if (IsUnityBasicType(fieldType))
        {
            return DrawElementGUI(fieldType, label, value);
        }
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            DrawDictionaryRecursive(label, (IDictionary)value, parentObject, fieldInfo);
        }
        else if (typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType != typeof(string))
        {
            DrawArrayOrListOrMultiArray(label, fieldType, (IEnumerable)value, parentObject, fieldInfo);
        }
        else if (fieldType.IsClass || (fieldType.IsValueType && !fieldType.IsPrimitive))
        {
             if (!foldoutStates.ContainsKey(label)) foldoutStates[label] = false;
            foldoutStates[label] = EditorGUILayout.Foldout(foldoutStates[label], label, true);

            if (foldoutStates[label] && value != null)
            {
                EditorGUI.indentLevel++;
                foreach (var nestedField in fieldType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    object nestedValue = nestedField.GetValue(value);
                    object newNestedValue = DrawFieldRecursive(nestedField.Name, nestedField.FieldType, nestedValue, value, nestedField);
                    
                    if (newNestedValue != nestedValue)
                    {
                        nestedField.SetValue(value, newNestedValue);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.LabelField(label, $"Type not supported for GUI: {fieldType.Name}");
        }
        
        return value; 
    }
    private bool IsUnityBasicType(Type type)
    {
        return type == typeof(int) || type == typeof(float) || type == typeof(string) || type == typeof(bool) ||
               type == typeof(Vector2) || type == typeof(Vector3);
    }
    
    private object DrawElementGUI(Type elementType, string label, object value)
    {
        if (elementType == typeof(int)) return EditorGUILayout.IntField(label, (int)(value ?? 0));
        if (elementType == typeof(float)) return EditorGUILayout.FloatField(label, (float)(value ?? 0f));
        if (elementType == typeof(string)) return EditorGUILayout.TextField(label, (string)(value ?? ""));
        if (elementType == typeof(bool)) return EditorGUILayout.Toggle(label, (bool)(value ?? false));
        if (elementType == typeof(Vector2)) return EditorGUILayout.Vector2Field(label, (Vector2)(value ?? Vector2.zero));
        if (elementType == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, (Vector3)(value ?? Vector3.zero));
        
        return value;
    }

    private void DrawArrayOrListOrMultiArray(string label, Type fieldType, IEnumerable enumerable, object parentObject, FieldInfo fieldInfo)
    {
        if (!foldoutStates.ContainsKey(label)) foldoutStates[label] = false;
     
        if (enumerable == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{label} (Null)", EditorStyles.foldout); 

            if (GUILayout.Button("Создать экземпляр", GUILayout.Width(120)))
            {
                object newInstance = CreateEmptyCollection(fieldType);
                UpdateFieldValue(fieldInfo, parentObject, newInstance); 
            }
            EditorGUILayout.EndHorizontal();
            return; 
        }
        
        List<object> list = enumerable?.Cast<object>().ToList();
        int count = list?.Count ?? 0;
        
        string sizeLabel = (enumerable == null) ? "Null" : $"Count: {count}";
        if (fieldType.IsArray) {
             sizeLabel = (enumerable == null) ? "Null" : $"Rank {fieldType.GetArrayRank()}, Size: {count}";
        }

        foldoutStates[label] = EditorGUILayout.Foldout(foldoutStates[label], $"{label} ({sizeLabel})", true);

        if (foldoutStates[label] && enumerable != null)
        {
            EditorGUI.indentLevel++;
            Type elementType;

            if (fieldType.IsArray)
            {
                elementType = fieldType.GetElementType();
            }
            else 
            {
                elementType = fieldType.GetGenericArguments().FirstOrDefault();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            int newCount = EditorGUILayout.IntField(count);
            
            if (newCount != count && fieldInfo.GetCustomAttribute<ShowStaticFieldAttribute>().isCanChange)
            {
                list = ResizeArrayOrList(list, elementType, newCount, fieldType.IsArray);
                UpdateFieldValue(fieldInfo, parentObject, list, fieldType.IsArray);
            }

            if (GUILayout.Button("Add", GUILayout.Width(60)) && fieldInfo.GetCustomAttribute<ShowStaticFieldAttribute>().isCanChange)
            {
                list.Add(CreateInstance(elementType));
                UpdateFieldValue(fieldInfo, parentObject, list, fieldType.IsArray);
            }
            EditorGUILayout.EndHorizontal();


            if (fieldType.IsArray && fieldType.GetArrayRank() > 1)
            {
                IterateMultiDimensionalArray((Array)enumerable, new int[((Array)enumerable).Rank], 0);
            }
            else 
            {
                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    object elementValue = list[i];
                    object newElementValue = DrawFieldRecursive($"Element {i}", elementType, elementValue, list, null);
                    
                    if (newElementValue != elementValue && fieldInfo.GetCustomAttribute<ShowStaticFieldAttribute>().isCanChange)
                    {
                        list[i] = newElementValue; 
                        UpdateFieldValue(fieldInfo, parentObject, list, fieldType.IsArray); 
                    }
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("-", GUILayout.Width(20)) && fieldInfo.GetCustomAttribute<ShowStaticFieldAttribute>().isCanChange)
                    {
                        list.RemoveAt(i);
                        UpdateFieldValue(fieldInfo, parentObject, list, fieldType.IsArray);
                        break; 
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;
        }
    }
    

    private void UpdateFieldValue(FieldInfo fieldInfo, object parentObject, List<object> newList, bool isArray)
    {
        if (fieldInfo == null) return;

        object finalValueToSet;
    
        if (isArray)
        {
            Type elementType = fieldInfo.FieldType.GetElementType();
            Array newArray = Array.CreateInstance(elementType, newList.Count);
            for (int i = 0; i < newList.Count; i++)
            {
                newArray.SetValue(newList[i], i);
            }
            finalValueToSet = newArray;
        }
        else if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
        {
            Debug.LogError("This UpdateFieldValue overload is not designed for Dictionaries. Use UpdateFieldValue(FieldInfo fieldInfo, object parentObject, object valueToSet) instead.");
            return; 
        }
        else
        {
            Type[] genericArgs = fieldInfo.FieldType.GetGenericArguments();
            Type specificListType = typeof(List<>).MakeGenericType(genericArgs);
            object specificListInstance = Activator.CreateInstance(specificListType);
        
            MethodInfo addMethod = specificListType.GetMethod("Add");

            foreach (object item in newList)
            {
                addMethod.Invoke(specificListInstance, new object[] { item });
            }

            finalValueToSet = specificListInstance;
        }


        UpdateFieldValue(fieldInfo, parentObject, finalValueToSet);
    }
    private void UpdateFieldValue(FieldInfo fieldInfo, object parentObject, object valueToSet)
    {
        if (fieldInfo == null) return;

        if (fieldInfo.IsStatic)
        {
            fieldInfo.SetValue(null, valueToSet);
        }
        else if (parentObject != null)
        {
            fieldInfo.SetValue(parentObject, valueToSet);
        }
        EditorUtility.SetDirty(target); 
    }
    
    
    private List<object> ResizeArrayOrList(List<object> sourceList, Type elementType, int newSize, bool isArray)
    {
        List<object> newList = new List<object>(newSize);
        for (int i = 0; i < newSize; i++)
        {
            if (i < sourceList.Count)
            {
                newList.Add(sourceList[i]);
            }
            else
            {
                newList.Add(CreateInstance(elementType));
            }
        }
        return newList;
    }
    private object CreateInstance(Type type)
    {
        if (type == typeof(string)) return "";
        if (type.IsValueType) return Activator.CreateInstance(type);
        return null; 
    }
    
    private object CreateEmptyCollection(Type collectionType)
    {
        if (collectionType.IsArray)
        {
            // Создаем пустой массив (длиной 0) нужного типа
            Type elementType = collectionType.GetElementType();
            return Array.CreateInstance(elementType, 0);
        }else if (typeof(IDictionary).IsAssignableFrom(collectionType))
        {
            // Создаем экземпляр словаря с помощью конструктора по умолчанию (например, new Dictionary<TKey, TValue>())
            // Это сработает, так как все стандартные дженерик-коллекции имеют конструктор без параметров.
            return Activator.CreateInstance(collectionType);
        }
        else if (typeof(IList).IsAssignableFrom(collectionType))
        {
            // Создаем пустой List<T> нужного типа
            Type listType = typeof(List<>).MakeGenericType(collectionType.GetGenericArguments());
            return Activator.CreateInstance(listType);
        }
        // Можно добавить поддержку для других коллекций (Dictionary, HashSet и т.д.)
        return null;
    }
    
    private void ApplyPendingKeyChange()
    {
        if (_pendingKeyChangeDictionary != null && _pendingKeyChangeOldKey != null && _pendingKeyChangeNewKey != null)
        {
            // Проверяем, существует ли уже новый ключ (избегаем дубликатов)
            if (_pendingKeyChangeDictionary.Contains(_pendingKeyChangeNewKey))
            {
                Debug.LogWarning($"Ключ '{_pendingKeyChangeNewKey}' уже существует в словаре. Изменение отменено.");
            }
            else if (_pendingKeyChangeDictionary.Contains(_pendingKeyChangeOldKey))
            {
                // Получаем старое значение, удаляем старую запись и добавляем новую
                object value = _pendingKeyChangeDictionary[_pendingKeyChangeOldKey];
                _pendingKeyChangeDictionary.Remove(_pendingKeyChangeOldKey);
                _pendingKeyChangeDictionary[_pendingKeyChangeNewKey] = value;
            
                // Сохраняем изменения обратно в поле
                UpdateFieldValue(_pendingKeyChangeFieldInfo, _pendingKeyChangeParentObject, _pendingKeyChangeDictionary);
            }

            // Сбрасываем временные переменные
            _pendingKeyChangeDictionary = null;
            _pendingKeyChangeOldKey = null;
            _pendingKeyChangeNewKey = null;
            _pendingKeyChangeFieldInfo = null;
            _pendingKeyChangeParentObject = null;
        }
    }

     private void IterateMultiDimensionalArray(Array array, int[] indices, int dimension)
    {
        if (dimension == array.Rank)
        {
            object value = array.GetValue(indices);
            Type elementType = array.GetType().GetElementType(); 

            DrawFieldRecursive(string.Join(",", indices), elementType, value, array, null);
            
            return;
        }

        for (int i = array.GetLowerBound(dimension); i <= array.GetUpperBound(dimension); i++)
        {
            indices[dimension] = i;
            IterateMultiDimensionalArray(array, indices, dimension + 1);
        }
    }
     
    private void AddNewDictionaryEntry(IDictionary dictionary, Type keyType, Type valueType, FieldInfo fieldInfo, object parentObject)
    {
        // Создаем экземпляр ключа и значения по умолчанию
        object newKey = CreateInstance(keyType);
        object newValue = CreateInstance(valueType);

        // Убеждаемся, что ключ уникален (особенно если это строка или int по умолчанию)
        int counter = 0;
        while (dictionary.Contains(newKey))
        {
            // Простой способ генерации уникального ключа, если базовый тип поддерживает его
            if (keyType == typeof(string))
            {
                newKey = $"New Key {counter++}";
            }
            else if (keyType == typeof(int))
            {
                newKey = (int)newKey + 1;
            }
            else
            {
                // Для сложных типов просто не добавляем, если ключ по умолчанию уже существует
                Debug.LogWarning("Cannot guarantee unique key for complex type in Dictionary GUI add operation.");
                return;
            }
        }

        dictionary.Add(newKey, newValue);
        UpdateFieldValue(fieldInfo, parentObject, dictionary); // Сохраняем изменения
    }

    private void DrawDictionaryRecursive(string label, IDictionary dictionary, object parentObject, FieldInfo fieldInfo)
    {
        // Убедитесь, что foldoutStates инициализировано для данного label
        if (!foldoutStates.ContainsKey(label)) foldoutStates[label] = false;

        // --- Обработка случая null ---
        // Если словарь равен null, показываем кнопку для создания экземпляра
        if (dictionary == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{label} (Null)", EditorStyles.foldout);
            if (GUILayout.Button("Создать Dictionary", GUILayout.Width(140)))
            {
                // Используем fieldInfo.FieldType, чтобы получить исходный тип поля
                object newInstance = CreateEmptyCollection(fieldInfo.FieldType);
                // Убедитесь, что у вас есть перегрузка UpdateFieldValue(FieldInfo fieldInfo, object parentObject, object valueToSet)
                UpdateFieldValue(fieldInfo, parentObject, newInstance);
            }
            EditorGUILayout.EndHorizontal();
            return; // Выходим из метода, дальше нечего рисовать
        }
        // ----------------------------

        Type[] genericArgs = dictionary.GetType().GetGenericArguments();
        Type keyType = genericArgs[0];
        Type valueType = genericArgs[1];

        foldoutStates[label] = EditorGUILayout.Foldout(foldoutStates[label], $"{label} (Count: {dictionary.Count})", true);

        if (foldoutStates[label])
        {
            EditorGUI.indentLevel++;

            // --- Кнопки управления словарем (Добавить/Очистить/Null) ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Добавить запись", GUILayout.Width(110)))
            {
                // Убедитесь, что AddNewDictionaryEntry принимает правильные параметры
                AddNewDictionaryEntry(dictionary, keyType, valueType, fieldInfo, parentObject);
            }
            if (GUILayout.Button("Очистить", GUILayout.Width(60)))
            {
                dictionary.Clear();
                // Используем UpdateFieldValue для сохранения изменений в поле
                UpdateFieldValue(fieldInfo, parentObject, dictionary);
            }
            if (GUILayout.Button("Null", GUILayout.Width(40)))
            {
                // Устанавливаем значение поля в null
                UpdateFieldValue(fieldInfo, parentObject, null); 
                GUIUtility.ExitGUI(); // Выходим, чтобы изменения применились немедленно
            }
            EditorGUILayout.EndHorizontal();
            // -----------------------------------------------------------

            // Создаем копию ключей для безопасной итерации (позволяет менять словарь внутри цикла)
            List<object> keys = new List<object>(dictionary.Keys.Cast<object>());

            // Сбрасываем флаг изменений перед итерацией, чтобы отловить изменения только внутри цикла
            GUI.changed = false; 

            foreach (object key in keys)
            {
                EditorGUILayout.BeginHorizontal();
                object currentValue = dictionary[key];
                
                // --- Ключ и Значение (мгновенное изменение при вводе) ---
                EditorGUILayout.LabelField("Ключ:", GUILayout.Width(50));
                object newKey = DrawElementGUI(keyType, "", key);

                EditorGUILayout.LabelField("Значение:", GUILayout.Width(70));
                object newValue = DrawElementGUI(valueType, "", currentValue);

                // Если GUI.changed == true, то либо ключ, либо значение изменились в этом кадре GUI
                if (GUI.changed) 
                {
                    if (newKey != null && !newKey.Equals(key))
                    {
                        // Логика изменения ключа
                        if (dictionary.Contains(newKey))
                        {
                            Debug.LogWarning($"Ключ '{newKey}' уже существует. Изменение отменено.");
                        }
                        else
                        {
                            dictionary.Remove(key); 
                            dictionary[newKey] = newValue;
                            UpdateFieldValue(fieldInfo, parentObject, dictionary); 
                            // Так как структура словаря изменилась (ключ заменен),
                            // нужно немедленно выйти из GUI, чтобы избежать ошибок макета в текущем кадре.
                            GUIUtility.ExitGUI(); 
                        }
                    }
                    else if (newValue != null && !newValue.Equals(currentValue))
                    {
                        // Логика изменения только значения
                        dictionary[key] = newValue;
                        UpdateFieldValue(fieldInfo, parentObject, dictionary);
                    }
                    
                    // Сбрасываем флаг, чтобы не повлиять на следующие элементы GUI
                    GUI.changed = false;
                }

                // Кнопка удаления
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    dictionary.Remove(key);
                    UpdateFieldValue(fieldInfo, parentObject, dictionary);
                    GUIUtility.ExitGUI(); // Выход после удаления
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }
}
