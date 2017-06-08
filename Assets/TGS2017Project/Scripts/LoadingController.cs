using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadingController : MonoBehaviour {
	public enum LoadingType
	{
		Shutter,
		Fade
	}
	[SerializeField]
	private LoadingType m_LoadingType;
	[SerializeField]
	private RectTransform m_Shutter;
	[SerializeField]
	private AnimationCurve m_ShutterCurve;
	[SerializeField]
	private float m_Time;
	[SerializeField]
	private float m_T;

	// Use this for initialization
	void Start () {
		GameManager.Instance.m_LoadingAnimationTime = m_Time;
		switch (m_LoadingType)
		{
			case LoadingType.Shutter:
				break;
			case LoadingType.Fade:
				break;
			default:
				break;
		}
		StartCoroutine(In());

	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch (m_LoadingType)
		{
			case LoadingType.Shutter:
				m_Shutter.sizeDelta = new Vector2(Mathf.Lerp(3840, 1920, m_ShutterCurve.Evaluate( m_T)), 0);
				break;
			case LoadingType.Fade:
				break;
			default:
				break;
		}

	}

	IEnumerator In()
	{
		for (float t = Time.deltaTime; t < m_Time; t += Time.deltaTime)
		{
			m_T = t / m_Time;
			yield return null;
		}
		m_T = 1;
		StartCoroutine(Out());
	}

	IEnumerator Out()
	{
		yield return new WaitUntil(() => GameManager.Instance.m_LoadingProgress == 1);
		for (float t = Time.deltaTime; t < m_Time; t += Time.deltaTime)
		{
			m_T = 1 - t / m_Time;
			yield return null;
		}
		m_T = 0;
	}
}

