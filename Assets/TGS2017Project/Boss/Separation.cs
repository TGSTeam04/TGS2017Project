using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour {

    enum SeparateState
    {
        Chase,
        Return
    }

    [SerializeField]
    private int m_ActivityTime = 10;
    [SerializeField]
    private float m_MoveSpeed = 10.0f;

    private Transform m_Target;

    [SerializeField]
    private SeparateState m_State = SeparateState.Chase;
    int m_Counter;

	// Use this for initialization
	void Start () {
        m_State = SeparateState.Chase;
        StartCoroutine(GoHome());
	}
	
	// Update is called once per frame
	void Update () {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.TwinRobot:
                GameObject L = GameManager.Instance.m_LRobot;
                GameObject R = GameManager.Instance.m_RRobot;
                break;
            case PlayMode.HumanoidRobot:
                m_Target = GameManager.Instance.m_HumanoidRobot.transform;
                break;
            case PlayMode.NoPlay:
            case PlayMode.Combine:
            case PlayMode.Release:
            default:
                return;
        }

        switch (m_State)
        {
            case SeparateState.Chase:
                break;
            case SeparateState.Return:
                break;
        }
	}
    IEnumerator GoHome()
    {
        m_Counter = m_ActivityTime;
        while(m_Counter > 0)
        {
            yield return new WaitForSeconds(1.0f);
            m_Counter--;
        }
        m_State = SeparateState.Return;
    }
}
