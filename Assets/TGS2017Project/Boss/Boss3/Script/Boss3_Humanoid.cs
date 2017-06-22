﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.AI;

[SelectionBase]
public class Boss3_Humanoid : MonoBehaviour
{
    //分裂時と合体時のコントロールをするクラスの参照
    private Boss3_Controller m_BossController;

    [SerializeField] float m_MaxHP;
    private float m_HP;
    //移動
    [SerializeField] float m_Speed;
    //ロケットパンチ
    [SerializeField] float m_RocketSpeed;
    [SerializeField] float m_RocketRange;
    [SerializeField] float m_RocketInterval;
    [SerializeField] float m_NeedMoveDistance;

    //各コンポーネント    
    private NavMeshAgent m_NavAgent;
    private Rigidbody m_Rb;
    private Animator m_Anim;
    private RocketBattery m_Battery;
    private Damageable m_DamageComp;

    //ビヘイビアと付随する値
    private BehaviorTree m_BT;
    private BBoard m_BB;

    //エフェクト
    [SerializeField] GameObject m_Explosion;
    [SerializeField] GameObject m_Efect_Numbness;
    [SerializeField] float m_NumbnessInterval = 30.0f;

    float m_RemainingNI;

    public GameObject tempTarget;

    private void Awake()
    {
        m_BossController = GetComponentInParent<Boss3_Controller>();
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rb = GetComponent<Rigidbody>();
        m_Anim = GetComponentInChildren<Animator>();
        m_Battery = GetComponent<RocketBattery>();
        m_DamageComp = GetComponent<Damageable>();
        m_DamageComp.Del_ReciveDamage = Damaged;

        m_BT = GetComponent<BehaviorTree>();
        m_BB = GetComponent<BBoard>();
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(UpdateTarget());
        //ビヘイビアツリーのセットアップ
        SetUpBT();
    }
    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            //GameManager gm = GameManager.Instance;
            //float Ldistance = (gm.m_LRobot.transform.position - transform.position).magnitude;
            //float Rdistance = (gm.m_RRobot.transform.position - transform.position).magnitude;
            //GameObject target = gm.m_PlayMode == PlayMode.HumanoidRobot
            //    ? gm.m_HumanoidRobot
            //    : Ldistance < Rdistance
            //            ? gm.m_LRobot
            //            : gm.m_RRobot;
            //Debug.Log("Target Update" + target.ToString());
            GameObject target = tempTarget;
            m_BB.GObjValues["target"] = target; //tempTarget;
            yield return new WaitForSeconds(3.0f);
        }
    }

    private void OnEnable()
    {
        m_HP = m_MaxHP;
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.0f);
        m_BT.IsStop = false;
    }
    // Update is called once per frame
    public void BossUpdate()
    {
        m_BT.BUpdate();

        //アニメーションの値をセット
        Vector3 vel = transform.rotation * -m_NavAgent.velocity;
        m_Anim.SetFloat("Forward", vel.z / m_NavAgent.speed);
        m_Anim.SetFloat("Right", vel.x / m_NavAgent.speed);
    }
    private void Damaged(float damage, MonoBehaviour src)
    {
        m_HP = Mathf.Max(0, m_HP - damage);
        m_RemainingNI -= damage;
        if(m_RemainingNI < 0)
        {
            Instantiate(m_Efect_Numbness, transform);
            m_RemainingNI = m_NumbnessInterval;
        }
        if (m_HP <= 0)
        {
            Release();
            return;
        }
        m_Anim.SetTrigger("Damage");
    }

    public void Dead()
    {
        Instantiate(m_Explosion, transform.position, transform.rotation);
    }

    public void Release()
    {
        m_BT.BTReset();
        m_BossController.ReleaseStart();
    }

    //ビヘイビアの設定
    private void SetUpBT()
    {
        m_BB = GetComponent<BBoard>();
        m_BT = GetComponent<BehaviorTree>();
        m_BT.Init();
        m_BT.SetBoard(m_BB);

        BParallel par_Look_Other = new BParallel();
        //LookAtPlayer
        BT_LookTarget lookTarget = new BT_LookTarget("target", 5.0f);
        BSelector sl_Rocket_Other = new BSelector();
        m_BT.SetRootNode(par_Look_Other);

        /*ロケットパンチ*/
        BT_FireRocket BTT_Rocket = new BT_FireRocket(GetComponent<RocketBattery>());
        BDecorator dec_RocektRnage = new BD_CloserThen("target", m_RocketRange);
        BTT_Rocket.AddDecorator(dec_RocektRnage);
        BTT_Rocket.AddDecorator(new BD_CoolTime(m_RocketInterval));

        //接近
        BSequence seq = new BSequence();
        BT_MoveTo moveToRange = new BT_MoveTo("target", m_Speed);
        moveToRange.m_StopDistance = m_RocketRange;
        moveToRange.m_IsCanCancelMove = false;
        //moveToRange.AddDecorator(new BD_CloserThen("target", m_NeedMoveDistance).Invert());

        seq.AddNode(moveToRange);

        sl_Rocket_Other.AddNode(BTT_Rocket);
        sl_Rocket_Other.AddNode(seq);
        par_Look_Other.AddNode(lookTarget);
        par_Look_Other.AddNode(sl_Rocket_Other);
    }
}
