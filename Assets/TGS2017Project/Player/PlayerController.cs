using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwinRoboMode
{
    A,
    B
}

public class PlayerController : MonoBehaviour
{
	private Dictionary<TwinRoboMode, Quaternion> m_RotateTwinRoboMode = new Dictionary<TwinRoboMode, Quaternion>();
	private TwinRoboMode m_LMode;
	private TwinRoboMode m_RMode;
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

	public GameObject m_ElectricGuid;

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
	private HumanoidConfig m_HumanoidT;
	[SerializeField]
	private HumanoidConfig m_HumanoidI;

	[SerializeField]
	private Material m_HumanoidMaterial;

	private float m_AxisL;
	private float m_AxisR;

	private void Awake()
	{
		m_Subject = new SubjectBase();
		GameManager.Instance.m_IsGameClear = false;
		GameManager.Instance.m_IsGameOver = false;
		GameManager.Instance.m_PlayScore = 0;
		GameManager.Instance.m_PlayTime = 0;
		//GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
	}


	// Use this for initialization
	void Start()
	{
		m_LRobotRigidbody = m_LRobot.GetComponent<Rigidbody>();
		m_RRobotRigidbody = m_RRobot.GetComponent<Rigidbody>();
		m_HumanoidRobotRigidbody = m_HRobot.GetComponent<Rigidbody>();
		m_HRobot.SetActive(false);
		m_Battery = m_HRobot.GetComponent<RocketBattery>();
		m_ElectricGuid.SetActive(false);

		m_Level = 1;
		m_Exp = 0;
		GameManager.Instance.m_CombineTime = m_CombineTime;
		GameManager.Instance.m_PlayerController = this;
		//		GameManager.Instance.m_StageManger.m_Observer.BindSubject(m_Subject);
		m_RotateTwinRoboMode.Add(TwinRoboMode.A, Quaternion.Euler(0, 0, 0));
		m_RotateTwinRoboMode.Add(TwinRoboMode.B, Quaternion.Euler(0, 90, 0));
		m_RMode = TwinRoboMode.A;
		m_LMode = TwinRoboMode.A;

		m_HumanoidMaterial.color = m_ModeAA;

	}

