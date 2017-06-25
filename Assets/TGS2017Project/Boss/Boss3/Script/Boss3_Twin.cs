using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Twin : MonoBehaviour
{
    [SerializeField] Boss3_Controller m_BossController;
    [SerializeField] Renderer m_ShildRender;
    [SerializeField] Gradient m_ShieldColor;   

    //エフェクト
    [SerializeField] GameObject m_Explosion;
    private float HP
    {
        get { return m_BossController.Hp; }
        set
        {
            m_BossController.Hp = value;
            //シールドに反映
            m_ShildRender.gameObject.SetActive(HP > 0);
            m_ShildRender.material.SetColor("_BaseColor", m_ShieldColor.Evaluate(HP / m_BossController.m_MaxHp));
        }
    }

    private void Awake()
    {       
        Damageable[] damageComps = GetComponentsInChildren<Damageable>();
        foreach (var comp in damageComps)
        {
            comp.Del_ReciveDamage = Damage;
        }
    }

    private void OnEnable()
    {
        m_ShildRender.gameObject.SetActive(HP != 0);
    }

    private void Damage(float d, MonoBehaviour s)
    {
        //ダメージ
        HP = Mathf.Max(0, HP - d);
    }
    public void Dead()
    {
        Instantiate(m_Explosion, transform.position, transform.rotation);
    }

    public void SetShieldActive(bool isActive)
    {
        m_ShildRender.gameObject.SetActive(isActive && HP != 0);
    }
}
