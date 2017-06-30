using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStatus : MonoBehaviour
{
    public Image m_Energy;
    private PlayerController m_Player;
    public GameObject m_Black;

    public AudioClip m_AudioClip;
    AudioSource m_Audio;

    public Image m_BossHP;

    [SerializeField]
    private GameObject m_ImageA;
    [SerializeField]
    private GameObject m_ImageB;
    [SerializeField] private Image m_LShield;
    [SerializeField] private Image m_RShield;
	[SerializeField] private TwinRobotBaseConfig m_TwinRobotBaseConfig;

    // Use this for initialization
    void Start()
    {
        m_Player = GameManager.Instance.m_PlayerController;
        m_Audio = GetComponent<AudioSource>();
        StartCoroutine(countdown());
    }

    // Update is called once per frame
    void Update()
    {
        float MaxShield = m_TwinRobotBaseConfig.m_MaxHP;
        m_LShield.fillAmount = m_Player.m_TwinRobotL.HP / MaxShield;
        m_RShield.fillAmount = m_Player.m_TwinRobotR.HP / MaxShield;
        m_Energy.fillAmount = m_Player.Energy / GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[4];
        m_BossHP.fillAmount = GameManager.Instance.m_BossHpRate;
    }
    IEnumerator countdown()
    {
        m_Black.SetActive(true);
        m_ImageA.SetActive(false);
        m_ImageB.SetActive(false);
        yield return new WaitForSeconds(GameManager.Instance.m_LoadingAnimationTime);
        yield return new WaitForSeconds(1);
        m_Audio.Play();
        m_ImageA.SetActive(true);
        yield return new WaitForSeconds(2);
        m_ImageA.SetActive(false);
        m_Audio.clip = m_AudioClip;
        m_Audio.Play();
        m_ImageB.SetActive(true);
        yield return new WaitForSeconds(1);
        m_Black.SetActive(false);
        GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
    }
}
