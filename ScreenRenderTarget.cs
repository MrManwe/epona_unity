using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{

    [RequireComponent(typeof(Camera))]
    public class ScreenRenderTarget : MonoBehaviour
    {

        RenderTexture m_targetTexture;
        Camera m_camera;
        // Use this for initialization
        void Awake()
        {
            m_camera = GetComponent<Camera>();
            m_targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            m_camera.targetTexture = m_targetTexture;
        }

        private void Start()
        {
        }


    }
}
