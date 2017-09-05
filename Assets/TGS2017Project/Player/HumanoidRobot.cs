using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanoidRobot : MonoBehaviour
{
    public PlayerController m_PlayerController;

    [SerializeField]
    private HumanoidBaseConfig m_BaseConfig;
    public HumanoidConfig m_Config;

    private Rigidbody m_Rigidbody;

    public float m_Energy;

    public Animator m_Animator;

    private float m_Speed;
    private float m_Rotate;

    private bool m_Charging;
    public bool m_IsBoost = false;

    [SerializeField]
    private RocketBattery m_Battery;

	[SerializeField] Damageable m_DamageComp;

    [SerializeField] private GameObject m_Effect_Damage;

	[SerializeField] private Transform m_TPSTarget;
	[SerializeField] private float m_PitchMax;
	[SerializeField] private float m_PitchMin;
	private float m_Pitch;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
		m_DamageComp.Del_ReciveDamage = Damage;
    }
    public void Damage(float damage, MonoBehaviour src)
    {
		if (GameManager.Instance.m_PlayMode == PlayMode.NoPlay)
			return;

        //ApplyDamageされたときの処理
        m_Energy -= damage;
        m_Animator.SetTrigger("Damage");
    }

	private void OnEnable()
	{
		m_Battery.SetIsKnockBack(m_Config.m_IsKnockBack);
	}



	// Update is called once per frame
	public void UpdateInput()
    {
		m_IsBoost = false;
		if (Input.GetButtonDown(m_BaseConfig.m_InputJump) && IsGround())
        {
            m_Animator.SetTrigger("Jump");
            m_Rigidbody.AddForce(Vector3.up * m_Config.m_JumpPower, ForceMode.Impulse);
        }

        if ((Input.GetButtonDown("ChargeL") || Input.GetAxis("ChargeL")>0.5f) && m_Energy >= m_Config.m_ChargeUseEnergy && !m_Charging && m_Battery.LIsCanFire)
        {
            m_PlayerController.StartCoroutine(Charge(true));
        }
        else if ((Input.GetButtonDown("ChargeR") || Input.GetAxis("ChargeR") > 0.5f) && m_Energy >= m_Config.m_ChargeUseEnergy && !m_Charging && m_Battery.RIsCanFire)
        {
            m_PlayerController.StartCoroutine(Charge(false));
        }

        float energy = 0;
        if (Input.GetButton(m_BaseConfig.m_InputBoost) && !m_Charging)
        {
            m_Animator.SetBool("IsBoost", true);
            m_Speed = m_Config.m_BoostSpeed;
            energy = m_Config.m_BoostUseEnergy;
			m_IsBoost = true;
        }
        else
        {
            m_Animator.SetBool("IsBoost", false);
            m_Speed = m_Config.m_NormalSpeed;
            energy = m_Config.m_NormalUseEnergy;
        }
		m_Animator.SetBool("Granded", IsGround());
		m_Energy -= Time.deltaTime * energy;
    }
    public IEnumerator Charge(bool L)
    {
        m_Charging = true;
        m_Energy -= m_Config.m_ChargeUseEnergy;
        //yield return new WaitForSeconds(1f);
        //		yield return new WaitForAnimation(m_Animator,0.3f);
        if (L)
        {
            StartCoroutine(m_Battery.LAnimatedFire());
        }
        else
        {
            StartCoroutine(m_Battery.RAnimatedFire());
        }
        //m_Battery.Fire();
        //	yield return new WaitForAnimation(m_Animator,0.7f);
        yield return new WaitForSeconds(1.5f);

        m_Charging = false;
    }

    public bool IsGround()
    {
        return Physics.CheckSphere(m_Rigidbody.position + Vector3.up * 0.7f, 0.72f, LayerMask.GetMask(new string[] { "Floor" }));
    }

    public void Move()
    {
        m_Rotate = m_Config.m_ChargeRotate;
        if (!m_Charging)
        {
            m_Rotate = m_Config.m_NormalRotate;
            Vector3 move_ = new Vector3(Input.GetAxis(m_BaseConfig.m_InputHorizontal), 0, Input.GetAxis(m_BaseConfig.m_InputVertical));
            m_Animator.SetFloat("Forward", move_.z);
            m_Animator.SetFloat("Right", move_.x);
            move_ = m_Rigidbody.rotation * move_;
            m_Rigidbody.MovePosition(m_Rigidbody.position + move_ * m_Speed * Time.fixedDeltaTime);
        }
        transform.Rotate(0, Input.GetAxis(m_BaseConfig.m_InputRotation) * m_Rotate * Time.fixedDeltaTime, 0);
		m_Pitch = Mathf.Clamp(m_Pitch + -Input.GetAxis("VerticalR") * m_Rotate * Time.fixedDeltaTime, m_PitchMin, m_PitchMax);
		m_TPSTarget.localRotation = Quaternion.Euler(m_Pitch, 0, 0);
        m_Animator.SetFloat("Up", m_Rigidbody.velocity.y);
    }
    void OnCollisionEnter(Collision other)
    {
//        return;
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.NoPlay:
                break;
            case PlayMode.TwinRobot:
                break;
            case PlayMode.HumanoidRobot:
                switch (other.gameObject.tag)
                {
                    case "Enemy":
                    case "Bullet":
                        foreach (var contact in other.contacts)
                        {
                            GameObject eff = Instantiate(m_Effect_Damage, transform);
                            eff.transform.position = contact.point;
                        }
                        break;
                    case "Floor":
                        //m_Animator.SetBool("Granded",true);
                        break;
                    default:
                        break;
                }
                break;
            case PlayMode.Combine:
                break;
            case PlayMode.Release:
                break;
            default:
                break;
        }
    }
}
