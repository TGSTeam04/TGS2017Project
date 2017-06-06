using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create BreakEnemyTable", fileName = "BreakEnemyTable")]
public class BreakEnemyTable : ScriptableObject {
	public List<float> m_AddEnergy;
}
