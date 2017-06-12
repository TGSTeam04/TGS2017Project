using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBreak : MonoBehaviour {
	[SerializeField]
	float m_Power = 200;
	[SerializeField]
	float m_Radius = 5;
	// Use this for initialization
	void Start () {
		var rigid = GetComponentsInChildren<Rigidbody>();
		foreach (var r in rigid)
		{
			r.AddExplosionForce(m_Power, transform.position, m_Radius);
		}
		GameObject.Destroy(this.gameObject, 5);
	}


}
