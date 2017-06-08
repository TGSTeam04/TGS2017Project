using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRoot : BComposite
{
    BNode m_RealRoot;
    public BRoot() : base()
    {
        m_RealRoot = new BTask();
    }
    protected override void OnExecute()
    {
        m_RealRoot.TryExecute();
    }
    public override void DeleteChild(BNode node)
    {
        m_RealRoot = new BTask();
    }
    public override void AddNode(BNode node)
    {
        m_RealRoot = node;
    }
}
