using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBreakParts : MonoBehaviour
{
    private float m_Timer = 0;
    private float m_DestroyTime = 1.0f;
    private bool m_IsFell = false;
    private void Update()
    {
        if (m_IsFell)
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
            m_IsFell = true;
            //StartCoroutine(this.Delay(new WaitForSeconds(1.0f), ()
            //    => Destroy(gameObject)
            //));
        }
    }
}
