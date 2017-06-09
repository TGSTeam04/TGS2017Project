using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create HumanoidBaseConfig", fileName = "HumanoidBaseConfig")]
public class HumanoidBaseConfig : ScriptableObject {
	public string m_InputHorizontal = "HorizontalL";
	public string m_InputVertical = "VerticalL";
	public string m_InputRotation = "HorizontalR";
	public string m_InputBoost = "Boost";
	public string m_InputJump = "Jump";
	public string m_InputCharge = "Charge";
}
