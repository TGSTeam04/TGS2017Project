﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour {

	public Text m_TimeText;
	public Text m_ScoreText;

	public float m_Time;
	public int m_Score;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(1);
		for (float t = 0; t < 1.0f ; t += Time.deltaTime)
		{
			int time = (int)Mathf.Lerp(0, m_Time, t);
			m_TimeText.text = time / 60 + ":" + (time % 60).ToString("00");
			yield return null;
		}
		m_TimeText.text = m_Time / 60 + ":" + (m_Time % 60).ToString("00");
		for (float t = 0; t < 1.0f; t += Time.deltaTime)
		{
			m_ScoreText.text = ((int)Mathf.Lerp(0, m_Score, t)).ToString();
			yield return null;
		}
		m_ScoreText.text = m_Score.ToString();
		yield return new WaitForSeconds(3);
		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}
}