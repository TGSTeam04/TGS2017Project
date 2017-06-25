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

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        Transform Humanoid = GameManager.Instance.m_HumanoidRobot.transform;

        Humanoid.parent = m_ParformAnimRootObj.transform;
        Humanoid.localPosition = Vector3.zero;
        Humanoid.localRotation = Quaternion.identity;

        m_HumanoidCenter = Humanoid.FindChild("LookPoint");
        m_HAnimator = Humanoid.FindChild("Model").GetComponent<Animator>();
        //m_HAnim = m_Humanoid.transform.FindChild("Model").GetComponent<Animation>();//m_Humanoid.GetComponentInChildren<Animator>();                
    }

    private void Start()
    {
        StartCoroutine(Parform());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Camera.transform.LookAt(m_HumanoidCenter);
    }

    IEnumerator Parform()
    {
        GameManager gm = GameManager.Instance;
        gm.m_PlayCamera.SetActive(false);
        m_Camera.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        m_HAnimator.Play("Wepon_R");
        yield return new WaitForSeconds(1.0f);
        m_PerformAnim.Play();
        yield return new WaitForSeconds(m_PerformAnim.clip.length);
        m_HAnimator.Play("Kimepo");
        yield return new WaitForSeconds(3.0f);

        GameManager.Instance.StartCoroutine(GameManager.Instance.m_GameStarter.LoadScene(8));
        var async = gm.m_GameStarter.AddScene("Result");
    }
}
