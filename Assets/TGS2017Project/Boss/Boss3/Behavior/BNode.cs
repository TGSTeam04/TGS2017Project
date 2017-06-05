using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BNode
{
    public enum BState
    {
        Ready,      //初期値
        Updating,   //実行中
        Success,    //実行成功
        Failure,    //実行失敗            
    }
    public BState m_State;
    public BBoard m_BB;
    public BehaviorTree m_BT;
    protected List<BDecorator> m_Decorators;
    protected BComposite m_Parent;
    public string NodeName { get; set; }

    public BNode(string name = "default")
    {
        NodeName = name;
        m_Decorators = new List<BDecorator>();
    }
    public BNode(BComposite parent, string name = "default")
    {
        NodeName = name;
        SetParent(parent);
        m_Decorators = new List<BDecorator>();
    }
    public void SetParent(BComposite parent)
    {
        m_BB = parent.m_BB;
        m_BT = parent.m_BT;
        foreach (var dec in m_Decorators)
        {
            dec.m_BB = m_BB;
        }
        m_Parent = parent;
    }
    public virtual void Reset()
    {
        m_State = BState.Ready;
        Initialize();
    }
    protected virtual void Succes()
    {
        m_Parent.ChildSuccess();
        Initialize();
        m_State = BState.Success;
    }
    protected virtual void Failure()
    {
        m_Parent.ChildFailure();
        Initialize();
        m_State = BState.Failure;
    }
    public virtual void Initialize() { }
    //public virtual void Enter() { }

    public void AddDecorator(BDecorator dec)
    {
        m_Decorators.Add(dec);
        dec.m_BB = m_BB;
    }

    public void Execute()
    {
        foreach (var dec in m_Decorators)
        {
            if (!dec.Check())
            {
                m_Parent.ChildFailure();
                return;
            }
        }
        OnExecute();
        m_State = BState.Updating;
    }
    protected virtual void OnExecute() { }
    //public virtual void Exit() { }

    public virtual void DeleteNode()
    {
        m_Parent.DeleteChild(this);
    }
}