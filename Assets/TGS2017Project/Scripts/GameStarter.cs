using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

public class GameStarter : MonoBehaviour
{

    [SerializeField]
    private int m_StartScene = 0;
    [SerializeField]
    private List<SceneList> m_SceneListTable = new List<SceneList>();

    private List<string> m_LoadedScenes = new List<string>();

    private AsyncOperation m_Async;

    [SerializeField]
    private string m_LoadingSceneName = "Loading";

    private bool m_Loading;

    private Scene m_Common;
    // Use this for initialization
    void Start()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		//Screen.lockCursor = false;
		m_Common = SceneManager.GetActiveScene();

		//SceneManager.LoadScene("Title", LoadSceneMode.Additive);
		//m_LoadedScenes.Add("Title");
        ChangeScenes(m_StartScene);
    }

    // Update is called once per frame
    void Update()
    {
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
		{
			//Debug.Log("Reselecting first input");
			EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
		}
			//if (Input.GetKeyDown(KeyCode.Alpha0))
			//{
			//    ChangeScenes(0);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha1))
			//{
			//    ChangeScenes(1);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha2))
			//{
			//    ChangeScenes(2);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha3))
			//{
			//    ChangeScenes(3);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha4))
			//{
			//    ChangeScenes(4);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha5))
			//{
			//    ChangeScenes(5);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha6))
			//{
			//    ChangeScenes(6);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha7))
			//{
			//    ChangeScenes(7);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha8))
			//{
			//    ChangeScenes(8);
			//}
			//if (Input.GetKeyDown(KeyCode.Alpha9))
			//{
			//    ChangeScenes(9);
			//}
		}

    public void ChangeScenes(int i)
    {
        StartCoroutine(LoadScene(i));
    }

    private IEnumerator LoadScene(int i)
    {
        GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
        if (m_Loading) yield break;
        m_Loading = true;
        GameManager.Instance.m_LoadingProgress = 0;
        m_Async = SceneManager.LoadSceneAsync(m_LoadingSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => m_Async.isDone);

        float time = GameManager.Instance.m_LoadingAnimationTime;
        yield return new WaitForSeconds(time);
        yield return null;

        SceneManager.SetActiveScene(m_Common);

        //アンロード
        int progresscount = m_LoadedScenes.Count + m_SceneListTable[i].Scene.Count;
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

        //ロード
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

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(time);

        m_Async = SceneManager.UnloadSceneAsync(m_LoadingSceneName);
        yield return new WaitUntil(() => m_Async.isDone);
        GameManager.Instance.m_LoadingProgress = 0;
        m_Loading = false;
    }
    //追加でロード
    public AsyncOperation AddScene(string name)
    {
        m_LoadedScenes.Add(name);
        return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }
    public AsyncOperation RemoveScene(string name)
    {
        m_LoadedScenes.Remove(name);
        return SceneManager.UnloadSceneAsync(name);
    }
}
