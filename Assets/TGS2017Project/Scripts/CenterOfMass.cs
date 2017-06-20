using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    [SerializeField] private Vector3 m_Center;
    private Rigidbody m_Rb;

    public Vector3 Center
    {
        get { return m_Rb.centerOfMass; }
        set { m_Rb.centerOfMass = value; }
    }

    // Use this for initialization
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.centerOfMass = m_Center;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * m_Center, 0.2f);
    }
}
