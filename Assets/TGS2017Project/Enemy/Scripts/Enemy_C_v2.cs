﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// スクリプト：敵タイプC（射撃型）Ver.2
/// 製作者：Ho Siu Ki（何兆祺）
/// </summary>

// 敵Cの状態
enum EnemyC_State_v2
{
    Entry,      // 登場
    Normal,     // 通常
    Damage,     // 被弾
    Paralysis   // 麻痺
}

public class Enemy_C_v2 : MonoBehaviour
{
    private Rigidbody m_Rigidbody;                          // Rigidbodyの参照
    private NavMeshAgent m_NavMeshAgent;                    // NavMeshAgentの参照
    private Animator m_Animator;                            // Animatorの参照

    private EnemyC_State_v2 m_State;                        // 敵Cの状態

    private Transform m_Target = null;                      // 追従目標
    [SerializeField]
    private int m_ChangeTargetInterval;                     // 追従目標の変更間隔（フレーム数を入力、60フレーム=1秒）
    private int m_ChangeTargetCount = 0;                    // 目標変更カウンター
    [SerializeField]
    private float m_RotateSpeed;                            // 回転速度

    [SerializeField]
    private float m_ViewingDistance;                        // 見える距離
    [SerializeField]
    private float m_ViewingAngle;                           // 視野角
    private GameObject m_Player = null;                     // プレイヤーの参照
    Transform m_PlayerLookPoint;                            // プレイヤーへの注視点
    [SerializeField]
    private Transform m_EyePoint;                           // 自身の目の位置

    [SerializeField]
    private int m_AttackInterval;                           // 攻撃間隔（フレーム数を入力、60フレーム=1秒）
    private int m_AttackCount = 120;                        // 攻撃カウンター
    [SerializeField]
    private float m_MinDistance;                            // 目標との最短距離

    [SerializeField]
    private Transform m_Muzzle1;                            // 銃口1の位置
    [SerializeField]
    private Transform m_Muzzle2;                            // 銃口2の位置
    [SerializeField]
    private Transform m_Muzzle3;                            // 銃口3の位置

    [SerializeField]
    private GameObject m_Bullet;                            // 弾
    public Transform m_BulletParent;

    [SerializeField]
    private int m_DamageTime;                               // のけぞり時間
    private int m_DamageTimeCounter;                        // のけぞりカウンター

    [SerializeField]
    private int m_ParalysisTime;                            // 麻痺時間（フレーム数を入力、60フレーム=1秒）
    private int m_ParalysisCount;                           // 麻痺時間カウンター

    [SerializeField]
    private int m_EntryTime;                                // 登場モーション時間

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();            // Rigidbodyを取得
        m_NavMeshAgent = GetComponent<NavMeshAgent>();      // NavMeshAgentを取得
        m_NavMeshAgent.destination = transform.position;    // 登場直後はしばらく移動しない
        m_Animator = GetComponentInChildren<Animator>();    // Animatorを取得

