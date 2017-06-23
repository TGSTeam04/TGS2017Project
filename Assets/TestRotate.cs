using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{

    Rigidbody m_Rb;
    BoxCollider m_BCollider;
    public Vector3 m_velocity = new Vector3(0, 0, 1);

    // Use this for initialization
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_BCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Rb.velocity = Vector3.zero;
        Vector3 velocity = m_velocity;
        m_Rb.MovePosition(m_Rb.position + m_Rb.rotation * velocity * Time.fixedDeltaTime);

        float pich = transform.rotation.eulerAngles.x;
        float forwardLen = (m_BCollider.size.z / 2 * transform.lossyScale.z) - m_Rb.centerOfMass.z;// * m_Collider.size.z;
        float borderHigth = forwardLen * Mathf.Sin(pich * Mathf.Deg2Rad);// + transform.lossyScale.y / 2;
        RaycastHit hitInfo;
        if (Physics.Raycast(m_Rb.position, Vector3.down * borderHigth * 2, out hitInfo, LayerMask.GetMask("Floor")))
        {
            float heigth = (m_Rb.position.y - hitInfo.point.y) - m_BCollider.size.y / 2;
            if (borderHigth > heigth && heigth > float.Epsilon)
            {
                float sin = heigth / forwardLen;
                float deg = Mathf.Asin(sin) * Mathf.Rad2Deg;
                //Debug.Log(deg);
                m_Rb.rotation = Quaternion.Euler(new Vector3(deg, m_Rb.rotation.eulerAngles.y, m_Rb.rotation.eulerAngles.z));
            }
        }
    }
}
