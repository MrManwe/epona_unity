using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{
    public interface IPoolElement
    {
        int poolId { get; set; }
    }

    [System.Serializable]
    public class SerializablePool<T>: System.Object where T : class, IPoolElement, new()
    {
        [SerializeField] T[] m_elements;

        public SerializablePool()
        {
            m_elements = new T[0];
        }

        public int size
        {
            get
            {
                return m_elements.Length;
            }
        }

        public void Set(int i_poolId, T i_element)
        {
            Resize(i_poolId + 1);
            m_elements[i_poolId] = i_element;
        }

        public T Get(int i_poolId)
        {
            if (m_elements.Length > i_poolId)
            {
                return m_elements[i_poolId];
            }
            else
            {
                return null;
            }
        }

        public void Delete(int i_poolId)
        {
            if (i_poolId < m_elements.Length)
            {
                m_elements[i_poolId] = null;
            }
        }

        public void Delete(T i_t)
        {
            Delete(i_t.poolId);
        }

        public T create()
        {
            //Look for empty dialog
            for (int i = 0; i < m_elements.Length; ++i)
            {
                if (m_elements[i] == null)
                {
                    m_elements[i] = new T();
                    m_elements[i].poolId = i;
                    return m_elements[i];
                }
            }
            Resize(m_elements.Length + 1);
            m_elements[m_elements.Length - 1] = new T();
            m_elements[m_elements.Length - 1].poolId = m_elements.Length - 1;
            return m_elements[m_elements.Length - 1];
        }

        private void Resize(int Size)
        {
            if (m_elements.Length >= Size)
            {
                return;
            }

            T[] temp = new T[Size];
            for (int c = 0; c < Mathf.Min(Size, m_elements.Length); c++)
            {
                temp[c] = m_elements[c];
            }
            m_elements = temp;
        }
    }
}