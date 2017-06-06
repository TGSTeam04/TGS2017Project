using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

//ロケット２つをラップしたロケット砲台クラス
//詳細パラメータは各ロケット　m_XRocket　のパラメータをいじってください。
public class RocketBattery : MonoBehaviour
{
    public Rocket m_LRocket;
    public Rocket m_RRocket;

    [SerializeField] private Transform m_LStandTnras;
    [SerializeField] private Transform m_RStandTrans;
    [SerializeField] private UnityEvent m_Del_Returned;
    [SerializeField] private UnityEvent<Collision> m_Del_Collide;

    private void Start()
    {        
        //Rocketが指定されていなければ生成
        if (m_LRocket == null)
        {
            if (m_RStandTrans == null) //RocketStandの存在チェック
                m_LStandTnras = transform.FindChild("RocketStand").transform;

            GameObject lArmObj = GameObject.Instantiate
                ((GameObject)Resources.Load("Prefub/Prefub_Rocket"));
            m_LRocket = lArmObj.GetComponent<Rocket>();
            m_LRocket.m_StandTrans = m_LStandTnras;
        }
        //Rocketが指定されていなければ生成
        if (m_RRocket == null)
        {
            if (m_RStandTrans == null) //RocketStandの存在チェック
                m_RStandTrans = transform.FindChild("RocketStand").transform;

            GameObject rArmObj = GameObject.Instantiate
                ((GameObject)Resources.Load("Prefub/Prefub_Rocket"));
            m_RRocket = rArmObj.GetComponent<Rocket>();
            m_RRocket.m_StandTrans = m_RStandTrans;        
        }
        Del_Collide = m_Del_Collide;
        Del_Returned = m_Del_Returned;
    }
    //LRどちらでもいいから発射可能か確認
    public bool IsCanFire { get { return m_LRocket.IsCanFire || m_RRocket.IsCanFire; } }
    //左方の発射可能か確認
    public bool IsLCanFire { get { return m_LRocket.IsCanFire; } }
    //右方の発射可能か確認
    public bool IsRCanFire { get { return m_RRocket.IsCanFire; } }

    //LRどちらでもいいから発射トライ　発射できればtrue
    public bool TryFire()
    {
        bool complete;
        complete = m_LRocket.TryFire();
        if (!complete)
            complete = m_RRocket.TryFire();
        return complete;
    }

    /************左右のパラメータを同時に設定（ロケットのパラメータを個々でいじりたいときはメンバのロケットにアクセスしてください。***************/

    public UnityEvent Del_Returned
    {
        get { return m_LRocket.m_Del_Returned; }
        set
        {
            m_Del_Returned = value;
            m_LRocket.m_Del_Returned = value;
            m_RRocket.m_Del_Returned = value;
        }
    }
    public UnityEvent<Collision> Del_Collide
    {
        get { return m_LRocket.m_Del_Collide; }
        set
        {
            m_Del_Collide = value;
            m_LRocket.m_Del_Collide = value;
            m_RRocket.m_Del_Collide = value;
        }
    }

    //左方の発射トライ　発射できればtrue
    public bool LTryFire()
    {
        return m_LRocket.TryFire();
    }
    //右方の発射トライ　発射できればtrue
    public bool RTryFire()
    {
        return m_RRocket.TryFire();
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
    public void SetDel_Returned()
    {

    }
}

