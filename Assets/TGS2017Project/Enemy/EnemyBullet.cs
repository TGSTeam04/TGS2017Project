using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;      // 弾速

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);
    }

    void FixedUpdate()
    {

    }

    // 接触判定
    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Player")
		{
			print("Hit player");
		}
		Destroy(gameObject);

	}

	public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        // プレイヤーに命中
        if (other.gameObject.tag == "Player")
        {
            print("Hit player");
        }
        Destroy(gameObject);
    }
}