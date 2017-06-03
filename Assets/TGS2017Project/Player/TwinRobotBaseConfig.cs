using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create TwinRobotBaseConfig", fileName = "TwinRobotBaseConfig")]
public class TwinRobotBaseConfig : ScriptableObject{
	public float m_MoveSpeed;
	public float m_MaxHP;

	public Gradient m_ShieldColor;
}
