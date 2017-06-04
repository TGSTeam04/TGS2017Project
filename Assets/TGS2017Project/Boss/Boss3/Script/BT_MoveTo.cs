using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_MoveTo : BTask<BB_Boss3>
{
    GameObject m_Target;
    float m_StopDistance;
    float m_Speed;

    public BT_MoveTo(Vector3 destination, float targetDistance = 0.0f, float speed = 10.0f)
    {
        m_Target = new GameObject();
        m_Target.transform.position = destination;
        m_StopDistance = targetDistance;
        m_Speed = speed;
    }
    public BT_MoveTo(GameObject target, float targetDistance = 0.0f, float speed = 10.0f)
    {
        m_Target = target;
        m_StopDistance = targetDistance;
        m_Speed = speed;
    }
    protected override void OnUpdate()
    {
        Vector3 destination = m_Target.transform.position;

        destination.y = m_BB.gameObject.transform.position.y;
        m_BB.gameObject.transform.LookAt(destination);
        if (m_State != BState.Updating)
        {
            m_BB.nAgent.speed = m_Speed;
            m_BB.nAgent.SetDestination(destination);
            m_BB.nAgent.stoppingDistance = m_StopDistance;
        }
        else
        {
            //if (m_BB.nAgent.isStopped)
            //{
            //    Succes();
            //    Debug.Log("stop");
            //}
            if (Vector3.Distance(m_BB.gameObject.transform.position, destination) < m_StopDistance)
            {                
                m_BB.nAgent.velocity = Vector3.zero;
                m_BB.nAgent.isStopped = true;
                Succes();
            }
        }
    }
}
