using System.Collections;
using UnityEngine;

public class Boss3Rocket : RocketBase
{
    public float m_ReflectDamage = 50.0f;
    public float m_ReActiveTime;
    public float m_RepairTime;

    //腕が消える処理
    //public IEnumerator Break()
    //{
    //    m_State = RocketState.Repair;
    //    SetLayer("BossBullet");
    //    gameObject.SetActive(false);
    //    m_StandTrans.localScale = Vector3.zero;
    //    //一定時間無効        
    //    yield return new WaitForSeconds(m_ReActiveTime);
    //    //Debug.Log("リペア開始");
    //    float repairTime = 0.0f;
    //    while (repairTime < m_RepairTime)
    //    {
    //        repairTime += Time.deltaTime;
    //        m_StandTrans.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, repairTime / m_RepairTime);
    //        yield return null;
    //    }
    //    m_StandTrans.localScale = Vector3.one;
    //    m_State = RocketState.Idle;
    //}

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.tag == "PlayerBullet" && m_State != RocketState.Buried)
        {
            //反射
            SetLayer("PlayerBullet");
            m_State = RocketState.Reflected;
        }
        else if (collision.gameObject == m_Battery.gameObject) //Enemy　PlayerBullet　以外に当たったらレイヤーを元に戻す
        {
            collision.gameObject.GetComponent<Damageable>().ApplyDamage(m_ReflectDamage, this);            
            m_State = RocketState.Idle;
            gameObject.SetActive(false);
            m_StandTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            SetLayer("BossBullet");
            Boss3_Humanoid boss3 = m_Battery.GetComponent<Boss3_Humanoid>();
            boss3.Release();
        }
    }
}
