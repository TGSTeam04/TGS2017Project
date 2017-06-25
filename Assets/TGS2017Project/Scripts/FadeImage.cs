using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeImage : MonoBehaviour
{
    [SerializeField] bool m_IsINFade = true;
    public float m_TimeRequired = 1.0f;
    public bool m_AutoStart = false;
    public UnityEvent m_Del_FadeEnd;

    private Image m_Image;
    private int m_InOutChange;
    private Coroutine m_Coroutine;

    public bool IsINFade
    {
        get { return m_IsINFade; }
        set
        {
            m_IsINFade = value;
            if (m_AutoStart && m_Coroutine == null)
                StartCoroutine(FadeStart());
        }
    }

    // Use this for initialization
    void Start()
    {
        m_Image = GetComponent<Image>();

        m_InOutChange = IsINFade ? 1 : -1;
        if (m_TimeRequired == 0)
            m_TimeRequired = 1f;
        if (m_AutoStart)
            m_Coroutine = StartCoroutine(FadeStart());
    }    

    public IEnumerator FadeStart()
    {
        Color defualt = m_Image.color;
        float alpha = m_Image.color.a; //IsINFade ? 0f : 1f;
        while (Mathf.Abs(m_Image.color.a - alpha) <= float.Epsilon)
        {
            alpha = Mathf.Clamp(alpha + (Time.deltaTime / m_TimeRequired * m_InOutChange), 0f, 1f);
            m_Image.color = new Color(defualt.r, defualt.g, defualt.b, alpha);
            yield return null;
        }
        m_Del_FadeEnd.Invoke();
    }

    public void ResetAlpha()
    {
        float alpha = IsINFade ? 0f : 1f;
    }
}
