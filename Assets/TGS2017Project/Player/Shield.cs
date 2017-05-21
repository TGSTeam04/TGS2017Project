using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
	public float m_HP =1;
	public Gradient m_Color;
	public GameObject m_Shield;
	private Renderer m_Renderer;
	// Use this for initialization
	void Start()
	{
		m_Renderer = m_Shield.GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnCollisionEnter(Collision other)
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				switch (other.gameObject.tag)
				{
					case "Enemy":
					case "Bullet":
						//Damage(0.1f);
						//Debug.Log("damege");
						break;
					default:
						break;
				}
				break;
			case PlayMode.HumanoidRobot:
				break;
			case PlayMode.Combine:
				break;
			case PlayMode.Release:
				break;
			default:
				break;
		}
	}

	public void Active(bool active)
	{
		m_Shield.SetActive(active);
	}
	public float Damage(float damage)
	{
		m_HP -= damage;
		if (m_HP < 0)
		{
			GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
			GameManager.Instance.m_IsGameOver = true;
		}
		Mathf.Clamp01(m_HP);
		m_Renderer.material.SetColor("_BaseColor", m_Color.Evaluate(m_HP));
		return m_HP;
	}
}
