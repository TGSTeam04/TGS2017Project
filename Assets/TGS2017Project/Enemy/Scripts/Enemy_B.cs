using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// スクリプト：敵タイプB（近距離型）
/// 製作者：Ho Siu Ki（何兆祺）
/// </summary>

// 敵Cの状態
enum EnemyB_State
{
    Entry,      // 登場
    Normal,     // 通常
    Damage,     // 被弾
    Paralysis   // 麻痺
}

public class Enemy_B : MonoBehaviour
{
    private Rigidbody m_Rigidbody;                          // Rigidbodyの参照
    private NavMeshAgent m_NavMeshAgent;                    // NavMeshAgentの参照
    private Animator m_Animator;                            // Animatorの参照

    private EnemyB_State m_State;                           // 敵Bの状態

    private Transform m_Target = null;                      // 追従目標
    [SerializeField]
    private float m_ChangeTargetInterval;                   // 追従目標の変更間隔（フレーム数を入力、60フレーム=1秒）
    private float m_ChangeTargetCount = 0;                  // 目標変更カウンター
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
    private float m_AttackInterval;                         // 攻撃間隔（フレーム数を入力、60フレーム=1秒）
    private float m_AttackCount = 120;                      // 攻撃カウンター
    [SerializeField]
    private float m_Range;                                  // 射程距離
    [SerializeField]
    private Transform m_Muzzle;                             // 銃口の位置
    [SerializeField]
    private GameObject m_Bullet;                            // 弾
    public Transform m_BulletParent;

    [SerializeField]
    private float m_DamageTime;                             // のけぞり時間
    private float m_DamageTimeCounter;                      // のけぞりカウンター

    [SerializeField]
    private float m_ParalysisTime;                          // 麻痺時間（フレーム数を入力、60フレーム=1秒）
    private float m_ParalysisCount;                         // 麻痺時間カウンター

    [SerializeField]
    private float m_EntryTime;                              // 登場モーション時間
    [SerializeField]
    private ParticleSystem m_FireEffect;                    // 攻撃エフェクト
    [SerializeField]
    private GameObject m_FireEffect2;

    // SE
    private AudioSource m_Fire;                             // 発砲

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();            // Rigidbodyを取得
        m_NavMeshAgent = GetComponent<NavMeshAgent>();      // NavMeshAgentを取得
        m_NavMeshAgent.destination = transform.position;    // 登場直後はしばらく移動しない
        m_Animator = GetComponentInChildren<Animator>();    // Animatorを取得

        AudioSource[] m_AudioSources = GetComponents<AudioSource>();
        m_Fire = m_AudioSources[0];

        // 追従目標を指定
        SetTarget();
        m_State = EnemyB_State.Entry;
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーが合体中・分離中でなければ
        if (GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release)
        {
            // 登場モーション
            if (m_State == EnemyB_State.Entry)
            {
                m_EntryTime -= Time.deltaTime;

                // 通常状態に移行
                if (m_EntryTime <= 0)
                    m_State = EnemyB_State.Normal;
            }
            // 通常状態
            else if (m_State == EnemyB_State.Normal)
            {
                // プレイヤーに向けて移動
                LookAtPlayer();
                if (m_Target != null)
                {
                    m_NavMeshAgent.destination = m_Target.position;
                    m_Animator.SetFloat("Speed", m_NavMeshAgent.velocity.sqrMagnitude);
                }

                // プレイヤーが見えると
                if (m_AttackCount >= m_AttackInterval)
                {
                    if (IsPlayerInViewingAngle() && IsPlayerInViewingDistance())
                    {
                        Attack();
                    }
                }

                // 追従目標を変更
                if (m_ChangeTargetCount >= m_ChangeTargetInterval)
                {
                    SetTarget();
                }

                // 攻撃カウンター
                m_AttackCount += Time.deltaTime;

                // 目標変更カウンター
                m_ChangeTargetCount += Time.deltaTime;
            }
            // 被弾した場合
            else if (m_State == EnemyB_State.Damage)
            {
                // のけぞり時間を加算
                m_DamageTimeCounter += Time.deltaTime;
                if (m_DamageTimeCounter >= m_DamageTime)
                {
                    m_State = EnemyB_State.Normal;

                }
            }
            // 麻痺中の場合
            else
            {
                // 麻痺時間を加算
                m_ParalysisCount += Time.deltaTime;
                if (m_ParalysisCount >= m_ParalysisTime)
                {
                    m_State = EnemyB_State.Normal;
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
        directionToPlayer.y = 0.0f;

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
        m_Animator.SetBool("Attack", true);
        m_Fire.Play();      // SEを再生
        //m_FireEffect.transform.rotation = m_Muzzle.transform.rotation;
        //m_FireEffect.Play();
        Instantiate(m_FireEffect2, m_Muzzle.position, m_Muzzle.rotation, m_BulletParent);
        Instantiate(m_Bullet, m_Muzzle.position, m_Muzzle.rotation, m_BulletParent);
        m_AttackCount = 0;
    }

    // 接触判定
    public void OnTriggerEnter(Collider other)
    {
        // 電撃を受けると麻痺
        if (other.name == "Cylinder")
        {
            // 麻痺してない場合、麻痺状態になる
            if (m_State != EnemyB_State.Paralysis)
                m_State = EnemyB_State.Paralysis;
            m_ParalysisCount = 0;
        }

        // ロケットパンチを受けると…
    }

    // Gizomsを描画（デバッグ用）
    public void OnDrawGizmosSelected()
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