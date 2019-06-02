#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

#region Inner-class ReorderableListProperty
class ReorderableListProperty
{
    public AnimBool IsExpanded { get; private set; }

    /// <summary>
    /// ref http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
    /// </summary>
    public ReorderableList List { get; private set; }

    private SerializedProperty _property;
    public SerializedProperty Property
    {
        get { return this._property; }
        set
        {
            this._property = value;
            this.List.serializedProperty = this._property;
        }
    }

    public ReorderableListProperty(SerializedProperty property)
    {
        this.IsExpanded = new AnimBool(property.isExpanded);
        this.IsExpanded.speed = 1f;
        this._property = property;
        this.CreateList();
    }

    ~ReorderableListProperty()
    {
        this._property = null;
        this.List = null;
    }

    private void CreateList()
    {
        bool dragable = true, header = true, add = true, remove = true;
        this.List = new ReorderableList(this.Property.serializedObject, this.Property, dragable, header, add, remove);
        this.List.drawHeaderCallback += rect => EditorGUI.LabelField(rect, this._property.arraySize.ToString() + " elements", EditorStyles.boldLabel);
        this.List.onCanRemoveCallback += (list) => { return this.List.count > 0; };
        this.List.drawElementCallback += this.drawElement;
        this.List.elementHeightCallback += (idx) => { return Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(this._property.GetArrayElementAtIndex(idx), GUIContent.none, true)) + 4.0f; };
    }

    private void drawElement(Rect rect, int index, bool active, bool focused)
    {
        if (this._property.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.Generic)
        {
            EditorGUI.LabelField(rect, this._property.GetArrayElementAtIndex(index).displayName);
        }
        //rect.height = 16;
        rect.height = EditorGUI.GetPropertyHeight(this._property.GetArrayElementAtIndex(index), GUIContent.none, true);
        rect.y += 1;
        EditorGUI.PropertyField(rect, this._property.GetArrayElementAtIndex(index), GUIContent.none, true);
        this.List.elementHeight = rect.height + 4.0f;
    }
}
#endregion

public abstract class Massive<T> : EditorWindow where T : ScriptableObject
{
    static int k_columnWidth = 250;

    List<SerializedObject> objs;
    Vector2 m_scrollPos = Vector2.zero;
    HashSet<string> m_expandedItems;
    bool m_expandAll = false;

    Dictionary<string, ReorderableListProperty> m_propertyLists;
    static protected void Initialize<W>(string i_windowName)
    {
        UnityEditor.EditorWindow window = GetWindow(typeof(W), false, i_windowName);
        window.Show();
    }

    public static void ShowWindow()
    {

    }

