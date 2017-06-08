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

    private GameObject m_TargetObj;
    float m_Timer;
    NavMeshAgent m_nAgent;

    public BT_MoveTo(string target, float speed = 10.0f, float targetDistance = 5.0f, float updateInterval = 1.0f)
    {
        m_TargetKey = target;
        m_Speed = speed;
        m_StopDistance = targetDistance;
        m_Interval = updateInterval;
        m_Timer = updateInterval;

        //RealUpdate = MoveToObject;
    }

    protected override void FirstExecute()
    {
        Debug.Log("MoveTo タスク");
        m_nAgent = m_BB.GetComponent<NavMeshAgent>();
        m_TargetObj = m_BB.GObjValues[m_TargetKey];
        m_nAgent.destination = m_TargetObj.transform.position;
        m_nAgent.speed = m_Speed;
        m_nAgent.stoppingDistance = m_StopDistance;
    }
    protected override void OnExcete()
    {
        m_Timer -= Time.deltaTime;
        float remainingDistance = m_nAgent.remainingDistance;
        float distance = Vector2.Distance(m_BB.transform.position, m_TargetObj.transform.position);
        if (m_Timer <= 0.0f)
        {
            m_Timer = m_Interval;
            m_TargetObj = m_BB.GObjValues[m_TargetKey];
            m_nAgent.destination = m_TargetObj.transform.position;
        }
        if (remainingDistance < m_StopDistance && distance < m_StopDistance)
        {            
            if (StopEase())
            {
                Succes();
            }
        }
    }

    private bool StopEase()
    {
        Vector3 vel = m_nAgent.velocity;
        m_nAgent.velocity = vel.sqrMagnitude < 2.0f
            ? Vector3.zero
            : vel - vel * 0.1f;
        return m_nAgent.velocity.magnitude < 1.0f;
    }
}
