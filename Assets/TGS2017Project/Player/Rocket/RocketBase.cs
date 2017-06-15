using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RocketState
{
    Idle,
    Fire,
    Back,
    Reflected,
    Buried,
    Repair,
}

public class RocketBase : MonoBehaviour
{
    //各コンポーネント
    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private AudioSource m_AudioSrc;
    [SerializeField] private AudioClip m_SEFire;
    [SerializeField] private AudioClip m_SEHit;
    [SerializeField] private GameObject m_EffectHitPrefub;
    //[SerializeField] private GameObject m_EffectExplosion;

    //ロケットの状態
    public RocketState m_State;
    //保持されている砲台
    public RocketBattery m_Battery;
    //戻るべきトランスフォーム
    public Transform m_StandTrans;
    //与えるダメージ
    public float m_ApplyDamage;
    //保持した子エネミーが与えるダメージ
    public float m_ChildApplyDamage;
    //前進速度
    public float m_Speed;
    //戻るときの速度
    public float m_BackSpeed;
    //前進時間
    public float m_AdvanceTime;
    //発射方向
    public Vector3 m_AdvanceDire;
    //発射からの経過時間
    public float m_Timer;
    public string m_TargetTag;
    //埋まる時間
    public float m_BuriedTime = 1.0f;

    //ノックバック状態か
    public bool m_IsKnockBack;
    //ノックバックの力
    public float m_KnockBackForce;

    //衝突した雑魚リスト
    public List<EnemyBase> m_ChildEnemys;

