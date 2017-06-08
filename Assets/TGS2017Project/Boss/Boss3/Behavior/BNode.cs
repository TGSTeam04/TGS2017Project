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
    private List<BDecorator> m_Decorators;
    //private BComposite m_Parent;
    Action m_ParentSuccess;
    Action m_ParentFailure;
    public string NodeName { get; set; }

    public BNode(string name = "default")
    {
        NodeName = name;
        m_State = BState.Ready;
        m_Decorators = new List<BDecorator>();
    }
    //public BNode(BComposite parent, string name = "default")
    //{
    //    NodeName = name;
    //    SetParent(parent);
    //    m_Decorators = new List<BDecorator>();
    //}

    public virtual void SetParent(BComposite parent)
    {
        m_BB = parent.m_BB;
        m_BT = parent.m_BT;
        foreach (var dec in m_Decorators)
        {
            dec.m_BT = parent.m_BT;
            dec.m_BB = m_BB;
        }
        m_ParentSuccess = parent.ChildSuccess;
        m_ParentFailure = parent.ChildFailure;
    }
    public virtual void Reset()
    {
        m_State = BState.Ready;
        Initialize();
    }
    protected virtual void Succes()
    {
        Initialize();
        m_ParentSuccess();
        m_State = BState.Success;        
        foreach (var dec in m_Decorators)
            dec.NodeSuccess();

    }
    protected virtual void Failure()
    {
        Initialize();
        m_ParentFailure();
        m_State = BState.Failure;
        foreach (var dec in m_Decorators)
            dec.NodeFailure();
    }
    public virtual void Initialize()
    {
        foreach (var dec in m_Decorators)
            dec.Initialize();

    }
    //public virtual void Enter() { }

    public void AddDecorator(BDecorator dec)
    {
        m_Decorators.Add(dec);
        dec.m_BT = m_BT;
        dec.m_BB = m_BB;
    }

    public void TryExecute()
    {
        foreach (var dec in m_Decorators)
        {
            if (!dec.Check())
            {
                //Debug.Log("フェイラー");
                Failure();
                return;
            }
        }
        Execute();
    }
    protected virtual void Execute() { }
    //public virtual void Exit() { }

    public virtual void OnDelete()
    {     
    }
}