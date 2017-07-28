using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBoss : MonoBehaviour
{
    public enum SecondBossState
    {
        Ready,          // 登場
        Battle,         // 戦闘
        Paralysis,      // 麻痺
        Invincible      // 無敵
    }

    [SerializeField]
    private float m_MoveSpeed = 150;
    [SerializeField]
    private float m_RotateSpeed = 120;
    [SerializeField]
    private float m_ShootAngle = 30.0f;
    [SerializeField]
    private float m_SearchAngle = 10.0f;

    public GameObject m_Bullet;
    public Transform m_GunBarrel;
    public GameObject m_Explosion;
    public GameObject m_LastExplosion;
    //public Image m_HitPointBar;
    public List<GameObject> m_Particle;

    public List<GameObject> m_Smoke;

    private Collider m_Collision;

    //ダメージコンポーネント
    private Damageable m_Damage;    
    private int m_AmmoCount = 4;
    private bool m_Fire = true;
    private bool m_Reload;

    private int m_BattleChangeSpeed = 5;
    private int m_ChangeCounter;

    private int m_AttackInterval = 2;
    private int m_AttackCounter;

    private int m_RecoverSpeed = 10;
    private int m_RecoverCounter;

    GameObject m_Target;
    Vector3 m_TargetPosition;

    public AudioClip m_CollideSound;
    public AudioClip m_MoveSound;
    public AudioClip m_StanSound;
    public AudioClip m_DriftSound;

    AudioSource m_Sound;
    Animator m_Anim;

    public static SecondBossState s_State = SecondBossState.Ready;

    private static float s_MaxHp = 100.0f;
    private static float s_HitPoint;
    public static float HitPoint
    {
        get { return s_HitPoint; }
        set
        {
            s_HitPoint = value;
            GameManager.Instance.m_BossHpRate = (HitPoint / s_MaxHp);
			GameManager.Instance.m_BossHpRate2 = (HitPoint / s_MaxHp);
		}
	}

    /***    okaku  ***/
    //加速度
    public float m_Accel = 245.7f;
    Vector3 m_Velocity;

    private void Awake()
    {
        HitPoint = s_MaxHp;
        GameManager.Instance.m_BossHpRate = 1.0f;
		GameManager.Instance.m_BossHpRate2 = 1.0f;
        m_Damage = GetComponent<Damageable>();
        m_Damage.Del_ReciveDamage = Damage;
    }

    //ダメージコンポーネントのダメージ
    private void Damage(float damage, MonoBehaviour src)
    {
        HitPoint -= damage;
        if (HitPoint <= 0)
        {
            s_State = SecondBossState.Paralysis;
            Instantiate(m_LastExplosion, transform.position, transform.rotation);
            //Pauser.Pause(PauseTag.Enemy);
            //GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
            StartCoroutine(this.Delay(new WaitForSeconds(4.0f), Dead));
        }
    }

    // Use this for initialization
    void Start()
    {
        m_Target = GameManager.Instance.m_LRobot;
        m_Sound = GetComponent<AudioSource>();
        m_Anim = GetComponentInChildren<Animator>();
        s_State = SecondBossState.Ready;
        m_Collision = GetComponent<Collider>();
        foreach (GameObject p in m_Particle)
        {
            p.SetActive(false);
        }
	}

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    m_Sound.clip = m_MoveSound;
        //    m_Sound.Play();
        //}
        if (HitPoint <= 0)
        {
            s_State = SecondBossState.Paralysis;
            //m_Sound.Stop();
            StartCoroutine(Death());
        }
        if (m_Velocity.magnitude < m_MoveSpeed)
        {
            m_Velocity += transform.forward * m_Accel * Time.deltaTime;
        }
        m_Velocity *= 0.95f;

		//m_HitPointBar.fillAmount = s_HitPoint;
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				// プレイヤーに追従
				m_Target = PlayerManager.Instance.NearPlayer(transform.position).gameObject;
				break;
			// デフォルト状態（何もしない）
			default:
				return;
		}

        m_TargetPosition = m_Target.transform.position;
        m_TargetPosition.y = transform.position.y;
        float angle = Vector3.Angle(transform.forward, m_TargetPosition - transform.position);
        switch (s_State)
        {
            case SecondBossState.Ready:
                m_Anim.speed = 1.0f;
                m_Collision.isTrigger = false;
                foreach (GameObject p in m_Particle)
                {
                    p.SetActive(false);
                }
                if (m_ChangeCounter <= 0)
                {
                    StartCoroutine(BattleChange());
                }
                if (angle > m_SearchAngle && GameManager.Instance.m_PlayMode != PlayMode.Combine)
                {
                    if (m_Sound.clip != m_MoveSound)
                    {
                        m_Sound.clip = m_MoveSound;
                        m_Sound.loop = true;
                        m_Sound.Play();
                    }
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
                    transform.Translate(Vector3.forward * m_MoveSpeed / 4 * Time.deltaTime);
                }
                else
                {
                    if (m_Sound.clip != m_MoveSound)
                    {
                        m_Sound.clip = m_MoveSound;
                        m_Sound.loop = true;
                        m_Sound.Play();
                    }
                    transform.Translate(Vector3.forward * m_MoveSpeed / 3 * Time.deltaTime);
                }
                break;
            case SecondBossState.Battle:
                m_Anim.speed = 1.0f;
                m_Collision.isTrigger = false;
                if (m_Fire == false && m_AttackCounter <= 0)
                {
                    StartCoroutine(Fire());
                }

                if (angle > m_SearchAngle)
                {
                    foreach (GameObject p in m_Particle)
                    {
                        p.SetActive(true);
                    }
                    if (m_Sound.clip != m_DriftSound)
                    {
                        m_Sound.clip = m_DriftSound;
                        m_Sound.loop = true;
                        m_Sound.Play();
                    }
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
                    //transform.Translate(m_Velocity * Time.deltaTime);
                    transform.position += m_Velocity * 0.8f * Time.deltaTime;
                }
                else
                {
                    foreach (GameObject s in m_Smoke)
                    {
                        s.GetComponent<ParticleSystem>().Play();
                    }
                    foreach (GameObject p in m_Particle)
                    {
                        p.SetActive(false);
                    }
                    if (m_Sound.clip != m_MoveSound)
                    {
                        m_Sound.clip = m_MoveSound;
                        m_Sound.loop = true;
                        m_Sound.Play();
                    }
                    transform.position += m_Velocity * Time.deltaTime;
                    //transform.Translate(m_Velocity * Time.deltaTime);
                }

                if (m_AmmoCount == 4)
                {
                    m_Reload = false;
                }
                else if (m_AmmoCount <= 0)
                {
                    m_Reload = true;
                }
                if (m_Reload == false && m_Fire == true && angle < m_ShootAngle)
                {
                    Instantiate(m_Bullet, m_GunBarrel.position, Quaternion.Euler(Vector3.zero));
                    m_AmmoCount--;
                    m_Fire = false;
                }
                break;
            case SecondBossState.Paralysis:
                m_Anim.speed = 0.0f;
                if (m_Sound.clip != m_StanSound)
                {
                    m_Sound.clip = m_StanSound;
                    m_Sound.loop = true;
                    m_Sound.Play();
                }
                foreach (GameObject p in m_Particle)
                {
                    p.SetActive(false);
                }
                foreach (GameObject s in m_Smoke)
                {
                    s.GetComponent<ParticleSystem>().Stop();
                }
                break;
            case SecondBossState.Invincible:
                m_Anim.speed = 1.0f;
                m_Sound.Stop();
                foreach (GameObject p in m_Particle)
                {
                    p.SetActive(false);
                }
                m_Collision.isTrigger = false;
                if (angle > m_SearchAngle)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
                }
                else
                {
                    s_State = SecondBossState.Ready;
                }
                break;
        }

    }
    void Dead()
    {
        GameManager.Instance.m_IsGameClear = true;
        Destroy(gameObject);
    }
    IEnumerator BattleChange()
    {
        m_ChangeCounter = m_BattleChangeSpeed;
        while (m_ChangeCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_ChangeCounter--;
        }
        s_State = SecondBossState.Battle;
    }
    IEnumerator Fire()
    {
        m_AttackCounter = m_AttackInterval;
        while (m_AttackCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_AttackCounter--;
        }
        m_Fire = true;
    }
    IEnumerator Recover()
    {
        m_RecoverCounter = m_RecoverSpeed;
        while (m_RecoverCounter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_RecoverCounter--;
        }
        if (s_State != SecondBossState.Invincible)
        {
            s_State = SecondBossState.Invincible;
        }
    }
    IEnumerator Death()
    {
        Instantiate(m_LastExplosion, transform.position, transform.rotation);
        yield return new WaitForSeconds(4.0f);
        Dead();
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall" && s_State == SecondBossState.Battle)
        {
            if (m_Sound.clip != m_CollideSound)
            {
                m_Sound.clip = m_CollideSound;
                m_Sound.loop = false;
                m_Sound.Play();
            }
            Instantiate(m_Explosion, transform.position, transform.rotation);
            if (m_RecoverCounter <= 0)
            {
                StartCoroutine(Recover());
            }
            s_State = SecondBossState.Paralysis;
        }
        if (other.gameObject.tag == "Enemy" && m_Reload)
        {
            m_AmmoCount++;
            other.gameObject.GetComponent<EnemyBase>().SetBreak();
        }
    }
}
