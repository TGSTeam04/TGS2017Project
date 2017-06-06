using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private BRoot m_VirtualRoot;
    public BBoard m_BB;
    public bool IsStop { get; set; }
    //public BehaviorTree()
    //{
    //    m_VirtualRoot = new BRoot();
    //    m_VirtualRoot.m_BB = m_Board;
    //    m_VirtualRoot.m_BT = this;
    //    IsStop = false;
    //}
    //public BehaviorTree(BBoard board)
    //{
    //    m_VirtualRoot = new BRoot();
    //    m_Board = board;
    //    m_VirtualRoot.m_BB = m_Board;
    //    m_VirtualRoot.m_BT = this;
    //    IsStop = false;
    //}
    public void Init()
    {        
        m_VirtualRoot = new BRoot();
        m_VirtualRoot.m_BB = m_BB;
        m_VirtualRoot.m_BT = this;
        IsStop = false;
    }
    public void SetBoard(BBoard board)
    {
        m_BB = board;
    }

    public void BUpdate()
    {
        if (!IsStop)
            m_VirtualRoot.TryExecute();
    }

    public void SetRootNode(BNode node)
    {
        m_VirtualRoot.AddNode(node);
        node.SetParent(m_VirtualRoot);
        node.m_BB = m_BB;
    }
}