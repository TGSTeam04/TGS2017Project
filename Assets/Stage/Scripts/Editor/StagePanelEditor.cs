﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 製作者：大格
/// 更新日：05/23
/// ステージパネルエディタ拡張
/// </summary>

//ステージパネルクラスのエディタ拡張
[CustomEditor(typeof(StagePanel))]
[CanEditMultipleObjects]
public class StagePanelEditor : Editor
{
    //六方の向きEnum
    enum HexagonDirection
    {
        left,
        leftUp,
        rightUp,
        right,
        rightBottom,
        leftBottom
    }

    //六方の向きベクトル
    private static Vector3[] m_Dires = {
        Vector3.left,
        new Vector3(-Mathf.Cos(Mathf.Deg2Rad * 60), 0f,Mathf.Sin(Mathf.Deg2Rad *60)),
        new Vector3(Mathf.Cos(Mathf.Deg2Rad * 60), 0f,Mathf.Sin(Mathf.Deg2Rad *60)),
        Vector3.right,
        new Vector3(Mathf.Cos(Mathf.Deg2Rad * 60), 0f,-Mathf.Sin(Mathf.Deg2Rad *60)),
        new Vector3(-Mathf.Cos(Mathf.Deg2Rad * 60), 0f, -Mathf.Sin(Mathf.Deg2Rad *60)),
    };

    //private int m_OnlyActiveStageLevel;
    private int m_WallHeight;

    private void OnEnable()
    {
        StagePanel thisPanel = (StagePanel)target;
        m_WallHeight = (int)(thisPanel.transform.localScale.y);
        //m_OnlyActiveStageLevel = thisPanel.m_UseStageLevel;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameObject[] allPanels = GameObject.FindGameObjectsWithTag("Panel");

        EditorGUILayout.Separator();

        //パネル生成
        LayoutCreatePanelButton();

        EditorGUILayout.Separator();

        //Activeパネル設定
        //OnlyActiveByUseLevel(allPanels);

        EditorGUILayout.Separator();

        //壁の高さ変更
        ChangeHeight(allPanels);

        EditorGUILayout.Separator();

        //不必要な壁削除
        LayoutCheckNeadWall(allPanels);
    }

    //壁の高さを変更
    private void ChangeHeight(GameObject[] allPanels)
    {
        EditorGUI.BeginChangeCheck();
        m_WallHeight = EditorGUILayout.IntField("壁の高さ", m_WallHeight);
        StagePanel thisPanelComp = (StagePanel)target;
        if (EditorGUI.EndChangeCheck())
        {

            Vector3 baseScale = thisPanelComp.transform.localScale;
            thisPanelComp.transform.localScale = new Vector3(baseScale.x, m_WallHeight, baseScale.z);
        }
        if (GUILayout.Button("全てのパネルに適用"))
        {
            foreach (var panel in allPanels)
            {
                //StagePanel panelComp = panel.GetComponent<StagePanel>();
                Vector3 baseScale = panel.transform.localScale;
                //if (panelComp.m_UseStageLevel == thisPanelComp.m_UseStageLevel)
                panel.transform.localScale = new Vector3(baseScale.x, m_WallHeight, baseScale.z);
            }
        }
    }

    private void LayoutCreatePanelButton()
    {
        EditorGUILayout.LabelField("パネル生成");
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Left Up"))
                CreateStagePanele(HexagonDirection.leftUp);
            if (GUILayout.Button("Right Up"))
                CreateStagePanele(HexagonDirection.rightUp);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Left"))
                CreateStagePanele(HexagonDirection.left);
            if (GUILayout.Button("Right"))
                CreateStagePanele(HexagonDirection.right);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Left Bottom"))
                CreateStagePanele(HexagonDirection.leftBottom);
            if (GUILayout.Button("Right Bottom"))
                CreateStagePanele(HexagonDirection.rightBottom);
        }
    }

    //ステージパネル作成
    private void CreateStagePanele(object dire)
    {
        //複数選択対応
        foreach (var thisObj in targets)
        {
            StagePanel thisPanel = (StagePanel)thisObj;

            //新規パネル生成
            int direInt = (int)dire;
            Transform baseTrans = thisPanel.transform;
            GameObject newObj = Instantiate(
                (GameObject)Resources.Load("Prefub/Prefub_StagePanel")
                , baseTrans.position
                , baseTrans.rotation);
            newObj.transform.localScale = baseTrans.localScale;
            StagePanel newPanel = newObj.GetComponent<StagePanel>();

            //位置を設定
            float worldScale = thisPanel.transform.lossyScale.x;
            Vector3 modify = m_Dires[direInt] * newPanel.m_InscribedR * 2 * worldScale;
            Debug.Log(newPanel.m_InscribedR);
            newPanel.transform.position += modify;
            Undo.RegisterCreatedObjectUndo(newObj, "Create StagePanel");
        }
    }

    private void LayoutCheckNeadWall(GameObject[] panels)
    {
        EditorGUILayout.LabelField("不必要な壁の削除");

        Color baseBackColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("DELETE"))
        {
            CheckNeadWall(panels);
        }
        GUI.backgroundColor = baseBackColor;
    }
    public void CheckNeadWall(GameObject[] panels)
    {
        LayerMask mask = LayerMask.GetMask(new string[] { "Wall" });
        //壁の選別
        foreach (var panel in panels)
        {
            Transform walls = panel.transform.FindChild("Walls");

            for (int i = 0; i < walls.childCount; i++)
            {
                Transform wall = walls.GetChild(i);
                RaycastHit hitInfo1;
                RaycastHit hitInfo2;
                Debug.DrawRay(wall.transform.position, wall.right * 10f,Color.red,5.0f);
                if (Physics.Raycast(wall.position, wall.right, out hitInfo1, Mathf.Infinity, mask)
                && Physics.Raycast(wall.position, -wall.right, out hitInfo2, Mathf.Infinity, mask))
                {
                    Debug.Log("test");
                    Undo.DestroyObjectImmediate(wall.gameObject);                    
                    //hitInfo1.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                    i--;
                }
            }
        }
    }

    //private void OnlyActiveByUseLevel(GameObject[] panels)
    //{
    //    EditorGUILayout.LabelField("対象のUseStageLevel以外を非アクティブに ");

    //    m_OnlyActiveStageLevel = EditorGUILayout.IntField("対象　UseStageLevel ", m_OnlyActiveStageLevel);
    //    if (GUILayout.Button("Apply"))
    //    {
    //        //処理が可能か判定
    //        foreach (var panel in panels)
    //        {
    //            StagePanel panelComp = panel.GetComponent<StagePanel>();
    //            if (panelComp.m_UseStageLevel != m_OnlyActiveStageLevel)
    //            panel.SetActive(false);
    //        }
    //    }
    //}
}
