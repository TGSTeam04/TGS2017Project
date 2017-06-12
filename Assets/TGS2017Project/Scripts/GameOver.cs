using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour {
	[SerializeField]
	private Image m_Image;
	IEnumerator Start()
	{
		m_Image.color = Color.clear;
		yield return new WaitForSeconds(GameManager.Instance.m_LoadingAnimationTime);
		for (float t = 0; t < 1.0f; t+=Time.deltaTime)
		{
			m_Image.color = Color.Lerp(Color.clear, Color.white, t);
			yield return null;
		}
		m_Image.color = Color.white;
		yield return new WaitForSeconds(1);
		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}
}
