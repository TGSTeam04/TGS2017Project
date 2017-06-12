﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public enum BossState
    {
        Move,
        Attack,
        Look,
        Paralysis,
        Invincible
    }

    [SerializeField]
    private float m_MoveSpeed = 20.0f;
    [SerializeField]
    private float m_RotateSpeed = 30.0f;
    [SerializeField]
    private int m_Interval = 10;
    [SerializeField]
    private int m_LookFrequency = 3;

    private int m_AttackCounter;
    private int m_LookCounter;

    private bool m_SwingAttack = false;

    public static bool s_IsDead = false;

    public GameObject m_LeftArm;
    public GameObject m_RightArm;
    public Transform m_Core;
    public GameObject m_SearchArea;
    public GameObject m_Explosion;
    public GameObject m_LastExplosion;
    //public Image m_HitPointBar;
    //ダメージコンポーネント
    private Damageable m_Damage;
    [SerializeField] private float m_MaxHp;
    public static float s_HitPoint = 1.0f;

    Transform m_Target;
    Vector3 m_TargetPosition;
    Animator m_Anim;


    public static BossState s_State = BossState.Move;

    private void Awake()
    {
        s_HitPoint = m_MaxHp;
        m_Damage = GetComponent<Damageable>();
        m_Damage.Event_Damaged = Damage;
    }

    //ダメージコンポーネントのダメージ
    private void Damage(float damage, MonoBehaviour src)
    {
        s_HitPoint -= damage;
        GameManager.Instance.m_BossHpRate = (s_HitPoint / m_MaxHp);
    }

    // Use this for initialization
    void Start()
    {
        s_HitPoint = 1.0f;
        s_State = BossState.Move;
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        //m_HitPointBar.fillAmount = s_Hitpoint;
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                m_Target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
            case PlayMode.HumanoidRobot:
                m_Target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
            case PlayMode.NoPlay:
            case PlayMode.Combine:
            case PlayMode.Release:
            default:
                return;
        }
        if (m_LookCounter <= 0)
        {
            StartCoroutine(Look());
        }
        m_TargetPosition = m_Target.position;
        m_TargetPosition.y = transform.position.y;

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    m_State = BossState.Paralysis;
        //}

        switch (s_State)
        {
            case BossState.Move:
                if (!AttackProcess.s_Chance)
                {
                    m_Anim.speed = 1.0f;
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 10)
                    {
                        transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
                        //transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;

                    }
                    else
                    {
                        s_State = BossState.Attack;
                    }
                }
                break;
            case BossState.Attack:
                if (!AttackProcess.s_Chance) m_Anim.speed = 1.0f;
                Attack();
                if (Vector3.Distance(transform.position, m_TargetPosition) > 10)
                {
                    s_State = BossState.Move;
                }
                break;
            case BossState.Look:
                float angle = Vector3.Angle(transform.forward, m_TargetPosition - transform.position);
                if (!AttackProcess.s_Chance)
                {
                    m_Anim.speed = 1.0f;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
                }
                if (angle < 15)
                {
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 10)
                    {
                        s_State = BossState.Move;
                    }
                    else
                    {
                        s_State = BossState.Attack;
                    }
                }
                break;
            case BossState.Paralysis:
                m_Anim.speed = 0f;
                break;
            case BossState.Invincible:
                angle = Vector3.Angle(transform.forward, m_TargetPosition - transform.position);
                if (!AttackProcess.s_Chance)
                {
                    m_Anim.speed = 1.0f;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
                }
                if (angle < 15)
                {
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 10)
                    {
                        s_State = BossState.Move;
                    }
                    else
                    {
                        s_State = BossState.Attack;
                    }
                }
                break;
        }
    }
    void Attack()
    {
        if (Searching.s_Find)
        {
            m_SearchArea.SetActive(false);
            if (m_LeftArm.activeSelf == true && m_RightArm.activeSelf == true)
            {
                if (Vector3.Distance(m_LeftArm.transform.position, m_Target.position) < Vector3.Distance(m_RightArm.transform.position, m_Target.position))
                {
                    m_Anim.SetBool("LeftPunch", true);
                }
                else
                {
                    m_Anim.SetBool("RightPunch", true);
                }
            }
            else
            {
                if (m_LeftArm.activeSelf == false && m_RightArm.activeSelf == true) m_Anim.SetBool("RightPunch", true);
                else if (m_RightArm.activeSelf == false && m_LeftArm.activeSelf == true) m_Anim.SetBool("LeftPunch", true);
                else m_Anim.SetBool("BodyAttack", true);
            }
            Searching.s_Find = false;
            //m_SwingAttack = true;
            StartCoroutine(AttackInterval());
        }

        //if (m_AttackCounter > 0 && m_AttackCounter < 8 && m_SwingAttack)
        //{
        //    if (Vector3.Distance(m_SearchArea.transform.position, m_Target.position) > 20)
        //    {
        //        m_SwingAttack = false;
        //        if (Random.Range(0, 200) > 100)
        //        {
        //            m_Anim.SetBool("LeftSwing", true);
        //        }
        //        else
        //        {
        //            m_Anim.SetBool("RightSwing", true);
        //        }
        //    }
        //}

    }
    void Dead()
    {
        GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
        GameManager.Instance.m_GameStarter.ChangeScenes(8);
        Destroy(gameObject);
    }
    IEnumerator AttackInterval()
    {
        m_AttackCounter = m_Interval;
        while (m_AttackCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_AttackCounter--;
        }
        m_SearchArea.SetActive(true);
    }
    IEnumerator Look()
    {
        m_LookCounter = m_LookFrequency;
        while (m_LookCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_LookCounter--;
        }
        if (s_State != BossState.Paralysis)
        {
            s_State = BossState.Look;
        }
    }
    IEnumerator Recovery()
    {
        yield return new WaitForSeconds(10.0f);
        s_State = BossState.Look;
        m_SearchArea.SetActive(true);
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(4.0f);
        Dead();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Beam")
        {
            m_SearchArea.SetActive(false);
            s_State = BossState.Paralysis;
            StartCoroutine(Recovery());
        }
        if (GameManager.Instance.m_PlayMode == PlayMode.Combine && other.name == "Break" && s_State != BossState.Invincible)
        {
            if (m_LeftArm.activeSelf == false && m_RightArm.activeSelf == false)
            {
                Instantiate(m_Explosion, transform.position, transform.rotation);
                s_HitPoint -= 0.25f;
                s_State = BossState.Invincible;
            }
            else
            {
                Instantiate(m_Explosion, transform.position, transform.rotation);
                s_HitPoint -= 0.05f;
                s_State = BossState.Invincible;
            }

            if (s_HitPoint <= 0.0f)
            {
                s_State = BossState.Paralysis;
                Instantiate(m_LastExplosion, transform.position, transform.rotation);
                StartCoroutine(Death());
            }
        }

    }
}
