using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    private SubjectBase m_Subject;
    //[SerializeField]
    //private float[] m_ScaleByStageLeve = { 1, 2, 5, 8, 10 };
    [SerializeField]
    private float m_ReSpawnWaitTime = 1.0f;
    protected override void Awake()
    {
        base.Awake();
        m_Subject = new SubjectBase();
        m_Subject.m_Del_OnRecive = OnReciveFromStageManager;
    }

    private void Start()
    {
        m_Subject.BindObserver(GameManager.Instance.m_StageManger.m_Observer);
    }
    private void OnReciveFromStageManager(string handle, object pram)
    {
    }

    public void ReSpawnEnemy(EnemyBase enemy)
    {
        StartCoroutine(this.Delay<EnemyBase>(
            new WaitForSeconds(m_ReSpawnWaitTime),
            ReSpawn, enemy));
    }

    void ReSpawn(EnemyBase enemy)
    {
        StageManager stageManager = GameManager.Instance.m_StageManger;
        
        //アクティブなパネルのうちランダムなものを取得
        List<StagePanel> panels = stageManager.m_ActivePanels;
        StagePanel panel = panels[Random.Range(0, panels.Count)];
        Transform panelTrans = panel.gameObject.transform;
        //パネルの中心から内接円の半径以下の距離の場所に移動
        float distance = Random.Range(0, panelTrans.lossyScale.x * StagePanel.m_InscribedR / 2f);
        Vector3 randVec = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
        Vector3 resPos = panelTrans.position + (randVec.normalized * distance);

        enemy.transform.position = resPos;
        enemy.GetComponent<EnemyBase>().NextTarget();
        enemy.GetComponent<EnemyBase>().m_IsDead = false;
        //enemy.transform.localScale = Vector3.one * 0.4f;// * scale;
        enemy.gameObject.SetActive(true);
    }
}
