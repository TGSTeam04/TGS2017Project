//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Combine : MonoBehaviour
//{
//    public GameObject m_TwinR;
//    public GameObject m_TwinL;
//    public GameObject m_Humanoid;
//    public GameObject m_ShieldR;
//    public GameObject m_ShieldL;

//    private Rigidbody m_Rb_TwinR;
//    private Rigidbody m_Rb_TwinL;

//    private void Awake()
//    {
//        m_Rb_TwinR = m_TwinR.GetComponent<Rigidbody>();
//        m_Rb_TwinL = m_TwinL.GetComponent<Rigidbody>();
//    }

//    public IEnumerator CombineStart()
//    {
//        //コンバイン準備
//        GameManager.Instance.m_PlayMode = PlayMode.Combine;
//        m_TwinL.SetActive(false);
//        m_TwinR.SetActive(false);
//        m_KeepEnemyPosWall.SetActive(true);
//        //コンバインに使うパラメータを設定
//        Vector3 StartPositionL = m_LRobotRigidbody.position;
//        Vector3 StartPositionR = m_RRobotRigidbody.position;
//        Vector3 Direction = Vector3.Normalize(StartPositionL - StartPositionR);
//        Vector3 CenterPosition = Vector3.Lerp(StartPositionL, StartPositionR, 0.5f);
//        Vector3 EndPositionL = CenterPosition + Direction * 1.0f;
//        Vector3 EndPositionR = CenterPosition - Direction * 1.0f;
//        m_HRobot.transform.position = CenterPosition;
//        m_HRobot.transform.LookAt(CenterPosition + Vector3.Cross(-Direction, Vector3.up));

//        //TPSのカメラの高さを弄る not boss
//        float distaceRate = 1.5f;
//        Vector3 v = m_TPSPosition.position;
//        v.y = 5.5f + distaceRate * Vector3.Distance(CenterPosition, StartPositionL);
//        m_TPSPosition.position = v;

//        //twinロボットをお互いの方向に向く
//        float rotateTime = 0.3f;
//        Quaternion StartRotationL = m_LRobotRigidbody.rotation;
//        Quaternion StartRotationR = m_RRobotRigidbody.rotation;
//        Quaternion EndRotationL = Quaternion.LookRotation(-Direction);
//        Quaternion EndRotationR = Quaternion.LookRotation(Direction);
//        for (float t = 0; t < rotateTime; t += Time.fixedDeltaTime)
//        {
//            m_LRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationL, EndRotationL, m_RotationCurve.Evaluate(t / rotateTime)));
//            m_RRobotRigidbody.MoveRotation(Quaternion.Slerp(StartRotationR, EndRotationR, m_RotationCurve.Evaluate(t / rotateTime)));
//            yield return new WaitForFixedUpdate();
//        }
//        m_LRobotRigidbody.rotation = EndRotationL;
//        m_RRobotRigidbody.rotation = EndRotationR;

//        //未実装（エネミーを中心に集める等の演出）
//        const float moveEnemyEffectTime = 0.3f;
//        yield return new WaitForSeconds(moveEnemyEffectTime);

//        //間にいるエネミーを取得
//        List<EnemyBase> enemys = new List<EnemyBase>();
//        float distance = Vector3.Distance(CenterPosition, StartPositionL);
//        Vector3 offset = Direction * distance / 2;
//        Collider[] collider = Physics.OverlapBox(CenterPosition - offset, new Vector3(m_TwinRobotL.BreakerSize, 2, distance), EndRotationL, LayerMask.GetMask(new string[] { "Enemy" }));

//        foreach (var item in collider)
//        {
//            EnemyBase enemy = item.GetComponent<EnemyBase>();
//            if (enemy == null || enemys.Contains(enemy)) continue;
//            enemys.Add(enemy);
//        }
//        collider = Physics.OverlapBox(CenterPosition + offset, new Vector3(m_TwinR.BreakerSize, 2, distance), EndRotationR, LayerMask.GetMask(new string[] { "Enemy" }));
//        foreach (var item in collider)
//        {
//            EnemyBase enemy = item.GetComponent<EnemyBase>();
//            if (enemy == null || enemys.Contains(enemy)) continue;
//            enemys.Add(enemy);
//        }

//        //エネミーの動きを止める
//        Pauser.Pause(PauseTag.Enemy);

//        IsCanCrash = false;
//        m_LRobotRigidbody.isKinematic = true;
//        m_RRobotRigidbody.isKinematic = true;

//        for (float t = 0; IsCanCrash && Vector3.MoveTowards(StartPositionL, EndPositionL, m_CombineDistance.Evaluate(t) * m_Speed * 2) != EndPositionL; t += Time.fixedDeltaTime)
//        {
//            m_LRobotRigidbody.MovePosition(Vector3.MoveTowards(StartPositionL, EndPositionL, m_CombineDistance.Evaluate(t) * m_Speed * 2));
//            m_RRobotRigidbody.MovePosition(Vector3.MoveTowards(StartPositionR, EndPositionR, m_CombineDistance.Evaluate(t) * m_Speed * 2));

//            v = m_TPSPosition.position;
//            v.y = 5.5f + distaceRate * Vector3.Distance(CenterPosition, m_LRobotRigidbody.position);
//            m_TPSPosition.position = v;

//            yield return new WaitForFixedUpdate();
//        }
//        m_LRobotRigidbody.isKinematic = false;
//        m_RRobotRigidbody.isKinematic = false;
//        //カメラをTPSの既定の位置に not boss
//        v = m_TPSPosition.position;
//        v.y = 5.5f;
//        m_TPSPosition.position = v;

//        //エネミーが存在しなかったとき not boss
//        if (!IsCanCrash || enemys.Count == 0)
//        {
//            m_KeepEnemyPosWall.SetActive(false);
//            Pauser.Resume(PauseTag.Enemy);
//            yield return StartCoroutine(Release(true));
//            yield break;
//        }

//        foreach (var item in enemys)
//        {
//            //not boss
//            item.SetBreakForPlayer();
//        }

//        //TPSロボットのEnagyを設定 not boss
//        float add = GameManager.Instance.m_BreakEnemyTable.m_AddEnergy[enemys.Count - 1];
//        //GameManager.Instance.m_PlayScore += (int)add;
//        m_TwinRobotL.HP += add;
//        m_TwinR.HP += add;
//        m_HumanoidRobot.m_Energy = add;

//        //ロボットの向きにより合体後のロボットのパラメータを変更　not boss
//        if (m_TwinRobotL.Mode != m_TwinR.Mode)
//        {
//            m_HumanoidMaterial.color = m_ModeAB;
//            m_HumanoidRobot.m_Config = m_HumanoidT;
//        }
//        else if (m_TwinRobotL.Mode == TwinRobotMode.A)
//        {
//            m_HumanoidMaterial.color = m_ModeAA;
//            m_HumanoidRobot.m_Config = m_HumanoidN;
//        }
//        else
//        {
//            m_HumanoidMaterial.color = m_ModeBB;
//            m_HumanoidRobot.m_Config = m_HumanoidI;
//        }

//        //合体時エフェクトを追加

//        m_KeepEnemyPosWall.SetActive(false);
//        m_HRobot.SetActive(true);
//        m_LRobot.SetActive(false);
//        m_RRobot.SetActive(false);
//        m_Electric.SetActive(false);
//        GameManager.Instance.m_PlayMode = PlayMode.HumanoidRobot;

//        Pauser.Resume(PauseTag.Enemy);
//    }
//}
