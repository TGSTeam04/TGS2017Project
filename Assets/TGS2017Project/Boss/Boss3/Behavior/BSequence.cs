using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSequence<T> : BComposite<T> where T : BBoard
{
    public BSequence()
    {
    }
    public override void ChildSuccess()
    {
        if (m_NowIndex >= m_Child.Count - 1)
        {
            Succes();
        }
        else
            m_NowIndex++;
    }
    public override void ChildFailure()
    {
        Failure();
    }
}