using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Boss3 : MonoBehaviour
{
    //基本ステ
    public int m_Hp;
    public float m_Speed;
    //ロケットパンチ
    public float m_RPunchSpeed;
    public float m_RPunchRange;
    public float m_RPunchInterval;

    BehaviorTree m_BT;
    BBoard m_BB;
    public GameObject m_Target;
    public GameObject m_Temp;

    // Use this for initialization
    void Start()
    {
        m_BB = GetComponent<BBoard>(); //new BB_Boss3(gameObject,this, GetComponent<NavMeshAgent>());
        m_BT = GetComponent<BehaviorTree>();
        m_BT.Init();
        m_BT.SetBoard(m_BB);
        m_BB.GObjValues["target"] = m_Target;

        BDecorator BDNeedChase = new BDecorator();
        BDNeedChase.m_Del_Check = () =>
        {
            float distance = Vector3.Distance(transform.position, m_BB.GObjValues["target"].transform.position);
            return distance > m_RPunchRange;
        };

        BSelector sel_punch_move = new BSelector();
        m_BT.SetRootNode(sel_punch_move);

        /*ロケットパンチ*/
        BT_Rocket BTT_Rocket = new BT_Rocket();
        BDecorator dec_RPRange = new BDecorator(() =>
        {
            float distance =
                Vector3.Distance(transform.position, m_BB.GObjValues["target"].transform.position);
            return distance < m_RPunchRange;
        });
        BTT_Rocket.AddDecorator(dec_RPRange);
        BTT_Rocket.AddDecorator(new BD_CoolTime(1.0f));

        BT_LookTarget lookTarget = new BT_LookTarget("target", 5.0f);

        BSequence seq = new BSequence();        
        BT_MoveTo moveToRange = new BT_MoveTo("target", m_Speed);
        moveToRange.NodeName = "ToRange";
        moveToRange.m_StopDistance = m_RPunchRange;        
        m_BB.GObjValues["temp"] = m_Temp;
        BT_MoveTo testMoveto = new BT_MoveTo("temp", m_Speed);
        seq.AddNode(lookTarget);
        seq.AddNode(moveToRange);
        seq.AddNode(testMoveto);

        sel_punch_move.AddNode(BTT_Rocket);
        sel_punch_move.AddNode(seq);        
    }

    // Update is called once per frame
    void Update()
    {
        m_BT.BUpdate();
    }
}
