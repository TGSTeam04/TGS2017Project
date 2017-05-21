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

	void ChangeScenes(int i)
	{
		foreach (var item in m_LoadedScenes)
		{
			SceneManager.UnloadSceneAsync(item);
		}
		m_LoadedScenes.Clear();
		foreach (var item in m_SceneListTable[i].Scene)
		{
			SceneManager.LoadScene(item, LoadSceneMode.Additive);
			m_LoadedScenes.Add(item);
		}
		
	}
}
