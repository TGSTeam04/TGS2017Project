using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidRobot : MonoBehaviour {

	public PlayerController m_PlayerController;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision other)
	{
		Debug.Log("damege");
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				break;
			case PlayMode.HumanoidRobot:
				switch (other.gameObject.tag)
				{
					case "Enemy":
					case "Bullet":
						m_PlayerController.m_Energy -= 2;
						Debug.Log("damege");
						break;
					default:
						break;
				}
				break;
			case PlayMode.Combine:
				break;
			case PlayMode.Release:
				break;
			default:
				break;
		}
	}
}
