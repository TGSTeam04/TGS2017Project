using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectMotionManager : MonoBehaviour {

    public GameObject m_Parts;
    public GameObject m_Star;
    public List<GameObject> m_PartsList = new List<GameObject>();
    public List<GameObject> m_Stars = new List<GameObject>();
    public List<GameObject> m_TutorialParts;
    public List<GameObject> m_StageLogoParts;
    public List<GameObject> m_TitleBack;

    public List<RectTransform> m_TutorialTrans;
    public List<RectTransform> m_StageLogoTrans;
    public List<RectTransform> m_Stage1Trans;
    public List<RectTransform> m_Stage2Trans;
    public List<RectTransform> m_Stage3Trans;
    public List<RectTransform> m_StarTrans;


    private bool m_SelectedTutorial = false;
    private bool m_SelectedTitleBack = false;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 74; i++)
        {
            m_PartsList.Add(Instantiate(m_Parts, transform.position + new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(-100.0f, 100.0f)), transform.rotation, transform));
        }
        for (int i = 0; i < 9; i++)
        {
            m_Stars.Add(Instantiate(m_Star, transform.position + new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(350.0f, 500.0f)), transform.rotation, transform));
        }
        for (int i = 0; i < ResultController.s_FirstStageLank; i++)
        {
            m_Stars[i].GetComponent<SelectMotion>().ChangeSprite();
        }
        for (int i = 0; i < ResultController.s_SecondStageLank; i++)
        {
            m_Stars[i + 3].GetComponent<SelectMotion>().ChangeSprite();
        }
        for (int i = 0; i < ResultController.s_ThirdStageLank; i++)
        {
            m_Stars[i + 6].GetComponent<SelectMotion>().ChangeSprite();
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < ResultController.s_FirstStageLank; i++)
        {
            m_Stars[i].GetComponent<SelectMotion>().ChangeSprite();
        }
        for (int i = 0; i < ResultController.s_SecondStageLank; i++)
        {
            m_Stars[i + 3].GetComponent<SelectMotion>().ChangeSprite();
        }
        for (int i = 0; i < ResultController.s_ThirdStageLank; i++)
        {
            m_Stars[i + 6].GetComponent<SelectMotion>().ChangeSprite();
        }
    }
    public void TutorialSelect()
    {
        foreach (GameObject parts in m_Stars)
        {
            parts.GetComponent<SelectMotion>().StarPosition();
        }
        foreach (GameObject parts in m_TutorialParts)
        {
            parts.GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        }
        foreach (GameObject parts in m_StageLogoParts)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }

        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion(m_PartsList);
        AllDiffusion(m_StageLogoParts);
        m_SelectedTutorial = true;
        for (int i = 0; i < m_TutorialTrans.Count; i++)
        {
            m_TutorialParts[i].GetComponent<SelectMotion>().m_Position = m_TutorialTrans[i].localPosition;
        }
        foreach (GameObject parts in m_PartsList)
        {
            parts.GetComponent<SelectMotion>().NextPosition();
            parts.GetComponent<SelectMotion>().m_IsStop = false;
        }
        AllDiffusion(m_StageLogoParts);
    }
    public void Select1()
    {
        foreach (GameObject parts in m_Stars)
        {
            parts.GetComponent<SelectMotion>().StarPosition();
        }
        foreach (GameObject parts in m_TutorialParts)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        }
        m_StageLogoParts[5].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        m_StageLogoParts[6].GetComponent<SelectMotion>().NormalColor();
        m_StageLogoParts[7].GetComponent<SelectMotion>().NormalColor();
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i].GetComponent<SelectMotion>().m_Position = m_StarTrans[i].localPosition;
            m_Stars[i].GetComponent<SelectMotion>().m_Rotation = m_StarTrans[i].localRotation;
        }

        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion(m_PartsList);
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage1Trans.Count; i++)
        {
            m_PartsList[i].GetComponent<SelectMotion>().m_Position = m_Stage1Trans[i].localPosition;
            m_PartsList[i].GetComponent<SelectMotion>().m_Rotation = m_Stage1Trans[i].localRotation;
            m_PartsList[i].GetComponent<SelectMotion>().m_IsStop = true;
        }

        StartCoroutine(SparkAndRebuilding());
        m_StageLogoParts[5].GetComponent<SelectMotion>().m_Position = m_StageLogoTrans[5].localPosition;
        m_StageLogoParts[6].GetComponent<SelectMotion>().NextPosition();
        m_StageLogoParts[7].GetComponent<SelectMotion>().NextPosition();

        AllDiffusion(m_TutorialParts);
    }
    public void Select2()
    {
        foreach (GameObject parts in m_Stars)
        {
            parts.GetComponent<SelectMotion>().StarPosition();
        }
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        }
        m_StageLogoParts[6].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        m_StageLogoParts[5].GetComponent<SelectMotion>().NormalColor();
        m_StageLogoParts[7].GetComponent<SelectMotion>().NormalColor();
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i + 3].GetComponent<SelectMotion>().m_Position = m_StarTrans[i].localPosition;
            m_Stars[i + 3].GetComponent<SelectMotion>().m_Rotation = m_StarTrans[i].localRotation;
        }

        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion(m_PartsList);
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage1Trans.Count; i++)
        {
            if (i < m_Stage2Trans.Count)
            {
                m_PartsList[i].GetComponent<SelectMotion>().m_Position = m_Stage2Trans[i].localPosition;
                m_PartsList[i].GetComponent<SelectMotion>().m_Rotation = m_Stage2Trans[i].localRotation;
                m_PartsList[i].GetComponent<SelectMotion>().m_IsStop = true;
            }
            else
            {
                m_PartsList[i].GetComponent<SelectMotion>().NextPosition();
                m_PartsList[i].GetComponent<SelectMotion>().m_IsStop = false;
            }
        }

        StartCoroutine(SparkAndRebuilding());
        m_StageLogoParts[6].GetComponent<SelectMotion>().m_Position = m_StageLogoTrans[5].localPosition;
        m_StageLogoParts[5].GetComponent<SelectMotion>().NextPosition();
        m_StageLogoParts[7].GetComponent<SelectMotion>().NextPosition();

        AllDiffusion(m_TutorialParts);
    }
    public void Select3()
    {
        foreach (GameObject parts in m_Stars)
        {
            parts.GetComponent<SelectMotion>().StarPosition();
        }
        foreach (GameObject parts in m_TutorialParts)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        }
        m_StageLogoParts[7].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        m_StageLogoParts[5].GetComponent<SelectMotion>().NormalColor();
        m_StageLogoParts[6].GetComponent<SelectMotion>().NormalColor();
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i + 6].GetComponent<SelectMotion>().m_Position = m_StarTrans[i].localPosition;
            m_Stars[i + 6].GetComponent<SelectMotion>().m_Rotation = m_StarTrans[i].localRotation;
        }

        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion(m_PartsList);
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage1Trans.Count; i++)
        {
            if (i < m_Stage3Trans.Count)
            {
                m_PartsList[i].GetComponent<SelectMotion>().m_Position = m_Stage3Trans[i].localPosition;
                m_PartsList[i].GetComponent<SelectMotion>().m_Rotation = m_Stage3Trans[i].localRotation;
                m_PartsList[i].GetComponent<SelectMotion>().m_IsStop = true;
            }
            else
            {
                m_PartsList[i].GetComponent<SelectMotion>().NextPosition();
                m_PartsList[i].GetComponent<SelectMotion>().m_IsStop = false;
            }
        }
        StartCoroutine(SparkAndRebuilding());
        m_StageLogoParts[7].GetComponent<SelectMotion>().m_Position = m_StageLogoTrans[5].localPosition;
        m_StageLogoParts[5].GetComponent<SelectMotion>().NextPosition();
        m_StageLogoParts[6].GetComponent<SelectMotion>().NextPosition();

        AllDiffusion(m_TutorialParts);
    }
    public void SelectTitle()
    {
        m_SelectedTitleBack = true;
        foreach (GameObject parts in m_StageLogoParts)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }
        foreach (GameObject parts in m_TutorialParts)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }
        foreach (GameObject parts in m_TitleBack)
        {
            parts.GetComponent<SelectMotion>().ChangeColor(Color.yellow);
        }
    }
    public void DeSelectTitle()
    {
        foreach (GameObject parts in m_TitleBack)
        {
            parts.GetComponent<SelectMotion>().NormalColor();
        }
    }
    public void AllDiffusion(List<GameObject> item) // パーツを散らす
    {
        foreach (GameObject parts in item)
        {
            parts.GetComponent<SelectMotion>().NextPosition();
        }
    }
    IEnumerator SparkAndRebuilding()
    {
        AllDiffusion(m_StageLogoParts);
        for (int i = 0; i < m_StageLogoTrans.Count; i++)
        {
            if (i < 5) m_StageLogoParts[i].GetComponent<SelectMotion>().NormalColor();
        }
        yield return new WaitForSeconds(0.4f);
        if (!m_SelectedTutorial)
        {
            for (int i = 0; i < m_StageLogoTrans.Count; i++)
            {
                if (i < 5)
                {
                    m_StageLogoParts[i].GetComponent<SelectMotion>().m_Position = m_StageLogoTrans[i].localPosition;
                    if (!m_SelectedTitleBack)
                    {
                        m_StageLogoParts[i].GetComponent<SelectMotion>().ChangeColor(Color.yellow);
                    }
                }
            }
        }
    }
}
