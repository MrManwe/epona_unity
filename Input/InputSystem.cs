using UnityEngine;

namespace epona
{
    public enum PlayerInputId
    {
        Player1
    }

    interface IInputSystem
    {
        IInput GetPlayerInput(PlayerInputId i_playerIndex);

        void RegisterInput(PlayerInputId i_playerIndex, IInput i_input);
    }

    class InputSystem: IInputSystem
    {
        public IInput GetPlayerInput(PlayerInputId i_playerIndex)
        {
            switch(i_playerIndex)
            {
                case PlayerInputId.Player1:
                    Debug.Assert(m_player1 != null);
                    return m_player1;
            }
            return null;
        }

        class Player1Input : IInput
        {
            static string k_fire1 = "Fire1";
            static string k_fire2 = "Fire2";
            static string k_thumbstickVertical = "Vertical";
            static string k_thumbstickHorizontal = "Horizontal";

            public bool GetKeyDown(InputKey i_inputKey)
            {
                switch (i_inputKey)
                {
                    case InputKey.Button_South:
                        return Input.GetButton(k_fire1);
                    case InputKey.Button_East:
                        return Input.GetButton(k_fire2);
                }

                return false;
            }

            public Vector2 GetLeftAxis()
            {
                Vector2 direction = Vector2.zero;

                direction = Input.GetAxis(k_thumbstickVertical) * Vector2.up + Input.GetAxis(k_thumbstickHorizontal) * Vector2.right;

                return direction;
            }
        }

        class InputWrapper : IInput
        {
            IInput m_input;

            public InputWrapper(IInput i_input)
            {
                m_input = i_input;
            }

            public void SetInput(IInput i_input)
            {
                m_input = i_input;
            }

            public bool GetKeyDown(InputKey i_inputKey)
            {
                return m_input.GetKeyDown(i_inputKey);
            }

            public Vector2 GetLeftAxis()
            {
                return m_input.GetLeftAxis();
            }
        }

        static InputSystem s_inputSystem;

        public static IInputSystem instance
        {
            get
            {
                if (s_inputSystem == null)
                {
                    s_inputSystem = new InputSystem();
                }
                return s_inputSystem;
            }
        }

        public InputSystem()
        {
            
        }

        public void RegisterInput(PlayerInputId i_playerIndex, IInput i_input)
        {
            m_player1 = i_input;
        }

        IInput m_player1;
        
    }
}