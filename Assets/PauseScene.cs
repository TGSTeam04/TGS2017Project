using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PauseScene : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.m_PlayMode != PlayMode.NoPlay)
		{
			if (Input.GetButtonDown("Pause"))
			{
				bool isPause = Pauser.s_TargetByTag[PauseTag.Pause].m_IsPause;
				if (isPause)
				{
					Pauser.Resume();
					GameManager.Instance.m_GameStarter.RemoveScene("Pause");
				}
			}
		}
	}
}