	// Update is called once per frame
	void LateUpdate()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				if (Input.GetButton("CombineL")&& Input.GetButton("CombineR"))
				{
					StartCoroutine(Combine());
				}
				if (Input.GetAxis("RotateL")>0.5f&&m_AxisL<=0.5f)
				{
					m_LMode = m_LMode == TwinRoboMode.A ? TwinRoboMode.B : TwinRoboMode.A;
				}
				m_AxisL = Input.GetAxis("RotateL");
				if (Input.GetAxis("RotateR")>0.5f && m_AxisR <= 0.5f)
				{
					m_RMode = m_RMode == TwinRoboMode.A ? TwinRoboMode.B : TwinRoboMode.A;
				}
				m_AxisR = Input.GetAxis("RotateR");
				break;
			case PlayMode.HumanoidRobot:
				if (m_Battery.LIsCanFire && m_Battery.RIsCanFire && ((Input.GetButton("CombineL") && Input.GetButton("CombineR")) || m_HumanoidRobot.m_Energy <= 0))
				{
					m_HumanoidRobot.m_Energy = 0;
					StartCoroutine(Release());
				}
				else
				{
					m_HumanoidRobot.UpdateInput();
				}
				break;
			case PlayMode.Combine:
			case PlayMode.Release:
			default:
				break;
		}
	}

	void FixedUpdate()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				m_TwinRobotL.Move();
				m_TwinRobotR.Move();

				ElectricUpdate();

				m_TwinRobotL.Look(m_RRobotRigidbody.position);
				m_TwinRobotR.Look(m_LRobotRigidbody.position);

				m_LRobotRigidbody.rotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position) * m_RotateTwinRoboMode[m_LMode];
				m_RRobotRigidbody.rotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position) * m_RotateTwinRoboMode[m_RMode];
				break;
			case PlayMode.HumanoidRobot:
				m_HumanoidRobot.Move();
				break;
			case PlayMode.Combine:
			case PlayMode.Release:
				ElectricUpdate();
				break;
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
		m_TwinRobotL.Active(false);
		m_TwinRobotR.Active(false);
		m_ElectricGuid.SetActive(true);
		Vector3 LPos = m_LRobotRigidbody.position;
		Vector3 RPos = m_RRobotRigidbody.position;
		Vector3 HitPos = Vector3.Lerp(LPos, RPos, 0.5f);
		Vector3 LFirstTarget = HitPos + Vector3.Normalize(LPos - HitPos) * (1.0f + 0.5f);
		Vector3 RFirstTarget = HitPos + Vector3.Normalize(RPos - HitPos) * (1.0f + 0.5f);
		Vector3 LSecondTarget = HitPos + Vector3.Normalize(LPos - HitPos) * 0.5f;
		Vector3 RSecondTarget = HitPos + Vector3.Normalize(RPos - HitPos) * 0.5f;
		m_HRobot.transform.position = m_Electric.transform.position;
		m_HRobot.transform.LookAt(m_Electric.transform.position + m_Electric.transform.right);

		m_CombineEffect.transform.position = m_Electric.transform.position + new Vector3(0, 0.5f, 0);
		StartCoroutine(CombineEffect());

		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			m_LRobotRigidbody.MovePosition(Vector3.Lerp(LPos, LFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			m_RRobotRigidbody.MovePosition(Vector3.Lerp(RPos, RFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			yield return new WaitForFixedUpdate();
		}
		LPos = HitPos + Vector3.Normalize(LPos - HitPos) * 8.0f;
		RPos = HitPos + Vector3.Normalize(RPos - HitPos) * 8.0f;

		bool breakable = true;
		int count = 0;
		Collider[] collider = Physics.OverlapBox(HitPos + Vector3.up / 2, new Vector3(1f, 0.5f, (m_LMode == m_RMode) && m_RMode == TwinRoboMode.A ? 1.5f : 0.5f), m_HRobot.transform.rotation, LayerMask.GetMask(new string[] { "Enemy" }));
		foreach (var item in collider)
		{
			EnemyBase enemy = item.GetComponent<EnemyBase>();
			if (enemy == null) continue;
			count++;
			if (!enemy.IsBreakable)
			{
				breakable = false;
			}
		}
		if (breakable)
		{
			foreach (var item in collider)
			{
				EnemyBase enemy = item.GetComponent<EnemyBase>();
				if (enemy == null)
					continue;
				enemy.SetBreak();
			}
		}

		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			m_LRobotRigidbody.MovePosition(Vector3.Lerp(LPos, Vector3.Lerp(LFirstTarget, LSecondTarget, f), m_CombineCurve.Evaluate(f / m_CombineTime + 1)));
			m_RRobotRigidbody.MovePosition(Vector3.Lerp(RPos, Vector3.Lerp(RFirstTarget, RSecondTarget, f), m_CombineCurve.Evaluate(f / m_CombineTime + 1)));
			yield return new WaitForFixedUpdate();
		}
		if (!breakable || count == 0)
		{
			m_ElectricGuid.SetActive(false);
			GameManager.Instance.m_PlayMode = PlayMode.Release;
			yield return StartCoroutine(Release());
			yield break;
		}

		GameManager.Instance.m_PlayScore += count;
		m_TwinRobotL.Damage(-0.2f * count);
		m_TwinRobotR.Damage(-0.2f * count);

		m_HumanoidRobot.m_Energy = GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[count - 1];
		int nextexp = GameManager.Instance.LevelParameter.m_NextExp;
		m_Exp += count;
		GameManager.Instance.m_Level += m_Exp / nextexp;
		m_Exp = m_Exp % nextexp;

		m_HumanoidMaterial.color = m_LMode != m_RMode ? m_ModeAB :
																m_RMode == TwinRoboMode.A ? m_ModeAA : m_ModeBB;
		m_HumanoidRobot.m_Config = m_LMode != m_RMode ? m_HumanoidT :
																m_RMode == TwinRoboMode.A ? m_HumanoidN : m_HumanoidI;

		m_LRobotRigidbody.MovePosition(LSecondTarget);
		m_RRobotRigidbody.MovePosition(RSecondTarget);
		//yield return null;
		m_ElectricGuid.SetActive(false);
		m_HRobot.SetActive(true);
		m_LRobot.SetActive(false);
		m_RRobot.SetActive(false);
		m_Electric.SetActive(false);
		GameManager.Instance.m_PlayMode = PlayMode.HumanoidRobot;
	}
	public IEnumerator Release()
	{
		GameManager.Instance.m_PlayMode = PlayMode.Release;
		m_HumanoidMaterial.color = m_ModeAA;
		m_HRobot.SetActive(false);
		m_LRobot.SetActive(true);
		m_RRobot.SetActive(true);
		m_Electric.SetActive(true);
		Vector3 move_ =
			m_HRobot.transform.right * Input.GetAxis("HorizontalL") +
			m_HRobot.transform.forward * Input.GetAxis("VerticalL");
		Vector3 vector = move_.magnitude == 0 ? m_HRobot.transform.right : -move_.normalized;
		m_LRobotRigidbody.position = m_HRobot.transform.position - (vector * 0.1f);
		m_RRobotRigidbody.position = m_HRobot.transform.position + (vector * 0.1f);
		Quaternion lRotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
		Quaternion rRotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
		float preMove = 0;
		float move;
		float t;
		float l = 10f;

		yield return null;
		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			t = m_ReleaseCurve.Evaluate(f / m_CombineTime);
			move = Mathf.Lerp(0.5f, l, t);
			m_LRobotRigidbody.MovePosition(m_LRobotRigidbody.position - vector * (move - preMove));
			m_RRobotRigidbody.MovePosition(m_RRobotRigidbody.position + vector * (move - preMove));
			preMove = move;
			m_LRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(lRotation, rRotation, t * 4));
			m_RRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(rRotation, lRotation, t * 4));
			yield return new WaitForFixedUpdate();
		}
		m_LRobotRigidbody.position -= vector * (l - preMove);
		m_RRobotRigidbody.position += vector * (l - preMove);
		m_TwinRobotL.Active(true);
		m_TwinRobotR.Active(true);
		GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
	}


	//public IEnumerator Charge(bool isLeft)
	//{
	//    m_IsBeamShooting = true;
	//    m_Energy -= 3.0f;//6.25f;
	//    m_ChargeEffect.SetActive(true);
	//    yield return new WaitForSeconds(1.5f);
	//    m_ChargeEffect.SetActive(false);
	//    m_Battery.Fire();
	//    m_IsBeamShooting = false;
	//}

	private IEnumerator CombineEffect()
	{
		yield return new WaitForSeconds(2f);
		m_CombineEffect.SetActive(true);
		yield return new WaitForSeconds(1);
		m_CombineEffect.SetActive(false);
	}
	public float Energy{get{ return m_HumanoidRobot.m_Energy; }}
}
