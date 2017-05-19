using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLine : MonoBehaviour {

	public Transform m_LRobot;
	public Transform m_RRobot;

	private LineRenderer m_LineRenderer;

	public float m_UpdateTime = 0.2f;

	public float m_Noise;

	public float m_OffsetX;

	private Vector3[] m_Positions;

	public bool m_Combine;


	// Use this for initialization
	void Start () {
		m_LineRenderer = GetComponent<LineRenderer>();
		StartCoroutine(UpdateLine());
	}
	
	// Update is called once per frame
	void Update () {
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
			case PlayMode.HumanoidRobot:
				m_LineRenderer.enabled = false;
				break;
			case PlayMode.TwinRobot:
			case PlayMode.Combine:
			case PlayMode.Release:
				if(m_Combine&&GameManager.Instance.m_PlayMode!= PlayMode.Combine)
				{
					m_LineRenderer.enabled = false;
					return;
				}
				m_LineRenderer.enabled = true;
				for (int i = 0; i < m_LineRenderer.positionCount; i++)
				{
					m_LineRenderer.SetPosition(i, Vector3.Lerp(m_LRobot.position + m_LRobot.right * m_OffsetX, m_RRobot.position + m_RRobot.right * -m_OffsetX, (float)i / (m_LineRenderer.positionCount - 1)) + m_Positions[i]);
				}

				break;
			default:
				break;
		}
	}

	private IEnumerator UpdateLine()
	{
		while (true)
		{
			int positions = m_LineRenderer.positionCount;
			m_LineRenderer.positionCount = (int)Vector3.Distance(m_LRobot.position, m_RRobot.position)+2;
			for (int i = positions; i < m_LineRenderer.positionCount; i++)
			{
				m_LineRenderer.SetPosition(i, m_RRobot.position);
			}
			m_LineRenderer.GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_LineRenderer.positionCount, 1);
			m_Positions = new Vector3[m_LineRenderer.positionCount];
			for (int i = 0; i < m_LineRenderer.positionCount; i++)
			{
				m_Positions[i] = Random.onUnitSphere * m_Noise;
			}
			yield return new WaitForSeconds(m_UpdateTime);
		}
	}
}
