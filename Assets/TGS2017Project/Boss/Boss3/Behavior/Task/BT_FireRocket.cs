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
            m_Battery.Fire();
            Succes();
        }
        else
            Failure();
    }
}