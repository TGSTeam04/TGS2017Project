using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BD_CloserThen : BDecorator
{
    string m_Target;
    float m_Distance;
    
    public BD_CloserThen(string target, float distance)
    {
        m_Target = target;
        m_Distance = distance;
    }
    public override bool OnCheck()
    {
        Vector3 pos = m_BB.transform.position;
        Vector3 targetPos = m_BB.GObjValues[m_Target].transform.position;
        float distance = Vector3.Distance(pos, targetPos);
        return distance < m_Distance;
    }
}
