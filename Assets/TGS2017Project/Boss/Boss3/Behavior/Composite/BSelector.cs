using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSelector : BComposite
{
    protected override void OnExecute()
    {
        //毎回0番目からトライする
        m_NowIndex = 0;
        base.OnExecute();
    }
    public override void ChildSuccess()
    {
        for (int i = m_NowIndex; i < m_Child.Count; i++)
        {
            if (m_Child[i].m_State != BState.Ready)
                m_Child[i].Reset();
        }
        Succes();
    }
    public override void ChildFailure()
    {
        m_NowIndex++;
        if (m_NowIndex >= m_Child.Count)
        {
            Failure();
        }
        else
        {
            m_Child[m_NowIndex].TryExecute();
        }
    }
}