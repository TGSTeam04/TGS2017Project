using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スクリプト：シーン管理者
/// 作成者：Ho Siu Ki（何兆祺）
/// </summary>

public class SceneController : MonoBehaviour
{
	public void ChangeSceneWithLoad(int scene)
	{
		GameManager.Instance.m_GameStarter.ChangeScenes(scene);
	}
}
