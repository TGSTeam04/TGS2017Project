using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBinder_FadeEnd : MonoBehaviour
{
    FadeImage m_Fade;
    public void BindEvent(UnityAction listener)
    {
        m_Fade.m_Del_FadeEnd.AddListener(listener);
    }
}
