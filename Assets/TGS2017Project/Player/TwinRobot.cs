﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwinRobotMode
{
	A,
	B
}
public class TwinRobot : MonoBehaviour
{
	[SerializeField] private PlayerController m_Controller;
    [SerializeField] Damageable m_CoreDamageComp;
    [SerializeField] private GameObject m_Shield;
	[SerializeField] private TwinRobotConfig m_Config;
	[SerializeField] private TwinRobotBaseConfig m_BaseConfig;
	[SerializeField] private float m_BreakerSizeS;
	[SerializeField] private float m_BreakerSizeL;    


	private float m_HP;
	private Renderer m_Renderer;
	private Rigidbody m_Rigidbody;
	private TwinRobotMode m_Mode;
	private float m_Axis;


	void Awake()
	{
		m_Renderer = m_Shield.GetComponent<Renderer>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Shield.GetComponent<Damageable>().Del_ReciveDamage = Damage;
        m_CoreDamageComp.Del_ReciveDamage = Damage;
        HP = m_BaseConfig.m_MaxHP;
	}

    private void OnEnable()
    {
        m_Shield.SetActive(HP != 0);
    }

    public void UpdateInput()
	{
		float axis = Input.GetAxis(m_Config.m_InputModeChange);
		if (axis >= 0.5f && m_Axis < 0.5f)
		{
			ModeChange();
		}
		m_Axis = axis;
	}

	private void ModeChange()
	{
		m_Mode = m_Mode == TwinRobotMode.A ? TwinRobotMode.B : TwinRobotMode.A;
	}

	public void Damage(float damage, MonoBehaviour src)
	{
		HP -= damage;
	}
	public void Move()
	{
		Vector3 move = new Vector3(
			Input.GetAxis(m_Config.m_InputHorizontal), 0,
			Input.GetAxis(m_Config.m_InputVertical));
		move = Vector3.ClampMagnitude(move, 1.0f) * m_BaseConfig.m_MoveSpeed * Time.fixedDeltaTime;

		m_Rigidbody.MovePosition(m_Rigidbody.position + move);
		if (move.magnitude != 0)
		{
			m_Rigidbody.MoveRotation(Quaternion.LookRotation(move));
		}
	}

	public void Look(Vector3 target, Quaternion quaternion)
	{
		m_Rigidbody.rotation = Quaternion.LookRotation(target - m_Rigidbody.position);
	}

	void OnCollisionEnter(Collision other)
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.TwinRobot:
				switch (other.gameObject.tag)
				{
					case "Enemy":
					case "Bullet":

						//HP -= 5f;
						break;
					default:
						break;
				}
				break;
			default:
				break;
		}

	}

    public void SetShieldActive(bool isActive)
    {
        m_Shield.SetActive(isActive && HP != 0);
    }

	private void OnTriggerEnter(Collider other)
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.Combine:
				switch (other.tag)
				{
					case "Wall":
						m_Controller.IsCanCrash = false;
						break;
					default:
						break;
				}
				break;
			default:
				break;
		}
	}




	void OnTriggerStay(Collider other)
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.Combine:
				switch (other.tag)
				{
					case "Wall":
						m_Controller.IsCanCrash = false;
						break;
					default:
						break;
				}
				break;
			default:
				break;
		}
	}

	public float HP
	{
		get { return m_HP; }
		set
		{
			m_HP = Mathf.Clamp(value, 0, m_BaseConfig.m_MaxHP);
			if (!m_Shield.activeSelf && m_HP <= 0 && GameManager.Instance.m_PlayMode != PlayMode.NoPlay)
			{
				GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
				GameManager.Instance.m_GameStarter.ChangeScenes(9);
			}
			ShieldUpdate();
		}
	}

	public TwinRobotMode Mode
	{
		get { return m_Mode; }
	}

	public float BreakerSize
	{
		get { return Mode == TwinRobotMode.A ? m_BreakerSizeS : m_BreakerSizeL; }
	}

	private void ShieldUpdate()
	{
		m_Shield.SetActive(HP != 0);
		m_Renderer.material.SetColor("_BaseColor", m_BaseConfig.m_ShieldColor.Evaluate(HP / m_BaseConfig.m_MaxHP));
	}
}
