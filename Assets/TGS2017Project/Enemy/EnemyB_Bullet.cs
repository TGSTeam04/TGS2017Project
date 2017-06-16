using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public class EnemyB_Bullet : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;          // 弾速
    [SerializeField]
    private float m_DeleteTimer;    // 消滅タイマー

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 前方へ飛ぶ
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

        // 当たり判定を拡大（散弾のように）
        gameObject.transform.localScale = new Vector3(
            gameObject.transform.localScale.x + 0.1f,
            gameObject.transform.localScale.y + 0.1f,
            gameObject.transform.localScale.z
            );

        // タイマーが0になったら消滅
        --m_DeleteTimer;
        if (m_DeleteTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 接触判定
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //print("Enemy B bullet hit player");
        }
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        // プレイヤーに命中
        if (other.gameObject.tag == "Player")
        {
            //print("Enemy B bullet hit player");
        }
        Destroy(gameObject);
    }
}
