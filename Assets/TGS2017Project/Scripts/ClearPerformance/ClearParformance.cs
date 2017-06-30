using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ClearParformance : MonoBehaviour
{
    //performanceをする位置
    public GameObject m_ParformAnimRootObj;
    //パフォーマンスのアニメ
    protected Animation m_PerformAnim;
    public AnimationClip m_PerformAnimClip;

    public Camera m_Camera;
    //TPSクリア時のカメラのカメラのペアレントにするべき対象
    public Transform m_CameraParent;

    public abstract bool CheckNecessary(GameManager gm);

    // Use this for initialization    

    //共通準備
    protected bool CommonRedy()
    {
        GameManager gm = GameManager.Instance;
        if (CheckNecessary(gm))
        {
            //自身の演出が必要でなければfalseを返すし、自身を無効化
            enabled = false;            
            return false;
        }

        //カメラの設定
        m_Camera.transform.parent = m_CameraParent;
        m_Camera.transform.localPosition = Vector3.zero;
        m_Camera.transform.localRotation = Quaternion.identity;

        //エネミー除去
        EnemyManager.Instance.gameObject.SetActive(false);

        //パフォーマンス位置設定
        Transform loc = gm.m_StageManger.m_ClearPerformLoc;
        if (loc != null)
            m_ParformAnimRootObj.transform.SetPositionAndRotation(loc.position, loc.rotation);
        //パフォーマンスアニメ設定
        m_PerformAnim = m_ParformAnimRootObj.GetComponent<Animation>();
        m_PerformAnim.clip = m_PerformAnimClip;        
        return true;
    }

    protected abstract void Redy();

    //共通演出等を実行し、各演出を管理するコルーチン
    protected IEnumerator PerformManagement()
    {
        GameManager gm = GameManager.Instance;
        //フェード
        var async = gm.m_GameStarter.AddScene("FadeScene");
        while (async.isDone != false) yield return null;
        //シーンのオブジェクトの作成を待つ（２フレ）
        yield return null;
        yield return null;

        //フェードイン
        bool fadeEnd = false;
        FadeSceneManager fadeMane = GameObject.Find("FadeSceneManager").GetComponent<FadeSceneManager>();
        fadeMane.EndEvent.AddListener(() => { fadeEnd = true; });
        fadeMane.FadeIn();

        while (!fadeEnd) yield return null;

        /*暗転状態で行いたい処理*/
        //各オブジェクトの配置を初期化
        if (!CommonRedy()) yield break;
        Redy();
        m_PerformAnim.clip.SampleAnimation(m_ParformAnimRootObj, 0.0f);
        GameManager.Instance.m_PlayCamera.SetActive(false);
        m_Camera.gameObject.SetActive(true);
        Pauser.Resume();
        yield return null;

        //フェードアウト
        fadeEnd = false;
        fadeMane.FadeOut();       
        while (!fadeEnd) yield return null;

        //演出スタート
        yield return StartCoroutine(PlayerParform());

        //後処理
        EndPerform();
    }

    //演出終了後の後処理
    protected void EndPerform()
    {
        var async = GameManager.Instance.m_GameStarter.AddScene("Result");
    }

    //各パフォーマンス
    protected abstract IEnumerator PlayerParform();

    private void OnDestroy()
    {
        //カメラを元に戻す
        GameManager.Instance.m_PlayCamera.SetActive(true);
    }
}
