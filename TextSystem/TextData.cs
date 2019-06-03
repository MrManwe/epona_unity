using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{

    [CreateAssetMenu(fileName = "Text Entry", menuName = "Text Entry", order = 1)]
    public class TextData : ScriptableObject
    {
        [SerializeField] string m_english;
        [SerializeField] string m_spanish;
        //[SerializeField] string m_catalan;
        //[SerializeField] string m_french;
        //[SerializeField] string m_german;

        LocalizationId m_id = null;

        public void Awake()
        {
            Debug.Assert(m_english.Length > 0, "Uninitialized main language translation [english]");
           
        }

        public string english
        {
            get
            {
                return m_english;
            }
        }

        public string spanish
        {
            get
            {
                return m_spanish;
            }
        }

        public string get(LocalizationCode i_localization)
        {
            switch(i_localization)
            {
                case LocalizationCode.en_EN:
                    return m_english;
                case LocalizationCode.es_ES:
                    return m_spanish;
            }
            return "";
        }

        public LocalizationId id
        {
            get
            {
                if (m_id == null || m_id.value.Length == 0)
                {
                    m_id = new LocalizationId(name);
                }
                return m_id;
            }
        }
    }


#if UNITY_EDITOR

    class MassiveTextDataEditor : epona.Massive<TextData>
    {
        [UnityEditor.MenuItem("Epona/Texts")]
        public static void Open()
        {
            Initialize<MassiveTextDataEditor>("Texts");
        }
    }

#endif
}