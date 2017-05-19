using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Bullet") return;

		// プレイヤーに命中
		if (other.gameObject.tag == "Player")
		{
			print("Hit player");
		}
		Destroy(gameObject);
	}
}
