using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDecorator : BNode
{
    public BAction m_Check_Del;
    public virtual bool Check()
    {
        foreach (var dec in m_Decorators)
        {
            if (!dec.Check())
            {
                m_Parent.ChildFailure();
                return false;
            }
        }
        return m_Check_Del();
    }
}