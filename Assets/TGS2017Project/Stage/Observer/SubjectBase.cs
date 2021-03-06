﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 製作者：大格
/// 更新日：05/19
/// オブサーバーパターンのSubject
/// </summary>

//OnDestroyでClearObserversを読んでください。
public class SubjectBase
{
    public OnReceieveNotify_Del m_Del_OnRecive;
    private List<ObserverBase> m_Observers;
    private string m_subjectName;
    public string SubjectName
    {
        set { m_subjectName = value; }
        get { return m_subjectName; }
    }

    public SubjectBase()
    {
        m_Observers = new List<ObserverBase>();
    }

    ~SubjectBase()
    {
        ClearObservers();
    }

    //Observer側から呼ぶため
    public void ForObserver_AddObserver(ObserverBase observer)
    {
        m_Observers.Add(observer);
    }
    public void BindObserver(ObserverBase observer)
    {
        m_Observers.Add(observer);
        observer.ForSubject_AddSubject(this);
    }
    public void ForObserver_RemoveObserver(ObserverBase obs)
    {
        for (int i = 0; i < m_Observers.Count; i++)
        {
            if (ReferenceEquals(m_Observers[i], obs))
            {
                m_Observers.RemoveAt(i);
            }
        }
    }
    public void UnBindObserver(ObserverBase obs)
    {
        for (int i = 0; i < m_Observers.Count; i++)
        {
            if (ReferenceEquals(m_Observers[i], obs))
            {
                m_Observers[i].ForSubject_RemoveSubject(this);
                m_Observers.RemoveAt(i);
            }
        }
    }

    public void ClearObservers()
    {
        for (int i = 0; i < m_Observers.Count; i++)
        {
            m_Observers[i].ForSubject_RemoveSubject(this);
        }
        m_Observers.Clear();
    }

    public virtual void NotifyToOnlyObserver(string handle = "null", object param = null)
    {
        foreach (var observer in m_Observers)
        {
            observer.ReceiveNotice(handle, param);
        }
    }
    public virtual void NotifyToAll(string handle = "null", object param = null)
    {
        foreach (var observer in m_Observers)
        {
            observer.ReceiveNoticeOnleyObserver(handle, param);
        }
    }

    public virtual void ReceiveNotice(string handle = "null", object param = null)
    {
        if (m_Del_OnRecive != null)
            m_Del_OnRecive(handle, param);
    }

    /*保留*/
    //public virtual void ReceiveFromObserver(ObserverBase form, string handle = "null", object param = null) { }
    //public virtual void ReceiveFromSubject(SubjectBase from, string handle = "null", object param = null) { }
}
