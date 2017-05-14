using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode {
	NoPlay,
	TwinRobot,
	HumanoidRobot,
	Combine,
	Release
}
public class GameManager : SingletonMonoBehaviour<GameManager> {

	public float m_TimeRate = 1.0f;
	public PlayMode m_PlayMode = PlayMode.NoPlay;
	public float m_CombineTime;
	public GameObject m_PlayCamera;
	public GameObject m_LRobot;
	public GameObject m_RRobot;
	public GameObject m_HumanoidRobot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