    private void OnEnable()
    {
        string[] objsGUID = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
        m_expandedItems = new HashSet<string>();
        m_propertyLists = new Dictionary<string, ReorderableListProperty>();
        int count = objsGUID.Length;
        objs = new List<SerializedObject>();
        for (int i = 0; i < count; i++)
        {
            objs.Add(new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(objsGUID[i]))));
        }
    }

    bool IsExpanded(string i_id)
    {
        if (m_expandedItems.Contains(i_id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetExpanded(bool i_expanded, string i_id)
    {
        if (i_expanded)
        {
            m_expandedItems.Add(i_id);
        }
        else
        {
            m_expandedItems.Remove(i_id);
        }
    }

    ReorderableListProperty ObtainList(SerializedProperty i_property, string i_id)
    {
        string key = i_id + "///" + i_property.name;
        if (m_propertyLists.ContainsKey(key))
        {
            return m_propertyLists[key];
        }
        else
        {
            ReorderableListProperty ret = new ReorderableListProperty(i_property);
            m_propertyLists.Add(key, ret);
            return ret;
        }
    }

    //private static GUIStyle s_TempStyle = new GUIStyle();

    void DrawSprite(Sprite i_sprite)
    {
        if (i_sprite != null)
        {
            Rect c = i_sprite.rect;
            float spriteW = c.width;
            float spriteH = c.height;
            float ratio = spriteW / spriteH;
            if (ratio > 1.0)
            {
                spriteH = 100.0f;
                spriteW = spriteH * ratio;
            }
            else
            {
                spriteH = 100.0f;
                spriteW = spriteH * ratio;
            }
            Rect rect = GUILayoutUtility.GetRect(spriteW, spriteH);
            if (Event.current.type == EventType.Repaint)
            {
                var tex = i_sprite.texture;
                c.xMin /= tex.width;
                c.xMax /= tex.width;
                c.yMin /= tex.height;
                c.yMax /= tex.height;
                Rect drawRect = new Rect(rect.x, rect.y, spriteW, spriteH);
                GUI.DrawTextureWithTexCoords(drawRect, tex, c);
            }
        }
    }

    void DisplaySprite(SerializedProperty i_property, bool i_expanded)
    {
        if (!i_expanded)
        {

            EditorGUILayout.PropertyField(i_property, GUIContent.none, GUILayout.Width(k_columnWidth));
        }
        else
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(k_columnWidth));
            EditorGUILayout.PropertyField(i_property, GUIContent.none, GUILayout.Width(k_columnWidth - 10));
            Sprite sprite = (Sprite)i_property.objectReferenceValue;
            DrawSprite(sprite);
            EditorGUILayout.EndVertical();
        }
    }

    void DisplayReference(SerializedProperty i_property, Type i_type, bool i_expanded)
    {
        if (!i_expanded)
        {
            EditorGUILayout.PropertyField(i_property, GUIContent.none, GUILayout.Width(k_columnWidth));
        }
        else
        {
            if (i_type == typeof(Sprite))
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(k_columnWidth));
                EditorGUILayout.PropertyField(i_property, GUIContent.none, GUILayout.Width(k_columnWidth));
                SerializedProperty prop = i_property.FindPropertyRelative("m_path");
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/" + prop.stringValue);
                DrawSprite(sprite);
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.PropertyField(i_property, GUIContent.none, GUILayout.Width(k_columnWidth));
            }
        }
    }

    void DisplayArray(SerializedProperty i_property, string i_id, bool i_expanded)
    {
        if (!i_expanded)
        {
            string content = "";
            for (int i = 0; i < i_property.arraySize; ++i)
            {
                SerializedProperty property = i_property.GetArrayElementAtIndex(i);
                if (i > 0)
                {
                    content += ", ";
                }
                if (property.type == "string")
                {
                    content += property.stringValue;
                }
                else if (property.type == "int")
                {
                    content += property.intValue.ToString();
                }
                else if (property.type == "PPtr<$Sprite>")
                {
                    content += ((Sprite)property.objectReferenceValue).name;
                }
                else
                {
                    Debug.LogWarning("Cannot handle property of type " + property.type + " in array. Ask a programmer to add it (it is easy)");
                }
            }
            content += "";
            EditorGUILayout.LabelField(content, GUILayout.Width(k_columnWidth));
        }
        else
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(k_columnWidth));
            ReorderableListProperty listData = ObtainList(i_property, i_id);
            listData.List.DoLayoutList();
            EditorGUILayout.EndVertical();
        }
    }

    void OnGUI()
    {
        Type targetType = typeof(T);
        FieldInfo[] fields = targetType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        EditorGUILayout.BeginVertical();

        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

        EditorGUILayout.BeginHorizontal();
        bool expandAllstatus = EditorGUILayout.Toggle(m_expandAll, GUILayout.Width(10));
        if (expandAllstatus != m_expandAll)
        {
            m_expandAll = expandAllstatus;
            if (m_expandAll)
            {
                m_expandedItems.Clear();
                foreach (SerializedObject obj in objs)
                {
                    m_expandedItems.Add(obj.targetObject.name);
                }
            }
            else
            {
                m_expandedItems.Clear();
            }
        }

        EditorGUILayout.LabelField("Id", EditorStyles.boldLabel, GUILayout.Width(k_columnWidth - 10));

        foreach (FieldInfo field in fields)
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), EditorStyles.boldLabel, GUILayout.Width(k_columnWidth));
        }
        EditorGUILayout.EndHorizontal();

        foreach (SerializedObject obj in objs)
        {
            EditorGUILayout.BeginHorizontal();
            bool wasExpanded = IsExpanded(obj.targetObject.name);
            bool isExpanded = EditorGUILayout.Toggle(wasExpanded, GUILayout.Width(10));
            if (wasExpanded != isExpanded)
            {
                SetExpanded(isExpanded, obj.targetObject.name);
            }

            EditorGUILayout.LabelField(obj.targetObject.name, GUILayout.Width(k_columnWidth - 10));
            EditorGUI.BeginChangeCheck();
            foreach (FieldInfo field in fields)
            {
                SerializedProperty property = obj.FindProperty(field.Name);
                if (property.isArray)
                {
                    DisplayArray(property, obj.targetObject.name, isExpanded);
                }
                else if (field.FieldType.BaseType != null && field.FieldType.BaseType.IsGenericType && field.FieldType.BaseType.GetGenericTypeDefinition() == typeof(AssetReference<>))
                {
                    Type[] genericArguments = field.FieldType.BaseType.GetGenericArguments();
                    DisplayReference(property, genericArguments[0], isExpanded);
                }
                else if (field.FieldType == typeof(Sprite))
                {
                    DisplaySprite(property, isExpanded);
                }
                else
                {
                    EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(k_columnWidth));
                }
            }
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                obj.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

}
#endif