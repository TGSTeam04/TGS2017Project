using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// スクリプト：エネミー Type:1 ver.2（近距離型）
/// 作成者：Ho Siu Ki（何兆祺）
/// </summary>

// エネミー１状態
enum Enemy1State
{
	Move,   // 移動
	Attack  // 攻撃
}

public class EnemyType1 : MonoBehaviour
{
	private Enemy1State m_State;            // エネミーの状態
	private Transform m_Target;             // 追従目標
	private Rigidbody m_Rigidbody;
	private NavMeshAgent m_Agent;           // 自身のNavMeshAgentをの参照

	[SerializeField]
	private int m_ChangeTargetInterval;   // 追従目標の変更間隔（フレーム数を入力、６０フレーム＝1秒）
	private int m_ChangeTargetCount;      // 目標変更カウンター

	[SerializeField]
	private int m_AttackInterval;         // 攻撃間隔（フレーム数を入力、６０フレーム＝1秒）
	private int m_AttackCount;            // 攻撃カウンター

	[SerializeField]
	private float m_Range;                  // 攻撃範囲

	// Use this for initialization
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.destination = new Vector3(0.0f, 0.0f, 0.0f);

		// 追従目標を指定
		SetTarget();
		m_State = Enemy1State.Move;
		m_AttackCount = 0;
	}

	// Update is called once per frame
	void Update()
	{
		// 一定間隔で追従目標を変更
		if (m_ChangeTargetCount >= m_ChangeTargetInterval)
		{
			SetTarget();
		}

		// プレイヤーが攻撃範囲に入ると、攻撃する
		if (m_Target == null) return;
		else
		{
			float m_Distance = Vector3.Distance(transform.position, m_Target.position);
			if (m_Distance <= m_Range && GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release)
			{
				Attack();
			}
		}

		// 目標変更カウンター
		++m_ChangeTargetCount;

		// 攻撃カウンター
		if (m_State == Enemy1State.Attack)
		{
			++m_AttackCount;
		}
		if (m_AttackCount >= m_AttackInterval)
		{
			m_State = Enemy1State.Move;
		}
	}

	void FixedUpdate()
	{
		m_Agent.isStopped = true;
		if (m_State == Enemy1State.Move)
		{
			Move();
		}
	}

	// 追従目標を指定
	void SetTarget()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
				GameObject m_PlayerL = GameManager.Instance.m_LRobot;
				GameObject m_PlayerR = GameManager.Instance.m_RRobot;

				// プレイヤーとの距離を取得
				float m_DistanceL = Vector3.Distance(transform.position, m_PlayerL.transform.position);
				float m_DistanceR = Vector3.Distance(transform.position, m_PlayerR.transform.position);

				// 近い方のプレイヤーに追従（距離が同じ場合、PlayerL優先）
				if (m_DistanceL <= m_DistanceR)
				{
					m_Target = m_PlayerL.transform;
				}
				else
				{
					m_Target = m_PlayerR.transform;
				}
				break;

			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				GameObject m_Player = GameManager.Instance.m_HumanoidRobot;

				// プレイヤーに追従
				m_Target = m_Player.transform;
				break;

			// デフォルト状態（何もしない）
			default:
				break;
		}
		// カウンターをリセット
		m_ChangeTargetCount = 0;
	}

	// 移動
	void Move()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
				m_Agent.isStopped = false;
				// プレイヤーに向かって移動
				m_Agent.destination = m_Target.position;
				break;

			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				m_Agent.isStopped = false;
				// プレイヤーに向かって移動
				m_Agent.destination = m_Target.position;
				break;

			// プレイヤーが合体・分離の間は移動しない
			case PlayMode.Combine:
				//m_Agent.destination = transform.position;
				break;

			case PlayMode.Release:
				//m_Agent.destination = transform.position;
				break;

			// デフォルト状態（何もしない）
			default:
				break;
		}
	}

	// 攻撃
	void Attack()
	{
		if (m_State != Enemy1State.Attack)
		{
			m_State = Enemy1State.Attack;
			// カウンターをリセット
			m_AttackCount = 0;
		}
	}
}