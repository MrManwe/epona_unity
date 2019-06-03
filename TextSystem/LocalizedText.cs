using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace epona
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour, IObserver<LanguageChanged>
    {
        [SerializeField] LocalizationId m_id;
        object[] m_args = new object[0];

        private void Start()
        {
            Text text = GetComponent<Text>();
            try
            {
                text.text = String.Format(LocalizationManager.friend_instance.Get(m_id), m_args);
            }
            catch (FormatException fe)
            {
                text.text = LocalizationManager.friend_instance.Get(m_id);
            }
            LocalizationManager.friend_instance.AddObserver(this);
        }

        private void OnDestroy()
        {
            LocalizationManager.friend_instance.RemoveObserver(this);
        }

        public void UpdateText(string _key, params object[] i_args)
        {
            m_id = new LocalizationId(_key);
            m_args = i_args;
            Text text = GetComponent<Text>();
            text.text = String.Format(LocalizationManager.friend_instance.Get(m_id), i_args);
        }

        public void UpdateText(epona.LocalizationId _key, params object[] i_args)
        {
            m_id = _key;
            m_args = i_args;
            Text text = GetComponent<Text>();
            string raw = LocalizationManager.friend_instance.Get(m_id);
            text.text = String.Format(raw, i_args);
        }

        public void OnEvent(LanguageChanged data)
        {
            Text text = GetComponent<Text>();
            text.text = String.Format(LocalizationManager.friend_instance.Get(m_id), m_args);
        }

        public epona.LocalizationId key
        {
            get
            {
                return m_id;
            }
            set
            {
                UpdateText(value);
            }
        }
    }
}