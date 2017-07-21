using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreak : MonoBehaviour
{
    [SerializeField]
    float m_Power = 200;
    [SerializeField]
    float m_Radius = 5;

    private float m_Timer = 5;

    // Use this for initialization
    void Start()
    {
        var rigid = GetComponentsInChildren<Rigidbody>();
        foreach (var r in rigid)
        {
            r.AddExplosionForce(m_Power, transform.position, m_Radius);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0)
            Destroy(gameObject);
    }
}
