﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void OnCollideEnter_Del();

public enum RocketState
{
    Idle,
    Fire,
    Back,
    Reflected,
    Buried,
}

public class Rocket : MonoBehaviour
{
    public RocketState m_State;
    //戻るべきトランスフォーム
    public Transform m_StandTrans;
    private Rigidbody m_Rigidbody;
    public float m_Speed;
    public float m_BackSpeed;
    //前進時間
    public float m_AdvanceTime;
    public float m_Timer;

    //衝突時イベントコールバック
    public event Action<Rocket, Collision> Del_Collide;
    //衝突した雑魚リスト
    public List<EnemyBase> m_ChildEnemys;

    private void Awake()
    {
        m_ChildEnemys = new List<EnemyBase>();
    }
    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (m_State)
        {
            case RocketState.Idle:
                break;
            case RocketState.Fire:
                m_Timer += Time.deltaTime;
                if (m_Timer > m_AdvanceTime)
                    m_State = RocketState.Back;
                break;
            case RocketState.Back:
                transform.LookAt(m_StandTrans);
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        switch (m_State)
        {
            case RocketState.Idle:
                break;
            case RocketState.Fire:
                m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Speed * Time.fixedDeltaTime);
                break;
            case RocketState.Back:
                m_Rigidbody.MovePosition(m_Rigidbody.position + (m_StandTrans.position - m_Rigidbody.position).normalized * m_BackSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(m_Rigidbody.position, m_StandTrans.position) < m_BackSpeed * Time.fixedDeltaTime)
                {
                    m_State = RocketState.Idle;
                    gameObject.SetActive(false);
                    m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                break;
            default:
                break;
        }
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        m_StandTrans.localScale *= 0.0f;
        transform.position = m_StandTrans.position;
        transform.rotation = m_StandTrans.rotation;
        m_State = RocketState.Fire;
        m_Timer = 0;
    }

    public bool IsCanFire
    {
        get { return m_State == RocketState.Idle; }
    }

    //ChildEnemyを追加するとき（衝突時等）
    public void AddChildEnemy(EnemyBase enemy)
    {
        enemy.transform.parent = transform;
        m_ChildEnemys.Add(enemy);
    }

    //ChildEnemyを全て破壊
    public void BreakChildEnemy()
    {
        foreach (var enemy in m_ChildEnemys)
        {
            enemy.transform.parent = null;
            enemy.SetBreak();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_State = RocketState.Back;
        if (Del_Collide != null)
            Del_Collide.Invoke(this, collision);
    }
    public void SetLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        gameObject.layer = layer;        
    }
    public IEnumerator Break(float repairTime)
    {
        //一定時間無効
        gameObject.SetActive(false);
        while (repairTime < 0)
        {
            repairTime -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(true);
        //float reSpanwTime = 2.0f;
        //while()
        //transform.localScale *= 0.0f;
    }
}