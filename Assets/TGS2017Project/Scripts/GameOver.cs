using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameOver : MonoBehaviour {
	IEnumerator Start()
	{
		yield return new WaitForSeconds(GameManager.Instance.m_LoadingAnimationTime);
		yield return new WaitForSeconds(3);
		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}
}
