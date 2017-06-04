using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree<T> where T : BBoard, new()
{
    private BRoot<T> m_VirtualRoot;
    public T m_Board;
    public bool IsStop { get; set; }
    public BehaviorTree()
    {
        m_VirtualRoot = new BRoot<T>();
        m_VirtualRoot.m_BB = m_Board;
        IsStop = false;
    }
    public BehaviorTree(T board)
    {
        m_VirtualRoot = new BRoot<T>();
        m_Board = board;
        m_VirtualRoot.m_BB = m_Board;
        IsStop = false;
    }
    public void SetBoard(T board)
    {
        m_Board = board;
    }

    public void Update()
    {
        if (!IsStop)
            m_VirtualRoot.Update();
    }

    public void SetRootNode(BNode<T> node)
    {
        m_VirtualRoot.AddNode(node);
        node.SetParent(m_VirtualRoot);
        node.m_BB = m_Board;
    }
}