using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BT_LookTarget : BTask
{
    public string m_Target;
    public float m_RotateSpeed;   

    public BT_LookTarget(string target, float rotateSpeed)
    {
        m_Target = target;
        m_RotateSpeed = rotateSpeed;
    }

    protected override void FirstExecute()
    {
    }
    protected override void OnExcete()
    {
        Transform trans = m_BB.transform;
        Quaternion baseRot = trans.rotation;
        Vector3 target = m_BB.GObjValues[m_Target].transform.position;
        trans.LookAt(target);
        Quaternion newRot = trans.rotation;
        trans.rotation = baseRot;

        float deltaAngle = Mathf.DeltaAngle(trans.eulerAngles.y, newRot.eulerAngles.y);     
        if (Mathf.Abs(deltaAngle) >= m_RotateSpeed)
            trans.Rotate(new Vector3(0f, m_RotateSpeed, 0f));
        else        
            Succes();        
    }
}
