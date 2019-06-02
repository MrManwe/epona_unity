using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace epona
{

    [System.Serializable]
    public class LocalizationId : Identifier<LocalizationId, string> { public LocalizationId(string s) : base(s) { } }



    public struct LanguageChanged
    {

    };

    public class LocalizationManager : IObservable<LanguageChanged>
    {

        Observable<LanguageChanged> m_langChangedObservable = new Observable<LanguageChanged>();

        Localization m_currentLocalization;
        string m_currentLang;
        static LocalizationManager s_localizationManager;

        public static LocalizationManager friend_instance
        {
            get
            {
                if(s_localizationManager == null)
                {
                    s_localizationManager = new LocalizationManager();
                }
                return s_localizationManager;
            }
        }

        public LocalizationManager()
        {
            Debug.Assert(s_localizationManager == null);
            s_localizationManager = this;
        }

        public string currentLanguage
        {
            get
            {
                return m_currentLang;
            }
        }

        public string Get(LocalizationId key)
        {
            if (m_currentLocalization != null)
            {
                return m_currentLocalization.Get(key);
            }
            else
            {
#if UNITY_EDITOR
                return key.value;
#else
            return "";
#endif
            }
        }

        public void LoadLanguage(string langCode)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("texts/texts.lang");
            Dictionary<string, Localization> localizations = LocalizationParser.Load(textAsset);

            //Dictionary<string, texts.Localization> localizations = texts.LocalizationParser.Load(Path.Combine(Application.streamingAssetsPath, "texts/texts.lang.xls"));
            if (localizations.ContainsKey(langCode))
            {
                m_currentLang = langCode;
                m_currentLocalization = localizations[langCode];
                m_langChangedObservable.Notify(new LanguageChanged());
            }
            else
            {
                m_currentLang = "";
                m_currentLocalization = null;
            }
        }

#if UNITY_EDITOR

    static EditorAsset<TextAsset> s_textFile;
    static Dictionary<string, Localization> s_localizations;

    public static Localization GetLocalization(string i_code)
    {
            if (s_textFile == null)
            {
                s_textFile = new EditorAsset<TextAsset>("texts/texts.lang", ".xml");
            }

            if (!s_textFile.IsUpToDate())
            {
                s_localizations = LocalizationParser.Load(s_textFile.LoadAsset());
            }

            if (s_localizations.ContainsKey(i_code))
            {
                return s_localizations[i_code];
            }
            else
            {
                return null;
            }
    }

#endif

        public void AddObserver(IObserver<LanguageChanged> observer)
        {
            m_langChangedObservable.AddObserver(observer);
        }

        public void RemoveObserver(IObserver<LanguageChanged> observer)
        {
            m_langChangedObservable.RemoveObserver(observer);
        }

        public List<LocalizationId> GetKeys()
        {
            return m_currentLocalization.GetKeys();
        }
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(epona.LocalizationId))]
[CanEditMultipleObjects]
public class LocalizationIdEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        epona.Localization loc = epona.LocalizationManager.GetLocalization("es");
        IList<epona.LocalizationId> keys = loc.GetKeys();
        
        string[] options = new string[keys.Count];
        int i = 0;
        foreach(epona.LocalizationId key in keys)
        {
            options[i] = key.value;
            ++i;
        }

        int index = 0;
        bool found = false;
        string value = GetValue(property);
        
        while (!found && index < options.Length)
        {
            if (options[index] == value)
            {
                found = true;
            }
            else
            {
                index++;
            }
        }
        index = EditorGUI.Popup(position, index, options);
        if (index >= options.Length)
        {
            index = 0;
        }
        SetValue(property, options[index]);
    }

    void SetValue(SerializedProperty i_property, string i_value)
    {
        SerializedProperty p = i_property.FindPropertyRelative("m_value");
        p.stringValue = i_value;
    }

    string GetValue(SerializedProperty i_property)
    {
        SerializedProperty p = i_property.FindPropertyRelative("m_value");
        return p.stringValue;
    }

        //public override void OnInspectorGUI()
        //{
        //    _object.Update();
        //    EditorGUILayout.ColorField("Color", Color.red);
        //    EditorGUILayout.LabelField("(Above this object)");
        //
        //}
    }
#endif