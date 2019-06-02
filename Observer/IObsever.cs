using UnityEngine;
using System.Collections;
namespace epona
{
    public interface IObserver<T>
    {
        void OnEvent(T data);
    }
}