using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "MyGame/Create HumanoidConfig", fileName = "HumanoidConfig")]
public class HumanoidConfig : ScriptableObject {
	public float m_NormalSpeed;
	public float m_BoostSpeed;
	public float m_NormalRotate;
	public float m_ChargeRotate;
	public float m_JumpPower;
	public float m_NormalUseEnergy;
	public float m_BoostUseEnergy;
	public float m_ChargeUseEnergy;
}
