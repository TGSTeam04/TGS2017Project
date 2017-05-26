using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElectricEffect : MonoBehaviour
{

	public Transform m_L;
	public Transform m_R;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.localScale = new Vector3(Vector3.Distance(m_L.position, m_R.position),5, 5);
		transform.position = Vector3.Lerp(m_L.position, m_R.position, 0.5f) + transform.up * 2f;
		transform.LookAt(transform.position + Vector3.Cross(m_L.position - transform.position, transform.up));
	}
}
