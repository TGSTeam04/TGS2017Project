using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TitleBGRotate : MonoBehaviour {

	public float m_Speed;

	void Update () {
		transform.Rotate(0, 0, m_Speed * Time.deltaTime);
	}
}
