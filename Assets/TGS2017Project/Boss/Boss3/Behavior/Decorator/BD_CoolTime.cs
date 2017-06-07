using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CheckされないとTimerが進まない問題がある
public class BD_CoolTime : BDecorator
{
    float m_CoolTime;
    float m_Timer;
    public BD_CoolTime(float coolTime)
    {
        m_CoolTime = coolTime;
        m_Timer = 0.0f;
    }
    public override bool Check()
    {
        return m_Timer <= float.Epsilon;
    }
    public override void NodeSuccess()
    {
        m_Timer = m_CoolTime;
        m_BB.StartCoroutine(TimerUpdate());
    }
    private IEnumerator TimerUpdate()
    {        
        while (m_Timer >= float.Epsilon)
        {
            m_Timer -= Time.deltaTime;     
            yield return null;
        }
    }
}

