using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Twins : MonoBehaviour
{
    Damageable m_Damage;
    Boss3_Controller m_Controller;
    private void Awake()
    {
        m_Damage = GetComponent<Damageable>();
        m_Damage.Event_Damaged = (float d, MonoBehaviour s) 
            => { m_Controller.Hp -= d; };
    }
}
