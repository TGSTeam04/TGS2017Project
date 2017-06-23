using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProcess : MonoBehaviour
{

    public GameObject m_LeftPunch;
    public GameObject m_LeftSwing;
    public GameObject m_RightPunch;
    public GameObject m_RightSwing;

    public GameObject m_LeftArm;
    public GameObject m_RightArm;

    [SerializeField]
    private int m_StopTime = 3;
    private int m_StopCounter;

    public static bool s_Chance = false;

    AudioSource m_Sound;
    Animator m_Anim;

    // Use this for initialization
    void Start()
    {
        s_Chance = false;
        m_Sound = GetComponent<AudioSource>();
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(m_StopCounter);
    }
    void LeftPunchStart()
    {
        m_LeftPunch.SetActive(true);
    }
    void LeftPunchEnd()
    {
        s_Chance = true;
        //m_LeftArm.GetComponent<Separation>().enabled = true;
        m_Sound.Play();
        m_Anim.speed = 0.0f;
        StartCoroutine(Stop());
        m_LeftPunch.SetActive(false);
    }
    void LeftPunchStateEnd()
    {
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
    void RightPunchStart()
    {
        m_RightPunch.SetActive(true);
    }
    void RightPunchEnd()
    {
        s_Chance = true;
        //m_RightArm.GetComponent<Separation>().enabled = true;
        m_Sound.Play();
        m_Anim.speed = 0.0f;
        StartCoroutine(Stop());
        m_RightPunch.SetActive(false);
    }
    void RightPunchStateEnd()
    {
        m_Anim.SetBool("RightPunch", false);
    }
    void RightSwingStart()
    {
        m_RightSwing.SetActive(true);
    }
    void RightSwingEnd()
    {
        m_RightSwing.SetActive(false);
    }
    void RightSwingStateEnd()
    {
        m_Anim.SetBool("RightSwing", false);
    }
    void AttackStart()
    {

    }
    void AttackEnd()
    {
        m_Sound.Play();
    }
    void StateEnd()
    {
        m_Anim.SetBool("BodyAttack", false);
    }
    IEnumerator Stop()
    {
        m_StopCounter = m_StopTime;
        while(m_StopCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_StopCounter--;
        }
        s_Chance = false;
        m_Anim.speed = 1.0f;
    }
}