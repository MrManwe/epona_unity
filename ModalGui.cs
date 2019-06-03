using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace epona
{

    [RequireComponent(typeof(Canvas))]
    public abstract class ModalMenu<Input, Result> : MonoBehaviour where Input : ModalMenuInput<Result>
    {
        [SerializeField] Selectable m_initialSelectable;
        ModalYield<Result> m_yield;

        // Use this for initialization
        string m_level;
        uint m_id;
        Canvas m_canvas;
        protected static ModalYield<Result> Show(Input i_input, string i_level)
        {
            return ModalManager.hidden_instance.LoadMenu(i_input, i_level);
        }

        protected abstract void Initialize(Input i_data);
        protected void NotifyDone(ModalManager manager, Result result)
        {
            manager.NotifyModalDone(result, m_id);
        }

        protected abstract Input DefaultInput();


        void Start()
        {
            m_canvas = GetComponent<Canvas>();
            ModalManager manager = ModalManager.hidden_instance;
            ModalMenuInput<Result> data = manager.ExtractData<Result>();
            if (data == null)
            {
                data = DefaultInput();
            }
            m_canvas.sortingOrder = manager.GetCurrentSortingLayer();
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject ev = new GameObject("tmp_EventSystem");
                ev.AddComponent<UnityEngine.EventSystems.EventSystem>();
            }
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_initialSelectable.gameObject);
            Initialize(data as Input);
            m_yield = data.yielder;
            m_id = data.id;
        }

        public void SetRespone(Result result)
        {
            if (m_yield != null)
            {
                m_yield.MarkAsFinished(result);
            }
            ModalManager manager = ModalManager.hidden_instance;
            NotifyDone(manager, result);
        }

        private void Update()
        {
            
        }
    }
}