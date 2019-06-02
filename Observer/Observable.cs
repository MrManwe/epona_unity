using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace epona
{

    public class Observable<T> : IObservable<T>
    {

        private Dictionary<IObserver<T>, bool> m_observers;

        public Observable()
        {
            m_observers = new Dictionary<IObserver<T>, bool>();
        }

        public void AddObserver(IObserver<T> observer)
        {
            if (!m_observers.ContainsKey(observer))
            {
                m_observers.Add(observer, true);
            }
        }

        public void RemoveObserver(IObserver<T> observer)
        {
            m_observers.Remove(observer);
        }

        public void Notify(T data)
        {
            Dictionary<IObserver<T>, bool> objervers = new Dictionary<IObserver<T>, bool>(m_observers);
            foreach (KeyValuePair<IObserver<T>, bool> entry in objervers)
            {
                entry.Key.OnEvent(data);
            }
        }

    }
}