using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.SerializableAttribute]
public class SceneList
{
	public List<string> Scene = new List<string>();
	public SceneList(List<string> list)
	{
		Scene = list;
	}
}

public class GameStarter : MonoBehaviour {

	[SerializeField]
	private List<SceneList> m_SceneListTable = new List<SceneList>();

	private List<string> m_LoadedScenes = new List<string>();

	private AsyncOperation m_Async;

	[SerializeField]
	private string m_LoadingSceneName = "Loading";
	// Use this for initialization
	void Start () {
		ChangeScenes(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			ChangeScenes(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			ChangeScenes(0);
		}


	}

	public void ChangeScenes(int i)
	{
		//		SceneManager.LoadScene("Loading",LoadSceneMode.Additive);
		//foreach (var item in m_LoadedScenes)
		//{
		//	SceneManager.UnloadSceneAsync(item);
		//}
		//m_LoadedScenes.Clear();
		//foreach (var item in m_SceneListTable[i].Scene)
		//{
		//	SceneManager.LoadScene(item, LoadSceneMode.Additive);
		//	m_LoadedScenes.Add(item);
		//}
		//		SceneManager.UnloadSceneAsync("Loading");

		StartCoroutine(LoadScene(i));
	}

	IEnumerator LoadScene(int i)
	{
		GameManager.Instance.m_LoadingProgress = 0;
		m_Async = SceneManager.LoadSceneAsync(m_LoadingSceneName, LoadSceneMode.Additive);
		yield return new WaitUntil(() => m_Async.isDone);

		float time = GameManager.Instance.m_LoadingAnimationTime;
		yield return new WaitForSeconds(time);

		int progresscount =m_LoadedScenes.Count + m_SceneListTable[i].Scene.Count;
		int count = 0;
		foreach (var item in m_LoadedScenes)
		{
			m_Async = SceneManager.UnloadSceneAsync(item);
			while (!m_Async.isDone)
			{
				GameManager.Instance.m_LoadingProgress = (m_Async.progress + count) / progresscount;
				yield return null;
			}
			count++;
		}
		m_LoadedScenes.Clear();
		foreach (var item in m_SceneListTable[i].Scene)
		{
			m_Async = SceneManager.LoadSceneAsync(item, LoadSceneMode.Additive);
			while (!m_Async.isDone)
			{
				GameManager.Instance.m_LoadingProgress = (m_Async.progress + count) / progresscount;
				yield return null;
			}
			count++;
			m_LoadedScenes.Add(item);
		}
		GameManager.Instance.m_LoadingProgress = 1;

		yield return new WaitForSeconds(time);
		yield return null;

		m_Async = SceneManager.UnloadSceneAsync(m_LoadingSceneName);
		yield return new WaitUntil(() => m_Async.isDone);
		GameManager.Instance.m_LoadingProgress = 0;

	}
}
