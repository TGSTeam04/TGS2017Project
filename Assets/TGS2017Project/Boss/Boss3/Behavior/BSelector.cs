using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSelector<T> : BComposite<T> where T : BBoard
{
    public override void ChildSuccess()
    {
        Succes();
    }
    public override void ChildFailure()
    {
        if (m_NowIndex < m_Child.Count - 1)
        {
            m_NowIndex++;
            m_Child[m_NowIndex].Update();
        }
        else
            Failure();
    }
}