using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class AutoSelected : MonoBehaviour {

    Selectable m_selectable;
    private void Awake()
    {
        m_selectable = GetComponent<Selectable>();
    }
    // Use this for initialization
    void Start ()
    {
        m_selectable.Select();
	}
	
}



