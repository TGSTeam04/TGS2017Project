using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
	public Transform m_Trap;
	public Transform m_Electric;
	public void SetTrap(Vector3 position, float scale)
	{
		m_Trap.position = position;
		m_Electric.position = Vector3.Lerp(transform.position, m_Trap.position, 0.5f);
		m_Electric.localScale = new Vector3(1, 0.5f, Vector3.Distance(transform.position, m_Trap.position));
		transform.localScale = new Vector3(scale, 1, 1);
	}
}
