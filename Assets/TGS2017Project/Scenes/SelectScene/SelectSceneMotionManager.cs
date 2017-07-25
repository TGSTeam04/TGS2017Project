using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectSceneMotionManager : MonoBehaviour {

    public GameObject m_Hexagon;
    public GameObject m_BlackHexagon;
    public GameObject m_Star;

    public List<GameObject> m_HexagonParts = new List<GameObject>();
    public List<GameObject> m_BlackHexagonParts = new List<GameObject>();
    public List<GameObject> m_Stars = new List<GameObject>();
    public List<GameObject> m_TutorialLogoParts;
    public List<GameObject> m_TitleBackLogoParts;
    public List<GameObject> m_StageLogoParts;
    public List<GameObject> m_ExtraLogoParts;
    public List<GameObject> m_ExtraParts;
    public List<GameObject> m_AllParts;

    public List<RectTransform> m_Tutorial_Logo_Pos;
    public List<RectTransform> m_Stage1_Hexagon_Pos;
    public List<RectTransform> m_Stage2_Hexagon_Pos;
    public List<RectTransform> m_Stage3_Hexagon_Pos;
    public List<RectTransform> m_Stage_Logo_Pos;
    public List<RectTransform> m_EX_Stage_Logo_Pos;
    public List<RectTransform> m_Star_Pos;

    public Sprite m_Sprite;

    private bool m_SelectedTutorial;
    private bool m_SelectedTitleBack;
    private bool m_SelectedEX;
    private bool num = false;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 74; i++)
        {
            m_HexagonParts.Add(Instantiate(m_Hexagon, transform.position, transform.rotation, transform));
        }
        for (int i = 0; i < 37; i++)
        {
            m_BlackHexagonParts.Add(Instantiate(m_BlackHexagon, transform.position, transform.rotation, transform));
        }
        for (int i = 0; i < 9; i++)
        {
            m_Stars.Add(Instantiate(m_Star, transform.position, transform.rotation, transform));
        }

        for (int i = 0; i < m_HexagonParts.Count; i++)
        {
            m_AllParts.Add(m_HexagonParts[i]);
        }
        for (int i = 0; i < m_TutorialLogoParts.Count; i++)
        {
            m_AllParts.Add(m_TutorialLogoParts[i]);
        }
        for (int i = 0; i < m_StageLogoParts.Count; i++)
        {
            m_AllParts.Add(m_StageLogoParts[i]);
        }
        for (int i = 0; i < m_BlackHexagonParts.Count; i++)
        {
            m_ExtraParts.Add(m_BlackHexagonParts[i]);
        }
        for (int i = 0; i < m_ExtraLogoParts.Count; i++)
        {
            m_ExtraParts.Add(m_ExtraLogoParts[i]);
        }
        foreach (GameObject item in m_AllParts)
        {
            if (Random.Range(0, 1) == 1)
            {
                num = true;
            }
            else
            {
                num = false;
            }
            item.GetComponentInChildren<Animator>().speed = num ? Random.Range(-1.0f, -0.3f) : Random.Range(0.3f, 1.0f);
        }
        foreach(GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        AllDiffusion();
        ExDiffusion();
    }
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject item in m_AllParts)
        {
            if (Vector3.Distance(item.transform.localPosition, item.GetComponent<Parts>().m_Position) > 0.1f || Mathf.Abs(Quaternion.Angle(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation)) > 0.1f)
            {
                item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, item.GetComponent<Parts>().m_Position, 3f * Time.deltaTime);
                item.transform.localRotation = Quaternion.Slerp(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation, 2f * Time.deltaTime);
            }
            else
            {
                item.transform.localPosition = item.GetComponent<Parts>().m_Position;
                item.transform.localRotation = item.GetComponent<Parts>().m_Rotation;
            }
        }
        foreach (GameObject item in m_ExtraParts)
        {
            if (Vector3.Distance(item.transform.localPosition, item.GetComponent<Parts>().m_Position) > 0.1f || Mathf.Abs(Quaternion.Angle(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation)) > 0.1f)
            {
                item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, item.GetComponent<Parts>().m_Position, 3f * Time.deltaTime);
                item.transform.localRotation = Quaternion.Slerp(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation, 2f * Time.deltaTime);
            }
            else
            {
                item.transform.localPosition = item.GetComponent<Parts>().m_Position;
                item.transform.localRotation = item.GetComponent<Parts>().m_Rotation;
            }
        }
        foreach (GameObject item in m_Stars)
        {
            if (Vector3.Distance(item.transform.localPosition, item.GetComponent<Parts>().m_Position) > 0.1f || Mathf.Abs(Quaternion.Angle(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation)) > 0.1f)
            {
                item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, item.GetComponent<Parts>().m_Position, 3f * Time.deltaTime);
                item.transform.localRotation = Quaternion.Slerp(item.transform.localRotation, item.GetComponent<Parts>().m_Rotation, 2f * Time.deltaTime);
            }
            else
            {
                item.transform.localPosition = item.GetComponent<Parts>().m_Position;
                item.transform.localRotation = item.GetComponent<Parts>().m_Rotation;
            }
        }
        for (int i = 0; i < ResultController.s_FirstStageLank; i++)
        {
            m_Stars[i].GetComponentInChildren<Image>().sprite = m_Sprite;
        }
        for (int i = 0; i < ResultController.s_SecondStageLank; i++)
        {
            m_Stars[i + 3].GetComponentInChildren<Image>().sprite = m_Sprite;
        }
        for (int i = 0; i < ResultController.s_ThirdStageLank; i++)
        {
            m_Stars[i + 6].GetComponentInChildren<Image>().sprite = m_Sprite;
        }
    }

    public void SelectedTutorial() // Tutorialボタンが選ばれた時の処理
    {
        foreach (GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        foreach (GameObject item in m_TutorialLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.yellow;
        }
        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        ExDiffusion();
        AllDiffusion();
        foreach (GameObject item in m_StageLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        for (int i = 0; i < m_Tutorial_Logo_Pos.Count; i++)
        {
            m_TutorialLogoParts[i].GetComponent<Parts>().m_Position = m_Tutorial_Logo_Pos[i].localPosition;
        }
        m_SelectedTutorial = true;
    }
    public void SelectedStage1() // Stage1ボタンが選ばれた時の処理
    {
        foreach (GameObject item in m_TutorialLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponentInChildren<Image>().color = Color.yellow;
        }
        m_StageLogoParts[5].GetComponentInChildren<Image>().color = Color.yellow;
        m_StageLogoParts[6].GetComponentInChildren<Image>().color = Color.white;
        m_StageLogoParts[7].GetComponentInChildren<Image>().color = Color.white;
        foreach (GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i].GetComponent<Parts>().m_Position = m_Star_Pos[i].localPosition;
            m_Stars[i].GetComponent<Parts>().m_Rotation = m_Star_Pos[i].localRotation;
        }
        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion();
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage1_Hexagon_Pos.Count; i++)
        {
            m_HexagonParts[i].GetComponent<Parts>().m_Position = m_Stage1_Hexagon_Pos[i].localPosition;
            m_HexagonParts[i].GetComponent<Parts>().m_Rotation = m_Stage1_Hexagon_Pos[i].localRotation;
            m_HexagonParts[i].GetComponentInChildren<Animator>().SetBool("IsStop", true);
        }
        StartCoroutine(SparkAndReBuildings());
        m_StageLogoParts[5].GetComponent<Parts>().m_Position = m_Stage_Logo_Pos[5].localPosition;
    }
    public void SelectedStage2() // Stage2ボタンが選ばれた時の処理
    {
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponentInChildren<Image>().color = Color.yellow;
        }
        m_StageLogoParts[6].GetComponentInChildren<Image>().color = Color.yellow;
        m_StageLogoParts[5].GetComponentInChildren<Image>().color = Color.white;
        m_StageLogoParts[7].GetComponentInChildren<Image>().color = Color.white;
        foreach (GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i + 3].GetComponent<Parts>().m_Position = m_Star_Pos[i].localPosition;
            m_Stars[i + 3].GetComponent<Parts>().m_Rotation = m_Star_Pos[i].localRotation;
        }
        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        AllDiffusion();
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage2_Hexagon_Pos.Count; i++)
        {
            m_HexagonParts[i].GetComponent<Parts>().m_Position = m_Stage2_Hexagon_Pos[i].localPosition;
            m_HexagonParts[i].GetComponent<Parts>().m_Rotation = m_Stage2_Hexagon_Pos[i].localRotation;
            m_HexagonParts[i].GetComponentInChildren<Animator>().SetBool("IsStop", true);
        }
        StartCoroutine(SparkAndReBuildings());
        m_StageLogoParts[6].GetComponent<Parts>().m_Position = m_Stage_Logo_Pos[5].localPosition;
    }
    public void SelectedStage3() // Stage3ボタンが選ばれた時の処理
    {
        for (int i = 0; i < 5; i++)
        {
            m_StageLogoParts[i].GetComponentInChildren<Image>().color = Color.yellow;
        }
        m_StageLogoParts[7].GetComponentInChildren<Image>().color = Color.yellow;
        m_StageLogoParts[5].GetComponentInChildren<Image>().color = Color.white;
        m_StageLogoParts[6].GetComponentInChildren<Image>().color = Color.white;
        foreach (GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        for (int i = 0; i < 3; i++)
        {
            m_Stars[i + 6].GetComponent<Parts>().m_Position = m_Star_Pos[i].localPosition;
            m_Stars[i + 6].GetComponent<Parts>().m_Rotation = m_Star_Pos[i].localRotation;
        }
        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        ExDiffusion();
        AllDiffusion();
        m_SelectedTutorial = false;
        for (int i = 0; i < m_Stage3_Hexagon_Pos.Count; i++)
        {
            m_HexagonParts[i].GetComponent<Parts>().m_Position = m_Stage3_Hexagon_Pos[i].localPosition;
            m_HexagonParts[i].GetComponent<Parts>().m_Rotation = m_Stage3_Hexagon_Pos[i].localRotation;
            m_HexagonParts[i].GetComponentInChildren<Animator>().SetBool("IsStop", true);
        }
        StartCoroutine(SparkAndReBuildings());
        m_StageLogoParts[7].GetComponent<Parts>().m_Position = m_Stage_Logo_Pos[5].localPosition;
    }
    public void SelectedExtra() // Extra
    {
        foreach (GameObject item in m_TutorialLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        foreach (GameObject item in m_Stars)
        {
            item.GetComponent<Parts>().m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
        }
        foreach (GameObject item in m_ExtraLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.yellow;
        }
        for (int i = 0; i < m_Stage1_Hexagon_Pos.Count; i++)
        {
            m_BlackHexagonParts[i].GetComponent<Parts>().m_Position = m_Stage1_Hexagon_Pos[i].localPosition;
        }
        for (int i = 0; i < m_EX_Stage_Logo_Pos.Count; i++)
        {
            m_ExtraLogoParts[i].GetComponent<Parts>().m_Position = m_EX_Stage_Logo_Pos[i].localPosition;
        }
        if (m_SelectedTitleBack) { m_SelectedTitleBack = false; return; }
        foreach (GameObject item in m_StageLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        AllDiffusion();
        m_SelectedEX = true;
    }
    public void DeselectedExtra()
    {
        m_SelectedEX = false;
    }
    public void SelectedTitleBack()
    {
        m_SelectedTitleBack = true;
        foreach (GameObject item in m_StageLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        foreach (GameObject item in m_ExtraLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        foreach (GameObject item in m_TutorialLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
        foreach (GameObject item in m_TitleBackLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.yellow;
        }
    }
    public void DeserectedTitleBack()
    {
        foreach (GameObject item in m_TitleBackLogoParts)
        {
            item.GetComponentInChildren<Image>().color = Color.white;
        }
    }

    public void AllDiffusion() // 全パーツを分散させる
    {
        SetNextPosition(m_AllParts);
        foreach (GameObject item in m_AllParts)
        {
            item.GetComponentInChildren<Animator>().SetBool("IsStop", false);
        }
    }
    public void ExDiffusion() // ExtraStageのパーツを画面外へ
    {
        foreach (GameObject item in m_ExtraParts)
        {
            item.GetComponent<Parts>().m_Position = ExtraRandomPosition();
        }
    }
    public void SetNextPosition(List<GameObject> parts) // パーツの位置をランダムに設定する
    {
        foreach (GameObject item in parts)
        {
            item.GetComponent<Parts>().m_Position = RandomPosition();
            item.GetComponent<Parts>().m_Rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    public Vector3 RandomPosition() // ランダムな位置を取得する
    {
        Vector3 position = Vector3.zero;
        do
        {
            position = new Vector3(Random.Range(-450.0f, 450.0f), Random.Range(-250.0f, 250.0f));
        } while ((position.x > -300 && position.x < 300) && (position.y > -400 && position.y < 150));
        return position;
    }
    public Vector3 ExtraRandomPosition() // 画面外のランダムな位置を取得する
    {
        Vector3 position = Vector3.zero;
        do
        {
            position = new Vector3(Random.Range(-700.0f, 700.0f), Random.Range(-500.0f, 500.0f));
        } while ((position.x > -500.0f && position.x < 500.0f) && (position.y > -300.0f && position.y < 300.0f));
        return position;
    }
    public void AllEnabled() // 全パーツのスクリプトを有効化
    {
        Enabled(m_AllParts);
    }
    public void AllDisabled() // 全パーツのスクリプトを無効化
    {
        Disabled(m_AllParts);
    }
    public void Enabled(List<GameObject> parts) // パーツのスクリプトを有効化
    {
        foreach (GameObject item in parts)
        {
            item.GetComponent<Parts>().enabled = true;
        }
    }
    public void Disabled(List<GameObject> parts) // パーツのスクリプトを無効化
    {
        foreach (GameObject item in parts)
        {
            item.GetComponent<Parts>().enabled = false;
        }
    }

    IEnumerator SparkAndReBuildings()
    {
        SetNextPosition(m_StageLogoParts);
        for (int i = 0; i < m_Stage_Logo_Pos.Count; i++)
        {
            if (i < 5) m_StageLogoParts[i].GetComponentInChildren<Image>().color = Color.white;
        }
        yield return new WaitForSeconds(0.4f);
        if (!m_SelectedTutorial && !m_SelectedEX)
        {
            for (int i = 0; i < m_Stage_Logo_Pos.Count; i++)
            {
                if (i < 5)
                {
                    m_StageLogoParts[i].GetComponent<Parts>().m_Position = m_Stage_Logo_Pos[i].localPosition;
                    if (!m_SelectedTitleBack)
                    {
                        m_StageLogoParts[i].GetComponentInChildren<Image>().color = Color.yellow;
                    }
                }
            }
        }
    }
}
