using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UITextFPS : MonoBehaviour {

	[SerializeField] Text m_Text;
	float m_Time;
	int m_Count;
	// Use this for initialization
	void Start () {
		m_Time = Time.time + 1;
	}
	
	// Update is called once per frame
	void Update () {
		m_Count++;
		if (Time.time >= m_Time)
		{
			m_Text.text = "FPS:" + m_Count;
			m_Count = 0;
			m_Time += 1;
		}
	}
}
