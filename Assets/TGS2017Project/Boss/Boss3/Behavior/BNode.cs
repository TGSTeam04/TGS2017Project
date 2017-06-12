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
        if (name == "default")
            NodeName = ToString();
        else
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
    //初期化
    public virtual void Initialize()
    {
        foreach (var dec in m_Decorators)
            dec.Initialize();
    }
    public virtual void Reset()
    {
        m_State = BState.Ready;
        StateChanged();
        OnReset();
        //Debug.Log(ToString());
        foreach (var dec in m_Decorators)
            dec.NodeStateChanged();
    }
    protected virtual void Succes()
    {
        StateChanged();
        m_ParentSuccess();
        m_State = BState.Success;
        foreach (var dec in m_Decorators)
            dec.NodeSuccess();

    }
    protected virtual void Failure()
    {
        StateChanged();
        m_ParentFailure();
        m_State = BState.Failure;
        foreach (var dec in m_Decorators)
            dec.NodeFailure();
    }

    //リセット、サクセス、フェイラーで共通の処理
    public virtual void StateChanged() { }    

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
    protected void Execute()
    {
        if (m_State != BState.Updating)
        {
            FirstExecute();
            m_State = BState.Updating;
        }
        else
            OnExecute();
        
        //Debug.Log(ToString());
    }
    protected virtual void FirstExecute() { }
    protected virtual void OnExecute() { }
    public virtual void OnReset() { }

    public virtual void OnDelete() { }
}