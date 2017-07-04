using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create TwinRobotConfig", fileName = "TwinRobotConfig")]
public class TwinRobotConfig : ScriptableObject {
	public string m_InputHorizontal = "Horizontal";
	public string m_InputVertical = "Vertical";
	public string m_InputModeChange = "Charge";
}
