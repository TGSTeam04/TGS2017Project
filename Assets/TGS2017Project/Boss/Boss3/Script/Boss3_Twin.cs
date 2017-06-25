using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Twin : MonoBehaviour
{    
    [SerializeField] float m_MaxHP = 100f;
    [SerializeField] Renderer m_ShildRender;
    [SerializeField] Gradient m_ShieldColor;

    private Boss3_Controller m_BossController;
    private float m_HP;

    private void Awake()
    {
        m_BossController = GetComponentInParent<Boss3_Controller>();

        m_HP = m_MaxHP;
        Damageable[] damageComps = GetComponentsInChildren<Damageable>();
        foreach (var comp in damageComps)
        {
            comp.Del_ReciveDamage = Damage;
        }
    }

    private void OnEnable()
    {
        m_ShildRender.gameObject.SetActive(m_HP != 0);
    }

    private void Damage(float d, MonoBehaviour s)
    {
        //ダメージ
        m_HP = Mathf.Max(0, m_HP - d);
        if (m_HP < 0 && !m_ShildRender.gameObject.activeSelf)
            GetComponentInParent<Boss3_Controller>().Dead();
        //シールドに反映
        m_ShildRender.gameObject.SetActive(m_HP > 0);
        m_ShildRender.material.SetColor("_BaseColor", m_ShieldColor.Evaluate(m_HP / m_MaxHP));
    }
    public void Dead()
    {

    }
}
