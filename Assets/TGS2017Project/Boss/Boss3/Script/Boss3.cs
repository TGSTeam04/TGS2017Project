using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss3 : MonoBehaviour
{
    //基本ステ
    public int m_Hp;
    public float m_Speed;
    //ロケットパンチ
    public float m_RPunchSpeed;
    public float m_RPunchRange;
    public float m_RPunchInterval;

    BehaviorTree m_BTree;
    BB_Boss3 m_BB;

    // Use this for initialization
    void Start()
    {
        m_BB = GetComponent<BB_Boss3>(); //new BB_Boss3(gameObject,this, GetComponent<NavMeshAgent>());
        m_BTree = new BehaviorTree(m_BB);
        //BDecorator<BB_Boss3> BDNeedChase = new BDecorator<BB_Boss3>();
        //BDNeedChase.m_Check_Del = () =>
        //{
        //    float distance = Vector3.Distance(transform.position, m_BB.target.transform.position);
        //    return distance < StagePanel.m_InscribedR * 3;
        //};
        //射程まで近づく
        m_BTree.SetRootNode(new BT_MoveTo(m_BB.target, m_RPunchRange, m_Speed));
        
        //GetComponent<NavMeshAgent>().SetDestination(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        m_BTree.Update();
    }
}
