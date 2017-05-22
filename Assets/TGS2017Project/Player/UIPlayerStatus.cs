using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : MonoBehaviour {
	public Text m_Level;
	public Image m_EXP;
	public Image m_Energy;
	public PlayerController m_Player;
	public GameObject m_GameOver;
	public GameObject m_GameClear;

	// Use this for initialization
	void Start () {
		m_Player = GameManager.Instance.m_PlayerController;
	}
	
	// Update is called once per frame
	void Update () {
		m_Level.text = GameManager.Instance.m_Level.ToString();
		m_EXP.fillAmount = (float)m_Player.m_Exp / GameManager.Instance.LevelParameter.m_NextExp;
		m_Energy.fillAmount = m_Player.m_Energy / 20;
		m_GameOver.SetActive(GameManager.Instance.m_IsGameOver);
		m_GameClear.SetActive(GameManager.Instance.m_IsGameClear);
	}
}
