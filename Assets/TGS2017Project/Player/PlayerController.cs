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

	public GameObject m_Trap;
	public GameObject m_TrapGuid;

	public float m_Energy;

	public GameObject m_ElectricGuid;

    // Use this for initialization
    void Start()
    {
        m_LRobotRigidbody = m_LRobot.GetComponent<Rigidbody>();
        m_RRobotRigidbody = m_RRobot.GetComponent<Rigidbody>();
        m_CharacterController = m_HumanoidRobot.GetComponent<CharacterController>();
        m_HumanoidRobot.SetActive(false);
		m_TrapGuid.SetActive(false);
		m_ElectricGuid.SetActive(false);
		GameManager.Instance.m_CombineTime = m_CombineTime;
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
                if (Input.GetButtonDown("Boost"))
                {
                    StartCoroutine(Boost());
                }
				if (Input.GetButtonDown("Combine"))
				{
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
                move = Vector3.ClampMagnitude(move, 1.0f) * m_MoveSpeed * Time.fixedDeltaTime;
                m_LRobotRigidbody.position += move;
                move = new Vector3(
                    Input.GetAxis("HorizontalR"), 0,
                    Input.GetAxis("VerticalR"));
                move = Vector3.ClampMagnitude(move, 1.0f) * m_MoveSpeed * Time.fixedDeltaTime;
                m_RRobotRigidbody.position += move;

                m_Electric.transform.position = Vector3.Lerp(m_LRobotRigidbody.position, m_RRobotRigidbody.position, 0.5f);
                m_Electric.transform.localScale = new Vector3(1, 1, Vector3.Distance(m_LRobotRigidbody.position,m_RRobotRigidbody.position));
                m_Electric.transform.LookAt(m_LRobot.transform);

                m_LRobotRigidbody.rotation = Quaternion.LookRotation(m_RRobotRigidbody.position - m_LRobotRigidbody.position);
                m_RRobotRigidbody.rotation = Quaternion.LookRotation(m_LRobotRigidbody.position - m_RRobotRigidbody.position);
                break;
            case PlayMode.HumanoidRobot:
                Vector3 move_ = new Vector3(
                    Input.GetAxis("HorizontalL"), 0,
                    Input.GetAxis("VerticalL"));
                move_ = 
                    m_HumanoidRobot.transform.right * move_.x +
                    m_HumanoidRobot.transform.forward * move_.z;

                m_CharacterController.Move(move_ * (m_Speed + m_BoostSpeed) * Time.fixedDeltaTime);
                m_HumanoidRobot.transform.Rotate(0, Input.GetAxis("HorizontalR") * m_RotateSpeed * Time.fixedDeltaTime, 0);
                break;
            case PlayMode.Combine:
				m_Electric.transform.position = Vector3.Lerp(m_LRobotRigidbody.position, m_RRobotRigidbody.position, 0.5f);
				m_Electric.transform.localScale = new Vector3(1, 1, Vector3.Distance(m_LRobotRigidbody.position, m_RRobotRigidbody.position));
				m_Electric.transform.LookAt(m_LRobot.transform);
				break;
            case PlayMode.Release:
				m_Electric.transform.position = Vector3.Lerp(m_LRobotRigidbody.position, m_RRobotRigidbody.position, 0.5f);
				m_Electric.transform.localScale = new Vector3(1, 1, Vector3.Distance(m_LRobotRigidbody.position, m_RRobotRigidbody.position));
				m_Electric.transform.LookAt(m_LRobot.transform);
				break;
            default:
                break;
        }
    }

    public IEnumerator Combine()
    {
        GameManager.Instance.m_PlayMode = PlayMode.Combine;
		m_ElectricGuid.SetActive(true);
        Vector3 LPos = m_LRobotRigidbody.position;
        Vector3 RPos = m_RRobotRigidbody.position;
        Vector3 HitPos = Vector3.Lerp(LPos, RPos, 0.5f);
        Vector3 LFirstTarget = HitPos + Vector3.Normalize(LPos - HitPos) * 1.5f;
        Vector3 RFirstTarget = HitPos + Vector3.Normalize(RPos - HitPos) * 1.5f;
		Vector3 LSecondTarget = HitPos + Vector3.Normalize(LPos - HitPos) * 0.5f;
		Vector3 RSecondTarget = HitPos + Vector3.Normalize(RPos - HitPos) * 0.5f;
		m_HumanoidRobot.transform.position = m_Electric.transform.position;
		m_HumanoidRobot.transform.LookAt(m_Electric.transform.position + m_Electric.transform.right);

		for (float f = 0; f < m_CombineTime; f += Time.fixedDeltaTime)
		{
			m_LRobotRigidbody.MovePosition(Vector3.Lerp(LPos, LFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			m_RRobotRigidbody.MovePosition(Vector3.Lerp(RPos, RFirstTarget, m_CombineCurve.Evaluate(f / m_CombineTime)));
			yield return new WaitForFixedUpdate();
		}
		LPos = HitPos + Vector3.Normalize(LPos - HitPos) * 8.0f;
		RPos = HitPos + Vector3.Normalize(RPos - HitPos) * 8.0f;
		for (float f = 0; f < m_CombineTime/2; f += Time.fixedDeltaTime)
		{
			m_LRobotRigidbody.MovePosition(Vector3.Lerp(LPos, Vector3.Lerp(LFirstTarget, LSecondTarget, f), m_CombineCurve.Evaluate(f / m_CombineTime +1)));
			m_RRobotRigidbody.MovePosition(Vector3.Lerp(RPos, Vector3.Lerp(RFirstTarget, RSecondTarget, f), m_CombineCurve.Evaluate(f / m_CombineTime +1)));
			yield return new WaitForFixedUpdate();
		}

		bool breakable = true;
		int count = 0;
		Collider[] collider =  Physics.OverlapBox(HitPos, new Vector3(8, 1f, 1.5f), m_HumanoidRobot.transform.rotation,LayerMask.GetMask(new string[]{ "Enemy"}));
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

		for (float f = m_CombineTime / 2; f < m_CombineTime; f += Time.fixedDeltaTime)
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
        m_LRobotRigidbody.position = m_HumanoidRobot.transform.position - (vector * 0.5f);
        m_RRobotRigidbody.position = m_HumanoidRobot.transform.position + (vector * 0.5f);
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
            m_LRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(lRotation,rRotation, t * 4));
            m_RRobotRigidbody.MoveRotation(Quaternion.SlerpUnclamped(rRotation,lRotation, t * 4));
            yield return new WaitForFixedUpdate();
        }
        m_LRobotRigidbody.position -= vector * (5 - preMove);
        m_RRobotRigidbody.position += vector * (5 - preMove);

        GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
    }

    public IEnumerator Boost()
    {
        m_BoostSpeed = m_BoostPower;
        yield return new WaitForSeconds(m_BoostTime);
        m_BoostSpeed = 0;
    }

	public IEnumerator Charge()
	{
		Vector3 position = m_HumanoidRobot.transform.position;
		m_TrapGuid.SetActive(true);
		float time = 0.1f;
		while (true)
		{
			time += Time.deltaTime;
			position = m_HumanoidRobot.transform.position + m_HumanoidRobot.transform.forward * time * 10;
			m_TrapGuid.transform.position = position;
			m_TrapGuid.transform.rotation = m_HumanoidRobot.transform.rotation;
			m_TrapGuid.transform.localScale = new Vector3(time + 1, 1, 1);
			if (Input.GetButtonUp("Charge")) break;
			yield return null;
		}
		m_TrapGuid.SetActive(false);
		GameObject trap =  Instantiate(m_Trap, m_HumanoidRobot.transform.position, m_HumanoidRobot.transform.rotation);
		trap.SetActive(true);
		trap.GetComponent<Trap>().SetTrap(position, time + 1);
		Destroy(trap, time * 2);
	}
}
