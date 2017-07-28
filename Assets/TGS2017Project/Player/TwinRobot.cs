using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwinRobotMode
{
    A,
    B
}
public class TwinRobot : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_Controller;
    [SerializeField]
    Damageable m_CoreDamageComp;
    [SerializeField]
    private GameObject m_Shield;
    [SerializeField]
    private TwinRobotConfig m_Config;
    [SerializeField]
    private TwinRobotBaseConfig m_BaseConfig;
    [SerializeField]
    private float m_BreakerSizeS;
    [SerializeField]
    private float m_BreakerSizeL;


    private float m_HP;
    private Renderer m_Renderer;
    private Rigidbody m_Rigidbody;
    private TwinRobotMode m_Mode;
    private float m_Axis;


    [SerializeField]
    GameObject m_Explosion;     // 爆発エフェクト

    /// <summary>
    /// 破片1、破片2、子オブジェクトの関数を追加（2017-07-21）
    /// 製作者：Ho Siu Ki（何兆祺）
    /// </summary>
    [SerializeField]
    GameObject m_Fragment1;     // 破片1
    [SerializeField]
    GameObject m_Fragment2;     // 破片2
    [SerializeField]
    private GameObject m_Child; // 子オブジェクト（自機のモデル）

    void Awake()
    {
        m_Renderer = m_Shield.GetComponent<Renderer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        //トリガー用
        m_Shield.GetComponent<Damageable>().Del_ReciveDamage = Damage;
        //コリジョン用
        GetComponent<Damageable>().Del_ReciveDamage = Damage;
        m_CoreDamageComp.Del_ReciveDamage = Damage;
        HP = m_BaseConfig.m_MaxHP;
    }

    private void OnEnable()
    {
        m_Shield.SetActive(HP != 0);
    }

    public void UpdateInput()
    {
        float axis = Input.GetAxis(m_Config.m_InputModeChange);
        if (axis >= 0.5f && m_Axis < 0.5f)
        {
            ModeChange();
        }
        m_Axis = axis;
    }

    private void ModeChange()
    {
        m_Mode = m_Mode == TwinRobotMode.A ? TwinRobotMode.B : TwinRobotMode.A;
    }

    public void Damage(float damage, MonoBehaviour src)
    {
		if (GameManager.Instance.m_PlayMode == PlayMode.NoPlay)
			return;

		HP -= damage;
    }
    public void Move()
    {
        Vector3 move = new Vector3(
            Input.GetAxis(m_Config.m_InputHorizontal), 0,
            Input.GetAxis(m_Config.m_InputVertical));
        move = Vector3.ClampMagnitude(move, 1.0f) * m_BaseConfig.m_MoveSpeed * Time.fixedDeltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + move);
        if (move.magnitude != 0)
        {
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(move));
        }
    }

    public void Look(Vector3 target, Quaternion quaternion)
    {
        m_Rigidbody.rotation = Quaternion.LookRotation(target - m_Rigidbody.position);
    }

    void OnCollisionEnter(Collision other)
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                switch (other.gameObject.tag)
                {
                    case "Enemy":
                    case "Bullet":

                        //HP -= 5f;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

    }

    public void SetShieldActive(bool isActive)
    {
        m_Shield.SetActive(isActive && HP != 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.Combine:
                switch (other.tag)
                {
                    case "Wall":
                        m_Controller.IsCanCrash = false;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }




    void OnTriggerStay(Collider other)
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.Combine:
                switch (other.tag)
                {
                    case "Wall":
                        m_Controller.IsCanCrash = false;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public float HP
    {
        get { return m_HP; }
        set
        {
            m_HP = Mathf.Clamp(value, 0, m_BaseConfig.m_MaxHP);
            if (!m_Shield.activeSelf && m_HP <= 0 && GameManager.Instance.m_PlayMode == PlayMode.TwinRobot)
            {
                GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
                /// <summary>
                /// 爆発時、破片が飛び出す演出を追加（2017-07-21）
                /// 製作者：Ho Siu Ki（何兆祺）
                /// </summary>
                Instantiate(m_Explosion, transform.position + Vector3.up * 1.5f, transform.rotation);

                StartCoroutine(this.Delay(new WaitForSeconds(0.5f), () =>
                {
                    Instantiate(m_Explosion, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(transform.position));
                }));

                StartCoroutine(this.Delay(new WaitForSeconds(0.9f), () =>
                {
                    Instantiate(m_Fragment1, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 157.0f, 0.0f));
                }));

                StartCoroutine(this.Delay(new WaitForSeconds(1.4f), () =>
                {
                    Instantiate(m_Fragment1, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 243.0f, 0.0f));
                }));

                StartCoroutine(this.Delay(new WaitForSeconds(1.9f), () =>
                {
                    Instantiate(m_Fragment1, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 195.0f, 0.0f));
                }));
                StartCoroutine(this.Delay(new WaitForSeconds(2.4f), () =>
                {
                    Instantiate(m_Fragment1, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 37.0f, 0.0f));
                }));
                StartCoroutine(this.Delay(new WaitForSeconds(2.9f), () =>
                {
                    Instantiate(m_Fragment2, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 100.0f, 0.0f));
                }));
                StartCoroutine(this.Delay(new WaitForSeconds(3.2f), () =>
                {
                    Instantiate(m_Fragment2, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 100.0f, 0.0f));
                }));
                StartCoroutine(this.Delay(new WaitForSeconds(3.5f), () =>
                {
                    // 自機のモデルを消す
                    m_Child.SetActive(false);
                    Instantiate(m_Fragment2, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 100.0f, 0.0f));
                    Instantiate(m_Fragment2, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 200.0f, 0.0f));
                    Instantiate(m_Fragment2, transform.position + Vector3.up * 1.5f + Vector3.forward * 1.5f, transform.rotation * Quaternion.Euler(0.0f, 300.0f, 0.0f));
                }));

                StartCoroutine(this.Delay(new WaitForSeconds(5.0f), () =>
                {
                    GameManager.Instance.m_GameStarter.AddScene("GameOver");
                }));
            }
            ShieldUpdate();
        }
    }

    public TwinRobotMode Mode
    {
        get { return m_Mode; }
    }

    public float BreakerSize
    {
        get { return Mode == TwinRobotMode.A ? m_BreakerSizeS : m_BreakerSizeL; }
    }

    private void ShieldUpdate()
    {
        m_Shield.SetActive(HP != 0);
        m_Renderer.material.SetColor("_BaseColor", m_BaseConfig.m_ShieldColor.Evaluate(HP / m_BaseConfig.m_MaxHP));
    }

    public float Distance(Vector3 position)
    {
        return Vector3.Distance(position, m_Rigidbody.position);
    }
}
