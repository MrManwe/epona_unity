using UnityEngine;
using System.Collections;
using System;

namespace epona
{
    public class Identifier<LockClass, T> : IComparable<LockClass> where LockClass : Identifier<LockClass, T> where T : IComparable<T>
    {
        [SerializeField] private T m_value;

        public Identifier(T t)
        {
            m_value = t;
        }

        public void assign(Identifier<LockClass, T> other)
        {
            m_value = other.m_value;
        }

        public bool Equals(Identifier<LockClass, T> other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return m_value.Equals(other.m_value);
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Identifier<LockClass, T> t2 = (Identifier<LockClass, T>)obj;
            return Equals(t2);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public int CompareTo(LockClass other)
        {
            return m_value.CompareTo(other.m_value);
        }

        static public bool operator ==(Identifier<LockClass, T> t1, Identifier<LockClass, T> t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                if (object.ReferenceEquals(t2, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return t1.Equals(t2);
            }

        }

        static public bool operator !=(Identifier<LockClass, T> t1, Identifier<LockClass, T> t2)
        {
            return !(t1 == t2);
        }

        public T value
        {
            get
            {
                return m_value;
            }
        }
    }

}