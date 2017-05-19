using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProcess : MonoBehaviour {

    public GameObject m_LeftPunch;
    public GameObject m_LeftSwing;
    public GameObject m_RightPunch;
    public GameObject m_RightSwing;

    Animator m_Anim;

	// Use this for initialization
	void Start () {
        m_Anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void LeftPunchStart() {
        m_LeftPunch.SetActive(true);
    }
    void LeftPunchEnd() {
        m_LeftPunch.SetActive(false);
    }
    void LeftPunchStateEnd() {
        m_Anim.SetBool("LeftPunch", false);
    }
    void LeftSwingStart()
    {
        m_LeftSwing.SetActive(true);
    }
    void LeftSwingEnd()
    {
        m_LeftSwing.SetActive(false);
    }
    void LeftSwingStateEnd()
    {
        m_Anim.SetBool("LeftSwing", false);
    }
    void RPunchStart() {
        m_RightPunch.SetActive(true);
    }
    void RPunchEnd() {
        m_RightPunch.SetActive(false);
    }
    void RPunchStateEnd() {
        m_Anim.SetBool("RightPunch", false);
    }
    void RSwingStart()
    {
        m_RightSwing.SetActive(true);
    }
    void RSwingEnd()
    {
        m_RightSwing.SetActive(false);
    }
    void RSwingStateEnd()
    {
        m_Anim.SetBool("RightSwing", false);
    }
}
