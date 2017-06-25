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

    private float m_TargetA;

    public bool IsINFade
    {
        get { return m_IsINFade; }
        set
        {
            m_IsINFade = value;
            m_InOutChange = value ? 1 : -1;
            m_TargetA = IsINFade ? 1f : 0f;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_Image = GetComponent<Image>();

        IsINFade = m_IsINFade;
        if (m_TimeRequired == 0)
            m_TimeRequired = 1f;
        if (m_AutoStart)
            m_Coroutine = StartCoroutine(FadeStart());
    }

    public void SetAlpha(float a)
    {
        Color defualt = m_Image.color;
        float alpha = Mathf.Clamp(a, 0, 1);
        m_Image.color = new Color(defualt.r, defualt.g, defualt.b, alpha);
    }

    public IEnumerator FadeStart()
    {
        float alpha = m_Image.color.a;

        while (Mathf.Abs(m_Image.color.a - m_TargetA) >= float.Epsilon)
        {
            alpha = Mathf.Clamp(m_Image.color.a + (Time.deltaTime / m_TimeRequired * m_InOutChange), 0f, 1f);
            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, alpha);
            yield return null;
        }

        m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, m_TargetA);
        m_Del_FadeEnd.Invoke();
    }

    public void ResetAlpha()
    {
        float alpha = IsINFade ? 0f : 1f;
    }
}
