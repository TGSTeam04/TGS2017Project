using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//タグ追加する際はPauserのs_TargetByTagの初期化部分も処理を増やしてください。
public enum PauseTag
{
    Pause,
    Enemy,
}

//ポーズ管理する際のメンバ郡（初期化だるいからクラス）
public class PauseParams
{
    public bool m_IsPause = false;
    //全体で共有するポーズ対象
    public List<Pauser> m_Targets = new List<Pauser>();   // ポーズ対象のスクリプト
}

/// <summary>
/// 製作者 ：大格
/// 更新日 ：5/26
/// 内容　 ：ポーザー
/// 使い方 ：ポーズしたいオブジェクトにこのコンポーネントを追加してください。
/// 備考　 ：
/// 1.ポーズするときに処理が重い可能性があるので、もし重そうだったら大格に連絡ください。
/// 2.コリジョンコンポーネントはenabledフラグを独自に持っており、Behaviourを継承せずに有効、無効を実現しているので反映されません。
/// 　ポーズを使うときは当たり判定があるものは全て止まると想定して、実装しませんが何か必要であれば大格に連絡ください。
/// </summary>

public class Pauser : MonoBehaviour
{
    static public Dictionary<PauseTag, PauseParams> s_TargetByTag = new Dictionary<PauseTag, PauseParams>()
    {
        { PauseTag.Pause,new PauseParams() },
        { PauseTag.Enemy,new PauseParams() }
    };

    //ポーズタグ
    public List<PauseTag> m_Tags = new List<PauseTag>() { PauseTag.Pause };

    //ポーズ中のコンポーネント
    Behaviour[] pauseBehavs = null;

    //ポーズ復帰時に使用するリジットボディとそのパラメータ
    Rigidbody[] rgBodies = null;
    Vector3[] rgBodyVels = null;
    Vector3[] rgBodyAVels = null;
    //2D
    Rigidbody2D[] rg2dBodies = null;
    Vector2[] rg2dBodyVels = null;
    float[] rg2dBodyAVels = null;

    // 初期化
    void Start()
    {
        // ポーズ対象に追加する        
        foreach (var tag in m_Tags)
        {
            s_TargetByTag[tag].m_Targets.Add(this);
        }
    }

    private void OnDestroy()
    {
        // ポーズ対象から除外する
        foreach (var tag in m_Tags)
        {
            s_TargetByTag[tag].m_Targets.Remove(this);
        }
    }

    // ポーズされたとき
    public void OnPause()
    {
        if (pauseBehavs != null)
        {
            return;
        }

        // 有効なコンポーネントを取得
        pauseBehavs = Array.FindAll(GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
        if (pauseBehavs == null)
            return;
        foreach (var com in pauseBehavs)
        {
            com.enabled = false;
        }

        rgBodies = Array.FindAll(GetComponentsInChildren<Rigidbody>(), (obj) => { return !obj.IsSleeping(); });
        rgBodyVels = new Vector3[rgBodies.Length];
        rgBodyAVels = new Vector3[rgBodies.Length];
        for (var i = 0; i < rgBodies.Length; ++i)
        {
            rgBodyVels[i] = rgBodies[i].velocity;
            rgBodyAVels[i] = rgBodies[i].angularVelocity;
            rgBodies[i].Sleep();
        }

        rg2dBodies = Array.FindAll(GetComponentsInChildren<Rigidbody2D>(), (obj) => { return !obj.IsSleeping(); });
        rg2dBodyVels = new Vector2[rg2dBodies.Length];
        rg2dBodyAVels = new float[rg2dBodies.Length];
        for (var i = 0; i < rg2dBodies.Length; ++i)
        {
            rg2dBodyVels[i] = rg2dBodies[i].velocity;
            rg2dBodyAVels[i] = rg2dBodies[i].angularVelocity;
            rg2dBodies[i].Sleep();
        }
    }

    // ポーズ解除されたとき
    public void OnResume()
    {
        if (pauseBehavs == null)
        {
            return;
        }

        // ポーズ前の状態にコンポーネントの有効状態を復元
        foreach (var com in pauseBehavs)
        {
            com.enabled = true;
        }

        for (var i = 0; i < rgBodies.Length; ++i)
        {
            rgBodies[i].WakeUp();
            rgBodies[i].velocity = rgBodyVels[i];
            rgBodies[i].angularVelocity = rgBodyAVels[i];
        }

        for (var i = 0; i < rg2dBodies.Length; ++i)
        {
            rg2dBodies[i].WakeUp();
            rg2dBodies[i].velocity = rg2dBodyVels[i];
            rg2dBodies[i].angularVelocity = rg2dBodyAVels[i];
        }

        pauseBehavs = null;

        rgBodies = null;
        rgBodyVels = null;
        rgBodyAVels = null;

        rg2dBodies = null;
        rg2dBodyVels = null;
        rg2dBodyAVels = null;
    }

    // ポーズ
    static public void Pause(PauseTag tag = PauseTag.Pause)
    {
        if (s_TargetByTag[tag].m_IsPause) return;

        s_TargetByTag[tag].m_IsPause = true;
        foreach (var obj in s_TargetByTag[tag].m_Targets)
        {
            obj.OnPause();
        }
    }

    // ポーズ解除
    static public void Resume(PauseTag tag = PauseTag.Pause)
    {
        if (!s_TargetByTag[tag].m_IsPause) return;

        s_TargetByTag[tag].m_IsPause = false;
        foreach (var obj in s_TargetByTag[tag].m_Targets)
        {
            obj.OnResume();
        }
    }
}