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
    private string m_NextScene;
    public string GetNextScene
    {
        get { return m_NextScene; }
    }

    void Awake()
    {
//        DontDestroyOnLoad(this);
    }

    // シーンの変更
    public void ChangeSceneWithLoad(string scene)
    {
        SceneManager.LoadScene("Loading");
        m_NextScene = scene;
    }

	public void ChangeSceneWithLoad(int scene)
	{
		GameManager.Instance.m_GameStarter.ChangeScenes(scene);
	}

	public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
