using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roll : MonoBehaviour
{
    public bool m_IsRollOnStart;
    public float m_Speed;
    public float m_EndLocalPosY;
    private Image m_Image;
    private RectTransform m_Trans;
    private bool m_IsRoll;    

    private void Start()
    {
        m_Image = GetComponent<Image>();
        m_Trans = GetComponent<RectTransform>();
        m_IsRoll = m_IsRollOnStart;
    }
    private void Update()
    {
        if (m_IsRoll && m_Trans.anchoredPosition.y < m_EndLocalPosY)
        {
            Vector3 temp = m_Trans.position;
            temp.y += Time.deltaTime * m_Speed;
            m_Trans.position = temp; 
        }
    }
}
