﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayMode
{
    NoPlay,
    TwinRobot,
    HumanoidRobot,
    Combine,
    Release,
    Clear
}
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public float m_TimeRate = 1.0f;
    public PlayMode m_PlayMode = PlayMode.NoPlay;
    public float m_CombineTime;
    public GameObject m_PlayCamera;
    public GameObject m_LRobot;
    public GameObject m_RRobot;
    public GameObject m_HumanoidRobot;
    public int m_Level;
    public PlayerController m_PlayerController;

    public StageManager m_StageManger;

    public LevelParameter m_LevelParameter;

    public bool m_IsGameOver;
    public bool m_IsGameClear;

    public GameStarter m_GameStarter;

    bool m_IsRun;

    [SerializeField]
    public BreakEnemyTable m_BreakEnemyTable;

    //	public LevelParameter LevelParameter { get { return m_LevelParameterTable.LPTable[m_Level]; } }
    public LevelParameter LevelParameter { get { return m_LevelParameter; } }

    public float m_LoadingAnimationTime;
    public float m_LoadingProgress;

    public int m_PlayScore;
    public float m_PlayTime;
    public float m_BossHpRate = 1.0f;

    public static int s_StageNumber;

    // Use this for initialization
    void Start()
    {
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_PlayMode = PlayMode.Clear;
        }
        if (m_PlayMode != PlayMode.NoPlay)
            m_PlayTime += Time.deltaTime;

        switch (m_PlayMode)
        {
            case PlayMode.NoPlay:
                break;
            case PlayMode.TwinRobot:
            case PlayMode.HumanoidRobot:
                break;
            case PlayMode.Combine:
            case PlayMode.Release:
                break;
            case PlayMode.Clear:
                Clear();
                break;
            default:
                break;
        }

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

    void Clear()
    {
        m_PlayCamera.GetComponent<PlayCameraController>().enabled = false;
        m_PlayCamera.GetComponent<Clear>().ClearMove();
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
        if (m_IsGameClear)
        {
            m_GameStarter.ChangeScenes(8);
        }
        m_IsGameClear = false;
        m_IsGameOver = false;
        m_IsRun = false;
    }
}
