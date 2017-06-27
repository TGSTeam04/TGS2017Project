using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour {

    Transform m_Target;
    public float m_MoveSpeed = 5.0f;
    public float m_ActivityTime = 7.0f;
    [SerializeField]
    private float m_ApplyDamage = 2.0f;

    public bool m_IsLeft;
    public bool m_IsRight;
    public GameObject m_Arm;

    bool m_Back = false;
    bool m_IsPlay = false;

    public GameObject m_Explosion;

    // Use this for initialization
    void Start () {
        transform.position = m_Arm.transform.position;
        transform.rotation = m_Arm.transform.rotation;
        if (m_IsPlay == false)
        {
            StartCoroutine(GoHome());
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (m_IsPlay == false)
        {
            StartCoroutine(GoHome());
        }
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                if (!m_Back)
                {
                    GameObject L = GameManager.Instance.m_LRobot;
                    GameObject R = GameManager.Instance.m_RRobot;
                    float LDistance = Vector3.Distance(transform.position, L.transform.position);
                    float RDistance = Vector3.Distance(transform.position, R.transform.position);
                    if (LDistance <= RDistance)
                    {
                        m_Target = L.transform;
                    }
                    else
                    {
                        m_Target = R.transform;
                    }
                }
                break;
            case PlayMode.HumanoidRobot:
                if (!m_Back) m_Target = GameManager.Instance.m_HumanoidRobot.transform;
                break;
            case PlayMode.NoPlay:
            case PlayMode.Combine:
                break;
            case PlayMode.Release:
            default:
                return;
        }
        if (m_Back)
        {
            m_Target = m_Arm.transform;
        }
        Vector3 m_TargetPosition = m_Target.position;
        m_TargetPosition.y = transform.position.y;
        Vector3 direction = m_TargetPosition - transform.position;
        transform.position += direction.normalized * m_MoveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
        if (m_Back && Vector3.Distance(transform.position, m_TargetPosition) < 0.1f)
        {
            m_Arm.SetActive(true);
            transform.position = m_Arm.transform.position;
            transform.rotation = m_Arm.transform.rotation;
            AttackProcess.s_Chance = false;
            m_Back = false;
            m_IsPlay = false;
            gameObject.SetActive(false);
        }
    }
    IEnumerator GoHome()
    {
        m_IsPlay = true;
        yield return new WaitForSeconds(m_ActivityTime);
        m_Back = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && m_Back == false && GameManager.Instance.m_PlayMode != PlayMode.Combine)
        {
            var damageComp = other.gameObject.GetComponent<Damageable>();
            if (damageComp != null)
                damageComp.ApplyDamage(m_ApplyDamage, this);
            m_Back = true;
        }
        if (other.gameObject.tag == "Player" && GameManager.Instance.m_PlayMode == PlayMode.Combine)
        {
            Boss.HitPoint -= 20.0f;
            Instantiate(m_Explosion, transform.position, transform.rotation);
            AttackProcess.s_Chance = false;
            gameObject.SetActive(false);
        }
    }
}
