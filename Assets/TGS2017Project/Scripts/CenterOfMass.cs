using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Vector3 m_Center;
    private Rigidbody m_Rb;

    // Use this for initialization
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.centerOfMass = m_Center;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * m_Center, 0.2f);
    }
}
