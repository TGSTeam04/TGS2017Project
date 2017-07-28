using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームオーバー画面、残骸回転スクリプト
/// 製作者：Ho Siu Ki（何兆祺）
/// </summary>

public class GameOverRotate : MonoBehaviour
{
    [SerializeField]
    private float m_Rotate_speed;   // 回転速度

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, m_Rotate_speed, 0.0f);
    }
}
