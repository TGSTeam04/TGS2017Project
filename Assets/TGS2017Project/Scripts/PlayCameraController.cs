using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraController : MonoBehaviour
{
	private Camera m_Camera;

	public Transform m_TopTransform;
	public Transform m_TPSTransform;
	public Transform m_TPSTarget;

	public AnimationCurve m_FOVCurve;
	public AnimationCurve m_CombinePositionCurve;
	public AnimationCurve m_RotationCurve;
	public AnimationCurve m_ReleasePositionCurve;

	public float m_MAXFOV;

	public float m_RotateSpeed;
	public float m_MoveSpeed;

	private PlayMode m_PreMode;
	private bool m_IsRunning = false;
	// Use this for initialization
	void Start()
	{
		m_Camera = GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				transform.position = m_TopTransform.transform.position;
				transform.rotation = m_TopTransform.transform.rotation;
				break;
			case PlayMode.HumanoidRobot:
				break;
			case PlayMode.Combine:
				if (m_PreMode == PlayMode.TwinRobot) StartCoroutine(Combine());
				break;
			case PlayMode.Release:
				if (m_PreMode != PlayMode.Release) StartCoroutine(Release());
				break;
			default:
				break;
		}
		m_PreMode = GameManager.Instance.m_PlayMode;
	}

	void FixedUpdate()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				break;
			case PlayMode.HumanoidRobot:
				transform.rotation = Quaternion.Slerp(transform.rotation, m_TPSTarget.rotation, m_RotateSpeed * Time.fixedDeltaTime);
				transform.position = Vector3.Lerp(transform.position, m_TPSTransform.position + transform.right * 2, m_MoveSpeed * Time.fixedDeltaTime);
				break;
			case PlayMode.Combine:
				break;
			case PlayMode.Release:
				break;
			default:
				break;
		}
	}

	IEnumerator Combine()
	{
		if (m_IsRunning) { yield break; }
		m_IsRunning = true;

		for (float f = 0; f < GameManager.Instance.m_CombineTime; f += Time.deltaTime)
		{
			m_Camera.fieldOfView = Mathf.Lerp(60, m_MAXFOV, m_FOVCurve.Evaluate(f));
			transform.position = Vector3.Lerp(m_TopTransform.position, m_TPSTransform.position, m_CombinePositionCurve.Evaluate(f));
			transform.rotation = Quaternion.Lerp(m_TopTransform.rotation, m_TPSTransform.rotation, m_RotationCurve.Evaluate(f));
			yield return null;
		}
		m_Camera.fieldOfView = 60;
		transform.position = m_TPSTransform.position;
		transform.rotation = m_TPSTransform.rotation;

		m_IsRunning = false;
	}

	IEnumerator Release()
	{
		if (m_IsRunning) { yield break; }
		m_IsRunning = true;

		for (float f = 0; f < GameManager.Instance.m_CombineTime; f += Time.deltaTime)
		{
			m_Camera.fieldOfView = Mathf.Lerp(60, 30 + m_MAXFOV / 2, m_FOVCurve.Evaluate(1 - f));
			transform.position = Vector3.Lerp(m_TPSTransform.position, m_TopTransform.position, m_ReleasePositionCurve.Evaluate(f));
			transform.rotation = Quaternion.Lerp(m_TopTransform.rotation, Quaternion.LookRotation(m_TPSTarget.position - transform.position), m_RotationCurve.Evaluate(1 - f));
			yield return null;
		}
		m_Camera.fieldOfView = 60;
		transform.position = m_TopTransform.position;
		transform.rotation = m_TopTransform.rotation;

		m_IsRunning = false;
	}
}
