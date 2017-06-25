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

	public float m_Scale;

	public int m_Row;

	private int m_index;
	private Vector2[] offsets;

	public float m_PositionCount;


	// Use this for initialization
	void Start () {
		m_LineRenderer = GetComponent<LineRenderer>();
		m_index = 0;
		offsets = new Vector2[m_Row];
		for (int i = 0; i < m_Row; i++)
		{
			offsets[i] = new Vector2(0,(float)i/m_Row);
		}
		m_LineRenderer.material.mainTextureScale = new Vector2(m_Scale, 1.0f / m_Row);
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
				//				m_LineRenderer.enabled = true;
				if (m_Combine)
				{
					m_LineRenderer.enabled = true;
				}
				else
				{
					m_LineRenderer.enabled = !m_LineRenderer.enabled;
				}
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
			m_LineRenderer.positionCount = (int)(Vector3.Distance(m_LRobot.position, m_RRobot.position)*m_PositionCount)+2;
			for (int i = positions; i < m_LineRenderer.positionCount; i++)
			{
				m_LineRenderer.SetPosition(i, m_RRobot.position);
			}
			//m_LineRenderer.GetComponent<Renderer>().material.mainTextureScale = new Vector2(m_LineRenderer.positionCount, 1);
			m_index++;
			if (m_index >= m_Row) m_index = 0;
			m_LineRenderer.material.mainTextureOffset = offsets[m_index];
			m_Positions = new Vector3[m_LineRenderer.positionCount];
			for (int i = 0; i < m_LineRenderer.positionCount; i++)
			{
				m_Positions[i] = Random.onUnitSphere * m_Noise;
			}
			yield return new WaitForSeconds(m_UpdateTime);
		}
	}
}
