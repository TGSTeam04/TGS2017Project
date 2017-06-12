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

    [SerializeField]
    private Animator m_Animator;

    private float m_Speed;
    private float m_Rotate;

    private bool m_Charging;

    [SerializeField]
    private RocketBattery m_Battery;
    //ダメージコンポーネント
    private Damageable m_Damage;

    private void Awake()
    {
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Damage = GetComponent<Damageable>();
        m_Damage.Del_ReciveDamage = Damage;
    }
    public void Damage(float damage, MonoBehaviour src)
    {
        //ApplyDamageされたときの処理
        m_Energy -= damage;
    }

    // Update is called once per frame
    public void UpdateInput()
    {
        if (Input.GetButtonDown(m_BaseConfig.m_InputJump) && IsGround())
        {
            m_Animator.SetTrigger("Jump");
            m_Rigidbody.AddForce(Vector3.up * m_Config.m_JumpPower, ForceMode.Impulse);
        }

			if (Input.GetButtonDown("RotateL") && m_Energy >= m_Config.m_ChargeUseEnergy && !m_Charging && m_Battery.LIsCanFire)
			{
				StartCoroutine(Charge(true));
			}
			else if (Input.GetButtonDown("RotateR") && m_Energy >= m_Config.m_ChargeUseEnergy && !m_Charging && m_Battery.RIsCanFire)
			{
				StartCoroutine(Charge(false));
			}

			float energy = 0;
        if (Input.GetButton(m_BaseConfig.m_InputBoost) && !m_Charging)
        {
            m_Animator.SetBool("IsBoost", true);
            m_Speed = m_Config.m_BoostSpeed;
            energy = m_Config.m_BoostUseEnergy;
        }
        else
        {
            m_Animator.SetBool("IsBoost", false);
            m_Speed = m_Config.m_NormalSpeed;
            energy = m_Config.m_NormalUseEnergy;
        }
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
			yield return new WaitForSeconds(1.0f);

			m_Charging = false;
		}

		public bool IsGround()
    {
        return Physics.CheckSphere(m_Rigidbody.position + Vector3.up * 0.7f, 0.72f, ~LayerMask.GetMask(new string[] { "Player" }));
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
        m_Animator.SetFloat("Up", m_Rigidbody.velocity.y);
    }
    void OnCollisionEnter(Collision other)
    {
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
                        m_Energy -= 2;
                        m_Animator.SetTrigger("Damage");
                        //Debug.Log("damege");
                        break;
                    case "Floor":
                        m_Animator.SetTrigger("Granded");
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
