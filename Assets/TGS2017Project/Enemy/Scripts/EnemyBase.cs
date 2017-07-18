using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public delegate void OnCollideEnter_Del(Collider other, EnemyBase enemy);

public enum BreakType
{
    Normal,
    Shock,
    UnBreak
}
public enum EnemyType
{
	Short,
	Long,
	Normal
}

public class EnemyBase : MonoBehaviour
{
    public float m_MoveSpeed;
    private float m_SpeedRate;
    public bool m_IsDead;
    public bool m_IsShock;
    public bool m_FreezeVelocity = true;
    private Rigidbody m_Rigidbody;

    public GameObject m_LRobot;
    public GameObject m_RRobot;
    public GameObject m_HumanoidRobot;

    private Vector3 m_LRobotPos;
    private Vector3 m_RRobotPos;

    private Vector3 m_Target;

    public GameObject m_Fragment;

    private NavMeshAgent m_NavMeshAgent;

    public OnCollideEnter_Del Del_Trigger;

	public EnemyType m_EnemyType;

    // Use this for initialization
    void Start()
    {
        m_SpeedRate = 1.0f;
        m_IsDead = false;
        m_IsShock = false;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_LRobot = GameManager.Instance.m_LRobot;
        m_RRobot = GameManager.Instance.m_RRobot;
        m_HumanoidRobot = GameManager.Instance.m_HumanoidRobot;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        //NextTarget();        
    }

    // Update is called once per frame
    void Update()
    {
        m_LRobot = GameManager.Instance.m_LRobot;
        m_RRobot = GameManager.Instance.m_RRobot;
        m_HumanoidRobot = GameManager.Instance.m_HumanoidRobot;
        if (m_LRobot != null) m_LRobotPos = m_LRobot.transform.position;
        if (m_RRobot != null) m_RRobotPos = m_RRobot.transform.position;

        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.NoPlay:
                break;
            case PlayMode.TwinRobot:
                //Move();
                break;
            case PlayMode.HumanoidRobot:
                //Move();
                break;
            case PlayMode.Combine:
                break;
            case PlayMode.Release:
                break;
            default:
                break;
        }
        if (m_FreezeVelocity)
            m_Rigidbody.velocity = Vector3.zero;
    }

    private void Move()
    {
        int count = 0;
        do
        {
            Vector3 forward = m_Target - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, forward, out hit, forward.magnitude + 2))
            {
                if (hit.collider.tag == "Fence")
                {
                    NextTarget();
                    count += 1;
                }
                else
                {
                    count = 5;
                }
            }
            else
            {
                count = 5;
            }
        }
        while (count < 5);
        m_Rigidbody.position += Vector3.Normalize(m_Target - transform.position) * m_MoveSpeed * m_SpeedRate * Time.deltaTime;
        if (Vector3.Distance(transform.position, m_Target) < 0.1f)
        {
            NextTarget();
        }
    }
    public void NextTarget()
    {
        m_Target = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
        transform.LookAt(m_Target);
    }

    public void SetBreak()
    {
        GameObject fragment = Instantiate(m_Fragment, transform.position, transform.rotation, transform.parent);
        fragment.transform.parent = null;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        Del_Trigger = null;
        m_IsDead = true;
        gameObject.SetActive(false);
        m_FreezeVelocity = true;
        EnemyManager.Instance.ReSpawnEnemy(this);
    }

    public void SetBreakForPlayer()
    {
        GameManager.Instance.m_PlayScore++;
        SetBreak();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Guid" && GameManager.Instance.m_PlayMode == PlayMode.TwinRobot)
        {
            m_SpeedRate = 0.5f;
        }

        if (Del_Trigger != null)
            Del_Trigger.Invoke(other, this);
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Guid" && GameManager.Instance.m_PlayMode == PlayMode.TwinRobot)
        {
            //m_Rigidbody.position += Vector3.Lerp(
            m_NavMeshAgent.Move(Vector3.Lerp(
                m_LRobot.transform.position - m_LRobotPos,
                m_RRobot.transform.position - m_RRobotPos,
                Vector3.Distance(transform.position, m_LRobot.transform.position) /
                (Vector3.Distance(transform.position, m_LRobot.transform.position) +
                 Vector3.Distance(transform.position, m_RRobot.transform.position))) * 0.1f);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Guid" && GameManager.Instance.m_PlayMode == PlayMode.TwinRobot)
        {
            m_SpeedRate = 1.0f;
        }
    }
    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Wall")
    //        Debug.Log("Enemy OnCollisionEnter");        
    //}
}
