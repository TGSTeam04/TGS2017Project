using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBreakParts : MonoBehaviour
{
	[SerializeField] private float m_DestroyTime = 1.0f;

	private float m_Timer;
	private bool m_IsFell;

	private void Start()
	{
		m_Timer = 0;
		m_IsFell = false;
	}

	private void Update()
	{
		if (!m_IsFell) return;

		m_Timer += Time.deltaTime;
		if (m_Timer <= m_DestroyTime) return;

		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Floor")
		{
			m_IsFell = true;
		}
	}
}
