using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Arrow : MonoBehaviour {

    [SerializeField]
    private bool m_IsLeft;

    [SerializeField]
    private GameObject m_Title;

    private Vector3 m_Scale;

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, m_Scale, 20.0f * Time.deltaTime);
        if (EventSystem.current.currentSelectedGameObject != m_Title)
        {// タイトルボタンを選択していなければ
            if (m_IsLeft)
            {
                if (Input.GetAxis("HorizontalL") < -0.6f)
                {
                    m_Scale = new Vector3(2, 2, 1);
                }
                else
                {
                    m_Scale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                if (Input.GetAxis("HorizontalL") > 0.6f)
                {
                    m_Scale = new Vector3(2, 2, 1);
                }
                else
                {
                    m_Scale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            m_Scale = new Vector3(1, 1, 1);
        }
	}
}
