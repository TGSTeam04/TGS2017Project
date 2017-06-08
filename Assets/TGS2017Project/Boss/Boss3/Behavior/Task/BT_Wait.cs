using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Wait : BTask
{
    float m_WaitTime;
    float m_RemainigTime;
    public BT_Wait(float waitTime)
    {
        m_WaitTime = waitTime;
        Reset();
    }
    protected override void FirstExecute()
    {
        if (m_RemainigTime < 0)
            Succes();
    }
    public override void Initialize()
    {
        m_RemainigTime = m_WaitTime;
    }
}
