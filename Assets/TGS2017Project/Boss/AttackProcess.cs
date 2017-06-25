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
    public GameObject m_LeftArmPrefab;
    public GameObject m_RightArmPrefab;

    Transform m_Target;
    Vector3 m_TargetPosition;
    Transform m_Origin;
    [SerializeField]
    private float m_ArmSpeed;

    [SerializeField]
    private float m_SeparationTime = 10.0f;

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

    }
    void LeftPunchStart()
    {
        m_LeftPunch.SetActive(true);
    }
    void LeftPunchEnd()
    {
        s_Chance = true;
        m_LeftArmPrefab.SetActive(true);
        m_LeftArm.SetActive(false);
        m_Anim.speed = 0.0f;
        m_Sound.Play();
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
        m_RightArmPrefab.SetActive(true);
        m_RightArm.SetActive(false);
        m_Anim.speed = 0.0f;
        m_Sound.Play();
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
}