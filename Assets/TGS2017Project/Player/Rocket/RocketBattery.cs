using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Events;

//ロケット２つをラップしたロケット砲台クラス
//詳細パラメータは各ロケット　m_XRocket　のパラメータをいじってください。
public class RocketBattery : MonoBehaviour
{
    [SerializeField] private GameObject m_RocketPrefub;    
    [SerializeField] private Transform m_LStandTrans;
    [SerializeField] private Transform m_RStandTrans;

    [HideInInspector] public RocketBase m_LRocket;
    [HideInInspector] public RocketBase m_RRocket;
    public string m_RocketLayer;
    public string m_TargetTag;
    private Animator m_Anim;

    private void Awake()
    {
        m_Anim = GetComponentInChildren<Animator>();        

        //ロケットの初期位置取得
        if (m_LStandTrans == null) //RocketStandの存在チェック
            m_LStandTrans = transform.FindChild("RocketStand").transform;
        if (m_RStandTrans == null) //RocketStandの存在チェック
            m_RStandTrans = transform.FindChild("RocketStand").transform;

        //Rocketが指定されていなければ生成
        if (m_LRocket == null)
        {
            //ロケットインスタンス化
            GameObject lRocketObj = Instantiate(m_RocketPrefub);
            m_LRocket = lRocketObj.GetComponent<RocketBase>();
            m_LRocket.m_StandTrans = m_LStandTrans;
        }
        //Rocketが指定されていなければ生成
        if (m_RRocket == null)
        {
            //ロケットインスタンス化
            GameObject rRocketObj = Instantiate(m_RocketPrefub);
            m_RRocket = rRocketObj.GetComponent<RocketBase>();
            m_RRocket.m_StandTrans = m_RStandTrans;
        }
        
        m_LRocket.SetLayer(m_RocketLayer);
        m_RRocket.SetLayer(m_RocketLayer);        
        m_RRocket.gameObject.SetActive(false);
        m_LRocket.gameObject.SetActive(false);
        m_LRocket.m_Battery = this;
        m_RRocket.m_Battery = this;
    }

    private void Start()
    {
    }

    //LRどちらでもいいから発射可能か確認
    public bool IsCanFire { get { return m_LRocket.IsCanFire || m_RRocket.IsCanFire; } }
    public bool BothIsCanFire { get { return m_LRocket.IsCanFire && m_RRocket.IsCanFire; } }
    //左方の発射可能か確認
    public bool LIsCanFire { get { return m_LRocket.IsCanFire; } }
    //右方の発射可能か確認
    public bool RIsCanFire { get { return m_RRocket.IsCanFire; } }

    //LRどっちでもいいから発射
    public void Fire()
    {
        if (m_LRocket.IsCanFire)
            StartCoroutine(LAnimatedFire());
        else
            StartCoroutine(RAnimatedFire());
    }
    //BoolによるLR指定の発射
    private void Fire(bool isLeft)
    {
        if (isLeft)
            StartCoroutine(LAnimatedFire());
        else
            StartCoroutine(RAnimatedFire());
    }

    //L発射
    public IEnumerator LAnimatedFire()
    {
        m_Anim.SetTrigger("LFire");
        yield return new WaitForAnimation(m_Anim, 0.7f);
        m_LRocket.Fire();
    }
    //R発射
    public IEnumerator RAnimatedFire()
    {
        m_Anim.SetTrigger("RFire");
        yield return new WaitForAnimation(m_Anim, 0.7f);
        m_RRocket.Fire();
    }   

    /************左右のパラメータを同時に設定（ロケットのパラメータを個々でいじりたいときはメンバのロケットにアクセスしてください。***************/

    public void SetSpeed(float speed)
    {
        m_LRocket.m_Speed = speed;
        m_RRocket.m_Speed = speed;
    }
    public void SetBackSpeed(float speed)
    {
        m_LRocket.m_BackSpeed = speed;
        m_RRocket.m_BackSpeed = speed;
    }
    public void SetAdvanceTime(float time)
    {
        m_LRocket.m_AdvanceTime = time;
        m_RRocket.m_AdvanceTime = time;
    }
}