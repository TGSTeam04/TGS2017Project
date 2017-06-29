using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour {
	[SerializeField]
	private PlayerController m_PlayerController;
	[SerializeField]
	private Transform m_RobotL;
	[SerializeField]
	private Transform m_RobotR;
	[SerializeField]
	private Transform m_TutorialMessage;
	[SerializeField]
	private Transform m_ComleteMessage;
	[SerializeField]
	private Transform m_TargetL;
	[SerializeField]
	private Transform m_TargetR;
	[SerializeField]
	private float m_Distance;
	[SerializeField]
	private int m_TutorialCount;
	private int m_TutorialNumber;

	[SerializeField]
	private Image m_FadeImage;
	[SerializeField]
	private Image m_CompleteImage;
	[SerializeField]
	private Text m_TutorialText;

	private string[][] m_Text = {
		new string[] { "訓練所へようこそ" , "あなた達には\n最終試験に向けての" , "操作確認を\n行ってもらいます" , "ナーサティア、ダフネを\n操作して目的地に\n向かってください"},
		new string[] { "2" },
		new string[] { "3" },
		new string[] { "4" },
		new string[] { "5" },
		new string[] { "6" },
		new string[] { "7" },
		new string[] { "8" },};
	// Use this for initialization
	IEnumerator Start () {
		m_PlayerController = GameManager.Instance.m_PlayerController;
		m_RobotL = m_PlayerController.m_LRobot.transform;
		m_RobotR = m_PlayerController.m_RRobot.transform;
		m_TutorialNumber = 0;
		while (m_TutorialNumber < m_TutorialCount)
		{
			TutorialStart();
			if(m_TutorialNumber > 0)
			{
				// フェード
				for (float t = 0; t < 0.5f; t += Time.deltaTime)
				{
					m_FadeImage.color = Color.Lerp(Color.black, Color.clear, t / 0.5f);
					yield return null;
				}
				m_FadeImage.color = Color.clear;
			}
			else
			{
				yield return new WaitForSeconds(2);
			}

			//テキスト表示
			for (int i = 0; i < m_Text[m_TutorialNumber].Length; i++)
			{
				yield return StartCoroutine(textset(i));
			}


			m_PlayerController.enabled = true;

			yield return new WaitUntil(() => Complete());

			m_PlayerController.enabled = false;

			// 演出
			for (float t = 0; t < 0.5f; t += Time.deltaTime)
			{
				m_CompleteImage.fillAmount = t / 0.5f;
				yield return null;
			}
			m_CompleteImage.fillAmount = 1;
			yield return new WaitForSeconds(1);

//			m_TutorialNumber++;
			if (m_TutorialNumber < m_TutorialCount)
			{
				//フェード
				for (float t = 0; t < 0.5f; t += Time.deltaTime)
				{
					m_FadeImage.color = Color.Lerp(Color.clear, Color.black, t / 0.5f);
					yield return null;
				}
				m_FadeImage.color = Color.black;
			}
		}

		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}

	private void TutorialStart()
	{
		m_PlayerController.enabled = false;
		m_CompleteImage.fillAmount = 0;
		m_FadeImage.color = Color.black;
		m_TutorialText.text = "";
		switch (m_TutorialNumber)
		{
			case 0:
				GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
				m_FadeImage.color = Color.clear;
				return;
			default:
				return;
		}
	}

	private bool Complete()
	{
		switch (m_TutorialNumber)
		{
			case 0:
				return
					m_Distance >= Vector3.Distance(m_RobotL.position, m_TargetL.position) &&
					m_Distance >= Vector3.Distance(m_RobotR.position, m_TargetR.position);
			default:
				return false;
		}
	}

	IEnumerator textset(int i)
	{
		var textSending = m_TutorialText.GetComponent<TextSending>();
		m_TutorialText.text = m_Text[m_TutorialNumber][i];
		textSending.SetRate(0.1f);
		textSending.Initialize();
		while (!textSending.IsEnd())
		{
			m_TutorialText.SetAllDirty();
			yield return null;
		}
		yield return new WaitForSeconds(1);
	}
}
