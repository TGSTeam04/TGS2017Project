using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTask : BNode
{
    protected override void Execute()
    {
        if (m_State != BState.Updating)
        {
            FirstExecute();
            m_State = BState.Updating;
        }
        else
        {
            OnExcete();
        }
    }
    protected virtual void FirstExecute() { }
    protected virtual void OnExcete() { }
}
