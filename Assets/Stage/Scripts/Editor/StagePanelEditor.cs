using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

    int onlyActiveStageLevel;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        LayoutCreatePanelButton();

        EditorGUILayout.Separator();

        OnlyActiveByUseLevel();

        EditorGUILayout.Separator();
        
        LayoutCheckNeadWall();
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
            StagePanel newPanel = newObj.GetComponent<StagePanel>();
            newPanel.transform.localScale = baseTrans.localScale;

            //位置を設定
            float worldScale = thisPanel.transform.lossyScale.x;
            Vector3 modify = m_Dires[direInt] * 100 * worldScale;
            newPanel.transform.position += modify;
            Undo.RegisterCreatedObjectUndo(newObj, "Create StagePanel");
        }
    }

    private void OnlyActiveByUseLevel()
    {
        EditorGUILayout.LabelField("対象のUseStageLevel以外を非アクティブに ");

        onlyActiveStageLevel = EditorGUILayout.IntField("対象　UseStageLevel ", onlyActiveStageLevel);
        if (GUILayout.Button("Apply"))
        {
            GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
            //処理が可能か判定
            foreach (var panel in panels)
            {
                StagePanel panelComp = panel.GetComponent<StagePanel>();
                if (panelComp.m_UseStageLevel != onlyActiveStageLevel)
                {
                    panel.SetActive(false);
                }
            }

        }
    }

    private void LayoutCheckNeadWall()
    {
        EditorGUILayout.LabelField("不必要な壁の削除");

        Color baseBackColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("DELETE"))
        {
            CheckNeadWall();
        }
        GUI.backgroundColor = baseBackColor;
    }
    public void CheckNeadWall()
    {
        GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
        
        /*処理が可能か判定*/
        //foreach (var panel in panels)
        //{
        //    StagePanel panelComp = panel.GetComponent<StagePanel>();
        //    if (panelComp.m_UseStageLevel != 0)
        //    {
        //        Debug.Log("UseStageLevelが０以外のパネルは非アクティブにしてください。");
        //        return;
        //    }
        //}

        LayerMask mask = LayerMask.GetMask(new string[] { "Wall" });
        //初期パネルの壁の選別
        foreach (var panel in panels)
        {
            StagePanel panelComp = panel.GetComponent<StagePanel>();
            Transform walls = panel.transform.FindChild("Walls");
            if (panelComp.m_UseStageLevel != 0 || walls == null)
                continue;

            for (int i = 0; i < walls.childCount; i++)
            {
                Transform wall = walls.GetChild(i);
                RaycastHit hitInfo1;
                RaycastHit hitInfo2;
                bool test = Physics.Raycast(wall.position, -wall.forward, out hitInfo2, Mathf.Infinity, mask);
                if (Physics.Raycast(wall.position, wall.forward, out hitInfo1, Mathf.Infinity, mask)
                && test)
                {
                    Undo.DestroyObjectImmediate(wall.gameObject);
                    i--;
                }
            }
        }
    }
}
