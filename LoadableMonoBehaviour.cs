using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{

    public enum LoadMode
    {
        Immediate,
        Smooth
    }

    public abstract class LoadableMonoBehaviour : MonoBehaviour
    {

        LoadMode m_mode = LoadMode.Immediate;
        ILoader m_loader = null;

        public void Configure(LoadMode i_mode, ILoader i_loader)
        {
            m_mode = i_mode;
        }

        protected abstract IEnumerator StartImpl();

        // Use this for initialization
        private IEnumerator Start()
        {
            System.Random loadRand = new System.Random();
            switch (m_mode)
            {
                case LoadMode.Smooth:
                    yield return new WaitForSeconds(((float)loadRand.Next(1000)) / 250.0f);
                    break;
            }
            yield return StartImpl();

            if (m_loader != null)
            {
                m_loader.OnLoaded();
            }
        }

    }
}