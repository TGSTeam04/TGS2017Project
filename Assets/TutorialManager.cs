using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	// Use this for initialization
	IEnumerator Start () {
		m_ComleteMessage.gameObject.SetActive(false);
		m_TutorialMessage.gameObject.SetActive(false);
		m_PlayerController.enabled = false;
		yield return new WaitForSeconds(1);
		m_PlayerController.enabled = true;
		m_TutorialMessage.gameObject.SetActive(true);
		yield return new WaitUntil(() => Complete());
		m_PlayerController.enabled = false;
		m_TutorialMessage.gameObject.SetActive(false);
		m_ComleteMessage.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}

	// Update is called once per frame
	void Update () {
		
	}

	private bool Complete()
	{
		return 
			m_Distance >= Vector3.Distance(m_RobotL.position, m_TargetL.position)&&
			m_Distance >= Vector3.Distance(m_RobotR.position, m_TargetR.position);
	}
}
