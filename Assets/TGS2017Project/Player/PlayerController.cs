using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public GameObject m_HumanoidRobot;
	public GameObject m_LRobot;
	public GameObject m_RRobot;
	public GameObject m_Electric;

	public float m_MoveSpeed;

	private Rigidbody m_LRobotRigidbody;
	private Rigidbody m_RRobotRigidbody;

	public float m_CombineTime;
	public AnimationCurve m_CombineCurve;

	public float m_Speed;
	public float m_RotateSpeed;
	private CharacterController m_CharacterController;

	public float m_BoostTime;
	public float m_BoostPower;
	private float m_BoostSpeed = 0f;

	public float m_Energy;

	public GameObject m_ElectricGuid;

	public int m_Level;
	public int m_Exp;
	public float m_Scale;

	public GameObject m_Beam;

	public Transform m_TPSPosition;
	public Transform m_ChargePosition;

	private bool m_IsBeamShooting;

	public float m_MoveY;

	public Shield m_LShield;
	public Shield m_RShield;

	private SubjectBase m_Subject;

	private void Awake()
	{
		m_Subject = new SubjectBase();
	}

	// Use this for initialization
	void Start()
	{
		m_LRobotRigidbody = m_LRobot.GetComponent<Rigidbody>();
		m_RRobotRigidbody = m_RRobot.GetComponent<Rigidbody>();
		m_CharacterController = m_HumanoidRobot.GetComponent<CharacterController>();
		m_HumanoidRobot.SetActive(false);
		m_ElectricGuid.SetActive(false);
		m_Beam.SetActive(false);
		m_Level = 1;
		m_Exp = 0;
		m_Scale = 1;
		m_Energy = 0;
		GameManager.Instance.m_CombineTime = m_CombineTime;
		GameManager.Instance.m_PlayerController = this;
		GameManager.Instance.m_StageManger.m_Observer.BindSubject(m_Subject);

	}

	// Update is called once per frame
	void LateUpdate()
	{
		switch (GameManager.Instance.m_PlayMode)
		{
			case PlayMode.NoPlay:
				break;
			case PlayMode.TwinRobot:
				if (Input.GetButtonDown("Combine"))
				{
					StartCoroutine(Combine());
				}
				break;
			case PlayMode.HumanoidRobot:
				m_Energy -= Time.deltaTime;
				if (Input.GetButtonDown("Jump")&&m_CharacterController.isGrounded)
				{
					m_MoveY = 1.5f;
				}
				Boost(Input.GetButton("Boost"));
				if (Input.GetButtonDown("Combine")||m_Energy<=0)
				{
					m_Energy = 0;
					StartCoroutine(Release());
				}
				if (Input.GetButtonDown("Charge"))
				{
					StartCoroutine(Charge());
				}
				break;
			case PlayMode.Combine:
				break;
			case PlayMode.Release:
				break;
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
				Vector3 move = new Vector3(
					Input.GetAxis("HorizontalL"), 0,
					Input.GetAxis("VerticalL"));
				move = Vector3.ClampMagnitude(move, 1.0f) * m_MoveSpeed * GameManager.Instance.LevelParameter.m_Speed * Time.fixedDeltaTime;
				m_LRobotRigidbody.position += move;
				move = new Vector3(
					Input.GetAxis("HorizontalR"), 0,
					Input.GetAxis("VerticalR"));
				move = Vector3.ClampMagnitude(move, 1.0f) * m_MoveSpeed * GameManager.Instance.LevelParameter.m_Speed * Time.fixedDeltaTime;
				m_RRobotRigidbody.position += move;

				ElectricUpdate();

				m_LRobotRigidbody.rotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
				m_RRobotRigidbody.rotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
				break;
			case PlayMode.HumanoidRobot:
				Vector3 move_ = new Vector3(
					Input.GetAxis("HorizontalL"), 0,
					Input.GetAxis("VerticalL"));

				if (m_MoveY<0&& m_CharacterController.isGrounded)
				{
					m_MoveY = 0;
				}
				else
				{
					m_MoveY -= 3 * Time.fixedDeltaTime;

				}
				move_ =
					m_HumanoidRobot.transform.right * move_.x +
					m_HumanoidRobot.transform.forward * move_.z +
					m_HumanoidRobot.transform.up * m_MoveY;

				float rotateSpeed = 1;
				if (!m_IsBeamShooting)
				{
					m_CharacterController.Move(move_ * (m_Speed + m_BoostSpeed) * GameManager.Instance.LevelParameter.m_Speed * Time.fixedDeltaTime);
				}
				else
				{
					rotateSpeed = 0.2f;
				}
				m_HumanoidRobot.transform.Rotate(0, Input.GetAxis("HorizontalR") * m_RotateSpeed*rotateSpeed * Time.fixedDeltaTime, 0);
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
		m_Electric.transform.localScale = new Vector3(m_Scale, 1, Vector3.Distance(m_LRobotRigidbody.position, m_RRobotRigidbody.position));
		m_Electric.transform.LookAt(m_LRobot.transform);
	}

	public IEnumerator Combine()
	{
		GameManager.Instance.m_PlayMode = PlayMode.Combine;
		m_LShield.Active(false);
		m_RShield.Active(false);
		m_ElectricGuid.SetActive(true);
		Vector3 LPos = m_LRobotRigidbody.position;
		Vector3 RPos = m_RRobotRigidbody.position;
		Vector3 HitPos = Vector3.Lerp(LPos, RPos, 0.5f);
		float scale = (GameManager.Instance.m_StageManger.m_StageLevel == 0 ? 1 :
			GameManager.Instance.m_StageManger.m_StageLevel == 1 ? 4 : 15);
		Vector3 LFirstTarget = HitPos + Vector3.Normalize(LPos - HitPos) * (1.0f * scale + 0.5f * m_Scale);
		Vector3 RFirstTarget = HitPos + Vector3.Normalize(RPos - HitPos) * (1.0f * scale + 0.5f * m_Scale);
		Vector3 LSecondTarget = HitPos + Vector3.Normalize(LPos - HitPos) * 0.5f * m_Scale;
		Vector3 RSecondTarget = HitPos + Vector3.Normalize(RPos - HitPos) * 0.5f * m_Scale;
		m_HumanoidRobot.transform.position = m_Electric.transform.position;
		m_HumanoidRobot.transform.LookAt(m_Electric.transform.position + m_Electric.transform.right);

		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			m_LRobotRigidbody.MovePosition(Vector3.Lerp(LPos, LFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			m_RRobotRigidbody.MovePosition(Vector3.Lerp(RPos, RFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			yield return new WaitForFixedUpdate();
		}
		LPos = HitPos + Vector3.Normalize(LPos - HitPos) * 8.0f * m_Scale;
		RPos = HitPos + Vector3.Normalize(RPos - HitPos) * 8.0f * m_Scale;

		bool breakable = true;
		int count = 0;
		Collider[] collider = Physics.OverlapBox(HitPos, new Vector3(1f, 1f, 1.5f * m_Scale), m_HumanoidRobot.transform.rotation, LayerMask.GetMask(new string[] { "Enemy" }));
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
				if (enemy == null) continue;
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

		GameManager.Instance.m_StageManger.m_KillNum += count;
		m_LShield.Damage(-0.2f*count);
		m_RShield.Damage(-0.2f*count);

		m_Energy += count * 5 + 10;
		int nextexp = GameManager.Instance.LevelParameter.m_NextExp;
		m_Exp += count;
		GameManager.Instance.m_Level += m_Exp / nextexp;
		m_Exp = m_Exp % nextexp;
		m_Scale = GameManager.Instance.LevelParameter.m_Scale = GameManager.Instance.m_LevelParameter.m_Speed =m_Scale * (1.0f+count*0.1f);
		m_LRobot.transform.localScale = Vector3.one * m_Scale;
		m_RRobot.transform.localScale = Vector3.one * m_Scale;
		m_HumanoidRobot.transform.localScale = Vector3.one * m_Scale;

		m_LRobotRigidbody.MovePosition(LSecondTarget);
		m_RRobotRigidbody.MovePosition(RSecondTarget);
		yield return null;
		m_ElectricGuid.SetActive(false);
		m_HumanoidRobot.SetActive(true);
		m_LRobot.SetActive(false);
		m_RRobot.SetActive(false);
		m_Electric.SetActive(false);
		GameManager.Instance.m_PlayMode = PlayMode.HumanoidRobot;
	}
	public IEnumerator Release()
	{
		GameManager.Instance.m_PlayMode = PlayMode.Release;
		m_HumanoidRobot.SetActive(false);
		m_LRobot.SetActive(true);
		m_RRobot.SetActive(true);
		m_Electric.SetActive(true);
		Vector3 vector = m_HumanoidRobot.transform.right;
		m_LRobotRigidbody.position = m_HumanoidRobot.transform.position - (vector * 0.5f * m_Scale);
		m_RRobotRigidbody.position = m_HumanoidRobot.transform.position + (vector * 0.5f * m_Scale);
		Quaternion lRotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
		Quaternion rRotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
		float preMove = 0;
		float move;
		float t;

		yield return null;
		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			t = 1 - m_CombineCurve.Evaluate(1 - f / m_CombineTime);
			move = Mathf.Lerp(0.5f, 5, t);
			m_LRobotRigidbody.position -= vector * (move - preMove);
			m_RRobotRigidbody.position += vector * (move - preMove);
			preMove = move;
			m_LRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(lRotation, rRotation, t * 4));
			m_RRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(rRotation, lRotation, t * 4));
			yield return new WaitForFixedUpdate();
		}
		m_LRobotRigidbody.position -= vector * (5 - preMove);
		m_RRobotRigidbody.position += vector * (5 - preMove);
		m_LShield.Active(true);
		m_RShield.Active(true);
		GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
	}

	public void Boost(bool boost)
	{
		m_BoostSpeed = boost ? m_BoostPower : 0;
	}

	public IEnumerator Charge()
	{
		if (m_IsBeamShooting){ yield break; }
		m_IsBeamShooting = true;
		Vector3 position = m_TPSPosition.localPosition;
		m_TPSPosition.localPosition = m_ChargePosition.localPosition;
		float time = 0.1f;
		while (true)
		{
			time += Time.deltaTime;
			if (Input.GetButtonUp("Charge")) break;
			yield return null;
		}
		m_Beam.transform.localScale = new Vector3(1, 1, 1 + time * 2);
		m_Beam.SetActive(true);
		yield return new WaitForSeconds(time / 2f);
		m_TPSPosition.localPosition = position;
		m_Beam.SetActive(false);
		m_IsBeamShooting = false;
	}

}
