using System.Collections;
using UnityEngine;

public class Boss3_Controller : MonoBehaviour
{
    public float m_MaxHp; 
    public GameObject m_TwinRobot;
    public Boss3_Humanoid m_HRobot;
    [SerializeField] float m_ReleaseTime;
    [SerializeField] AnimationClip m_CombineAnim;
    [SerializeField] GameObject m_CombineEffect;
    [SerializeField] Animator m_HAnimator;
    [SerializeField] AudioClip m_SEKimepo;
    [SerializeField] AudioSource m_EffectAudioSrc;
    [SerializeField] AnimationClip m_ReleaseAnim;        

    private PlayMode m_State;
    private float m_StateTimer;
    private float m_Hp;        

    //合体完了時イベント        
    //public UnityAction m_CombineEnd;
    //public UnityEvent m_CombineEnd;

    //プレイヤーと共有するとき使う
    //各イベント
    //public UnityEvent m_ReleaseStart;
    //public UnityEvent m_ReleaseEnd;
    //public UnityEvent m_CombineStart;
    //public UnityEvent m_CombineEnd;
    //public UnityEvent<List<Collider>> m_CombineCollide;
    //操作等の行動を無効化するためのスクリプトの参照
    //public MonoBehaviour m_Controller;

    public float Hp
    {
        get { return m_Hp; }
        set
        {
            m_Hp = value;
            //Debug.Log("残りHP" + value);
            if (Hp <= 0)
            {
                Dead();
            }
            GameManager.Instance.m_BossHpRate = m_Hp / m_MaxHp;
        }
    }

    private void Awake()
    {
        GameManager.Instance.m_BossHpRate = 1.0f;
        m_Hp = m_MaxHp;
        m_State = PlayMode.HumanoidRobot;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ReleaseStart();

        if (GameManager.Instance.m_PlayMode == PlayMode.NoPlay)
            return;        

        m_StateTimer += Time.deltaTime;
        switch (m_State)
        {
            case PlayMode.HumanoidRobot:
                m_HRobot.BossUpdate();
                break;
            case PlayMode.Release:
                if (m_StateTimer > m_ReleaseTime)
                    CombineStart();
                break;
            default:
                break;
        }
    }

    public void Dead()
    {
        if (m_State == PlayMode.HumanoidRobot)
            m_HRobot.Dead();

        m_State = PlayMode.NoPlay;
        GameManager.Instance.m_IsGameClear = true;
    }

    public void CombineStart()
    {
        StartCoroutine(Combine());
    }
    public void ReleaseStart()
    {
        if (m_Hp > 0)
            StartCoroutine(Release());
    }

    private IEnumerator Combine()
    {
        m_StateTimer = 0.0f;
        Vector3 pos = m_TwinRobot.transform.position;
        transform.position = pos;
        m_HRobot.transform.position = pos;
        m_CombineEffect.transform.position = pos + new Vector3(0, 1, 0);
        float timer = 0.0f;
        while (timer < m_CombineAnim.length)
        {
            timer += Time.deltaTime;
            m_CombineAnim.SampleAnimation(gameObject, timer);
            yield return null;
        }
        m_EffectAudioSrc.clip = m_SEKimepo;
        m_EffectAudioSrc.playOnAwake = true;
        StartCoroutine(CombineEffect());
        m_HAnimator.SetTrigger("Combined");
        //m_CombineEnd.Invoke();
        m_State = PlayMode.HumanoidRobot;

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator Release()
    {
        m_StateTimer = 0.0f;
        m_State = PlayMode.Release;
        transform.position = m_HAnimator.transform.position;
        m_EffectAudioSrc.playOnAwake = false;
        StartCoroutine(CombineEffect());
        float timer = 0.0f;
        while (timer < m_ReleaseAnim.length)
        {
            timer += Time.deltaTime * 0.3f;
            m_ReleaseAnim.SampleAnimation(gameObject, timer);
            yield return null;
        }
    }

    private IEnumerator CombineEffect()
    {
        m_CombineEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        m_CombineEffect.SetActive(false);
    }
}
