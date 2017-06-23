//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class KnockBackEnemy : MonoBehaviour
//{
//    public Vector3 m_Direction;
//    public float m_Force;
//    public EnemyBase m_EnemyBase;
//    public string m_TargetTag;
//    // Use this for initialization
//    void Start()
//    {
//        m_EnemyBase = GetComponent<EnemyBase>();
//        Rigidbody rb = GetComponent<Rigidbody>();

//        rb.constraints = RigidbodyConstraints.None;
//        rb.AddForce(m_Direction.normalized * m_Force, ForceMode.Impulse);        
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.tag == m_TargetTag)
//        {//攻撃対象と衝突
//            other.gameObject.GetComponent<Damageable>().ApplyDamage(m_ChildApplyDamage, this);
//            Debug.Log("ノックバックEnemy　が　Player　と衝突");
//        }
//        if (other.gameObject.tag != gameObject.tag)
//        {
//            m_EnemyBase.SetBreakForPlayer();
//        }
//    }
//}
