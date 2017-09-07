using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 製作者：大格
/// 更新日時：05/20
/// ステージのマネージャ
/// </summary>

public class StageManager : MonoBehaviour
{
    public Transform m_ClearPerformLoc;
    public GameObject m_Boss;
    public ObserverBase m_Observer;
    private int m_killNum;
    [SerializeField] GameObject m_BGM;

    [HideInInspector]
    public List<StagePanel> m_ActivePanels;

    //GameManagerにStageManagerを保持させて、直接 Player か Enemy からEnemy死亡時に呼ぶ
    //またはObserverを利用して呼ぶ。
    public int KillNum
    {
        get { return m_killNum; }
        set
        {
            m_killNum = value;
        }
    }

    private void Awake()
    {
        m_Observer = new ObserverBase();
        m_ActivePanels = new List<StagePanel>();
        m_Observer.ObserverName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "Manager";
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.m_PlayMode != PlayMode.NoPlay)
        {
            if (Input.GetButtonDown("Pause"))
            {
                bool isPause = Pauser.s_TargetByTag[PauseTag.Pause].m_IsPause;
                if (!isPause)
                {
					Pauser.Pause();
					GameManager.Instance.m_GameStarter.AddScene("Pause");
                }
     //           else
     //           {
					//Pauser.Resume();
					//GameManager.Instance.m_GameStarter.RemoveScene("Pause");
     //           }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            bool isPause = Pauser.s_TargetByTag[PauseTag.Enemy].m_IsPause;
            if (!isPause)
                Pauser.Pause(PauseTag.Enemy);
            else
                Pauser.Resume(PauseTag.Enemy);
        }
    }

    public void StageClear()
    {
        m_BGM.GetComponent<AudioSource>().enabled = false;
    }

    private void OnDestroy()
    {
        //全てのサブジェクトとのBindを切る
        m_Observer.ClearSubject();
    }

    //ステージレベル仕様がなくなった為削除
    //（色々なクラスで参照しているので、他のクラスが仕様削除に対応でき次第削除）
    public int StageLevel
    {
        get; set;
    }
}
