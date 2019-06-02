using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{

    public class CoroutineActionProcessor<T>
    {
        public delegate IEnumerator CoroutineAction(T i_data);
        List<CoroutineAction> m_actions = new List<CoroutineAction>();
    
        public IEnumerator Process(T i_data)
        {
            IEnumerable<CoroutineAction> actionsToProcess = m_actions;
            m_actions = new List<CoroutineAction>();
            foreach (CoroutineAction action in actionsToProcess)
            {
                yield return action.Invoke(i_data);
            }
        }

    
        public void Add(CoroutineAction i_action)
        {
            m_actions.Add(i_action);
        }
    }


    
}