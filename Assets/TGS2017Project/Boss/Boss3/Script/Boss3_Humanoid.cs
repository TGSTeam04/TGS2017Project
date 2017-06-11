using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.AI;

[SelectionBase]
public class Boss3_Humanoid : MonoBehaviour
{

    public float m_MaxHp;
    [HideInInspector] public float m_Hp;
    public float m_Speed;

    //ロケットパンチ
    public float m_RocketSpeed;
    public float m_RocketRange;
    public float m_RocketInterval;
    public float m_NeedMoveDistance;

    //各コンポーネント
    public NavMeshAgent m_NavAgent;
    public Rigidbody m_Rb;
    public Animator m_Anim;
    public RocketBattery m_Battery;
    public Damageable m_DamageComp;

    //ビヘイビアと付随する値
    BehaviorTree m_BT;
    BBoard m_BB;
    public GameObject m_Target;
    public GameObject m_Temp;

    private void Awake()
    {
        m_Hp = m_MaxHp;

        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rb = GetComponent<Rigidbody>();
        m_Anim = GetComponentInChildren<Animator>();
        m_Battery = GetComponent<RocketBattery>();
        m_DamageComp = GetComponent<Damageable>();
        m_DamageComp.Event_Damaged = Damaged;
    }
    // Use this for initialization
    void Start()
    {
        //ビヘイビアツリーのセットアップ
        SetUpBT();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_DamageComp.ApplyDamage(5, this);
        }

        m_BT.BUpdate();

        //アニメーションの値をセット
        Vector3 vel = transform.rotation * -m_NavAgent.velocity;
        m_Anim.SetFloat("Forward", vel.z / m_NavAgent.speed);
        m_Anim.SetFloat("Right", vel.x / m_NavAgent.speed);
    }
    private void Damaged(float damage, MonoBehaviour src)
    {
        m_Hp -= damage;
        if (m_Hp < 0)
        {
            Debug.Log("Boss3死亡");
            m_BT.IsStop = true;
        }
        m_Anim.SetTrigger("Damage");
    }

    //ビヘイビアの設定
    private void SetUpBT()
    {
        m_BB = GetComponent<BBoard>();
        m_BT = GetComponent<BehaviorTree>();
        m_BT.Init();
        m_BT.SetBoard(m_BB);
        m_BB.GObjValues["target"] = m_Target;

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
        moveToRange.AddDecorator(new BD_CloserThen("target", m_NeedMoveDistance).Invert());
        m_BB.GObjValues["temp"] = m_Temp;

        seq.AddNode(moveToRange);

        sl_Rocket_Other.AddNode(BTT_Rocket);
        sl_Rocket_Other.AddNode(seq);
        par_Look_Other.AddNode(lookTarget);
        par_Look_Other.AddNode(sl_Rocket_Other);
    }
}
