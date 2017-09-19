using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwinRoboMode
{
    A,
    B
}

public enum HumanoidMode
{
	Normal,
	Speed,
	Power
}

/// <summary>
/// プレイヤーの制御
/// 製作者：野澤翔太
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Dictionary<TwinRobotMode, Quaternion> m_RotateTwinRoboMode = new Dictionary<TwinRobotMode, Quaternion>();
    public GameObject m_HRobot;
    public GameObject m_LRobot;
    public GameObject m_RRobot;
    public GameObject m_Electric;

    public float m_MoveSpeed;

    private Rigidbody m_LRobotRigidbody;
    private Rigidbody m_RRobotRigidbody;
    private Rigidbody m_HumanoidRobotRigidbody;

    public float m_CombineTime;
    public AnimationCurve m_CombineCurve;
    public AnimationCurve m_ReleaseCurve;

    public float m_Speed;
    public float m_RotateSpeed;

    public float m_BoostTime;
    public float m_BoostPower;
    private float m_BoostSpeed = 0f;

    public GameObject m_KeepEnemyPosWall;

    public int m_Level;
    public int m_Exp;

    //ロケット砲台コンポーネント
    public RocketBattery m_Battery;

    public Transform m_TPSPosition;
    public Transform m_ChargePosition;

    private bool m_IsBeamShooting;

    public float m_JumpMove;

    public TwinRobot m_TwinRobotL;
    public TwinRobot m_TwinRobotR;
    public HumanoidRobot m_HumanoidRobot;

    private SubjectBase m_Subject;

    public GameObject m_Boost;

    public Color m_ModeAA;
    public Color m_ModeAB;
    public Color m_ModeBB;

    public GameObject m_ChargeEffect;
    public GameObject m_CombineEffect;

    [SerializeField]
    private HumanoidConfig m_HumanoidN;
    [SerializeField]
    public HumanoidConfig m_HumanoidT;
    [SerializeField]
    public HumanoidConfig m_HumanoidI;

    [SerializeField]
    private Material m_HumanoidMaterial;

    private float m_AxisL;
    private float m_AxisR;

    [SerializeField]
    private AnimationCurve m_CombineDistance;

    [SerializeField]
    private AnimationCurve m_RotationCurve;

	public HumanoidMode m_HMode;

	public bool m_CanRelease;

	private IEnumerator m_Coroutine;

    private void Awake()
    {
        m_Subject = new SubjectBase();
        GameManager.Instance.m_IsGameClear = false;
        GameManager.Instance.m_IsGameOver = false;
        GameManager.Instance.m_PlayScore = 0;
        GameManager.Instance.m_PlayTime = 0;
		GameManager.Instance.m_PlayerController = this;
		//GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
		GameManager.Instance.m_BossHpRate1 = 0;
		GameManager.Instance.m_BossHpRate2 = 0;
		GameManager.Instance.m_BossHpRate3 = 0;
	}


	// Use this for initialization
	void Start()
    {
        m_LRobotRigidbody = m_LRobot.GetComponent<Rigidbody>();
        m_RRobotRigidbody = m_RRobot.GetComponent<Rigidbody>();
        m_HumanoidRobotRigidbody = m_HRobot.GetComponent<Rigidbody>();
        m_HRobot.SetActive(false);
        //m_Battery = m_HRobot.GetComponent<RocketBattery>();
        m_KeepEnemyPosWall.SetActive(false);

        m_Level = 1;
        m_Exp = 0;
        GameManager.Instance.m_CombineTime = m_CombineTime;
        //		GameManager.Instance.m_StageManger.m_Observer.BindSubject(m_Subject);
        m_RotateTwinRoboMode.Add(TwinRobotMode.A, Quaternion.Euler(0, 0, 0));
        m_RotateTwinRoboMode.Add(TwinRobotMode.B, Quaternion.Euler(0, 90, 0));

        m_HumanoidMaterial.color = m_ModeAA;
		m_CanRelease = true;
    }

    void LateUpdate()
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                if ((Input.GetButton("CombineL") && Input.GetButtonDown("CombineR")) || (Input.GetButtonDown("CombineL") && Input.GetButton("CombineR")))
                {
					m_Coroutine = Combine();
                    StartCoroutine(m_Coroutine);
                    break;
                }
                m_TwinRobotL.UpdateInput();
                m_TwinRobotR.UpdateInput();
                break;
            case PlayMode.HumanoidRobot:
                if (m_CanRelease && m_Battery.LIsCanFire && m_Battery.RIsCanFire && ((Input.GetButton("CombineL") && Input.GetButtonDown("CombineR")) || (Input.GetButtonDown("CombineL") && Input.GetButton("CombineR")) || m_HumanoidRobot.m_Energy <= 0))
                {
					m_Coroutine = Release(false);
					StartCoroutine(m_Coroutine);
                    break;
                }
                m_HumanoidRobot.UpdateInput();
                break;
            case PlayMode.Combine:
            case PlayMode.Release:
            case PlayMode.NoPlay:
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                m_TwinRobotL.Move();
                m_TwinRobotR.Move();
                ElectricUpdate();
                break;
            case PlayMode.HumanoidRobot:
                m_HumanoidRobot.Move();
                break;
            case PlayMode.Combine:
            case PlayMode.Release:
                ElectricUpdate();
                break;
            case PlayMode.NoPlay:
            default:
                break;
        }
    }

    private void ElectricUpdate()
    {
        m_Electric.transform.position = Vector3.Lerp(m_LRobotRigidbody.position, m_RRobotRigidbody.position, 0.5f);
        m_Electric.transform.localScale = new Vector3(1.2f, 1.2f, Vector3.Distance(m_LRobotRigidbody.position, m_RRobotRigidbody.position));
        m_Electric.transform.LookAt(m_LRobot.transform);
    }

    public IEnumerator Combine()
    {
        GameManager.Instance.m_PlayMode = PlayMode.Combine;
        m_TwinRobotL.SetShieldActive(false);
        m_TwinRobotR.SetShieldActive(false);
        m_KeepEnemyPosWall.SetActive(true);

        Vector3 StartPositionL = m_LRobotRigidbody.position;
        Vector3 StartPositionR = m_RRobotRigidbody.position;
        Vector3 Direction = Vector3.Normalize(StartPositionL - StartPositionR);
        Vector3 CenterPosition = Vector3.Lerp(StartPositionL, StartPositionR, 0.5f);
		float Distance = Vector3.Distance(CenterPosition, StartPositionL);
		Vector3 EndPositionL = CenterPosition + Direction * 1.0f;
        Vector3 EndPositionR = CenterPosition - Direction * 1.0f;
        m_HRobot.transform.position = CenterPosition;
        m_HRobot.transform.LookAt(CenterPosition + Vector3.Cross(-Direction, Vector3.up));

        m_TPSPosition.parent.localRotation = Quaternion.identity;
		TPSPositionSet(Distance);
        m_CombineEffect.transform.position = m_Electric.transform.position + new Vector3(0, 0.5f, 0);

        float time = 0.3f;
        Quaternion StartRotationL = m_LRobotRigidbody.rotation;
        Quaternion StartRotationR = m_RRobotRigidbody.rotation;
        Quaternion EndRotationL = Quaternion.LookRotation(-Direction);
        Quaternion EndRotationR = Quaternion.LookRotation(Direction);
        for (float t = 0; t < time; t += Time.fixedDeltaTime)
        {
            m_LRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationL, EndRotationL, m_RotationCurve.Evaluate(t / time)));
            m_RRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationR, EndRotationR, m_RotationCurve.Evaluate(t / time)));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.rotation = EndRotationL;
        m_RRobotRigidbody.rotation = EndRotationR;

        yield return new WaitForSeconds(0.3f);

        List<EnemyBase> enemys = new List<EnemyBase>();
        List<Separation> arms = new List<Separation>();
		Collider[] collider = Physics.OverlapBox(CenterPosition, new Vector3(m_TwinRobotL.BreakerSize, 2, Distance * 2), EndRotationL, LayerMask.GetMask(new string[] { "Enemy","BossArm" }));
        foreach (var item in collider)
        {
			if (item.gameObject.layer == LayerMask.NameToLayer("Enemy")){
				EnemyBase enemy = item.GetComponent<EnemyBase>();
				if (enemy == null || enemys.Contains(enemy)) continue;
				enemys.Add(enemy);
			}
			else
			{
				Separation arm = item.GetComponent<Separation>();
				if (arm == null || arms.Contains(arm)) continue;
				arms.Add(arm);
			}
		}

        Pauser.Pause(PauseTag.Enemy, false);
        IsCanCrash = true;
        m_LRobotRigidbody.isKinematic = true;
        m_RRobotRigidbody.isKinematic = true;

        for (float t = 0; IsCanCrash && Vector3.MoveTowards(StartPositionL, EndPositionL, m_CombineDistance.Evaluate(t) * m_Speed * 2) != EndPositionL; t += Time.fixedDeltaTime)
        {
            m_LRobotRigidbody.MovePosition(Vector3.MoveTowards(StartPositionL, EndPositionL, m_CombineDistance.Evaluate(t) * m_Speed * 2));
            m_RRobotRigidbody.MovePosition(Vector3.MoveTowards(StartPositionR, EndPositionR, m_CombineDistance.Evaluate(t) * m_Speed * 2));
			TPSPositionSet(Vector3.Distance(CenterPosition, m_LRobotRigidbody.position));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.isKinematic = false;
        m_RRobotRigidbody.isKinematic = false;
		TPSPositionSet(0);

		arms.ForEach(x => x.Death());

        if (!IsCanCrash || enemys.Count == 0)
        {
            m_KeepEnemyPosWall.SetActive(false);
            Pauser.Resume(PauseTag.Enemy, false);
            yield return StartCoroutine(Release(true));
            yield break;
        }

		StartCoroutine(CombineEffect());

		enemys.ForEach(x => x.SetBreakForPlayer());

		AddEnergy(enemys.Count);

		ModeSet(enemys);

		m_KeepEnemyPosWall.SetActive(false);
        m_HRobot.SetActive(true);
        m_LRobot.SetActive(false);
        m_RRobot.SetActive(false);
        m_Electric.SetActive(false);
        GameManager.Instance.m_PlayMode = PlayMode.HumanoidRobot;

		m_HumanoidRobot.m_Animator.SetTrigger("Combined");


		Pauser.Resume(PauseTag.Enemy, false);
		m_Coroutine = null;
	}
    public IEnumerator Release(bool isCombine)
    {
        m_HumanoidRobot.m_Energy = 0;
        GameManager.Instance.m_PlayMode = PlayMode.Release;
        m_HumanoidMaterial.color = m_ModeAA;
        m_HRobot.SetActive(false);
        m_LRobot.SetActive(true);
        m_RRobot.SetActive(true);
        m_Electric.SetActive(true);
        Vector3 move_ =
            m_HRobot.transform.right * Input.GetAxis("HorizontalL") +
            m_HRobot.transform.forward * Input.GetAxis("VerticalL");
        Vector3 vector = move_.magnitude == 0 || isCombine ? m_HRobot.transform.right : -move_.normalized;
        vector *= vector.x > vector.z ? 1 : -1;
        if (isCombine == false)
        {
            m_LRobotRigidbody.position = m_HRobot.transform.position - (vector * 0.1f);
            m_RRobotRigidbody.position = m_HRobot.transform.position + (vector * 0.1f);
        }

        Quaternion lRotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
        Quaternion rRotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
        float preMove = 0;
        float move;
        float t;
        float l = 10f;
        float radius = 2 * 1.6f;
        RaycastHit hit;
        int layermask = LayerMask.GetMask(new string[] { "Wall" });
        yield return null;
        for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
        {
            t = m_ReleaseCurve.Evaluate(f / m_CombineTime);
            move = Mathf.Lerp(0.5f, l, t) - preMove;
            if (!Physics.CheckSphere(m_LRobotRigidbody.position, radius, layermask))
            {
                if (Physics.SphereCast(m_LRobotRigidbody.position, radius, -vector, out hit, move, layermask))
                {
                    m_LRobotRigidbody.MovePosition(m_LRobotRigidbody.position - vector * hit.distance);
                }
                else
                {
                    m_LRobotRigidbody.MovePosition(m_LRobotRigidbody.position - vector * move);
                }
            }
            if (!Physics.CheckSphere(m_RRobotRigidbody.position, radius, layermask))
            {
                if (Physics.SphereCast(m_RRobotRigidbody.position, radius, vector, out hit, move, layermask))
                {
                    m_RRobotRigidbody.MovePosition(m_RRobotRigidbody.position + vector * hit.distance);
                }
                else
                {
                    m_RRobotRigidbody.MovePosition(m_RRobotRigidbody.position + vector * move);
                }
            }
            preMove += move;
            m_LRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(lRotation, rRotation, t * 4));
            m_RRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(rRotation, lRotation, t * 4));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.position -= vector * (l - preMove);
        m_RRobotRigidbody.position += vector * (l - preMove);
        m_TwinRobotL.SetShieldActive(true);
        m_TwinRobotR.SetShieldActive(true);
        GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
		m_Coroutine = null;
	}

	private IEnumerator CombineEffect()
    {
        m_CombineEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        m_CombineEffect.SetActive(false);
    }
    public float Energy { get { return m_HumanoidRobot.m_Energy; } }
    public bool IsCanCrash { get; set; }

	private void ModeSet(List<EnemyBase> enemys)
	{
		bool breakTypeS = false;
		bool breakTypeL = false;
		foreach (var item in enemys)
		{
			if (item.m_EnemyType == EnemyType.Short) breakTypeS = true;
			if (item.m_EnemyType == EnemyType.Long) breakTypeL = true;
		}
		if (breakTypeS && breakTypeL)
		{
			m_HumanoidMaterial.color = m_ModeAB;
			m_HumanoidRobot.m_Config = m_HumanoidT;
			m_HMode = HumanoidMode.Speed;
		}
		else if (breakTypeS)
		{
			m_HumanoidMaterial.color = m_ModeAA;
			m_HumanoidRobot.m_Config = m_HumanoidN;
			m_HMode = HumanoidMode.Normal;
		}
		else
		{
			m_HumanoidMaterial.color = m_ModeBB;
			m_HumanoidRobot.m_Config = m_HumanoidI;
			m_HMode = HumanoidMode.Power;
		}
	}

	private void AddEnergy(int enemy)
	{
		float add = GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[enemy - 1];
		m_TwinRobotL.HP += add;
		m_TwinRobotR.HP += add;
		m_HumanoidRobot.m_Energy = add;
	}

	private void TPSPositionSet(float distance)
	{
		Vector3 v = m_TPSPosition.position;
		v.y = 5.5f + 1.5f * distance;
		m_TPSPosition.position = v;
	}

	private void OnDisable()
	{
		if (m_Coroutine == null) return;
		StopCoroutine(m_Coroutine);
	}

	private void OnEnable()
	{
		if (m_Coroutine == null) return;
		StartCoroutine(m_Coroutine);
	}
}
