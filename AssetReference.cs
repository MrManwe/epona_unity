using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AssetReference<T> where T: UnityEngine.Object
{
    [SerializeField] private string m_path;

    public T asset
    {
        get
        {
            if (m_path != "")
            {
                return (T)Resources.Load<T>(m_path);

            }
            else
            {
                return null;
            }
        }
    }

}

[System.Serializable]
public class SpriteRef : AssetReference<Sprite> { }


#if UNITY_EDITOR
class AssetReferenceEditor<T>: UnityEditor.PropertyDrawer where T : UnityEngine.Object
{
    //T regexAttribute { get { return ((T)attribute); } }

    public override void OnGUI(Rect position, SerializedProperty root, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        SerializedProperty prop = root.FindPropertyRelative("m_path");
        if (prop == null)
        {
            //wtf?
            Debug.Log("Null path!");
        }

        T target = AssetDatabase.LoadAssetAtPath<T>("Assets/Resources/" + prop.stringValue);

        //public static UnityEngine.Object ObjectField(Rect position, GUIContent label, UnityEngine.Object obj, Type objType, bool allowSceneObjects);
        T newItem = (T)EditorGUI.ObjectField(position, label, target, typeof(T), false);
        if (newItem != null)
        {
            string path = AssetDatabase.GetAssetPath(newItem);
            string resourcesPath = "Assets/Resources/";
            if (path.StartsWith(resourcesPath))
            {
                path = path.Substring(resourcesPath.Length);
                prop.stringValue = path;
            }
            else
            {
                EditorUtility.DisplayDialog("Warning!", "The provided Resources is not located in the resources folder.", "Ok");
            }
        }
        else if (prop.stringValue != "")
        {
            prop.stringValue = "";
        }
        
    }
}
[UnityEditor.CustomPropertyDrawer(typeof(SpriteRef))]
class SpriteRefEditor : AssetReferenceEditor<Sprite> { }


#endif