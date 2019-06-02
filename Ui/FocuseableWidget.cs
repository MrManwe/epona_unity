using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace epona
{
    class FocuseableWidget : Selectable, epona.IInput
    {
        [SerializeField] epona.PlayerInputId m_playerId = PlayerInputId.Player1;

        bool m_isSelected = false;
        IInput m_input;

        protected override void Start()
        {
            base.Start();
            m_input = InputSystem.instance.GetPlayerInput(m_playerId);
        }

        public bool GetKeyDown(InputKey i_inputKey)
        {
            if (!m_isSelected)
            {
                return false;
            }

            return m_input.GetKeyDown(i_inputKey);
        }

        public Vector2 GetLeftAxis()
        {
            if (!m_isSelected)
            {
                return Vector2.zero;
            }

            return m_input.GetLeftAxis();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            m_isSelected = false;
            base.OnDeselect(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            //StartCoroutine(RenebaleDelayed());
            m_isSelected = true;
            base.OnSelect(eventData);
        }
    }
}