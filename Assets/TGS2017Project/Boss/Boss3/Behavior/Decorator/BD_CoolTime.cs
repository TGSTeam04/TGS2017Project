using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CheckされないとTimerが進まない問題がある
public class BD_CoolTime : BDecorator
{
    float m_CoolTime;
    float m_RemainingTime;
    public BD_CoolTime(float coolTime)
    {
        m_CoolTime = coolTime;
        m_RemainingTime = 0.0f;
    }
    public override bool OnCheck()
    {
        return m_RemainingTime <= float.Epsilon;
    }
    public override void NodeSuccess()
    {
        m_RemainingTime = m_CoolTime;
        m_BB.StartCoroutine(TimerUpdate());
    }
    private IEnumerator TimerUpdate()
    {        
        while (m_RemainingTime >= float.Epsilon)
        {
            m_RemainingTime -= Time.deltaTime;     
            yield return null;
        }
    }
}

