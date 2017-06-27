using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearParformHumanoid : ClearParformance
{
    //ロボットの中心位置（カメラの注視点にする）
    private Transform m_HumanoidCenter;
    private Animator m_HAnimator;

    public override bool CheckNecessary(GameManager gm)
    {
        GameObject Humanoid = GameManager.Instance.m_HumanoidRobot;
        return !Humanoid.activeSelf;
    }

    protected override void Redy()
    {
        Transform Humanoid = GameManager.Instance.m_HumanoidRobot.transform;

        Humanoid.parent = m_ParformAnimRootObj.transform;
        Humanoid.localPosition = Vector3.zero;
        Humanoid.localRotation = Quaternion.identity;

        m_HumanoidCenter = Humanoid.FindChild("LookPoint");
        m_HAnimator = Humanoid.FindChild("Model").GetComponent<Animator>();
        m_HAnimator.Play("WalkBlendTree");
        //m_HAnim = m_Humanoid.transform.FindChild("Model").GetComponent<Animation>();//m_Humanoid.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        StartCoroutine(PerformManagement());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Camera.transform.LookAt(m_HumanoidCenter);
    }

    protected override IEnumerator PlayerParform()
    {
        yield return new WaitForSeconds(1.0f);
        m_HAnimator.Play("Wepon_R");
        yield return new WaitForSeconds(1.0f);
        m_PerformAnim.Play();
        yield return new WaitForSeconds(m_PerformAnim.clip.length);
        m_HAnimator.Play("Kimepo");
        yield return new WaitForSeconds(3.0f);
    }
}
