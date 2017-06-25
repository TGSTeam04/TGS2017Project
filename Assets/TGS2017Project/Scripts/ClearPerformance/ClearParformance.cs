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

    protected bool Redy()
    {
        GameManager gm = GameManager.Instance;
        if (CheckNecessary(gm))
        {
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
        m_PerformAnim.clip.SampleAnimation(m_ParformAnimRootObj, 0.0f);
        return true;
    }

    protected IEnumerator PerformManagement()
    {
        GameManager gm = GameManager.Instance;
        var async = gm.m_GameStarter.AddScene("Fade");
        while (async.isDone != false)
        {
            yield return null;
        }
        //フェード
    }

    //protected IEnumerator LookBoss()
    //{
    //    GameManager gm = GameManager.Instance;
    //    GameObject playCamera = gm.m_PlayCamera;
    //    NavMeshAgent agent = playCamera.AddComponent<NavMeshAgent>();
    //    agent.speed = 30.0f;
    //    Vector3 bossPos  = gm.m_StageManger.m_Boss.transform.position;
    //    agent.destination = bossPos;
    //    while (playCamera)
    //    {

    //    }
    //}

    private void OnDestroy()
    {
        //カメラを元に戻す
        GameManager.Instance.m_PlayCamera.SetActive(true);
    }
}
