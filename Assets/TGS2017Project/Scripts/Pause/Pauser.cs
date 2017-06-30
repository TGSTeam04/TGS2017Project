using System;
using System.Collections.Generic;
using UnityEngine;

//タグ追加する際はPauserのs_TargetByTagの初期化部分も処理を増やしてください。
public enum PauseTag
{
    Pause,
    Enemy,
}

//ポーズ管理する際のメンバ郡（初期化だるいからクラス）
[System.SerializableAttribute]
public class PauseManageParam
{
    public bool m_IsPause = false;
    public bool m_IsColPause = false;
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
    static public Dictionary<PauseTag, PauseManageParam> s_TargetByTag = new Dictionary<PauseTag, PauseManageParam>()
    {
        { PauseTag.Pause , new PauseManageParam() },
        { PauseTag.Enemy , new PauseManageParam() }
    };
    // ポーズ
    static public void Pause(PauseTag tag = PauseTag.Pause, bool IncludeCollider = true)
    {
        //対象タグが既にポーズ中ならリターン
        if (!s_TargetByTag[tag].m_IsPause)
        {
            s_TargetByTag[tag].m_IsPause = true;
            foreach (var obj in s_TargetByTag[tag].m_Targets)
            {
                obj.OnPause();
            }
        }
        if (IncludeCollider && !s_TargetByTag[tag].m_IsColPause)
        {
            s_TargetByTag[tag].m_IsColPause = true;
            foreach (var obj in s_TargetByTag[tag].m_Targets)
            {
                obj.DisableCollider();
            }
        }
    }

    // ポーズ解除
    static public void Resume(PauseTag tag = PauseTag.Pause, bool IncludeCollider = true)
    {
        //対象タグがポーズされていなければリターン
        if (!s_TargetByTag[tag].m_IsPause) return;

        s_TargetByTag[tag].m_IsPause = false;
        foreach (var obj in s_TargetByTag[tag].m_Targets)
        {
            if (obj != null)
                obj.OnResume();
        }

        if (IncludeCollider)
        {
            foreach (var obj in s_TargetByTag[tag].m_Targets)
            {
                if (obj != null)
                    obj.EnableCollider();
            }
        }
    }
    static public bool IsTagPause(PauseTag tag = PauseTag.Pause) { return s_TargetByTag[tag].m_IsPause; }

    //ポーズタグ
    public List<PauseTag> m_Tags = new List<PauseTag>() { PauseTag.Pause };
    private int m_PauseCunt = 0;
    private int PauseCunt
    {
        get { return m_PauseCunt; }
        set { m_PauseCunt = Mathf.Max(0, value); }
    }
    private int m_ColliderPauseCunt = 0;
    private int ColliderPauseCunt
    {
        get { return m_ColliderPauseCunt; }
        set { m_ColliderPauseCunt = Mathf.Max(0, value); }
    }

    //ポーズ中のコンポーネント
    Behaviour[] m_PauseBehavs = null;

    //コリジョン
    Collider[] m_PauseCols = null;

    //ポーズ復帰時に使用するリジットボディとそのパラメータ
    Rigidbody[] rgBodies = null;
    Vector3[] rgBodyVels = null;
    Vector3[] rgBodyAVels = null;
    //2D
    Rigidbody2D[] rg2dBodies = null;
    Vector2[] rg2dBodyVels = null;
    float[] rg2dBodyAVels = null;

    public bool IsPause { get { return PauseCunt > 0; } }

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

    public void DisableCollider()
    {
        //カウンタ更新
        ColliderPauseCunt++;
        //初回のみ無効化
        if (ColliderPauseCunt != 1)
            return;

        //有効なコリジョンを取得、無効化
        m_PauseCols = Array.FindAll(GetComponentsInChildren<Collider>(), (col) => { return col.enabled; });

        foreach (var com in m_PauseCols)
            com.enabled = false;
    }
    public void EnableCollider()
    {
        //カウンタ更新
        ColliderPauseCunt--;
        //他のタグでポーズされていれば無視
        if (ColliderPauseCunt != 0)
            return;

        if (m_PauseCols == null)
            return;

        foreach (var col in m_PauseCols)
        {
            if (col != null)
                col.enabled = true;
        }
    }

    // ポーズされたとき
    public void OnPause()
    {
        //カウンタ更新
        PauseCunt++;
        //初回のみポーズ
        if (PauseCunt != 1)
            return;

        // 有効なコンポーネントを取得、無効化
        m_PauseBehavs = Array.FindAll(GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
        //if (pauseBehavs == null)
        //    return;
        foreach (var com in m_PauseBehavs)
            com.enabled = false;

        //リジットボディのデータを保存、スリープ
        rgBodies = Array.FindAll(GetComponentsInChildren<Rigidbody>(), (obj) => { return !obj.IsSleeping(); });
        rgBodyVels = new Vector3[rgBodies.Length];
        rgBodyAVels = new Vector3[rgBodies.Length];
        for (var i = 0; i < rgBodies.Length; ++i)
        {
            rgBodyVels[i] = rgBodies[i].velocity;
            rgBodyAVels[i] = rgBodies[i].angularVelocity;
            rgBodies[i].isKinematic = true;
            rgBodies[i].Sleep();
        }
        //２Dリジットボディ
        rg2dBodies = Array.FindAll(GetComponentsInChildren<Rigidbody2D>(), (obj) => { return !obj.IsSleeping(); });
        rg2dBodyVels = new Vector2[rg2dBodies.Length];
        rg2dBodyAVels = new float[rg2dBodies.Length];
        for (var i = 0; i < rg2dBodies.Length; ++i)
        {
            rg2dBodyVels[i] = rg2dBodies[i].velocity;
            rg2dBodyAVels[i] = rg2dBodies[i].angularVelocity;
            rg2dBodies[i].isKinematic = true;
            rg2dBodies[i].Sleep();
        }
    }

    // ポーズ解除されたとき
    public void OnResume()
    {
        //カウンタ更新
        PauseCunt--;
        //他のタグでポーズされていれば無視
        if (PauseCunt != 0)
            return;

        if (m_PauseBehavs != null)
        {
            // ポーズ前の状態にコンポーネントの有効状態を復元
            foreach (var com in m_PauseBehavs)
            {
                if (com != null)
                    com.enabled = true;
            }
        }

        for (var i = 0; i < rgBodies.Length; ++i)
        {
            rgBodies[i].WakeUp();
            rgBodies[i].isKinematic = false;
            rgBodies[i].velocity = rgBodyVels[i];
            rgBodies[i].angularVelocity = rgBodyAVels[i];
        }

        for (var i = 0; i < rg2dBodies.Length; ++i)
        {
            rg2dBodies[i].WakeUp();
            rg2dBodies[i].isKinematic = false;
            rg2dBodies[i].velocity = rg2dBodyVels[i];
            rg2dBodies[i].angularVelocity = rg2dBodyAVels[i];
        }

        m_PauseBehavs = null;

        rgBodies = null;
        rgBodyVels = null;
        rgBodyAVels = null;

        rg2dBodies = null;
        rg2dBodyVels = null;
        rg2dBodyAVels = null;
    }
}