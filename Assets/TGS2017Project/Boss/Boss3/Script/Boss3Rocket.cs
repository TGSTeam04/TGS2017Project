using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Rocket : RocketBase
{
    public float m_ReflectDamage = 50.0f;
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        // CommonCollide(collision);

        if (collision.gameObject.tag == "PlayerBullet" && m_State != RocketState.Buried)
        {
            //反射
            SetLayer("PlayerBullet");
            m_State = RocketState.Reflected;
        }
        else if (collision.gameObject == m_Battery.gameObject) //Enemy　PlayerBullet　以外に当たったらレイヤーを元に戻す
        {
            collision.gameObject.GetComponent<Damageable>().ApplyDamage(m_ReflectDamage, this);
            SetLayer("EnemyBullet");
            m_State = RocketState.Idle;
            m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            gameObject.SetActive(false);
        }
    }
}
