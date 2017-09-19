using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField]
	private float m_Speed;      // 弾速
	[SerializeField] private float m_ApplyDamage = 10.0f;

	float m_DeadCount = 10;

	[SerializeField]
	ParticleSystem m_Particle;

	// Use this for initialization
	void Start()
	{
		//StartCoroutine(Death());
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
			m_Particle.Stop();
		}

		// 壁に当たる（９月５日に追加　カ）
		if (other.gameObject.tag == "Wall")
		{
			//Debug.Log("Hit wall");
			m_Particle.Stop();
		}
	}
	public void OnTriggerExit(Collider other)
	{
		// プレイヤーに命中
		if (other.gameObject.tag == "Player")
		{
			//Debug.Log(other.gameObject);
			Destroy(gameObject);
		}

		// 壁に当たる（９月５日に追加　カ）
		if (other.gameObject.tag == "Wall")
		{
			//Debug.Log("Hit wall");
			Destroy(gameObject);
		}
	}

	IEnumerator Death()
	{
		yield return new WaitForSeconds(m_DeadCount);
		Destroy(gameObject);
	}
}