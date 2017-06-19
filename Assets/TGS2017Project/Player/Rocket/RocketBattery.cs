using System.Collections;
using UnityEngine;
//using UnityEngine.Events;

//ロケット２つをラップしたロケット砲台クラス
//詳細パラメータは各ロケット　m_XRocket　のパラメータをいじってください。
public class RocketBattery : MonoBehaviour
{
    [SerializeField] private GameObject m_RocketPrefub;
    [SerializeField] private Transform m_LStandTrans;
    [SerializeField] private Transform m_RStandTrans;
    [SerializeField] private float m_KnockBackForce;
    [SerializeField] private bool m_IsKnockBack;
    [SerializeField] private string m_RocketLayer;
    [SerializeField] private string m_TargetTag;
    [SerializeField] private AudioClip m_SEColectRocet;
    [SerializeField] private GameObject m_Effect_Chage;
    private Vector3 m_EffectChagePos = new Vector3(0, 3, 0);

    [HideInInspector] public RocketBase m_LRocket;
    [HideInInspector] public RocketBase m_RRocket;
    private Animator m_Anim;
    private AudioSource m_AudioSrc;

    private void Awake()
    {
        m_Anim = GetComponentInChildren<Animator>();
        m_AudioSrc = gameObject.AddComponent<AudioSource>();
        m_AudioSrc.spatialBlend = 1.0f;
        m_AudioSrc.playOnAwake = false;

        //ロケットの初期位置取得
        if (m_LStandTrans == null) //RocketStandの存在チェック
            m_LStandTrans = transform.FindChild("RocketStand").transform;
        if (m_RStandTrans == null) //RocketStandの存在チェック
            m_RStandTrans = transform.FindChild("RocketStand").transform;

        //Rocketが指定されていなければ生成
        if (m_LRocket == null)
        {
            //ロケットインスタンス化
            GameObject lRocketObj = Instantiate(m_RocketPrefub);
            m_LRocket = lRocketObj.GetComponent<RocketBase>();
            m_LRocket.m_StandTrans = m_LStandTrans;
        }
        //Rocketが指定されていなければ生成
        if (m_RRocket == null)
        {
            //ロケットインスタンス化
            GameObject rRocketObj = Instantiate(m_RocketPrefub);
            m_RRocket = rRocketObj.GetComponent<RocketBase>();
            m_RRocket.m_StandTrans = m_RStandTrans;
        }

        SetIsKnockBack(m_IsKnockBack);
        SetKnockBackForce(m_KnockBackForce);
        //m_LRocket.SetLayer(m_RocketLayer);
        //m_RRocket.SetLayer(m_RocketLayer);
        m_RRocket.gameObject.SetActive(false);
        m_LRocket.gameObject.SetActive(false);
        m_LRocket.m_Battery = this;
        m_RRocket.m_Battery = this;

        m_EffectChagePos = m_Effect_Chage.transform.localPosition;
    }

    public void CollectRocket()
    {
        m_AudioSrc.PlayOneShot(m_SEColectRocet);
    }

    //LRどちらでもいいから発射可能か確認
    public bool IsCanFire { get { return m_LRocket.IsCanFire || m_RRocket.IsCanFire; } }
    public bool BothIsCanFire { get { return m_LRocket.IsCanFire && m_RRocket.IsCanFire; } }
    //左方の発射可能か確認
    public bool LIsCanFire { get { return m_LRocket.IsCanFire; } }
    //右方の発射可能か確認
    public bool RIsCanFire { get { return m_RRocket.IsCanFire; } }

    //LRどっちでもいいから発射
    public void Fire()
    {
        if (m_LRocket.IsCanFire)
            StartCoroutine(LAnimatedFire());
        else
            StartCoroutine(RAnimatedFire());
    }
    //BoolによるLR指定の発射
    private void Fire(bool isLeft)
    {
        if (isLeft)
        {
            if (m_LRocket.IsCanFire)
                StartCoroutine(LAnimatedFire());
        }
        else
        {
            if (m_RRocket.IsCanFire)
                StartCoroutine(RAnimatedFire());
        }
    }

    //L発射
    public IEnumerator LAnimatedFire()
    {
        m_Anim.SetTrigger("LFire");
        m_Effect_Chage.transform.position = transform.position + m_EffectChagePos;
        m_Effect_Chage.SetActive(true);
        yield return new WaitForAnimation(m_Anim, 0.7f);
        //m_Effect_Chage.SetActive(false);
        m_LRocket.Fire();
    }
    //R発射
    public IEnumerator RAnimatedFire()
    {
        m_Anim.SetTrigger("RFire");
        m_Effect_Chage.transform.position = transform.position + m_EffectChagePos;
        m_Effect_Chage.SetActive(true);
        yield return new WaitForAnimation(m_Anim, 0.7f);
        //m_Effect_Chage.SetActive(false);
        m_RRocket.Fire();
    }

    /************左右のパラメータを同時に設定（ロケットのパラメータを個々でいじりたいときはメンバのロケットにアクセスしてください。***************/

    public void SetSpeed(float speed)
    {
        m_LRocket.m_Speed = speed;
        m_RRocket.m_Speed = speed;
    }
    public void SetBackSpeed(float speed)
    {
        m_LRocket.m_BackSpeed = speed;
        m_RRocket.m_BackSpeed = speed;
    }
    public void SetAdvanceTime(float time)
    {
        m_LRocket.m_AdvanceTime = time;
        m_RRocket.m_AdvanceTime = time;
    }
    public void SetKnockBackForce(float force)
    {
        m_LRocket.m_KnockBackForce = force;
        m_RRocket.m_KnockBackForce = force;
    }
    public void SetIsKnockBack(bool isKnockBack)
    {
        m_LRocket.m_IsKnockBack = isKnockBack;
        m_RRocket.m_IsKnockBack = isKnockBack;
    }
}