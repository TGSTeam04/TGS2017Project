using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スクリプト：ゲーム管理者
/// 作成者：Ho Siu Ki（何兆祺）
/// </summary>

public class GameController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    // ゲーム終了
    public void Exit()
    {
        Application.Quit();
    }
}
