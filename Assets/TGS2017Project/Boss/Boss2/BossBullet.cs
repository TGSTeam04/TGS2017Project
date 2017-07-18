using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField]
    private float m_ApplyDamage = 50.0f;
    [SerializeField]
    private float m_Speed = 90.0f;

    private Vector3 m_Position;
    private Vector3 m_TargetPosition;
    private int m_DeathCount = 4;
    AudioSource m_Sound;

    // Use this for initialization
    void Start()
    {
        m_Position = transform.position;
        m_Sound = GetComponent<AudioSource>();
        switch (GameManager.Instance.m_PlayMode)
        {
			// プレイヤー分離時
			case PlayMode.TwinRobot:
			// プレイヤー合体時
			case PlayMode.HumanoidRobot:
				// プレイヤーに追従
				m_TargetPosition = PlayerManager.Instance.NearPlayer(transform.position).position;
				break;
			default:
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Dead());
        transform.Translate(Vector3.Normalize(m_TargetPosition - m_Position) * m_Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        var damageComp = other.GetComponent<Damageable>();
        if (damageComp != null)
            damageComp.ApplyDamage(m_ApplyDamage, this);
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(m_DeathCount);
        Destroy(gameObject);
    }
}
