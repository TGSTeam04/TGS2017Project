using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public enum BossState
	{
		Move,
		Attack,
		Look,
		Paralysis
	}

	[SerializeField]
	private float m_MoveSpeed = 20.0f;
	[SerializeField]
	private float m_RotateSpeed = 30.0f;
	[SerializeField]
	private int m_Interval = 10;
	[SerializeField]
	private int m_LookFrequency = 3;

	private int m_AttackCounter;
	private int m_LookCounter;

	private bool m_SwingAttack = false;

	public static bool s_IsDead = false;

	public Transform m_LeftRocket;
	public Transform m_RightRocket;
	public Transform m_Core;
	public GameObject m_SearchArea;
	public GameObject m_Explosion;

	Transform m_Target;
	Vector3 m_TargetPosition;

	Animator m_Anim;

	[SerializeField]
	BossState m_State = BossState.Move;

	// Use this for initialization
	void Start()
	{
		m_Target = GameObject.FindGameObjectWithTag("Player").transform;
		m_Anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.TwinRobot:
				break;
			case PlayMode.HumanoidRobot:
				break;
			case PlayMode.NoPlay:
			case PlayMode.Combine:
			case PlayMode.Release:
			default:
				return;
		}
		if (GameManager.Instance.m_StageManger.StageLevel < 1)
		{
			return;
		}
		if (m_LookCounter <= 0)
		{
			StartCoroutine(Look());
		}
		m_TargetPosition = m_Target.position;
		m_TargetPosition.y = transform.position.y;
		if (Input.GetKeyDown(KeyCode.L))
		{
			m_State = BossState.Paralysis;
		}

		switch (m_State)
		{
			case BossState.Move:
				m_Anim.speed = 1.0f;
				if (Vector3.Distance(transform.position, m_TargetPosition) > 60 && GameManager.Instance.m_StageManger.StageLevel > 1)
				{
					transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
				}
				else
				{
					m_State = BossState.Attack;
				}
				break;
			case BossState.Attack:
				m_Anim.speed = 1.0f;
				Attack();
				if (Vector3.Distance(transform.position, m_TargetPosition) > 60)
				{
					m_State = BossState.Move;
				}
				break;
			case BossState.Look:
				m_Anim.speed = 1.0f;
				float angle = Vector3.Angle(transform.forward, m_TargetPosition - transform.position);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_TargetPosition - transform.position), m_RotateSpeed * Time.deltaTime);
				if (angle < 15)
				{
					if (Vector3.Distance(transform.position, m_TargetPosition) > 60)
					{
						m_State = BossState.Move;
					}
					else
					{
						m_State = BossState.Attack;
					}
				}
				break;
			case BossState.Paralysis:
				m_Anim.speed = 0f;
				break;
		}
	}
	void Attack()
	{
		if (Searching.s_Find)
		{
			m_SearchArea.SetActive(false);
			if (Vector3.Distance(m_LeftRocket.position, m_Target.position) < Vector3.Distance(m_RightRocket.position, m_Target.position))
			{
				m_Anim.SetBool("LeftPunch", true);
			}
			else
			{
				m_Anim.SetBool("RightPunch", true);
			}
			Searching.s_Find = false;
			m_SwingAttack = true;
			StartCoroutine(AttackInterval());
		}
		if (m_AttackCounter > 0 && m_AttackCounter < 8 && m_SwingAttack && GameManager.Instance.m_StageManger.StageLevel > 1)
		{
			if (Vector3.Distance(m_SearchArea.transform.position, m_Target.position) > 20)
			{
				m_SwingAttack = false;
				if (Random.Range(0, 200) > 100)
				{
					m_Anim.SetBool("LeftSwing", true);
				}
				else
				{
					m_Anim.SetBool("RightSwing", true);
				}
			}
		}
	}
	void Dead()
	{
		GameManager.Instance.m_IsGameClear = true;
		Instantiate(m_Explosion, m_Core.position, transform.rotation);
		Destroy(gameObject);
	}
	IEnumerator AttackInterval()
	{
		m_AttackCounter = m_Interval;
		while (m_AttackCounter > 0)
		{
			yield return new WaitForSeconds(1.0f);
			m_AttackCounter--;
		}
		m_SearchArea.SetActive(true);
	}
	IEnumerator Look()
	{
		m_LookCounter = m_LookFrequency;
		while (m_LookCounter > 0)
		{
			yield return new WaitForSeconds(1.0f);
			m_LookCounter--;
		}
		if (m_State != BossState.Paralysis && GameManager.Instance.m_StageManger.StageLevel > 1)
		{
			m_State = BossState.Look;
		}
	}
	IEnumerator Recovery()
	{
		yield return new WaitForSeconds(10.0f);
		GetComponent<EnemyBase>().m_IsShock = false;
		m_State = BossState.Look;
		m_SearchArea.SetActive(true);
	}
	public void OnTriggerEnter(Collider other)
	{
		if (other.name == "Beam" && GameManager.Instance.m_StageManger.StageLevel > 1)
		{
			m_SearchArea.SetActive(false);
			GetComponent<EnemyBase>().m_IsShock = true;
			m_State = BossState.Paralysis;
			StartCoroutine(Recovery());
		}
		if (GameManager.Instance.m_PlayMode == PlayMode.Combine && m_State == BossState.Paralysis && other.name == "Break")
		{
			Dead();
		}
	}
}
