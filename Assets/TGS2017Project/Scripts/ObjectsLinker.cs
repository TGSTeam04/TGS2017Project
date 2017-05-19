using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsLinker : MonoBehaviour {

	[SerializeField]
	public Transform[] m_TopTransform;
	public Transform m_TPSTransform;
	public Transform m_TPSTarget;
	public GameObject m_LRobot;
	public GameObject m_RRobot;
	public GameObject m_HumanoidRobot;
	public StageManager m_StageManager;

	private PlayCameraController m_Camera;
	// Use this for initialization
	void Awake () {
		m_Camera = GameManager.Instance.m_PlayCamera.GetComponent<PlayCameraController>();
		if (m_TopTransform.Length>0) m_Camera.m_TopTransform = m_TopTransform;
		if (m_TPSTarget != null) m_Camera.m_TPSTarget = m_TPSTarget;
		if (m_TPSTransform != null) m_Camera.m_TPSTransform = m_TPSTransform;
		if (m_LRobot != null) GameManager.Instance.m_LRobot = m_LRobot;
		if (m_RRobot != null) GameManager.Instance.m_RRobot = m_RRobot;
		if (m_HumanoidRobot != null) GameManager.Instance.m_HumanoidRobot = m_HumanoidRobot;
		if (m_StageManager != null) GameManager.Instance.m_StageManger = m_StageManager;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
