using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakLeg : MonoBehaviour {

    public GameObject m_Explosion;
    public Collider m_Collision;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        print(SecondBoss.s_HitPoint);
	}

    public void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.m_PlayMode == PlayMode.Combine && other.name == "Break" && SecondBoss.s_State == SecondBoss.SecondBossState.Paralysis)
        {
            Instantiate(m_Explosion, transform.position, transform.rotation);
            m_Collision.isTrigger = false;
            SecondBoss.s_HitPoint -= 0.25f;
            SecondBoss.s_State = SecondBoss.SecondBossState.Invincible;
        }
    }
}
