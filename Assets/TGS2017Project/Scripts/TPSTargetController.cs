using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリプト：TPS注視点コントローラー
/// 製作者；Ho Siu Ki（何兆祺）
/// </summary>
public class TPSTargetController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;    // プレイヤー
    [SerializeField]
    private float m_Speed;          // 回転速度
    [SerializeField]
    private float m_MaxHeight;      // 最大高度
    [SerializeField]
    private float m_MinHeight;      // 最小高度

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの位置
        Vector3 player_position = m_Player.transform.position;
        // 注視点の上下移動
        float moveY = Input.GetAxis("VerticalR") * m_Speed * Time.deltaTime;
        transform.Translate(new Vector3(0, moveY, 0));
        // 高度制限
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, player_position.y + m_MinHeight, player_position.y + m_MaxHeight), transform.position.z);
    }
}
