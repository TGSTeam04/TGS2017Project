using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// スクリプト：エネミー Type:2（遠距離型）
/// 作成者：Ho Siu Ki（何兆祺）
/// 参考：http://gametukurikata.com/program/addforcebullet （弾を撃つ）
/// </summary>

// エネミー２状態
enum Enemy2State
{
	Forward,    // 前進
	Backward,   // 後退
	Attack      // 攻撃
}

public class EnemyType2 : MonoBehaviour
{
	private Enemy2State m_State;            // エネミーの状態
	private Transform m_Target;             // 追従目標
	private Rigidbody m_Rigidbody;
	private NavMeshAgent m_Agent;           // 自身のNavMeshAgentをの参照

	[SerializeField]
	private int m_ChangeTargetInterval;     // 追従目標の変更間隔（フレーム数を入力、６０フレーム＝1秒）
	private int m_ChangeTargetCount;        // 目標変更カウンター

	[SerializeField]
	private int m_AttackInterval;           // 攻撃間隔（フレーム数を入力、６０フレーム＝1秒）
	private int m_AttackCount;              // 攻撃カウンター

	[SerializeField]
	private float m_MaxRange;               // 最大射程
	[SerializeField]
	private float m_MinRange;               // 最短射程

	[SerializeField]
	private Transform m_Muzzle;             // 銃口
	[SerializeField]
	private GameObject m_Bullet;            // 弾
	[SerializeField]
	private float m_BulletPower;            // 弾を飛ばす力

	public Transform m_BulletParent;

	// Use this for initialization
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.destination = new Vector3(0.0f, 0.0f, 0.0f);

		// 追従目標を指定
		SetTarget();
		m_State = Enemy2State.Attack;
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

		// 目標との距離を取得
		if (m_Target == null) return;
		float m_Distance = Vector3.Distance(transform.position, m_Target.position);

		// 目標が近すぎると後退
		if (m_Distance < m_MinRange)
		{
			m_State = Enemy2State.Backward;
		}
		// 目標が遠すぎると前進
		else if (m_Distance > m_MaxRange)
		{
			m_State = Enemy2State.Forward;
		}
		// 目標が射程距離内に居れば攻撃
		else
		{
			if (m_AttackCount >= m_AttackInterval && GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release)
			{
				Attack();
			}
		}

		// 目標変更カウンター
		++m_ChangeTargetCount;

		// 攻撃カウンター
		++m_AttackCount;
	}

	void FixedUpdate()
	{
		m_Agent.isStopped = true;
		// 移動処理
		if (m_State == Enemy2State.Forward)
		{
			Forward();
		}
		if (m_State == Enemy2State.Backward)
		{
			Backward();
		}

		// 追従目標に向ける
		if (GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release)
			transform.LookAt(m_Target.transform);
	}

	// 追従目標の変更
	void SetTarget()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
				GameObject m_PlayerL = GameManager.Instance.m_LRobot;
				GameObject m_PlayerR = GameManager.Instance.m_RRobot; ;

				// プレイヤーとの距離を取得
				float m_DistanceL = Vector3.Distance(transform.position, m_PlayerL.transform.position);
				float m_DistanceR = Vector3.Distance(transform.position, m_PlayerR.transform.position);

				// 遠い方のプレイヤーに追従（距離が同じ場合、PlayerL優先）
				if (m_DistanceL >= m_DistanceR)
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

	// 前進
	void Forward()
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

	// 後退
	void Backward()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
				m_Agent.isStopped = false;
				// 
				m_Agent.destination = -m_Target.position;
				break;

			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				m_Agent.isStopped = false;
				// 
				m_Agent.destination = -m_Target.position;
				break;

			// プレイヤーが合体・分離の間は移動しない
			case PlayMode.Combine:
				m_Agent.destination = transform.position;
				break;

			case PlayMode.Release:
				m_Agent.destination = transform.position;
				break;

			// デフォルト状態（何もしない）
			default:
				break;
		}
	}

	// 攻撃
	void Attack()
	{
		m_State = Enemy2State.Attack;
		var bulletInstance = GameObject.Instantiate(m_Bullet, m_Muzzle.position, m_Muzzle.rotation, m_BulletParent);
		bulletInstance.GetComponent<Rigidbody>().AddForce(bulletInstance.transform.forward * m_BulletPower);
		Destroy(bulletInstance, 5);
		m_AttackCount = 0;
	}
}