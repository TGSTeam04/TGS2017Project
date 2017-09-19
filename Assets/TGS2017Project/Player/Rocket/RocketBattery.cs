using System.Collections;
using UnityEngine;
//using UnityEngine.Events;

//ロケット２つをラップしたロケット砲台クラス
//詳細パラメータは各ロケット　m_XRocket　のパラメータをいじってください。
public class RocketBattery : MonoBehaviour
{
    [SerializeField] GameObject m_RocketPrefub;
    [SerializeField] Transform m_LStandTrans;
    [SerializeField] Transform m_RStandTrans;
    [SerializeField] string m_RocketLayer;
    [SerializeField] AudioClip m_SEColectRocet;
    [SerializeField] GameObject m_Effect_Chage;
    [SerializeField] Animator m_HumanoidAnim;
    const float FireRateInAnim = 0.5f;
    private Vector3 m_EffectChagePos = new Vector3(0, 3, 0);

    [HideInInspector] public RocketBase m_LRocket;
    [HideInInspector] public RocketBase m_RRocket;

    private AudioSource m_AudioSrc;

    private void Awake()
    {
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
            GameObject lRocketObj = Instantiate(m_RocketPrefub, transform);
            lRocketObj.transform.parent = null;
            m_LRocket = lRocketObj.GetComponent<RocketBase>();
            m_LRocket.m_StandTrans = m_LStandTrans;
        }
        //Rocketが指定されていなければ生成
        if (m_RRocket == null)
        {
            //ロケットインスタンス化
            GameObject rRocketObj = Instantiate(m_RocketPrefub, transform);
            rRocketObj.transform.parent = null;
            m_RRocket = rRocketObj.GetComponent<RocketBase>();
            m_RRocket.m_StandTrans = m_RStandTrans;
        }

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
        m_HumanoidAnim.SetTrigger("LFire");
        m_Effect_Chage.transform.position = transform.position + m_EffectChagePos;
        m_Effect_Chage.SetActive(true);
        yield return new WaitForAnimation(m_HumanoidAnim, FireRateInAnim);
        m_Effect_Chage.SetActive(false);
        m_LRocket.Fire();
    }
    //R発射
    public IEnumerator RAnimatedFire()
    {
        m_HumanoidAnim.SetTrigger("RFire");
        m_Effect_Chage.transform.position = transform.position + m_EffectChagePos;
        m_Effect_Chage.SetActive(true);
        yield return new WaitForAnimation(m_HumanoidAnim, FireRateInAnim);
        m_Effect_Chage.SetActive(false);
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
	public void SetApplyDamage(float amount)
	{
		m_LRocket.m_ApplyDamage = amount;
		m_RRocket.m_ApplyDamage = amount;
	}
}