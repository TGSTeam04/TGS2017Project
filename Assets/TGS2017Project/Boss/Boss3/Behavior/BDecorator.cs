using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDecorator// : BNode
{
    public BBoard m_BB;
    public BehaviorTree m_BT;
    public BAction m_Del_Check;
    public BDecorator()
    {
        m_Del_Check = new BAction(() => { return true; });
    }
    public BDecorator(BAction check)
    {
        m_Del_Check = check;
    }
    public virtual bool Check()
    {
        return m_Del_Check();
    }

    public virtual void Initialize() { }

    public virtual void NodeSuccess() { }
    public virtual void NodeFailure() { }
}