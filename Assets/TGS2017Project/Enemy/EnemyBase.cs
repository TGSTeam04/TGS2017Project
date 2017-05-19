using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum BreakType
{
	Normal,
	Shock,
	UnBreak
}
public class EnemyBase : MonoBehaviour
{
	public float m_MoveSpeed;
	private float m_SpeedRate;
	public bool m_IsDead;
	private bool m_IsShock;
	private Rigidbody m_Rigidbody;

	public GameObject m_LRobot;
	public GameObject m_RRobot;
	public GameObject m_HumanoidRobot;

	private Vector3 m_LRobotPos;
	private Vector3 m_RRobotPos;

	private Vector3 m_Target;

	private EnemyManager m_EnemyManager;

	public BreakType m_BreakType;

	public GameObject m_Fragment;

	private NavMeshAgent m_NavMeshAgent;

	public bool IsBreakable
	{
		get
		{
			switch (m_BreakType)
			{
				case BreakType.Normal:
					return true;
				case BreakType.Shock:
					return m_IsShock;
			}
			return false;
		}
	}

	// Use this for initialization
	void Start()
	{
		m_SpeedRate = 1.0f;
		m_IsDead = false;
		m_IsShock = false;
		m_Rigidbody = GetComponent<Rigidbody>();
		m_LRobot = GameManager.Instance.m_LRobot;
		m_RRobot = GameManager.Instance.m_RRobot;
		m_HumanoidRobot = GameManager.Instance.m_HumanoidRobot;
		m_EnemyManager = GetComponentInParent<EnemyManager>();
		m_NavMeshAgent = GetComponent<NavMeshAgent>();
		//NextTarget();
	}

	// Update is called once per frame
	void Update()
	{
		m_LRobot = GameManager.Instance.m_LRobot;
		m_RRobot = GameManager.Instance.m_RRobot;
		m_HumanoidRobot = GameManager.Instance.m_HumanoidRobot;
		m_LRobotPos = m_LRobot.transform.position;
		m_RRobotPos = m_RRobot.transform.position;

		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				//Move();
				break;
			case PlayMode.HumanoidRobot:
				//Move();
				break;
			case PlayMode.Combine:
				break;
			case PlayMode.Release:
				break;
			default:
				break;
		}
		m_Rigidbody.velocity = Vector3.zero;
	}

	private void Move()
	{
		int count = 0;
		do
		{
			Vector3 forward = m_Target - transform.position;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, forward, out hit, forward.magnitude + 2))
			{
				if (hit.collider.tag == "Fence")
				{
					NextTarget();
					count += 1;
				}
				else
				{
					count = 5;
				}
			}
			else
			{
				count = 5;
			}
		}
		while (count < 5);
		m_Rigidbody.position += Vector3.Normalize(m_Target - transform.position) * m_MoveSpeed * m_SpeedRate * Time.deltaTime;
		if (Vector3.Distance(transform.position, m_Target) < 0.1f)
		{
			NextTarget();
		}
	}
	public void NextTarget()
	{
		m_Target = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
		transform.LookAt(m_Target);
	}

	public void SetBreak()
	{
		m_EnemyManager.ReSpawnEnemy(gameObject);
		m_IsDead = true;

	}

	public void OnTriggerEnter(Collider other)
	{
		if (GameManager.Instance.m_PlayMode == PlayMode.Combine && other.name == "Break" && m_IsDead)
		{
			gameObject.SetActive(false);
			//m_Fragment.SetActive(true);
		}
		if (GameManager.Instance.m_PlayMode == PlayMode.TwinRobot && other.tag == "Guid")
		{
			m_SpeedRate = 0.5f;
			m_LRobotPos = m_LRobot.transform.position;
			m_RRobotPos = m_RRobot.transform.position;
		}
	}
	public void OnTriggerStay(Collider other)
	{
		if (GameManager.Instance.m_PlayMode == PlayMode.TwinRobot && other.tag == "Guid")
		{
			//m_Rigidbody.position += Vector3.Lerp(
			m_NavMeshAgent.Move(Vector3.Lerp(
				m_LRobot.transform.position - m_LRobotPos,
				m_RRobot.transform.position - m_RRobotPos,
				Vector3.Distance(transform.position, m_LRobot.transform.position) /(
				Vector3.Distance(transform.position, m_LRobot.transform.position) +
				Vector3.Distance(transform.position, m_RRobot.transform.position))) * 0.3f);
		}
	}
	public void OnTriggerExit(Collider other)
	{
		if (GameManager.Instance.m_PlayMode == PlayMode.TwinRobot && other.tag == "Guid")
		{
			m_SpeedRate = 1.0f;
		}

	}
}
