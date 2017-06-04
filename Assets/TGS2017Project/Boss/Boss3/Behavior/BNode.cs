using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BNode<T> where T : BBoard
{
    public enum BState
    {
        Ready,      //初期値
        Updating,   //実行中
        Success,    //実行成功
        Failure,    //実行失敗            
    }

    public BState m_State;
    public T m_BB;
    private List<BDecorator<T>> m_Decorators;
    private BComposite<T> m_Parent;

    public BNode()
    {
        m_Decorators = new List<BDecorator<T>>();
    }
    public BNode(BComposite<T> parent)
    {
        SetParent(parent);
        m_Decorators = new List<BDecorator<T>>();
    }
    public void SetParent(BComposite<T> parent)
    {
        m_BB = parent.m_BB;
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

    public void AddDecorator(BDecorator<T> dec)
    {
        m_Decorators.Add(dec);
        dec.m_BB = m_BB;
    }

    public void Update()
    {
        bool isCanUpdate = true;
        foreach (var dec in m_Decorators)
        {
            if (!dec.Check())
            {
                isCanUpdate = false;
                m_Parent.ChildFailure();
                break;
            }
        }
        if (isCanUpdate)
        {            
            OnUpdate();
            m_State = BState.Updating;
        }
    }
    protected virtual void OnUpdate() { }
    //public virtual void Exit() { }

    public virtual void DeleteNode()
    {
        m_Parent.DeleteChild(this);
    }
}