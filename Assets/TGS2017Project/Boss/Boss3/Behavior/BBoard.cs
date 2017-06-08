using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//値設定するのに便利だからMonoBehavior継承。
public class BBoard : MonoBehaviour
{
    private Dictionary<string, object> m_ObjValues;
    private Dictionary<string, bool> m_BoolValues;
    private Dictionary<string, int> m_IntValues;
    private Dictionary<string, float> m_FloatValues;
    private Dictionary<string, Vector2> m_Vec2Values;
    private Dictionary<string, Vector3> m_Vec3Values;
    private Dictionary<string, Transform> m_TransValues;
    private Dictionary<string, GameObject> m_GameObjValus;
    private Dictionary<string, MonoBehaviour> m_MonoValues;

    public Dictionary<string, object> ObjValues { get { return m_ObjValues; } }
    public Dictionary<string, bool> BoolValues { get { return m_BoolValues; } }
    public Dictionary<string, int> IntValues { get { return m_IntValues; } }
    public Dictionary<string, float> FloatValues { get { return m_FloatValues; } }
    public Dictionary<string, Vector2> Vec2Values { get { return m_Vec2Values; } }
    public Dictionary<string, Vector3> Vec3Values { get { return m_Vec3Values; } }
    public Dictionary<string, Transform> TransValues { get { return m_TransValues; } }
    public Dictionary<string, GameObject> GObjValues { get { return m_GameObjValus; } }
    public Vector3 PositionValus(string name)
    {
        GameObject obj;
        if (m_GameObjValus.TryGetValue(name, out obj))
            return obj.transform.position;
        else
            return m_Vec3Values[name];
    }
    public Dictionary<string, MonoBehaviour> MonoValues { get { return m_MonoValues; } }

    public BBoard()
    {
        m_ObjValues = new Dictionary<string, object>();
        m_BoolValues = new Dictionary<string, bool>();
        m_IntValues = new Dictionary<string, int>();
        m_FloatValues = new Dictionary<string, float>();
        m_Vec2Values = new Dictionary<string, Vector2>();
        m_Vec3Values = new Dictionary<string, Vector3>();
        m_TransValues = new Dictionary<string, Transform>();
        m_GameObjValus = new Dictionary<string, GameObject>();
        m_MonoValues = new Dictionary<string, MonoBehaviour>();
    }
}
