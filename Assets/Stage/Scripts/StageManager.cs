using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public ObserverBase m_Observer;

    [SerializeField]//ボスが出てくるStageLevel
    private int m_MaxStageLevel;
    [SerializeField]//ボスが出てくるStageLevel
    private int[] m_ChangeLevelKillNum;
    private int m_stageLevel;
    private int m_killNum;

    //GameManagerにStageManagerを保持させて、直接 Player か Enemy からEnemy死亡時に呼ぶ
    //またはObserverを利用して呼ぶ。
    public int m_KillNum
    {
        get { return m_killNum; }
        set
        {
            m_killNum = value;
            if (value > m_ChangeLevelKillNum[m_StageLevel])
                m_StageLevel++;
        }
    }

    //ステージレベルの変更がKill以外にあった場合publicに
    public int m_StageLevel
    {
        get { return m_stageLevel; }
        set
        {
            m_stageLevel = value;
            if (value > m_MaxStageLevel)
            {
                m_Observer.NotifyToSubjects("BossCommingLevel");
            }
            //print("StageLevelUp");
            m_Observer.NotifyToSubjects("StageLevelUp", m_stageLevel);
        }
    }

    private List<List<StagePanel>> m_StagePanels;

    private void Awake()
    {
        m_stageLevel = 0;
        m_Observer = new ObserverBase();
        TestGameManager.Instance.m_StageManager = this;
        m_Observer.m_ObserverName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "Manager";
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
            m_StageLevel += 1;
        }
    }
}
