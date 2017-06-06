using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void OnCollideEnter_Del();

public enum RocketState
{
    Idle,
    Fire,
    Back
}

public class Rocket : MonoBehaviour
{    
    public RocketState m_State;
    //戻るべきトランスフォーム
    public Transform m_StandTrans;
    private Rigidbody m_Rigidbody;
    public float m_Speed;
    public float m_BackSpeed;
    //前進時間
    public float m_AdvanceTime;
    //腕が戻った時のデリゲート
    public UnityEvent m_Del_Returned;
    public UnityEvent<Collision> m_Del_Collide;

    private float m_Timer;
    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (m_State)
        {
            case RocketState.Idle:
                break;
            case RocketState.Fire:
                m_Timer += Time.deltaTime;
                if (m_Timer > m_AdvanceTime)
                    m_State = RocketState.Back;
                break;
            case RocketState.Back:
                transform.LookAt(m_StandTrans);
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        switch (m_State)
        {
            case RocketState.Idle:
                break;
            case RocketState.Fire:
                m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Speed * Time.fixedDeltaTime);
                break;
            case RocketState.Back:
                m_Rigidbody.MovePosition(m_Rigidbody.position + (m_StandTrans.position - m_Rigidbody.position).normalized * m_BackSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(m_Rigidbody.position, m_StandTrans.position) < m_BackSpeed * Time.fixedDeltaTime)
                {
                    m_State = RocketState.Idle;
                    gameObject.SetActive(false);
                    if (m_Del_Returned != null)
                        m_Del_Returned.Invoke();
                }
                break;
            default:
                break;
        }
    }
    
    public bool TryFire()
    {
        if (!IsCanFire)
            return false;

        transform.position = m_StandTrans.position;
        transform.rotation = m_StandTrans.rotation;
        m_State = RocketState.Fire;
        m_Timer = 0;

        return true;
    }
    public void Hit()
    {
        m_State = RocketState.Back;
    }

    public bool IsCanFire
    {
        get { return m_State == RocketState.Idle; }
    }
}
