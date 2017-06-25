using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Controller : MonoBehaviour
{
    public float m_MaxHp;
    public GameObject m_TwinRobot;
    public Boss3_Twin m_LRobot;
    public Boss3_Twin m_RRobot;
    public Boss3_Humanoid m_HRobot;
    [SerializeField] float m_ReleaseTime;
    [SerializeField] AnimationClip m_CombineAnim;
    [SerializeField] GameObject m_CombineEffect;
    [SerializeField] Animator m_HAnimator;
    [SerializeField] AudioClip m_SEKimepo;
    [SerializeField] AudioSource m_EffectAudioSrc;
    [SerializeField] AnimationClip m_ReleaseAnim;

    //野沢君のコンバイン、リリースで必要な変数
    private Rigidbody m_LRobotRigidbody;
    private Rigidbody m_RRobotRigidbody;
    //[SerializeField] GameObject m_KeepEnemyPosWall;
    [SerializeField] AnimationCurve m_RotationCurve;
    [SerializeField] AnimationCurve m_ReleaseCurve;

    private PlayMode m_State;
    private float m_StateTimer;
    private float m_Hp;

    //合体完了時イベント        
    //public UnityAction m_CombineEnd;
    //public UnityEvent m_CombineEnd;

    //プレイヤーと共有するとき使う
    //各イベント
    //public UnityEvent m_ReleaseStart;
    //public UnityEvent m_ReleaseEnd;
    //public UnityEvent m_CombineStart;
    //public UnityEvent m_CombineEnd;
    //public UnityEvent<List<Collider>> m_CombineCollide;
    //操作等の行動を無効化するためのスクリプトの参照
    //public MonoBehaviour m_Controller;

    public float Hp
    {
        get { return m_Hp; }
        set
        {
            m_Hp = value;
            //Debug.Log("残りHP" + value);
            if (Hp <= 0)
            {
                Dead();
            }
            GameManager.Instance.m_BossHpRate = m_Hp / m_MaxHp;
        }
    }

    private void Awake()
    {
        GameManager.Instance.m_BossHpRate = 1.0f;
        m_Hp = m_MaxHp;
        m_State = PlayMode.HumanoidRobot;

        m_LRobotRigidbody = m_LRobot.GetComponent<Rigidbody>();
        m_RRobotRigidbody = m_RRobot.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ReleaseStart();

        if (GameManager.Instance.m_PlayMode == PlayMode.NoPlay)
            return;

        m_StateTimer += Time.deltaTime;
        switch (m_State)
        {
            case PlayMode.HumanoidRobot:
                m_HRobot.BossUpdate();
                break;
            case PlayMode.Release:
                if (m_StateTimer > m_ReleaseTime)
                    CombineStart();
                break;
            default:
                break;
        }
    }

    public void Dead()
    {
        if (m_State == PlayMode.HumanoidRobot)
            m_HRobot.Dead();
        else
            m_LRobot.Dead(); m_RRobot.Dead();

        m_State = PlayMode.NoPlay;
        GameManager.Instance.m_IsGameClear = true;
    }

    public void CombineStart()
    {
        StartCoroutine(Combine());
    }
    public void ReleaseStart()
    {
        if (m_Hp > 0)
            StartCoroutine(Release());
    }

    private IEnumerator Combine()
    {
        m_LRobot.SetShieldActive(false);
        m_RRobot.SetShieldActive(false);
        //m_KeepEnemyPosWall.SetActive(true);
        Vector3 StartPositionL = m_LRobotRigidbody.position;
        Vector3 StartPositionR = m_RRobotRigidbody.position;
        Vector3 Direction = Vector3.Normalize(StartPositionL - StartPositionR);
        Vector3 CenterPosition = Vector3.Lerp(StartPositionL, StartPositionR, 0.5f);
        //Vector3 EndPositionL = CenterPosition + Direction * 1.0f;
        //Vector3 EndPositionR = CenterPosition - Direction * 1.0f;
        m_HRobot.transform.position = CenterPosition;
        m_HRobot.transform.LookAt(CenterPosition + Vector3.Cross(-Direction, Vector3.up));

        //float distaceRate = 1.5f;
        m_CombineEffect.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        StartCoroutine(CombineEffect());

        float time = 0.3f;
        Quaternion StartRotationL = m_LRobotRigidbody.rotation;
        Quaternion StartRotationR = m_RRobotRigidbody.rotation;
        Quaternion EndRotationL = Quaternion.LookRotation(-Direction);
        Quaternion EndRotationR = Quaternion.LookRotation(Direction);
        for (float t = 0; t < time; t += Time.fixedDeltaTime)
        {
            m_LRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationL, EndRotationL, m_RotationCurve.Evaluate(t / time)));
            m_RRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationR, EndRotationR, m_RotationCurve.Evaluate(t / time)));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.rotation = EndRotationL;
        m_RRobotRigidbody.rotation = EndRotationR;

        yield return new WaitForSeconds(0.3f);     

        m_HRobot.gameObject.SetActive(true);
        m_TwinRobot.SetActive(false);
        m_State = PlayMode.Combine;
    }

    private IEnumerator Release()
    {
        m_State = PlayMode.Release;
        m_HRobot.gameObject.SetActive(false);
        m_TwinRobot.gameObject.SetActive(true);

        Vector3 vector = m_HRobot.transform.right;
        vector *= vector.x > vector.z ? 1 : -1;

        Quaternion lRotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
        Quaternion rRotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
        float preMove = 0;
        float move;
        float t;
        float l = 10f;
        float radius = 2 * 1.6f;
        RaycastHit hit;
        int layermask = LayerMask.GetMask(new string[] { "Wall" });
        yield return null;
        const float m_CombineTimeRequierd = 3;
        for (float f = 0; f < m_CombineTimeRequierd; f += Time.fixedDeltaTime)
        {
            t = m_ReleaseCurve.Evaluate(f / m_CombineTimeRequierd);
            move = Mathf.Lerp(0.5f, l, t) - preMove;
            if (!Physics.CheckSphere(m_LRobotRigidbody.position, radius, layermask))
            {
                if (Physics.SphereCast(m_LRobotRigidbody.position, radius, -vector, out hit, move, layermask))
                {
                    m_LRobotRigidbody.MovePosition(m_LRobotRigidbody.position - vector * hit.distance);
                }
                else
                {
                    m_LRobotRigidbody.MovePosition(m_LRobotRigidbody.position - vector * move);
                }
            }
            if (!Physics.CheckSphere(m_RRobotRigidbody.position, radius, layermask))
            {
                if (Physics.SphereCast(m_RRobotRigidbody.position, radius, vector, out hit, move, layermask))
                {
                    m_RRobotRigidbody.MovePosition(m_RRobotRigidbody.position + vector * hit.distance);
                }
                else
                {
                    m_RRobotRigidbody.MovePosition(m_RRobotRigidbody.position + vector * move);
                }
            }
            preMove += move;
            m_LRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(lRotation, rRotation, t * 4));
            m_RRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(rRotation, lRotation, t * 4));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.position -= vector * (l - preMove);
        m_RRobotRigidbody.position += vector * (l - preMove);
        m_TwinRobot.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_ReleaseTime);
        StartCoroutine(Combine());
    }

    private IEnumerator CombineEffect()
    {
        m_CombineEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        m_CombineEffect.SetActive(false);
    }
}
