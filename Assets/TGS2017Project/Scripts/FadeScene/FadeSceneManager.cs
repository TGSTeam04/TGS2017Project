using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeSceneManager : MonoBehaviour
{
    [SerializeField] FadeImage m_FadeImage;

    public UnityEvent EndEvent { get { return m_FadeImage.m_Del_FadeEnd; } }

    public void FadeIn()
    {
        m_FadeImage.SetAlpha(0);
        m_FadeImage.IsINFade = true;
        StartCoroutine(m_FadeImage.FadeStart());
    }
    public void FadeOut()
    {
        m_FadeImage.SetAlpha(1);
        m_FadeImage.IsINFade = false;
        StartCoroutine(m_FadeImage.FadeStart());
    }
}
