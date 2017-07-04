using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    public Animator m_Anim;
    public GameObject[] m_BoostStartEff;
    public GameObject[] m_BoostEndEff;

    private void Start()
    {
        var sb = m_Anim.GetBehaviour<SB_BoostStart>();
        sb.m_BoostManager = this;
    }

    public void StartBoost()
    {
        foreach (var eff in m_BoostStartEff)
            eff.SetActive(true);
    }
    public void EndBoost()
    {
        for (int i = 0; i < m_BoostStartEff.Length; ++i)
        {
            m_BoostStartEff[i].SetActive(false);
            m_BoostEndEff[i].SetActive(true);
        }
    }
}
