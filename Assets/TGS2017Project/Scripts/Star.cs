using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour {

    RectTransform m_Rect;
    Image m_Image;

    [SerializeField] private float m_Width = 5;
    [SerializeField] private float m_Height = 5;
    [SerializeField] private float m_Reduced = 3f;
    [SerializeField] private float m_MaxAlpha = 1f;
    private float m_Alpha = 0.0f;


    // Use this for initialization
    void Start () {
        m_Rect = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
        m_Image.color = new Color(0, 0, 0, m_Alpha);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Width > 1 || m_Height > 1)
        {
            m_Width -= m_Reduced * Time.deltaTime;
            m_Height -= m_Reduced * Time.deltaTime;
        }
        if (m_Width <= 1 || m_Height <= 1)
        {
            m_Width = 1;
            m_Height = 1;
        }
        if(m_Alpha < m_MaxAlpha)
        {
            m_Alpha += Time.deltaTime;
        }
        if (m_Alpha >= m_MaxAlpha)
        {
            m_Alpha = m_MaxAlpha;
        }
        m_Rect.localScale = new Vector2(m_Width, m_Height);
        m_Image.color = new Color(1, 1, 1, m_Alpha);
	}
}
