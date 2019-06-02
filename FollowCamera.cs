using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    [SerializeField] Transform m_target;
    Vector3 m_delta;
	// Use this for initialization
	void Start ()
    {
        m_delta = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position = Vector3.Slerp(transform.position, m_target.position + m_delta, 12.0f * Time.deltaTime);
	}
}
