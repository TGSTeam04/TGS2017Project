using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Rocket : BTask
{
    RocketBattery m_Battery;
    protected override void FirstExecute()
    {
        m_Battery = m_BB.GetComponent<RocketBattery>();
        m_Battery.Fire();
        Succes();
        Debug.Log("ロケット");
    }
}