using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : MonoBehaviour {
	public Image m_Energy;
	public PlayerController m_Player;
	public GameObject m_Black;
	public Text m_CountDown;

	// Use this for initialization
	void Start () {
		m_Player = GameManager.Instance.m_PlayerController;
		StartCoroutine(countdown());
	}

	// Update is called once per frame
	void Update () {
		m_Energy.fillAmount = m_Player.Energy / GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[4];
	}
	IEnumerator countdown()
	{
		m_Black.SetActive(true);
		yield return new WaitForSeconds(GameManager.Instance.m_LoadingAnimationTime);
		m_CountDown.text = "3";
		yield return new WaitForSeconds(1);
		m_CountDown.text = "2";
		yield return new WaitForSeconds(1);
		m_CountDown.text = "1";
		yield return new WaitForSeconds(1);
		m_Black.SetActive(false);
		GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
	}
}
