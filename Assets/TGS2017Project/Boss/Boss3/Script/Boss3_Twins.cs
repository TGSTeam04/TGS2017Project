using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Twins : MonoBehaviour
{
    private void Awake()
    {
        var bossCon = GetComponentInParent<Boss3_Controller>();
        Damageable[] damageComps = GetComponentsInChildren<Damageable>();
        foreach (var comp in damageComps)
        {
            comp.Event_Damaged = (float d, MonoBehaviour s)
            => { bossCon.Hp -= d; };
        }        
    }
}
