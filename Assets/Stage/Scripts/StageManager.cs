using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 製作者：大格
/// 更新日時：05/20
/// ステージのマネージャ
/// </summary>

public class StageManager : MonoBehaviour
{
    public ObserverBase m_Observer;

    [SerializeField]//ボスが出てくるStageLevel
    private int m_MaxStageLevel;
    [SerializeField]//次のレベルに進むまでのKill数
    private int[] m_ChangeLevelKillNum;
    private int m_stageLevel;
    private int m_killNum;

    //ステージパネル
    [HideInInspector]
    public List<List<StagePanel>> m_StagePanels;
    public List<StagePanel> m_ActivePanels;

    //GameManagerにStageManagerを保持させて、直接 Player か Enemy からEnemy死亡時に呼ぶ
    //またはObserverを利用して呼ぶ。
    public int KillNum
    {
        get { return m_killNum; }
        set
        {
            m_killNum = value;
            if (value > m_ChangeLevelKillNum[StageLevel] && StageLevel < m_MaxStageLevel)
                StageLevel++;
        }
    }

    //ステージレベルの変更がKill以外にあった場合publicに
    public int StageLevel
    {
        get { return m_stageLevel; }
        set
        {
            m_stageLevel = value;
            m_Observer.NotifyToSubjects("StageLevelUp", m_stageLevel);
            if (m_stageLevel >= m_MaxStageLevel)
            {
                m_Observer.NotifyToSubjects("BossCommingLevel");
            }
            m_ActivePanels.AddRange(m_StagePanels[m_stageLevel]);
        }
    }

    private void Awake()
    {
        m_stageLevel = 0;
        m_Observer = new ObserverBase();
        m_StagePanels = new List<List<StagePanel>>();
        m_ActivePanels = new List<StagePanel>();
        for (int i = 0; i < m_MaxStageLevel; i++)
        {
            m_StagePanels.Add(new List<StagePanel>());
        }
        m_Observer.ObserverName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "Manager";
    }

    // Use this for initialization
    void Start()
    {
        //m_Observer.NotifyToSubjects("")
    }

    // Update is called once per frame
    void Update()
    {
        //デバッグ用処理
        if (Input.GetKeyDown("1"))
        {
            StageLevel += 1;
        }
    }

    private void OnDestroy()
    {
        //全てのサブジェクトとのBindを切る
        m_Observer.ClearSubject();
    }
}
