using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRoot<T> : BComposite<T> where T : BBoard
{
    BNode<T> m_RealRoot;
    public BRoot() : base()
    {
        m_RealRoot = new BTask<T>();
    }
    protected override void OnUpdate()
    {
        m_RealRoot.Update();
    }
    public override void DeleteChild(BNode<T> node)
    {
        m_RealRoot = new BTask<T>();
    }
    public override void AddNode(BNode<T> node)
    {
        m_RealRoot = node;
    }
}