        // 追従目標を指定
        SetTarget();
        m_State = EnemyC_State_v2.Entry;
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーが合体中・分離中でなければ
        if (GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release)
        {
            // 登場
            if (m_State == EnemyC_State_v2.Entry)
            {
                --m_EntryTime;

                // 通常状態に移行
                if (m_EntryTime <= 0)
                    m_State = EnemyC_State_v2.Normal;
            }
            // 通常状態
            else if (m_State == EnemyC_State_v2.Normal)
            {
                // プレイヤーに向ける
                LookAtPlayer();

                // プレイヤーに近すぎると後退
                if (IsPlayerNear() && m_Target != null)
                {
                    m_NavMeshAgent.destination = -m_Target.position;
                }

                // 攻撃
                if (m_AttackCount >= m_AttackInterval)
                {
                    // プレイヤーが見えると攻撃
                    if (CanSeePlayer())
                        Attack();
                }

                // 追従目標を変更
                if (m_ChangeTargetCount >= m_ChangeTargetInterval)
                {
                    SetTarget();
                }

                // 攻撃カウンター
                ++m_AttackCount;

                // 目標変更カウンター
                ++m_ChangeTargetCount;
            }
            // 被弾した場合
            else if (m_State == EnemyC_State_v2.Damage)
            {
                // のけぞり時間を加算
                ++m_DamageTimeCounter;
                if (m_DamageTimeCounter >= m_DamageTime)
                {
                    m_State = EnemyC_State_v2.Normal;

                }
            }
            // 麻痺中の場合
            else
            {
                // 麻痺時間を加算
                ++m_ParalysisCount;
                if (m_ParalysisCount >= m_ParalysisTime)
                {
                    m_State = EnemyC_State_v2.Normal;
                }
            }
        }
    }

    // 追従目標を指定
    void SetTarget()
    {
		switch (GameManager.Instance.m_PlayMode)
		{
			// プレイヤー分離時
			case PlayMode.TwinRobot:
			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				// プレイヤーに追従
				m_Target = PlayerManager.Instance.NearPlayer(transform.position);
				m_Player = m_Target.gameObject;
				break;
			// デフォルト状態（何もしない）
			default:
				break;
		}

        // カウンターをリセット
        m_ChangeTargetCount = 0;

        // プレイヤーがいなかったら何もしない
        if (m_Player == null || m_Target == null) return;

        // プレイヤーの注視点を名前で検索して保持
        m_PlayerLookPoint = m_Player.transform.Find("LookPoint");
    }

    // プレイヤーが見える距離内にいるか
    bool IsPlayerInViewingDistance()
    {
        // プレイヤーがいなかったら、falseを返す
        if (m_PlayerLookPoint == null) return false;

        // 自身からプレイヤーまでの距離
        float distanceToPlayer = Vector3.Distance(m_PlayerLookPoint.position, m_EyePoint.position);

        // プレイヤーが見える距離内にいるかどうかを返却
        return (distanceToPlayer <= m_ViewingDistance);
    }

    // プレイヤーが見える視野角内にいるか
    bool IsPlayerInViewingAngle()
    {
        // プレイヤーがいなかったら、falseを返す
        if (m_PlayerLookPoint == null) return false;

        // 自分からプレイヤーへの方向ベクトル
        Vector3 directionToPlayer = m_PlayerLookPoint.position - m_EyePoint.position;
        Vector3 forward = m_EyePoint.forward;
        directionToPlayer.y = 0f;
        forward.y = 0f;
        // 自分の正面向きベクトルとプレイヤーへの方向ベクトルの差分角度
        float angleToPlayer = Vector3.Angle(forward, directionToPlayer);

        // 見える視野角内にいるかどうかを返却
        return (Mathf.Abs(angleToPlayer) <= m_ViewingAngle);
    }

    // プレイヤーにRayを飛ばしたら当たるか
    bool CanHitRayToPlayer()
    {
        // プレイヤーがいなかったら、falseを返す
        if (m_PlayerLookPoint == null) return false;

        // 自分からプレイヤーへの方向ベクトル
        Vector3 directionToPlayer = m_PlayerLookPoint.position - m_EyePoint.position;

        // 壁の向かう側などにいる場合は見えない
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(m_EyePoint.position, directionToPlayer, out hitInfo);
        Debug.DrawLine(m_EyePoint.position, m_EyePoint.position + directionToPlayer);

        // プレイヤーにRayが当たったかどうかを返却
        return (hit && hitInfo.collider.tag == "Player");
    }

    // プレイヤーを見えるか
    bool CanSeePlayer()
    {
        // 見える距離の範囲内にプレイヤーがいない場合→見えない
        if (!IsPlayerInViewingDistance())
            return false;

        // 見える視野角の範囲内にプレイヤーがいない場合→見えない
        if (!IsPlayerInViewingAngle())
            return false;

        // Rayを飛ばして、それがプレイヤーに当たらない場合→見えない
        if (!CanHitRayToPlayer())
            return false;

        // ここまで到達したら、それはプレイヤーが見えるということ
        return true;
    }

    // プレイヤーと近すぎるか
    bool IsPlayerNear()
    {
        // プレイヤーがいなかったら、falseを返す
        if (m_Target == null) return false;

        float distanceToPlayerX = transform.position.x - m_Target.position.x;  // x軸距離
        float distanceToPlayerZ = transform.position.z - m_Target.position.z;  // z軸距離

        return (distanceToPlayerX < m_MinDistance || distanceToPlayerZ < m_MinDistance);
    }

    // プレイヤーに向ける
    void LookAtPlayer()
    {
        // プレイヤーがいなかったら何もしない
        if (m_Target == null) return;

        // 敵との相対位置を取得
        Vector3 relative_position = m_Target.position - transform.position;

        var rotation = Quaternion.LookRotation(relative_position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_RotateSpeed);
    }

    // 攻撃
    void Attack()
    {
        Instantiate(m_Bullet, m_Muzzle1.position, m_Muzzle1.rotation, m_BulletParent);
        Instantiate(m_Bullet, m_Muzzle2.position, m_Muzzle2.rotation, m_BulletParent);
        Instantiate(m_Bullet, m_Muzzle3.position, m_Muzzle3.rotation, m_BulletParent);
        m_AttackCount = 0;
    }

    // 接触判定
    public void OnTriggerEnter(Collider other)
    {
        // 電撃を受けると麻痺
        if (other.name == "Cylinder")
        {
            m_State = EnemyC_State_v2.Paralysis;
            m_ParalysisCount = 0;
        }

        // ロケットパンチを受けると…

    }

    // Gizomsを描画（デバッグ用）
    public void OnDrawGizmos()
    {
        // 見える視野角を描画
        if (m_EyePoint != null)
        {
            // 線の色
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
            // 目の位置
            Vector3 eyePosition = m_EyePoint.position;
            // 正面向きの視線
            Vector3 forward = m_EyePoint.forward * m_ViewingDistance;

            // 正面向きの視線を描画
            Gizmos.DrawRay(eyePosition, forward);
            // 視界の右端を描画
            Gizmos.DrawRay(eyePosition, Quaternion.Euler(0, m_ViewingAngle, 0) * forward);
            // 視界の左端を描画
            Gizmos.DrawRay(eyePosition, Quaternion.Euler(0, -m_ViewingAngle, 0) * forward);
        }

        // 最短射程を描画
        Vector3 min_distance = transform.forward * m_MinDistance;

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawRay(transform.position, min_distance);

        // 文字で情報を表示
        if (m_NavMeshAgent != null)
        {
            string text =
                "Name: " + name
                + "\nState: " + m_State
                + "\nTarget:" + m_Target
                + "\nCanSeePlayer:" + CanSeePlayer()
                + "\nSpeed: " + m_NavMeshAgent.velocity.magnitude;

            //UnityEditor.Handles.Label(transform.position, text);
        }
    }
}