using System.Collections;
using System.Collections.Generic;

namespace epona
{
    public class Localization
    {
        private Dictionary<LocalizationId, string> m_texts;

        public Localization()
        {
            m_texts = new Dictionary<LocalizationId, string>();
        }

        public void Set(LocalizationId key, string value)
        {
            if (m_texts.ContainsKey(key))
            {
                m_texts[key] = value;
            }
            else
            {
                m_texts.Add(key, value);
            }
        }

        public string Get(LocalizationId key)
        {
            if (m_texts.ContainsKey(key))
            {
                return m_texts[key];
            }
            else
            {
                return "";
            }
        }

        public List<LocalizationId> GetKeys()
        {
            List<LocalizationId> keys = new List<LocalizationId>(m_texts.Keys);
            keys.Sort();
            return keys;
        }

    }
}
