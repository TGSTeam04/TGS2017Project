using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// スクリプト：エネミー（遠距離型）ver.2
/// 作成者：Ho Siu Ki（何兆祺）
/// </summary>

// エネミー２状態
enum Enemy2State_New
{
    FollowPlayer,       // プレイヤーを追従
    AdjustPosition,     // 位置を調整（他の敵と射線を被らないようにする）
    Attack              // 攻撃
}

public class EnemyType2v2 : MonoBehaviour
{
    private NavMeshAgent m_Agent;           // 自身のNavMeshAgentを参照
    private Rigidbody m_Rigidbody;          // 自身のRigidbodyを参照

    private Enemy2State_New m_State;        // 自身の状態

    private Transform m_Target;             // 追従目標
    [SerializeField]
    private int m_ChangeTargetInterval;     // 追従目標の変更間隔（フレーム数を入力、６０フレーム＝1秒）
    private int m_ChangeTargetCount = 0;    // 目標変更カウンター

    [SerializeField]
    private int m_AttackInterval;           // 攻撃間隔（フレーム数を入力、６０フレーム＝1秒）
    private int m_AttackCount = 0;          // 攻撃カウンター

    [SerializeField]
    private float m_MaxRange;               // 最大射程
    [SerializeField]
    private float m_MinRange;               // 最短射程

    [SerializeField]
    private Transform m_Muzzle;             // 銃口
    [SerializeField]
    private Transform m_EyePoint;           // 自身の目の位置
    [SerializeField]
    private GameObject m_Bullet;            // 弾

    public Transform m_BulletParent;

    [SerializeField]
    private int m_ParalysisTime;            // 麻痺時間（フレーム数を入力、６０フレーム＝1秒）
    private int m_ParalysisCount;           // 麻痺時間カウンター
    private bool m_IsParalysis = false;     // 麻痺しているか（デフォルトはfalse）

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.destination = new Vector3(0.0f, 0.0f, 0.0f);

        // 追従目標を指定
        SetTarget();
        m_State = Enemy2State_New.FollowPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーが合体中・分離中でなければ
        if (GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release && GameManager.Instance.m_PlayMode != PlayMode.NoPlay)
        {
            // 麻痺していなければ
            if (!m_IsParalysis)
            {
                // プレイヤーに追従中
                if (m_State == Enemy2State_New.FollowPlayer)
                {
                    // プレイヤーが射程内にいれば攻撃
                    if (IsPlayerInRange())
                    {
                        m_State = Enemy2State_New.Attack;
                    }
                }
                // プレイヤーに攻撃
                else if (m_State == Enemy2State_New.Attack)
                {
                    if (m_AttackCount >= m_AttackInterval)
                    {
                        Attack();
                    }

                    // プレイヤーが射程内にいなければ、移動を再開
                    if (!IsPlayerInRange())
                    {
                        m_State = Enemy2State_New.FollowPlayer;
                    }
                }
            }

            // 一定間隔で追従目標を変更
            if (m_ChangeTargetCount >= m_ChangeTargetInterval)
            {
                SetTarget();
            }

            // 攻撃カウンター
            ++m_AttackCount;

            // 目標変更カウンター
            ++m_ChangeTargetCount;
        }

        // 麻痺カウンター
        --m_ParalysisCount;
        // 麻痺時間終了
        if (m_ParalysisCount <= 0)
        {
            m_IsParalysis = false;
        }
    }

    void FixedUpdate()
    {
        // プレイヤーが合体中・分離中でなければ
        if (GameManager.Instance.m_PlayMode != PlayMode.Combine && GameManager.Instance.m_PlayMode != PlayMode.Release && GameManager.Instance.m_PlayMode != PlayMode.NoPlay)
        {
            // 麻痺していなければ
            if (!m_IsParalysis)
            {
                // プレイヤーに追従中
                if (m_State == Enemy2State_New.FollowPlayer)
                    Move();     // 移動処理

				if (m_Target == null) return;
                // 追従目標に向ける
                transform.LookAt(m_Target.transform);
            }
        }
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

    // プレイヤーが射程圏内にいるか
    bool IsPlayerInRange()
    {
		if (m_Target == null) return false;
        // float distanceToPlayer = Vector3.Distance(m_EyePoint.position, m_Target.position);
        // return (distanceToPlayer < m_MaxRange && distanceToPlayer > m_MinRange);
        float distanceToPlayerX = m_EyePoint.position.x - m_Target.position.x;  // x軸距離
        float distanceToPlayerZ = m_EyePoint.position.z - m_Target.position.z;  // z軸距離

        return (distanceToPlayerX < m_MaxRange && distanceToPlayerX > m_MinRange
            && distanceToPlayerZ < m_MaxRange && distanceToPlayerZ > m_MinRange);
    }

    // プレイヤーにRayを飛ばしたら当たるか
    bool CanHitRayToPlayer()
    {
        // 自分からプレイヤーへの方向ベクトル
        Vector3 directionToPlayer = m_Target.position - m_EyePoint.position;
        // 壁の向こう側などにいる場合は見えない
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(m_EyePoint.position, directionToPlayer, out hitInfo);

        // プレイヤーにRayが当たったかどうかを返却
        return (hit && hitInfo.collider.tag == "Player");
    }

    // 移動
    void Move()
    {
		if (m_Target == null) return;
        float distanceToPlayer = Vector3.Distance(m_EyePoint.position, m_Target.position);

        // 目標が近すぎると後退
        if (distanceToPlayer <= m_MinRange)
            Backward();
        // 目標が遠すぎると前進
        else if (distanceToPlayer >= m_MaxRange)
            Forward();
    }

    // 前進
    void Forward()
    {
        m_Agent.isStopped = false;
        // プレイヤーに向かって移動
        m_Agent.destination = m_Target.position;
    }

    // 後退
    void Backward()
    {
        m_Agent.isStopped = false;
        // プレイヤーの逆方向へ移動
        m_Agent.destination = -m_Target.position;
    }

    // 位置を調整
    void AdjustPosition()
    {
        float speed = 1.0f;
        int setAxis = Random.Range(0, 2);
        Vector3 axis;       // 回転軸
        // ランダムで回り方向を決める
        if (setAxis == 0)
            axis = transform.TransformDirection(Vector3.up);    // 時計回り
        else
            axis = transform.TransformDirection(Vector3.down);  // 反時計回り

        transform.RotateAround(m_Target.position, axis, speed * Time.deltaTime);
    }

    // 攻撃
    void Attack()
    {
        Instantiate(m_Bullet, m_Muzzle.position, m_Muzzle.rotation, m_BulletParent);
        m_AttackCount = 0;
    }

    // 接触判定
    public void OnTriggerEnter(Collider other)
    {
        // 電撃を受けると麻痺
        if (other.name == "Cylinder")
        {
            m_IsParalysis = true;
            m_ParalysisCount = m_ParalysisTime;
        }
    }
}