using UnityEngine;

public class AppPaused : MonoBehaviour
{
    bool isPaused = false;

    void OnGUI()
    {
        if (isPaused){
            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
            Debug.Log("Game paused");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
        Debug.Log("Application focus");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }
}