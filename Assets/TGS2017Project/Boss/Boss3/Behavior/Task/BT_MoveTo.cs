using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BT_MoveTo : BTask
{
    public string m_TargetKey;
    public float m_Speed;
    public float m_StopDistance;
    public float m_Interval;
    public string m_BBVTarget;
    public bool m_IsCanCancelMove;

    private GameObject m_TargetObj;
    float m_Timer;
    NavMeshAgent m_NavAgent;

    public BT_MoveTo(string target, float speed = 10.0f, float targetDistance = 5.0f, float updateInterval = 1.0f)
    {
        m_TargetKey = target;
        m_Speed = speed;
        m_StopDistance = targetDistance;
        m_Interval = updateInterval;
        m_Timer = updateInterval;
        m_IsCanCancelMove = true;

        //RealUpdate = MoveToObject;
    }

    protected override void FirstExecute()
    {        
        m_NavAgent = m_BB.m_NavAgent;
        m_TargetObj = m_BB.GObjValues[m_TargetKey];
        m_NavAgent.destination = m_TargetObj.transform.position;
        m_NavAgent.speed = m_Speed;
        m_NavAgent.stoppingDistance = m_StopDistance;
    }
    protected override void OnExecute()
    {
        //ナビメッシュの更新
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0.0f)
        {
            m_Timer = m_Interval;
            m_TargetObj = m_BB.GObjValues[m_TargetKey];
            m_NavAgent.destination = m_TargetObj.transform.position;
        }

        //到達したかの確認
        float remainingDistance = m_NavAgent.remainingDistance;
        float distance = Vector2.Distance(m_BB.transform.position, m_TargetObj.transform.position);
        if (remainingDistance < m_StopDistance && distance < m_StopDistance)
        {
            if (StopEase())
            {
                Succes();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        if (m_IsCanCancelMove)//移動中止            
            m_BB.StartCoroutine(m_BB.UpdateWhileMethodBool(StopEase));
    }

    private bool StopEase()
    {
        Vector3 vel = m_NavAgent.velocity;
        m_NavAgent.velocity = vel.sqrMagnitude < 2.0f
            ? Vector3.zero
            : vel - vel * 0.1f;
        return m_NavAgent.velocity.magnitude < 1.0f;
    }
}
