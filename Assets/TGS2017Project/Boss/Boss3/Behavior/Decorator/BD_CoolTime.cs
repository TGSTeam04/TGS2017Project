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
        m_Timer -= Time.deltaTime;
        return m_Timer <= 0.0f;
    }
    public override void NodeSuccess()
    {
        m_Timer = m_CoolTime;
    }
}

