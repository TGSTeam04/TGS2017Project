using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 電撃線の制御
/// 製作者：野澤翔太
/// </summary>
public class ElectricLine : MonoBehaviour
{
	public LineRenderer m_LineRenderer;

	public Transform m_LRobot;
	public Transform m_RRobot;

	public float m_UpdateTime = 0.2f;

	public float m_Noise;

	public float m_PositionCount;

	public float m_OffsetX;

	public bool m_Combine;

	public float m_Scale;

	public int m_Row;


	private Vector3[] m_Noises;

	IEnumerator Start()
	{
		m_LineRenderer.material.mainTextureScale = new Vector2(m_Scale, 1.0f / m_Row);

		Vector2[] textureOffsets = new Vector2[m_Row];
		for (int i = 0; i < m_Row; i++)
		{
			textureOffsets[i] = new Vector2(0, (float)i / m_Row);
		}

		Vector3[] randomPositions = new Vector3[10];
		for (int i = 0; i < randomPositions.Length; i++)
		{
			randomPositions[i] = Random.onUnitSphere;
		}

		int index = 0;
		WaitForSeconds wait = new WaitForSeconds(m_UpdateTime);
		while (true)
		{
			int i = m_LineRenderer.positionCount;
			m_LineRenderer.positionCount = (int)(Vector3.Distance(m_LRobot.position, m_RRobot.position) * m_PositionCount) + 2;
			for (; i < m_LineRenderer.positionCount; i++)
			{
				m_LineRenderer.SetPosition(i, m_RRobot.position);
			}

			index++;
			if (index >= m_Row) index = 0;
			m_LineRenderer.material.mainTextureOffset = textureOffsets[index];

			m_Noises = new Vector3[m_LineRenderer.positionCount];
			for (i = 0; i < m_LineRenderer.positionCount; i++)
			{
				m_Noises[i] = randomPositions[i * (int)Time.time % 10] * m_Noise;
			}
			yield return wait;
		}
	}

	void Update()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
			case PlayMode.HumanoidRobot:
				m_LineRenderer.enabled = false;
				break;
			case PlayMode.TwinRobot:
			case PlayMode.Release:
				if (m_Combine)
				{
					m_LineRenderer.enabled = false;
					return;
				}
				LineMove();
				break;
			case PlayMode.Combine:
				LineMove();
				break;
			default:
				break;
		}
	}

	private void LineMove()
	{
		m_LineRenderer.enabled = true;

		Vector3 offset = Vector3.Cross(Vector3.Normalize(m_LRobot.position - m_RRobot.position), Vector3.up) * m_OffsetX;
		Vector3 A = m_LRobot.position + offset;
		Vector3 B = m_RRobot.position + offset;
		for (int i = 0; i < m_LineRenderer.positionCount; i++)
		{
			m_LineRenderer.SetPosition(i, Vector3.Lerp(A, B, (float)i / (m_LineRenderer.positionCount - 1)) + m_Noises[i]);
		}
	}
}
