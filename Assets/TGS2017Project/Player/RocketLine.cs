using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RocketLine : MonoBehaviour
{
	[SerializeField] RocketBattery m_RocketBattery;
	[SerializeField] LineRenderer m_LineRendererL;
	[SerializeField] LineRenderer m_LineRendererR;
	[SerializeField] Transform m_HumanoidRobot;
	Transform m_PlayCamera;
	[SerializeField] LayerMask m_HitLayer;
	[SerializeField] float m_Distance;
	Vector3 m_HitPosition;
	bool m_FireL = true;
	[SerializeField] Vector3 m_Offset;
	void Start()
	{
		m_PlayCamera = GameManager.Instance.m_PlayCamera.transform;
	}

	void Update()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.HumanoidRobot:
				m_LineRendererL.enabled = true;
				m_LineRendererR.enabled = true;

				RaycastHit raycast;
				if (Physics.Raycast(m_PlayCamera.position, m_PlayCamera.forward, out raycast, m_Distance, m_HitLayer.value))
				{
					m_HitPosition = raycast.point;
					if (m_RocketBattery.LIsCanFire)
					{
						m_LineRendererL.SetPositions(new Vector3[] { m_RocketBattery.m_LRocket.m_StandTrans.position + m_Offset, m_HitPosition });
					}
					if (m_RocketBattery.RIsCanFire)
					{
						m_LineRendererR.SetPositions(new Vector3[] { m_RocketBattery.m_RRocket.m_StandTrans.position + m_Offset, m_HitPosition });
					}
					if (Input.GetButtonDown("ChargeL"))
					{
						m_FireL = true;
					}
					else if (Input.GetButtonDown("ChargeR"))
					{
						m_FireL = false;
					}
					if (m_FireL)
					{
						transform.rotation = Quaternion.LookRotation(m_HitPosition - m_RocketBattery.m_LRocket.m_StandTrans.position - m_Offset);
					}
					else
					{
						transform.rotation = Quaternion.LookRotation(m_HitPosition - m_RocketBattery.m_RRocket.m_StandTrans.position - m_Offset);
					}
				}
				else
				{
					if (m_RocketBattery.LIsCanFire)
					{
						m_LineRendererL.SetPositions(new Vector3[] { m_RocketBattery.m_LRocket.m_StandTrans.position + m_Offset, m_RocketBattery.m_LRocket.m_StandTrans.position + m_Offset + m_HumanoidRobot.forward * m_Distance });
					}
					if (m_RocketBattery.RIsCanFire)
					{
						m_LineRendererR.SetPositions(new Vector3[] { m_RocketBattery.m_RRocket.m_StandTrans.position + m_Offset, m_RocketBattery.m_RRocket.m_StandTrans.position + m_Offset + m_HumanoidRobot.forward * m_Distance });
					}
					transform.localRotation = m_HumanoidRobot.rotation;
				}
				m_LineRendererL.enabled = m_RocketBattery.LIsCanFire;
				m_LineRendererR.enabled = m_RocketBattery.RIsCanFire;

				break;
			case PlayMode.NoPlay:
			case PlayMode.TwinRobot:
			case PlayMode.Combine:
			case PlayMode.Release:
			default:
				m_LineRendererL.enabled = false;
				m_LineRendererR.enabled = false;
				break;
		}
	}
}
