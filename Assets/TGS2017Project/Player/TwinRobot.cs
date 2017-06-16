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
	[SerializeField] private PlayerController m_Controller;
    [SerializeField] private GameObject m_Shield;
    [SerializeField] private TwinRobotConfig m_Config;
    [SerializeField] private TwinRobotBaseConfig m_BaseConfig;
	[SerializeField] private float m_BreakerSizeS;
	[SerializeField] private float m_BreakerSizeL;


	private float m_HP;
    private Renderer m_Renderer;
    private Rigidbody m_Rigidbody;
    private TwinRobotMode m_Mode;
	private float m_Axis;

    //ダメージコンポーネント
    private Damageable m_Damage;

    void Awake()
    {
        m_Damage = GetComponent<Damageable>();
        m_Damage.Del_ReciveDamage = Damage;
        m_Renderer = m_Shield.GetComponent<Renderer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        HP = m_BaseConfig.m_MaxHP;
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
        HP -= damage;
    }

    public void Move()
    {
        Vector3 move = new Vector3(
            Input.GetAxis(m_Config.m_InputHorizontal), 0,
            Input.GetAxis(m_Config.m_InputVertical));
        move = Vector3.ClampMagnitude(move, 1.0f) * m_BaseConfig.m_MoveSpeed * Time.fixedDeltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + move);
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
                        if (!m_Shield.activeSelf)
                        {
                            GameManager.Instance.m_PlayMode = PlayMode.NoPlay;
                            GameManager.Instance.m_GameStarter.ChangeScenes(9);
                        }
                        HP -= 0.1f;
                        break;
                    default:
                        break;
                }
                break;
			case PlayMode.Combine:
				switch (other.gameObject.tag)
				{
					case "Wall":
						m_Controller.Crushable = false;
						break;
					default:
						break;
				}
				break;
            default:
                break;
        }
    }

    public void Active(bool active)
    {
        m_Shield.SetActive(active && HP != 0);
    }

    public float HP
    {
        get { return m_HP; }
        set
        {
            m_HP = Mathf.Clamp(value, 0, m_BaseConfig.m_MaxHP);
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
}
