using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmState
{
	Idle,
	Fire,
	Back
}
public class Arm : MonoBehaviour
{
	public ArmState m_ArmState;
	public Transform m_FirePosition;
	private Rigidbody m_Rigidbody;
	public float m_Speed;
	public float m_BackSpeed;
	public float m_Time;

	private float m_Timer;
	// Use this for initialization
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		m_ArmState = ArmState.Idle;
		gameObject.SetActive(false);
	}

	void Update()
	{
		switch (m_ArmState)
		{
			case ArmState.Idle:
				break;
			case ArmState.Fire:
				m_Timer += Time.deltaTime;
				if (m_Timer > m_Time)
				{
					Hit();
				}
				break;
			case ArmState.Back:
				transform.LookAt(m_FirePosition);
				break;
			default:
				break;
		}
	}

	void FixedUpdate()
	{
		switch (m_ArmState)
		{
			case ArmState.Idle:
				break;
			case ArmState.Fire:
				m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Speed * Time.fixedDeltaTime);
				break;
			case ArmState.Back:
				m_Rigidbody.MovePosition(m_Rigidbody.position + (m_FirePosition.position - m_Rigidbody.position).normalized * m_BackSpeed * Time.fixedDeltaTime);
				if (Vector3.Distance(m_Rigidbody.position, m_FirePosition.position) < m_BackSpeed * Time.fixedDeltaTime)
				{
					m_ArmState = ArmState.Idle;
					gameObject.SetActive(false);
				}
				break;
			default:
				break;
		}
	}

	public void Fire()
	{
		transform.position = m_FirePosition.position;
		transform.rotation = m_FirePosition.rotation;
		m_ArmState = ArmState.Fire;
		m_Timer = 0;
	}
	public void Hit()
	{
		m_ArmState = ArmState.Back;
	}
}
