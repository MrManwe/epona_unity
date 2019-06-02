using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace epona
{

    public enum GuiYieldState
    {
        IN_PROGRESS,
        FINISHED
    }

    public class ModalYield<Result> : CustomYieldInstruction
    {
        List<Action<Result>> m_callbacks = new List<Action<Result>>();

        GuiYieldState m_state = GuiYieldState.IN_PROGRESS;
        Result m_result;
        public override bool keepWaiting
        {
            get
            {
                return m_state == GuiYieldState.IN_PROGRESS;
            }
        }

        public void MarkAsFinished(Result result)
        {
            m_state = GuiYieldState.FINISHED;
            m_result = result;
            foreach (Action<Result> callback in m_callbacks)
            {
                callback.Invoke(result);
            }
        }


        public Result result
        {
            get
            {
                return m_result;
            }
        }

        public void Listen(Action<Result> callback)
        {
            m_callbacks.Add(callback);
        }


    }

    public interface IModalMenuInput
    {
        uint id { get; }
    }
    public abstract class ModalMenuInput<Result> : IModalMenuInput
    {
        private ModalYield<Result> m_yielder = new ModalYield<Result>();
        private uint m_id = NextId();

        private static uint s_id = 0;

        private static uint NextId()
        {
            s_id++;
            return s_id - 1;
        }

        public ModalYield<Result> yielder
        {
            get
            {
                return m_yielder;
            }
        }

        public uint id
        {
            get
            {
                return m_id;
            }
        }
    }
    public class ModalManager
    {


        static ModalManager s_modalManager;

        public static ModalManager hidden_instance //Do NOT USE. EVER. This is C# stupidity
        {
            get
            {
                if (s_modalManager == null)
                {
                    s_modalManager = new ModalManager();
                }

                return s_modalManager;
            }
        }

        struct ModalInfo
        {
            public Type type;
            public string level;
            public IModalMenuInput modalData;
            public IModalGameplay gameplay;
            public Selectable lastSelectable;

        }

        Stack<ModalInfo> m_levels;




        // Use this for initialization
        public ModalManager()
        {
            m_levels = new Stack<ModalInfo>();
        }

        static Type GetClassType<Result>()
        {
            return typeof(ModalMenuInput<Result>).GetType();
        }

        public ModalYield<Result> LoadMenu<Result>(ModalMenuInput<Result> i_modalData, string i_menuLevel)
        {
            Type t = GetClassType<Result>();
            ModalInfo info = new ModalInfo();
            info.modalData = i_modalData;
            info.level = i_menuLevel;
            info.type = t;
            info.lastSelectable = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            if (m_levels.Count > 0)
            {
                ModalInfo stackedModalGameplay = m_levels.Peek();
                if (stackedModalGameplay.gameplay != null)
                {
                    stackedModalGameplay.gameplay.Suspend();
                }
            }
            m_levels.Push(info);

            SceneManager.LoadScene(i_menuLevel, LoadSceneMode.Additive);
            return i_modalData.yielder;
        }


        public void NotifyModalDone<Result>(Result result, uint i_modalId)
        {
            Type t = GetClassType<Result>();
            ModalInfo info = m_levels.Pop();
            Debug.Assert(info.type == t);
            Debug.Assert(info.modalData.id == i_modalId);
            for (int i = SceneManager.sceneCount - 1; i >= 0; --i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == info.level)
                {
                    SceneManager.UnloadSceneAsync(scene);
                    break;
                }
            }

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(info.lastSelectable.gameObject);
            if (m_levels.Count > 0)
            {
                ModalInfo stackedModalGameplay = m_levels.Peek();
                stackedModalGameplay.lastSelectable = null;
                if (stackedModalGameplay.gameplay != null)
                {
                    stackedModalGameplay.gameplay.Resume();
                }
            }

        }

        public ModalMenuInput<Result> ExtractData<Result>(IModalGameplay i_gameplay = null)
        {
            //Type t = GetClassType<Result>();
            if (m_levels.Count == 0)
            {
                return null;
            }
            ModalInfo info = m_levels.Peek();
            info.gameplay = i_gameplay;
            return info.modalData as ModalMenuInput<Result>;
        }

        public int GetCurrentSortingLayer()
        {
            return 20 + m_levels.Count;
        }

        public void StartWithGameplay<Result>(ModalMenuInput<Result> i_modalData, IModalGameplay i_gameplay)
        {
            Debug.Assert(m_levels.Count == 0);
            Type t = GetClassType<Result>();
            ModalInfo info = new ModalInfo();
            info.modalData = i_modalData;
            info.type = t;
            info.gameplay = i_gameplay;
            m_levels.Push(info);
        }

        /////////////////////////////////////////////////////////////////////////////////

    }
}