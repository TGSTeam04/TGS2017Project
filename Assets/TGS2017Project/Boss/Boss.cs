using System.Collections;
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
    private int m_LookFrequency = 2;

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

    Transform m_Target;
    Vector3 m_TargetPosition;
    Animator m_Anim;

    public static BossState s_State = BossState.Move;
    private static float s_MaxHp = 100.0f;
    private static float s_HitPoint;
    public static float HitPoint
    {
        get { return s_HitPoint; }
        set
        {
            s_HitPoint = Mathf.Max(0, value);
            GameManager.Instance.m_BossHpRate = (HitPoint / s_MaxHp);
        }
    }

    private void Awake()
    {
        s_HitPoint = s_MaxHp;
        GameManager.Instance.m_BossHpRate = 1.0f;
        GameManager.s_StageNumber = 1;
        m_Damage = GetComponent<Damageable>();
        m_Damage.Del_ReciveDamage = Damage;
    }

    //ダメージコンポーネントのダメージ
    private void Damage(float damage, MonoBehaviour src)
    {
        HitPoint -= damage;
        if (HitPoint <= 0)
        {
            s_State = BossState.Paralysis;
            Instantiate(m_LastExplosion, transform.position, transform.rotation);
            Pauser.Pause(PauseTag.Enemy);
            GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
            StartCoroutine(this.Delay(new WaitForSeconds(4.0f), Dead));
        }
    }

    // Use this for initialization
    void Start()
    {
        s_State = BossState.Move;
        m_LookCounter = 0;
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(s_State);
        //m_HitPointBar.fillAmount = s_Hitpoint;
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                GameObject L = GameManager.Instance.m_LRobot;
                GameObject R = GameManager.Instance.m_RRobot;
                float LDistance = Vector3.Distance(transform.position, L.transform.position);
                float RDistance = Vector3.Distance(transform.position, R.transform.position);
                if (LDistance <= RDistance)
                {
                    m_Target = L.transform;
                }
                else
                {
                    m_Target = R.transform;
                }
                break;
            case PlayMode.HumanoidRobot:
                m_Target = GameManager.Instance.m_HumanoidRobot.transform;
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
        //print("Chance:" + AttackProcess.s_Chance);
        switch (s_State)
        {
            case BossState.Move:
                if (!AttackProcess.s_Chance)
                {
                    m_Anim.speed = 1.0f;
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 7)
                    {
                        transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
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
                if (Vector3.Distance(transform.position, m_TargetPosition) > 7)
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
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 7)
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
                    if (Vector3.Distance(transform.position, m_TargetPosition) > 7)
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
        GameManager.Instance.m_IsGameClear = true;
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
        Instantiate(m_LastExplosion, transform.position, transform.rotation);
        yield return new WaitForSeconds(4.0f);
        Dead();
    }
}
