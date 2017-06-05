using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 製作者：大格
/// 更新日：05/19
/// ステージのパネル
/// </summary>

[SelectionBase]
public class StagePanel : MonoBehaviour
{
    [HideInInspector]
    public static float m_InscribedR = 50;
    //private SubjectBase m_Subject;
    [HideInInspector]
    public List<Wall> m_Walls;
    //[SerializeField]
    //public int m_UseStageLevel = 0;

    private void Awake()
    {        
        m_Walls = new List<Wall>();
        //サブジェクトとして通知を受ける関数をバインド
        //m_Subject = new SubjectBase();
        //m_Subject.m_Del_OnRecive = ReciveNotice;
    }

    void Start()
    {
        StageManager stageManager = GameManager.Instance.m_StageManger;
        stageManager.m_ActivePanels.Add(this);
        /*ステージレベル仕様がなくなった為　コメントアウト*/
        //ステージマネージャーにパネルを登録
        //stageManager.m_StagePanels[m_UseStageLevel].Add(this);
        //壁を取得
        //Transform walls = transform.FindChild("Walls");
        //for (int i = 0; i < walls.childCount; i++)
        //{
        //    Wall wall = walls.GetChild(i).gameObject.GetComponent<Wall>();
        //    wall.m_UseStageLevel = m_UseStageLevel;
        //    m_Walls.Add(wall);
        //}
        //オブサーバーとバインド
        //m_Subject.BindObserver(stageManager.m_Observer);
        //使用するかcheck
        //if (m_UseStageLevel != 0)
        //    gameObject.SetActive(false);
        //else
        //    stageManager.m_ActivePanels.Add(this);
    }

    //private void ReciveNotice(string handle, object param)
    //{
    //    if (handle == "StageLevelUp")
    //    {
    //        //アクティブにするかcheck
    //        if ((int)param == m_UseStageLevel)
    //            gameObject.SetActive(true);
    //        //壁が壊せるようになるかcheck（全てのパネルが出現するまでまってから）
    //        if (gameObject.activeSelf)
    //            StartCoroutine(this.Delay(null, CheckNecessityWall));
    //    }
    //}

    //public void CheckNecessityWall()
    //{
    //    //print("CheckNecessityWall");
    //    for (int i = 0; i < m_Walls.Count; i++)
    //    {
    //        if (!m_Walls[i].CheckNecessity())
    //        {
    //            m_Walls.RemoveAt(i);
    //            i--;
    //        }
    //    }
    //}
}
