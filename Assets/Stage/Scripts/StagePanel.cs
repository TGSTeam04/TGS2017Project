using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StagePanel : MonoBehaviour
{
    private SubjectBase m_Subject;
    [HideInInspector]
    public List<Wall> m_Walls;
    [SerializeField]
    public int m_UseStageLevel = 0;

    private void Awake()
    {
        m_Subject = new SubjectBase();
        m_Walls = new List<Wall>();
        m_Subject.m_Del_OnRecive = ReciveNotice;
    }

    // Use this for initialization
    void Start()
    {
        TestGameManager.Instance.m_StageManager.m_Observer.BindSubject(m_Subject);

        Transform walls = transform.FindChild("Walls");
        for (int i = 0; i < walls.childCount; i++)
        {
            Wall wall = walls.GetChild(i).gameObject.GetComponent<Wall>();
            wall.m_UseStageLevel = m_UseStageLevel;
            m_Walls.Add(wall);
        }

        if (m_UseStageLevel != 0)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("2"))
        {
            RaycastHit info;
            if (Physics.Raycast(transform.position + Vector3.up * 2, transform.forward, out info, Mathf.Infinity))
                print(name + "test ray cast is " + LayerMask.LayerToName(info.collider.gameObject.layer));
            Debug.DrawRay(transform.position, transform.right * 1000, Color.blue, 10.0f);
        }
    }

    private void ReciveNotice(string handle, object param)
    {
        if (handle == "StageLevelUp")
        {
            if ((int)param == m_UseStageLevel)
                gameObject.SetActive(true);
            if (gameObject.activeSelf)
                //全てのStagePanelの生成を待つ
                StartCoroutine(this.Delay(null, CheckNecessityWall));
        }
    }

    public void CheckNecessityWall()
    {
        print("CheckNecessityWall");
        for (int i = 0; i < m_Walls.Count; i++)
        {
            if (!m_Walls[i].CheckNecessity())
            {
                m_Walls.RemoveAt(i);
                i--;
            }
        }
    }

    //public void RemoveWall(Wall wall)
    //{
    //    for (int i = 0; i < m_Walls.Count; i++)
    //    {
    //        if (m_Walls[i] == wall)
    //        {
    //            m_Walls.RemoveAt(i);
    //        }
    //    }
    //}
}
