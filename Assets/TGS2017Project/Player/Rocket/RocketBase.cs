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
    private Rigidbody m_Rb;
    private BoxCollider m_BCollider;
    //private CenterOfMass m_MassCenter;
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
    public float m_ChildApplyDamage = 5;
    //前進速度
    public float m_Speed;
    //戻るときの速度
    public float m_BackSpeed;
    //前進時間
    public float m_AdvanceTime;
    //ターゲットのタグ
    public string m_TargetTag;
    //埋まる時間
    public float m_BuriedTime = 1.0f;
    //ノックバック状態か
    public bool m_IsKnockBack;
    //ノックバックの力
    public float m_KnockBackForce;

    //衝突した雑魚リスト
    public List<EnemyBase> m_ChildEnemys;
    //ステートの経過時間
    private float m_Timer;

    private void Awake()
    {
        m_ChildEnemys = new List<EnemyBase>();
        m_AudioSrc = gameObject.AddComponent<AudioSource>();
        m_Rb = GetComponent<Rigidbody>();
        m_BCollider = GetComponent<BoxCollider>();
        m_AudioSrc.spatialBlend = 1.0f;
        m_IsKnockBack = true;
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
                Move(transform.forward * m_Speed);
                break;
            case RocketState.Back:
                Move((m_StandTrans.position - m_Rb.position).normalized * m_BackSpeed);
                if (Vector3.Distance(m_StandTrans.position, m_Rb.position) <= m_BackSpeed * Time.fixedDeltaTime * 2)
                    Colected();
                break;
            case RocketState.Reflected:
                Vector3 velocity = m_Rb.position + (m_StandTrans.position - m_Rb.position).normalized * m_BackSpeed;
                m_Rb.MovePosition(velocity * Time.fixedDeltaTime);
                break;
            default:
                break;
        }
    }

    public void Move(Vector3 velocity)
    {
        m_Rb.MovePosition(m_Rb.position + velocity * Time.fixedDeltaTime);

        float pich = m_Rb.rotation.eulerAngles.x;
        float forwardLen = (m_BCollider.size.z / 2 * transform.lossyScale.z) - m_Rb.centerOfMass.z;
        float borderHigth = forwardLen * Mathf.Sin(pich * Mathf.Deg2Rad);
        RaycastHit hitInfo;
        if (Physics.Raycast(m_Rb.position, Vector3.down, out hitInfo, borderHigth * 2, LayerMask.GetMask("Floor")))
        {
            float heigth = (m_Rb.position.y - hitInfo.point.y) - m_BCollider.size.y / 2;
            if (borderHigth > heigth && heigth > float.Epsilon)
            {
                float sin = heigth / forwardLen;
                float deg = Mathf.Asin(sin) * Mathf.Rad2Deg;                
                m_Rb.rotation = Quaternion.Euler(new Vector3(deg, m_Rb.rotation.eulerAngles.y, m_Rb.rotation.eulerAngles.z));
            }
        }
    }

    public void Colected()
    {
        m_State = RocketState.Idle;
        gameObject.SetActive(false);
        m_Battery.CollectRocket();
        m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        m_StandTrans.localScale *= 0.0f;
        transform.position = m_StandTrans.position;
        transform.rotation = m_Battery.transform.rotation;
        m_Rb.rotation = transform.rotation;
        //transform.rotation = Quaternion.Euler(new Vector3(20, 0, 0));
        //Debug.Log(transform.rotation.eulerAngles);
        m_State = RocketState.Fire;
        m_AudioSrc.PlayOneShot(m_SEFire);
        m_Timer = 0;
    }

    public bool IsCanFire
    {
        get { return m_State == RocketState.Idle; }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log(collision.gameObject);
    }

    //Player、Boss3共通処理オーバーライドするときにbaseの関数を呼んでください。
    protected virtual void OnCollisionEnter(Collision collision)
    {        
        GameObject obj = collision.gameObject;
        EnemyBase enemy = obj.GetComponent<EnemyBase>();
        m_AudioSrc.PlayOneShot(m_SEHit);

        //if(collision.gameObject.tag == "Floor") 
        //衝突エフェクト
        foreach (var col in collision.contacts)
        {
            GameObject EffectHit = Instantiate(m_EffectHitPrefub, transform);
            EffectHit.transform.position = col.point;
            EffectHit.SetActive(true);
            StartCoroutine(m_Battery.Delay(new WaitForSeconds(0.5f)
                , () => Destroy(EffectHit.gameObject)));
        }
        if (obj.tag == m_TargetTag)
        {//攻撃対象に当たったら
            foreach (var contact in collision.contacts)
            {
                CollideTarget(obj, collision.contacts[0].point);
            }
        }
        else if (enemy != null && m_State == RocketState.Fire)
        {//雑魚と衝突
         //Debug.Log("エネミーヒット");
            if (m_IsKnockBack)
            {//ノックバック
             //Debug.Log("ノックバック");
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;
                enemy.Del_Trigger = KnockBackEnemyTrigger;
                enemy.m_FreezeVelocity = false;

                Vector3 dire = enemy.transform.position - transform.position;
                rb.AddForce(dire.normalized * m_KnockBackForce, ForceMode.Impulse);
            }
            else
            {//子オブジェクトに追加
             //Debug.Log("Rocket　Add　Enemy");
                AddChildEnemy(enemy);
            }
        }
        if (obj.tag == "Wall")
        {//壁に埋まる処理                  
            BreakChildEnemys();
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
        Damageable damageComp = target.GetComponent<Damageable>();
        if(damageComp != null) damageComp.ApplyDamage(damage, this);
        GameObject explosion = Instantiate(m_EffectHitPrefub, transform);
        explosion.SetActive(true);
        m_Battery.StartCoroutine(this.Delay(new WaitForSeconds(0.5f)
            , () => Destroy(explosion.gameObject)));

        BreakChildEnemys();
        //Debug.Log("ターゲットダメージ" + damage);
        m_State = RocketState.Back;
    }

    //ChildEnemyを追加するとき（衝突時等）
    public void AddChildEnemy(EnemyBase enemy)
    {
        if (m_ChildEnemys.Contains(enemy))
            return;

        enemy.GetComponent<Pauser>().OnPause();
        enemy.transform.parent = transform;
        enemy.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        enemy.Del_Trigger = HasEnemyTrigger;
        m_ChildEnemys.Add(enemy);
    }
    //ChildEnemyを全て破壊
    public void BreakChildEnemys()
    {
        foreach (var enemy in m_ChildEnemys)
        {
            enemy.GetComponent<Pauser>().OnResume();
            enemy.transform.parent = GameManager.Instance.m_StageManger.transform;
            BreakEnemy(enemy);
        }
        m_ChildEnemys.Clear();
    }
    private void BreakEnemy(EnemyBase enemy)
    {
        if (m_TargetTag == "Boss")
            enemy.SetBreakForPlayer();
        else
            enemy.SetBreak();
    }
    //保持しているEnemyが何かにヒットしたとき
    protected void HasEnemyTrigger(Collider other, EnemyBase enemy)
    {
        if (other.gameObject == gameObject || other.gameObject.tag == "Panel")
            return;
        //Debug.Log(enemy.ToString() + " hit :" + other.gameObject);

        GameObject obj = other.gameObject;
        EnemyBase otherEnemy = other.gameObject.GetComponent<EnemyBase>();
        RocketBase rocket = GetComponent<RocketBase>();

        if (obj.tag == m_TargetTag)
        {//攻撃対象と衝突
            CollideTarget(obj, enemy.transform.position);
            GameObject EffectHit = Instantiate(m_EffectHitPrefub, transform);
            EffectHit.transform.position = enemy.transform.position;
            EffectHit.SetActive(true);
            StartCoroutine(m_Battery.Delay(new WaitForSeconds(0.5f)
                , () => Destroy(EffectHit.gameObject)));
        }

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
    }
    protected virtual void KnockBackEnemyTrigger(Collider other, EnemyBase enemy)
    {
        if (other.tag == tag || other.tag == "Floor") return;

        if (other.tag == m_TargetTag)
        {//攻撃対象と衝突
            other.gameObject.GetComponent<Damageable>().ApplyDamage(m_ChildApplyDamage, this);
            Debug.Log("ノックバックEnemy　が　ターゲット　と衝突");
        }

        //Enmeyの消滅処理
        BreakEnemy(enemy);
    }
}