using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private BRoot m_VirtualRoot;
    public BBoard m_BB;
    private bool m_IsStop;
    public bool IsStop
    {
        get { return m_IsStop; }
        set
        {
            m_IsStop = value;
            if (!value)
                m_VirtualRoot.Initialize();
        }
    }
    private void OnEnable()
    {
        if (m_VirtualRoot != null)
            m_VirtualRoot.Initialize();
    }

    public void Init()
    {
        m_VirtualRoot = new BRoot();
        m_VirtualRoot.m_BB = m_BB;
        m_VirtualRoot.m_BT = this;
        IsStop = false;
    }
    public void BTReset()
    {
        m_VirtualRoot.Reset();
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