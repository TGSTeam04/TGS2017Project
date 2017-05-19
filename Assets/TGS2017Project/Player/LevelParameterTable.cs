using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyGame/Create LevelParameterTable",fileName ="LevelParameterTable")]
public class LevelParameterTable : ScriptableObject {

	public List<LevelParameter> LPTable;

}

[System.SerializableAttribute]
public class LevelParameter
{
	public int m_NextExp = 1;
	public float m_Scale = 1;
	public float m_Speed = 1;
}
