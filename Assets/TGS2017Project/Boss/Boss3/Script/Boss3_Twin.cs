using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Twin : MonoBehaviour
{
    [SerializeField] Renderer m_ShildRender;
    [SerializeField] Gradient m_ShieldColor;

    private Boss3_Controller m_BossController;
    private float HP
    {
        get { return m_BossController.Hp; }
        set { m_BossController.Hp = value; }
    }

    private void Awake()
    {
        m_BossController = GetComponentInParent<Boss3_Controller>();

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
        //シールドに反映
        m_ShildRender.gameObject.SetActive(HP > 0);
        m_ShildRender.material.SetColor("_BaseColor", m_ShieldColor.Evaluate(HP / m_BossController.m_MaxHp));
    }
    public void Dead()
    {

    }
}
