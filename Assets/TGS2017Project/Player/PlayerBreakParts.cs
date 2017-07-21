using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBreakParts : MonoBehaviour
{
    private float m_Timer = 0;
    private float m_DestroyTime = 1.0f;
    private bool m_IsGrounded = false;

    // Update is called once per frame
    void Update()
    {
        if (m_IsGrounded)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer > m_DestroyTime)
                Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            m_IsGrounded = true;
        }
    }
}
