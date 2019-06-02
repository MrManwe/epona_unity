using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace epona
{
    public interface IModalGameplay
    {
        void Suspend();
        void Resume();
    }

    public abstract class ModalGameplay<Input, Result> : MonoBehaviour, IModalGameplay where Input : ModalMenuInput<Result>
    {
        [SerializeField] Selectable m_initialSelectable;
        ModalYield<Result> m_yield;


        uint m_id;

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
            
            ModalManager manager = ModalManager.hidden_instance;
            ModalMenuInput<Result> data = manager.ExtractData<Result>(this);
            if (data == null)
            {
                data = DefaultInput();
                manager.StartWithGameplay(data, this);
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

        public abstract void Suspend();
        public abstract void Resume();
    }
}