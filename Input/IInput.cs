using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{
    public enum InputKey
    {
        Button_South, //X in Playstation, A in Xbox, B in Nintendo
        Button_East, //O in Playstation, B in Xbox, A in Nintendo
    }

    public interface IInput
    {
        bool GetKeyDown(InputKey i_inputKey);
        Vector2 GetLeftAxis();
    }
}
