using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode {
	NoPlay,
	TwinRobot,
	HumanoidRobot,
	Combine,
	Release
}
public class GameManager : SingletonMonoBehaviour<GameManager> {

	public float m_TimeRate = 1.0f;
	public PlayMode m_PlayMode = PlayMode.NoPlay;
	public float m_CombineTime;
	public GameObject m_PlayCamera;
	public GameObject m_LRobot;
	public GameObject m_RRobot;
	public GameObject m_HumanoidRobot;
	[SerializeField]
	private LevelParameterTable m_LevelParameterTable;
	public int m_Level;
	public PlayerController m_PlayerController;

	public StageManager m_StageManger;

	public LevelParameter m_LevelParameter;

	public bool m_IsGameOver;
	public bool m_IsGameClear;

	public GameStarter m_GameStarter;

	bool m_IsRun;

	//	public LevelParameter LevelParameter { get { return m_LevelParameterTable.LPTable[m_Level]; } }
	public LevelParameter LevelParameter { get { return m_LevelParameter; } }

	// Use this for initialization
	void Start () {
		Update();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
            Pauser.Pause();
        if (Input.GetKeyDown(KeyCode.O))
            Pauser.Resume();

        if (m_IsGameClear || m_IsGameOver)
		{
			StartCoroutine(GameEnd());
		}
		if (m_LRobot == null)
		{
			m_LRobot = GameObject.Find("LRobot");
		}
		if (m_RRobot == null)
		{
			m_RRobot = GameObject.Find("RRobot");
		}
		if (m_HumanoidRobot == null)
		{
			m_HumanoidRobot = GameObject.Find("HumanoidRobot");
		}
	}

	IEnumerator GameEnd()
	{
		if (m_IsRun)
		{
			yield break;
		}
		m_IsRun = true;
		yield return new WaitForSeconds(3);
		m_GameStarter.ChangeScenes(0);
		m_IsGameClear = false;
		m_IsGameOver = false;
		m_IsRun = false;
	}
}
