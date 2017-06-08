using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDecorator// : BNode
{
    public BBoard m_BB;
    public BehaviorTree m_BT;
    public event BAction Del_Check;
    public bool m_Invert;
    public BDecorator()
    {
        Del_Check = OnCheck;
    }
    public BDecorator(BAction check)
    {
        Del_Check = check;
    }
    public BDecorator Invert()
    {
        m_Invert = !m_Invert;
        return this;
    }
    public bool Check()
    {
        return Del_Check() == !m_Invert;
    }
    public virtual bool OnCheck()
    {
        Debug.Log("デコレータがデフォルトのままです");
        return false;
    }

    public virtual void Reset()
    {
        Initialize();
    }

    public virtual void Initialize() { }

    public virtual void NodeSuccess() { }
    public virtual void NodeFailure() { }
}