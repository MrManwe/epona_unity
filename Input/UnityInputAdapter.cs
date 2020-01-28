using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace epona
{
    public class UnityInputAdapter : MonoBehaviour, IInput
    {
        Vector2 m_leftThumbstick = Vector2.zero;
        Dictionary<InputKey, bool> m_buttonState;
        // Start is called before the first frame update
        void Awake()
        {
            epona.InputSystem.instance.RegisterInput(PlayerInputId.Player1, this);
            m_buttonState = new Dictionary<InputKey, bool>();
            foreach(InputKey key in Enum.GetValues(typeof(InputKey)))
            {
                m_buttonState.Add(key, false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Move(InputAction.CallbackContext context)
        {
            m_leftThumbstick = context.ReadValue<Vector2>();
        }

        public void OnButtonEast(InputAction.CallbackContext context)
        {
            m_buttonState[InputKey.Button_East] = context.ReadValue<float>() > 0.5f;
        }

        public void OnButtonSouth(InputAction.CallbackContext context)
        {
            m_buttonState[InputKey.Button_South] = context.ReadValue<float>() > 0.5f;
        }

        public bool GetKeyDown(InputKey i_inputKey)
        {
            return m_buttonState[i_inputKey];
        }

        public Vector2 GetLeftAxis()
        {
            return m_leftThumbstick;
        }
    }
}
