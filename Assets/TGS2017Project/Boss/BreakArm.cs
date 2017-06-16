using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakArm : MonoBehaviour
{

    public GameObject m_Explosion;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void Dead()
    {
        Instantiate(m_Explosion, transform.position, transform.rotation);
        this.gameObject.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)
    {        
        if (GameManager.Instance.m_PlayMode == PlayMode.Combine && AttackProcess.s_Chance)
        {
            Boss.HitPoint -= 30.0f;
            Boss.s_State = Boss.BossState.Invincible;
            Dead();
        }
        var damageComp = other.gameObject.GetComponent<Damageable>();
        if (damageComp != null)
            damageComp.ApplyDamage(150, this);
    }
}