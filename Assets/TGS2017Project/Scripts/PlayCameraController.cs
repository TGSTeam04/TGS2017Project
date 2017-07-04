using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraController : MonoBehaviour
{
    private Camera m_Camera;

    public Transform m_TopTransform;
    public Transform m_TPSTransform;    // TPSモードでのプレイヤーの位置
    public Transform m_TPSTarget;       // TPSモードでのカメラの注視点

    public AnimationCurve m_FOVCurve;
    public AnimationCurve m_CombinePositionCurve;
    public AnimationCurve m_RotationCurve;
    public AnimationCurve m_ReleasePositionCurve;

    public float m_MAXFOV;

    public float m_RotateSpeed;
    public float m_MoveSpeed;

    private PlayMode m_PreMode;
    private bool m_IsRunning = false;
    private Coroutine m_combineCor;
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
                if (m_PreMode == PlayMode.TwinRobot) m_combineCor = StartCoroutine(Combine());
                break;
            case PlayMode.Release:
                if (m_PreMode != PlayMode.Release) StartCoroutine(Release(m_PreMode == PlayMode.Combine));
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
                Vector3 target_position = m_TPSTarget.position;     // 注視点の位置

                // カメラ、プレイヤーのy軸位置を2として計算
                camera_position.y = 2;
                target_position.y = 2;
                // カメラとプレイヤーの距離を取得
                float distance = Vector3.Distance(camera_position, target_position);
                // Rayでカメラが壁に遮られるかどうかを判定
                Ray ray = new Ray(target_position + Vector3.up, camera_position - target_position);
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
                RaycastHit hitInfo;
                // カメラが壁に遮られた場合
                if (Physics.Raycast(ray, out hitInfo, distance, LayerMask.GetMask("Wall")))
                {
                    //Debug.Log("壁に遮られた");
                    // カメラの位置を壁に保持
                    var height = transform.position.y - hitInfo.point.y;
                    //transform.position = hitInfo.point;
                    //transform.Translate(new Vector3(0, height, 0));
					transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + height, hitInfo.point.z);
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
        while (GameManager.Instance.m_PlayMode == PlayMode.Combine)
        {
            transform.position = m_TPSTransform.position;
            transform.rotation = Quaternion.LookRotation(m_TPSTarget.position - m_TPSTransform.position);
            yield return null;
        }

        transform.position = m_TPSTransform.position;
        transform.rotation = m_TPSTransform.rotation;

        m_IsRunning = false;
    }

    IEnumerator Release(bool combine)
    {
        if (m_IsRunning && !combine) { yield break; }
        m_IsRunning = true;

        if (m_combineCor != null) StopCoroutine(m_combineCor);

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
