using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : MonoBehaviour {
	public Image m_Energy;
	public PlayerController m_Player;

	// Use this for initialization
	void Start () {
		m_Player = GameManager.Instance.m_PlayerController;
	}
	
	// Update is called once per frame
	void Update () {
		m_Energy.fillAmount = m_Player.m_Energy / GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[4];
	}
}
