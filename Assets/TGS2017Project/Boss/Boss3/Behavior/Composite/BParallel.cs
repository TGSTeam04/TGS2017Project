using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BParallel : BComposite
{
    //int m_SucceseNum;
    //int m_FailureNum;
    protected override void OnExecute()
    {
        foreach (var child in m_Child)
        {
            //BState state = child.m_State;
            //if (state != BState.Success && state != BState.Failure)
                child.TryExecute();
        }
    }
    public override void ChildSuccess()
    {
        //m_SucceseNum++;
        //if (m_SucceseNum >= m_Child.Count)
        //    Succes();
    }
    public override void ChildFailure()
    {
        //m_FailureNum++;
        //if (m_FailureNum >= m_Child.Count)
        //    Failure();
    }
    public override void Initialize()
    {
        base.Initialize();
        //m_SucceseNum = 0;
        //m_FailureNum = 0;
    }
}
