using System;
using System.Collections;
using System.Collections.Generic;

namespace epona
{
    public struct Delayer<T> where T : IComparable
    {
        float m_ellapsedSeconds;
        float m_delaySeconds;
        T m_value;
        T m_targetValue;

        public Delayer(T i_initialValue, float i_delaySeconds)
        {
            m_ellapsedSeconds = 0;
            m_delaySeconds = i_delaySeconds;
            m_value = i_initialValue;
            m_targetValue = i_initialValue;
        }

        public void SetValueDelayed(T i_value)
        {
            if (m_targetValue.CompareTo(i_value) != 0)
            {
                m_targetValue = i_value;
                m_ellapsedSeconds = 0;
            }
        }

        public void SetValueImmediately(T i_value)
        {
            m_targetValue = i_value;
            m_ellapsedSeconds = 0;
            m_value = i_value;
        }

        public T currentValue
        {
            get
            {
                return m_value;
            }
        }

        public void Update (float i_deltaSeconds)
        {
            if (m_value.CompareTo(m_targetValue) != 0)
            {
                m_ellapsedSeconds += i_deltaSeconds;
                if (m_ellapsedSeconds >= m_delaySeconds)
                {
                    m_value = m_targetValue;
                }
            }
        }

        public void Reset()
        {
            m_ellapsedSeconds = 0.0f;
        }
    }
}