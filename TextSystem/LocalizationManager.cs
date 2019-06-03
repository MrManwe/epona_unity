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

    public enum LocalizationCode
    {
        en_EN,
        es_ES
    }

    public struct LanguageChanged
    {

    };

    public class LocalizationManager : IObservable<LanguageChanged>
    {

        Observable<LanguageChanged> m_langChangedObservable = new Observable<LanguageChanged>();

        Localization m_currentLocalization;
        LocalizationCode m_currentLang;
        static LocalizationManager s_localizationManager;

        public static LocalizationManager friend_instance
        {
            get
            {
                if(s_localizationManager == null)
                {
                    s_localizationManager = new LocalizationManager(LocalizationCode.en_EN);
                }
                return s_localizationManager;
            }
        }

        public LocalizationManager()
        {
            Debug.Assert(s_localizationManager == null);
            s_localizationManager = this;
        }

        public LocalizationManager(LocalizationCode i_code)
        {
            Debug.Assert(s_localizationManager == null);
            s_localizationManager = this;
            LoadLanguage(i_code);
        }

        public LocalizationCode currentLanguage
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

        private static Localization LoadLocalization(LocalizationCode i_localization)
        {
            Localization localization = new Localization();
            TextData[] texts = Resources.LoadAll<TextData>("texts/");
            foreach (TextData text in texts)
            {
                localization.Set(text.id, text.get(i_localization));
            }

            return localization;
        }

        public void LoadLanguage(LocalizationCode i_localization)
        {
            Localization localization = LoadLocalization(i_localization);

            m_currentLang = i_localization;
            m_currentLocalization = localization;
            m_langChangedObservable.Notify(new LanguageChanged());
        }

        //Deprecated
        //public void LoadLanguageFromExcel(string langCode)
        //{
        //    TextAsset textAsset = (TextAsset)Resources.Load("texts/texts.lang");
        //    Dictionary<string, Localization> localizations = LocalizationParser.Load(textAsset);
        //
        //    //Dictionary<string, texts.Localization> localizations = texts.LocalizationParser.Load(Path.Combine(Application.streamingAssetsPath, "texts/texts.lang.xls"));
        //    if (localizations.ContainsKey(langCode))
        //    {
        //        m_currentLang = langCode;
        //        m_currentLocalization = localizations[langCode];
        //        m_langChangedObservable.Notify(new LanguageChanged());
        //    }
        //    else
        //    {
        //        m_currentLang = "";
        //        m_currentLocalization = null;
        //    }
        //}

#if UNITY_EDITOR

    //static EditorAsset<TextAsset> s_textFile;
    //static Dictionary<LocalizationCode, Localization> s_localizations;

    public static Localization GetLocalization(LocalizationCode i_code)
    {
            return LoadLocalization(i_code);
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
        epona.Localization loc = epona.LocalizationManager.GetLocalization(epona.LocalizationCode.en_EN);
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
        
        //if (GUILayout.Button("Refresh"))
        //{
        //
        //}
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