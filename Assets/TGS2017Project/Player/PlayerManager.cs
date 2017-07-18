using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
	public PlayerController m_PlayerController;
	public HumanoidRobot m_HumanoidRobot;
	public TwinRobot m_TwinRobotL;
	public TwinRobot m_TwinRobotR;

	public Transform NearPlayer(Vector3 position)
	{
		return GameManager.Instance.m_PlayMode == PlayMode.HumanoidRobot ?			m_HumanoidRobot.transform :
			m_TwinRobotL.Distance(position) <= m_TwinRobotR.Distance(position) ?	m_TwinRobotL.transform :
																					m_TwinRobotR.transform;
	}
}
