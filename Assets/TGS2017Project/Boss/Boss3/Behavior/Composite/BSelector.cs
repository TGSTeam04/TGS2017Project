using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSelector : BComposite
{
    public override void ChildSuccess()
    {
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