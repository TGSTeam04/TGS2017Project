using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnReceieveNotify_Del(string handle = "null", object param = null);

public class ObserverBase
{
    public OnReceieveNotify_Del m_Del_OnReceive;
    protected List<SubjectBase> m_Subjects;
    private string m_observerName;
    public ObserverBase()
    {
        m_Subjects = new List<SubjectBase>();
    }
    public string m_ObserverName
    {
        set { m_observerName = value; }
        get { return m_observerName; }
    }

    public void ForSubject_AddSubject(SubjectBase subject)
    {
        m_Subjects.Add(subject);
    }

    public void BindSubject(SubjectBase subject)
    {
        m_Subjects.Add(subject);
        subject.ForObserver_AddObserver(this);
    }

    public void RemoveSubject(SubjectBase subject)
    {
        for (int i = 0; i < m_Subjects.Count; i++)
        {
            if (m_Subjects[i] == subject)
                m_Subjects.RemoveAt(i);
        }
    }

    public void RemoveSubject(string SubjectName)
    {
        for (int i = 0; i < m_Subjects.Count; i++)
        {
            if (m_Subjects[i].m_SubjectName == SubjectName)
                m_Subjects.RemoveAt(i);
        }
    }

    public void ClearSubject()
    {
        m_Subjects.Clear();
    }

    public void NotifyToSubjects(string handle = "null", object param = null)
    {
        foreach (var subject in m_Subjects)
        {
            subject.ReceiveNotice(handle, param);
        }
    }
    public void ReceiveNotice(string handle = "null", object param = null)
    {
        OnReceive(handle, param);
        foreach (var subobject in m_Subjects)
        {
            subobject.ReceiveNotice(handle, param);
        }
    }
    public void ReceiveNoticeOnleyObserver(string handle = "null", object param = null)
    {
        OnReceive(handle, param);
    }
    protected virtual void OnReceive(string handle = "null", object param = null)
    {
        m_Del_OnReceive(handle, param);
    }

    /*保留*/
    //public void ReceiveFromSubject(SubjectBase from, string handle = "null", object param = null)
    //{
    //    OnReceive(from, handle, param);
    //    foreach (var each in m_Subjects)
    //    {
    //        each.ReceiveFromSubject(from);
    //    }
    //}

    //public void NotifyToSubjectsFromThis(string handle = "null", object param = null)
    //{
    //    foreach (var subject in m_Subjects)
    //    {
    //        subject.ReceiveFromObserver(this, handle, param);
    //    }
    //}

    //protected virtual void OnReceive(SubjectBase from, string handle = "null", object param = null) { }
}

