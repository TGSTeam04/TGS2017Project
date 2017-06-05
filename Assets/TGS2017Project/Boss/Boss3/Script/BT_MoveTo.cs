using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BT_MoveTo : BTask
{
    public GameObject m_Target;
    public Vector3 Destination {
        get { return m_Target.transform.position; }
        set { m_Target.transform.position = value; } }
    public float m_Speed;
    public float m_StopDistance;
    public float m_Interval;

    //Action RealUpdate;
    float m_Timer;
    NavMeshAgent m_nAgent;

    public BT_MoveTo(Vector3 destination, float targetDistance = 0.0f, float speed = 10.0f)
    {
        m_Target = new GameObject();
        Destination = destination;
        m_StopDistance = targetDistance;
        m_Speed = speed;
        m_Interval = 1.0f;

        //RealUpdate = MoveToLocation;
    }
    public BT_MoveTo(GameObject target, float targetDistance = 0.0f, float speed = 10.0f, float updateInterval = 1.0f)
    {
        m_Target = target;
        m_StopDistance = targetDistance;
        m_Speed = speed;
        m_Interval = updateInterval;
        m_Timer = updateInterval;

        //RealUpdate = MoveToObject;
    }

    protected override void OnExecute()
    {
        if (m_State != BState.Updating)
            Redy();

        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0.0f)
        {
            m_Timer = m_Interval;
            m_nAgent.destination = m_Target.transform.position;
        }

        else if (m_nAgent.remainingDistance < m_StopDistance)
        {
            Stop();
        }
    }

    private void MoveToLocation()
    {
        if (m_State != BState.Updating)
            Redy();

        else if (m_nAgent.remainingDistance < m_StopDistance)
        {
            Stop();
        }
    }

    private void MoveToObject()
    {
        if (m_State != BState.Updating)
            Redy();

        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0.0f)
        {
            m_Timer = m_Interval;
            m_nAgent.destination = m_Target.transform.position;
        }

        else if (m_nAgent.remainingDistance < m_StopDistance)
        {
            Stop();
        }
    }

    private void Redy()
    {
        m_nAgent = m_BB.GetComponent<NavMeshAgent>();
        m_nAgent.speed = m_Speed;
        m_nAgent.SetDestination(m_Target.transform.position);
        m_nAgent.stoppingDistance = m_StopDistance;
    }

    private void Stop()
    {
        Vector3 vel = m_nAgent.velocity;
        m_nAgent.velocity = vel.sqrMagnitude < 2.0f
            ? Vector3.zero
            : vel - vel * 0.1f;
        Succes();
    }
}
