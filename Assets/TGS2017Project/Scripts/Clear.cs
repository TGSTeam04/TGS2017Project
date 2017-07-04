using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour {

    Animator m_Animator;

	// Use this for initialization
	void Start () {
        m_Animator.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update() {

    }
    public void ClearMove()
    {
        m_Animator.SetBool("Clear", true);
    }
    void CameraEnd()
    {
        m_Animator.SetBool("Clear", false);
        GameManager.Instance.m_IsGameClear = true;
    }
}
