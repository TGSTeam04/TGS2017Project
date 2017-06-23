using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour {

	public Text m_TimeText;
	public Text m_ScoreText;

	public float m_Time;
	public int m_Score;

    public float m_NormaTime;
    public int m_NormaScore;

    private bool m_TimeNormaClear;
    private bool m_ScoreNormaClear;

    public Image m_Star;
    public List<Transform> m_StarTrans;

    public AudioClip m_Clip;

    [SerializeField]
    private int m_Lank = 1;

    public static int s_FirstStageLank;
    public static int s_SecondStageLank;
    public static int s_ThirdStageLank;

    AudioSource m_Audio;

	IEnumerator Start()
	{
        m_Audio = GetComponent<AudioSource>();
        m_Time = GameManager.Instance.m_PlayTime;
        m_Score = GameManager.Instance.m_PlayScore;
        yield return new WaitForSeconds(1);
        m_Audio.Play();
        yield return null;
        for (float t = 0; t < 1.0f ; t += Time.deltaTime)
		{
			int time = (int)Mathf.Lerp(0, m_Time, t);
			m_TimeText.text = time / 60 + ":" + (time % 60).ToString("00");
			yield return null;
		}
		m_TimeText.text = (int)m_Time / 60 + ":" + ((int)m_Time % 60).ToString("00");
		for (float t = 0; t < 1.0f; t += Time.deltaTime)
		{
			m_ScoreText.text = ((int)Mathf.Lerp(0, m_Score, t)).ToString();
			yield return null;
		}
		m_ScoreText.text = m_Score.ToString();
        m_Audio.Stop();

        if (m_Time <= m_NormaTime) m_TimeNormaClear = true;
        if (m_Score >= m_NormaScore) m_ScoreNormaClear = true;
        
        if (m_TimeNormaClear && m_ScoreNormaClear)
        {
            m_Lank = 3;
        }
        else if (m_TimeNormaClear || m_ScoreNormaClear)
        {
            m_Lank = 2;
        }
        else
        {
            m_Lank = 1;
        }

        if (GameManager.s_StageNumber == 1)
        {
            if (s_FirstStageLank < m_Lank)
                s_FirstStageLank = m_Lank;
        }
        if (GameManager.s_StageNumber == 2)
        {
            if (s_SecondStageLank < m_Lank)
                s_SecondStageLank = m_Lank;
        }
        if (GameManager.s_StageNumber == 3)
        {
            if (s_ThirdStageLank < m_Lank)
                s_ThirdStageLank = m_Lank;
        }

        yield return new WaitForSeconds(1);
        if (m_Lank == 1)
        {
            Instantiate(m_Star, m_StarTrans[0].position, m_StarTrans[0].rotation, m_StarTrans[0]);
            yield return new WaitForSeconds(1.5f);
            if (m_Audio.clip != m_Clip)
            {
                m_Audio.clip = m_Clip;
                m_Audio.loop = false;
                m_Audio.Play();
            }
        }
        if (m_Lank == 2)
        {
            Instantiate(m_Star, m_StarTrans[3].position, m_StarTrans[3].rotation, m_StarTrans[3]);
            yield return new WaitForSeconds(0.5f);
            Instantiate(m_Star, m_StarTrans[4].position, m_StarTrans[4].rotation, m_StarTrans[4]);
            yield return new WaitForSeconds(1.5f);
            if (m_Audio.clip != m_Clip)
            {
                m_Audio.clip = m_Clip;
                m_Audio.loop = false;
                m_Audio.Play();
            }
        }
        if (m_Lank == 3)
        {
            Instantiate(m_Star, m_StarTrans[1].position, m_StarTrans[1].rotation, m_StarTrans[1]);
            yield return new WaitForSeconds(0.5f);
            Instantiate(m_Star, m_StarTrans[2].position, m_StarTrans[2].rotation, m_StarTrans[2]);
            yield return new WaitForSeconds(0.7f);
            Instantiate(m_Star, m_StarTrans[0].position, m_StarTrans[0].rotation, m_StarTrans[0]);
            yield return new WaitForSeconds(1.5f);
            if (m_Audio.clip != m_Clip)
            {
                m_Audio.clip = m_Clip;
                m_Audio.loop = false;
                m_Audio.Play();
            }
        }
        yield return new WaitForSeconds(3);
        GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}
}
