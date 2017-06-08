using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

//ロケット２つをラップしたロケット砲台クラス
//詳細パラメータは各ロケット　m_XRocket　のパラメータをいじってください。
public class RocketBattery : MonoBehaviour
{
    //public List<Rocket> m_Rockets;
    public Rocket m_LRocket;
    public Rocket m_RRocket;
    public string m_RocketLayer;

    [SerializeField] private Transform m_LStandTrans;
    [SerializeField] private Transform m_RStandTrans;

    private void Awake()
    {
        //m_Rockets = new List<Rocket>();               
    }
    private void Start()
    {
        //Rocketが指定されていなければ生成
        if (m_LRocket == null)
        {
            //ロケットの初期位置取得
            if (m_LStandTrans == null) //RocketStandの存在チェック
                m_LStandTrans = transform.FindChild("RocketStand").transform;

            //ロケットインスタンス化
            GameObject lRocketObj = GameObject.Instantiate
                ((GameObject)Resources.Load("Prefub/Prefub_Rocket"));
            m_LRocket = lRocketObj.GetComponent<Rocket>();
            m_LRocket.m_StandTrans = m_LStandTrans;
        }
        //Rocketが指定されていなければ生成
        if (m_RRocket == null)
        {
            //ロケットの初期位置取得
            if (m_RStandTrans == null) //RocketStandの存在チェック
                m_RStandTrans = transform.FindChild("RocketStand").transform;

            //ロケットインスタンス化
            GameObject rRocketObj = GameObject.Instantiate
                ((GameObject)Resources.Load("Prefub/Prefub_Rocket"));
            m_RRocket = rRocketObj.GetComponent<Rocket>();
            m_RRocket.m_StandTrans = m_RStandTrans;
        }
        m_LRocket.SetLayer(m_RocketLayer);
        m_RRocket.SetLayer(m_RocketLayer);
        m_RRocket.gameObject.SetActive(false);
        m_LRocket.gameObject.SetActive(false);
    }    

    //LRどちらでもいいから発射可能か確認
    public bool IsCanFire { get { return m_LRocket.IsCanFire || m_RRocket.IsCanFire; } }
    public bool BothIsCanFire { get { return m_LRocket.IsCanFire && m_RRocket.IsCanFire; } }
    //左方の発射可能か確認
    public bool LIsCanFire { get { return m_LRocket.IsCanFire; } }
    //右方の発射可能か確認
    public bool RIsCanFire { get { return m_RRocket.IsCanFire; } }

    //LRどちらでもいいから発射トライ　発射できればtrue
    public void Fire()
    {
        if (m_LRocket.IsCanFire)
            m_LRocket.Fire();
        else
            m_RRocket.Fire();
    }
    //引数による左右対応した　発射
    public void Fire(bool isLeft)
    {
        if (isLeft)
            m_LRocket.Fire();
        else
            m_RRocket.Fire();
    }

    /************左右のパラメータを同時に設定（ロケットのパラメータを個々でいじりたいときはメンバのロケットにアクセスしてください。***************/

    public UnityEvent<Rocket, Collision> Del_Collide
    {
        get { return m_LRocket.m_Del_Collide; }
        set
        {
            m_LRocket.m_Del_Collide = value;
            m_RRocket.m_Del_Collide = value;
        }
    }
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

