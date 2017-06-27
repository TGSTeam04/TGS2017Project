using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_FireRocket : BTask
{
    RocketBattery m_Battery;

    public BT_FireRocket(RocketBattery battery)
    {
        m_Battery = battery;
    }
    protected override void OnExecute()
    {
        if (m_Battery.IsCanFire)
        {
            //砲台をターゲット方向
            m_Battery.gameObject.transform.LookAt(m_BB.GObjValues["target"].transform.position);

            //足元が見えなければ（壁がある）前方向に発射
            Vector3 stand = m_Battery.m_LRocket.m_StandTrans.position;
            Vector3 dire = m_BB.GObjValues["target"].transform.position - m_Battery.gameObject.transform.position;            
            if (Physics.Raycast(stand, dire, dire.magnitude, LayerMask.GetMask("Wall")))
            {
                Quaternion rot = m_Battery.transform.rotation;
                rot.x = 0;
                m_Battery.transform.rotation = rot;
            }
            m_Battery.Fire();
            Succes();
        }
        else
            Failure();
    }
}