﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField]
	private float m_Speed;      // 弾速
	[SerializeField] private float m_ApplyDamage = 10.0f;

    float m_DeadCount = 10;

	// Use this for initialization
	void Start()
	{
        StartCoroutine(Death());
	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
	}

	void FixedUpdate()
	{

	}

	public void OnTriggerEnter(Collider other)
	{
		// プレイヤーに命中
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log(other.gameObject);
			other.gameObject.GetComponent<Damageable>().ApplyDamage(m_ApplyDamage, this);
            Destroy(gameObject);
		}
	}
    IEnumerator Death()
    {
        yield return new WaitForSeconds(m_DeadCount);
        Destroy(gameObject);
    }
}