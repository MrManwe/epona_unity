using UnityEngine;
using System.Collections;

namespace epona
{

    public interface IObservable<T>
    {

        void AddObserver(IObserver<T> observer);

        void RemoveObserver(IObserver<T> observer);
    }
}