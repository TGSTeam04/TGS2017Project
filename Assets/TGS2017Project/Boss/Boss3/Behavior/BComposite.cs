using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BComposite : BNode
{
    protected List<BNode> m_Child;
    protected int m_NowIndex;
    public BComposite() : base()
    {
        m_Child = new List<BNode>();
        m_NowIndex = 0;
    }
    public BComposite(BComposite pearent) : base(pearent)
    {
        m_Child = new List<BNode>();
        m_NowIndex = 0;
    }

    public virtual void AddNode(BNode node)
    {
        m_Child.Add(node);
        node.SetParent(this);
    }
    public void RemoveNode(BNode node)
    {
        m_Child.Remove(node);
        node.SetParent(null);
    }

    public override void Reset()
    {
        base.Reset();
        foreach (var c in m_Child)
        {
            c.Reset();
        }
        Initialize();
    }

    protected override void OnExecute()
    {
        m_Child[m_NowIndex].Execute();
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (var c in m_Child)
        {
            c.Initialize();
        }
        m_NowIndex = 0;
    }

    public virtual void ChildSuccess() { }
    public virtual void ChildFailure() { }
    public virtual void DeleteChild(BNode node)
    {
        m_Child.Remove(node);
    }
    public virtual void ClearChild()
    {
        m_Child.Clear();
    }
}