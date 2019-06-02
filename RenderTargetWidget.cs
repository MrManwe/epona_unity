using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace epona
{
    [RequireComponent(typeof(RawImage))]
    public class RenderTargetWidget : MonoBehaviour
    {

        [SerializeField] Camera m_camera;
        RawImage m_image;
        // Use this for initialization
        public virtual void Awake()
        {
            m_image = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Start()
        {
            m_image.texture = m_camera.targetTexture;
            m_image.color = Color.white;
        }
    }
}