    private void Awake()
    {
        m_ChildEnemys = new List<EnemyBase>();
        m_AudioSrc = gameObject.AddComponent<AudioSource>();
        m_AudioSrc.spatialBlend = 1.0f;
        m_IsKnockBack = true;
    }

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer > 10)
        {
            m_State = RocketState.Idle;
            gameObject.SetActive(false);
            m_Battery.CollectRocket();
            m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        switch (m_State)
        {
            case RocketState.Idle:
                m_Timer = 0.0f;
                break;
            case RocketState.Fire:
                if (m_Timer > m_AdvanceTime)
                {
                    //雑魚を巻き込んでいなければ時間で戻る
                    if (m_ChildEnemys.Count == 0 && m_State != RocketState.Buried)
                        m_State = RocketState.Back;
                }
                break;
            case RocketState.Back:
                transform.LookAt(m_StandTrans);
                break;
            case RocketState.Reflected:
                transform.LookAt(m_StandTrans);
                break;
            case RocketState.Buried:
                if (m_Timer > m_BuriedTime)
                    m_State = RocketState.Back;
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        switch (m_State)
        {
            case RocketState.Idle:
                break;
            case RocketState.Fire:
                Move(m_AdvanceDire * m_Speed);
                break;
            case RocketState.Back:
                Move((m_StandTrans.position - m_Rigidbody.position).normalized * m_BackSpeed);
                m_Rigidbody.MovePosition(m_Rigidbody.position + (m_StandTrans.position - m_Rigidbody.position).normalized * m_BackSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(m_Rigidbody.position, m_StandTrans.position) < m_BackSpeed * Time.fixedDeltaTime)
                {
                    m_State = RocketState.Idle;
                    gameObject.SetActive(false);
                    m_Battery.CollectRocket();
                    m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                break;
            case RocketState.Reflected:
                m_Rigidbody.MovePosition(m_Rigidbody.position + (m_StandTrans.position - m_Rigidbody.position).normalized * m_BackSpeed * Time.fixedDeltaTime);
                break;
            default:
                break;
        }
    }

    public void Move(Vector3 direction)
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + direction * Time.fixedDeltaTime);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * Quaternion.Euler(new Vector3(360, 0, 0) * Time.fixedDeltaTime));
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        m_StandTrans.localScale *= 0.0f;
        transform.position = m_StandTrans.position;
        transform.rotation = m_Battery.transform.rotation;
        m_State = RocketState.Fire;
        m_AudioSrc.PlayOneShot(m_SEFire);
        m_AdvanceDire = transform.forward;
        m_Timer = 0;
    }

    public bool IsCanFire
    {
        get { return m_State == RocketState.Idle; }
    }

    //ChildEnemyを追加するとき（衝突時等）
    public void AddChildEnemy(EnemyBase enemy)
    {
        enemy.transform.parent = transform;
        enemy.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        enemy.Del_Trigger = HasEnemyCollide;
        m_ChildEnemys.Add(enemy);
    }

    //ChildEnemyを全て破壊
    public void BreakChildEnemys()
    {
        foreach (var enemy in m_ChildEnemys)
        {
            enemy.SetBreakForPlayer();
        }
        m_ChildEnemys.Clear();
    }

    //Player、Boss3共通処理オーバーライドするときにbaseの関数を呼んでください。
    protected virtual void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        EnemyBase enemy = obj.GetComponent<EnemyBase>();
        m_AudioSrc.PlayOneShot(m_SEHit);

        foreach (var col in collision.contacts)
        {
            Debug.Log(collision.gameObject);
            GameObject EffectHit = Instantiate(m_EffectHitPrefub, transform);
            EffectHit.transform.position = col.point;
            EffectHit.SetActive(true);
        }
        if (obj.tag == m_TargetTag)
        {//攻撃対象に当たったら
            foreach (var contact in collision.contacts)
            {
                CollideTarget(obj, contact.point);
            }            
        }
        else if (enemy != null)
        {//雑魚と衝突
            if (m_IsKnockBack)
            {//ノックバック
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                enemy.Del_Trigger = KnockBackEnemyCollide;

                Vector3 dire = enemy.transform.position - transform.position;
                rb.AddForce(dire.normalized * m_KnockBackForce, ForceMode.Impulse);
            }
            else
            {//子オブジェクトに追加
                AddChildEnemy(enemy);
                //collision.collider.isTrigger = true;

                m_State = RocketState.Fire;
            }
        }
        else if (obj.tag == "Wall" && m_ChildEnemys.Count == 0)
        {//壁に埋まる処理
            m_State = RocketState.Buried;
            m_Timer = 0.0f;
            Vector3 center = Vector3.Lerp(transform.position, collision.contacts[0].point, 0.7f);
            float distance = (transform.position - center).magnitude;
            Vector3 destination = transform.position + transform.forward * distance;
            StartCoroutine(this.UpdateWhileMethodBool(() =>
            {
                transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * 10.0f);
                return m_Timer < m_BuriedTime;
            }));
        }
    }

    protected void CollideTarget(GameObject target, Vector3 hitpos)
    {
        float damage = m_ApplyDamage + m_ChildEnemys.Count * m_ChildApplyDamage;
        target.GetComponent<Damageable>().ApplyDamage(damage, this);
        GameObject explosion = Instantiate(m_EffectHitPrefub, transform);

        BreakChildEnemys();
        //Debug.Log("ターゲットダメージ" + damage);
        m_State = RocketState.Back;
    }

    //保持しているEnemyが何かにヒットしたとき
    protected void HasEnemyCollide(Collider other, EnemyBase enemy)
    {
        if (other.gameObject == gameObject || other.gameObject.tag == "Panel")
            return;
        Debug.Log(enemy.ToString() + " hit :" + other.gameObject);

        GameObject obj = other.gameObject;
        EnemyBase otherEnemy = other.gameObject.GetComponent<EnemyBase>();
        RocketBase rocket = GetComponent<RocketBase>();

        if (otherEnemy != null)
        {//雑魚と衝突    子オブジェクトに追加
            AddChildEnemy(otherEnemy);
            m_State = RocketState.Fire;
        }
        else
        {
            BreakChildEnemys();
            m_State = RocketState.Back;
        }

        if (obj.tag == m_TargetTag)
        {//攻撃対象と衝突
            CollideTarget(obj, enemy.transform.position);
        }
    }

    protected virtual void KnockBackEnemyCollide(Collider other, EnemyBase enemy)
    {
        if (other.tag == m_TargetTag)
        {//攻撃対象と衝突
            other.gameObject.GetComponent<Damageable>().ApplyDamage(m_ChildApplyDamage, this);
            Debug.Log("ノックバックEnemy　が　Player　と衝突");
        }
        if (other.gameObject.tag != gameObject.tag)
        {
            enemy.SetBreakForPlayer();
        }
    }

    public void SetLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
