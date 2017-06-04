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

    BehaviorTree<BB_Boss3> m_BTree;
    BB_Boss3 m_Board;

    // Use this for initialization
    void Start()
    {
        m_Board = GetComponent<BB_Boss3>(); //new BB_Boss3(gameObject,this, GetComponent<NavMeshAgent>());
        m_BTree = new BehaviorTree<BB_Boss3>(m_Board);
        m_BTree.SetRootNode(new BT_MoveTo(new Vector3(0, 0, 0), m_RPunchRange, m_Speed));

        //GetComponent<NavMeshAgent>().SetDestination(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        m_BTree.Update();
    }
}
