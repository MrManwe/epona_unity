using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{
    [RequireComponent(typeof(FocuseableWidget))]
    public class WorldWidget : RenderTargetWidget
    {
        FocuseableWidget m_focuseableWidget;
        public override void Awake()
        {
            base.Awake();
            m_focuseableWidget = GetComponent<FocuseableWidget>();
        }

        public epona.IInput input
        {
            get
            {
                return m_focuseableWidget;
            }
        }
    }

}