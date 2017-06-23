using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraController : MonoBehaviour
{
    private Camera m_Camera;

    public Transform m_TopTransform;
    public Transform m_TPSTransform;    // TPSモードでのプレイヤーの位置
    public Transform m_TPSTarget;       // TPSモードでのカメラの注視点
    private RaycastHit hit;             // 障害物判定用のRaycast

    public AnimationCurve m_FOVCurve;
    public AnimationCurve m_CombinePositionCurve;
    public AnimationCurve m_RotationCurve;
    public AnimationCurve m_ReleasePositionCurve;

    public float m_MAXFOV;

    public float m_RotateSpeed;
    public float m_MoveSpeed;

    private PlayMode m_PreMode;
    private bool m_IsRunning = false;
    // Use this for initialization
    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.NoPlay:
                if (m_TopTransform != null)
                {
                    transform.position = m_TopTransform.transform.position;
                    transform.rotation = m_TopTransform.transform.rotation;
                }
                break;
            case PlayMode.TwinRobot:
                transform.position = m_TopTransform.transform.position;
                transform.rotation = m_TopTransform.transform.rotation;
                break;
            case PlayMode.HumanoidRobot:
                break;
            case PlayMode.Combine:
                if (m_PreMode == PlayMode.TwinRobot) StartCoroutine(Combine());
                break;
            case PlayMode.Release:
                if (m_PreMode != PlayMode.Release) StartCoroutine(Release());
                break;
            default:
                break;
        }
        m_PreMode = GameManager.Instance.m_PlayMode;
    }

    void FixedUpdate()
    {
        switch (GameManager.Instance.m_PlayMode)
        {
            case PlayMode.NoPlay:
                break;
            case PlayMode.TwinRobot:
                break;
            case PlayMode.HumanoidRobot:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_TPSTarget.position - m_TPSTransform.position), m_RotateSpeed * Time.fixedDeltaTime);
                transform.position = Vector3.Lerp(transform.position, m_TPSTransform.position, m_MoveSpeed * Time.fixedDeltaTime);

                // カメラが壁に遮られキャラクターが見えなくなったらカメラを移動させる
                // 作成者：Ho Siu Ki（何兆祺）
                Vector3 camera_position = transform.position;       // カメラの位置
                Vector3 player_position = m_TPSTransform.position;  // プレイヤーの位置
                // カメラ、プレイヤーのy軸位置を2として計算
                camera_position.y = 2;
                player_position.y = 2;

                // カメラが壁に遮られた場合
                if (Physics.Linecast(player_position, camera_position, LayerMask.GetMask("Wall")))
                {
                    Debug.Log("壁に遮られた");
                    // カメラの移動
                    transform.position = Vector3.Lerp(transform.position, m_TPSTransform.position / 2, m_MoveSpeed * Time.fixedDeltaTime);
                    transform.Translate(new Vector3(0, 0.1f, 0));
                }

                break;
            case PlayMode.Combine:
                break;
            case PlayMode.Release:
                break;
            default:
                break;
        }
    }

    IEnumerator Combine()
    {
        if (m_IsRunning) { yield break; }
        m_IsRunning = true;

        Quaternion LookRobot = Quaternion.LookRotation(m_TPSTarget.position - m_TPSTransform.position);

        float time = 0.3f;
        for (float f = 0; f < time; f += Time.deltaTime)
        {
            //m_Camera.fieldOfView = Mathf.Lerp(60, m_MAXFOV, m_FOVCurve.Evaluate(f));
            transform.position = Vector3.Lerp(m_TopTransform.position, m_TPSTransform.position, m_CombinePositionCurve.Evaluate(f / time));
            transform.rotation = Quaternion.Lerp(m_TopTransform.rotation, LookRobot, m_RotationCurve.Evaluate(f / time));
            yield return null;
        }
        m_Camera.fieldOfView = 60;
        yield return new WaitForSeconds(0.3f);
        time = 1.0f;
        for (float f = 0; f < time; f += Time.deltaTime)
        {
            transform.position = m_TPSTransform.position;
            transform.rotation = Quaternion.LookRotation(m_TPSTarget.position - m_TPSTransform.position);
            yield return null;
        }

        transform.position = m_TPSTransform.position;
        transform.rotation = m_TPSTransform.rotation;

        m_IsRunning = false;
    }

    IEnumerator Release()
    {
        if (m_IsRunning) { yield break; }
        m_IsRunning = true;

        for (float f = 0; f < GameManager.Instance.m_CombineTime; f += Time.deltaTime)
        {
            m_Camera.fieldOfView = Mathf.Lerp(60, 30 + m_MAXFOV / 2, m_FOVCurve.Evaluate(1 - f));
            transform.position = Vector3.Lerp(m_TPSTransform.position, m_TopTransform.position, m_ReleasePositionCurve.Evaluate(f));
            transform.rotation = Quaternion.Lerp(m_TopTransform.rotation, Quaternion.LookRotation(m_TPSTarget.position - transform.position), m_RotationCurve.Evaluate(1 - f));
            yield return null;
        }
        m_Camera.fieldOfView = 60;
        transform.position = m_TopTransform.position;
        transform.rotation = m_TopTransform.rotation;

        m_IsRunning = false;
    }
}
