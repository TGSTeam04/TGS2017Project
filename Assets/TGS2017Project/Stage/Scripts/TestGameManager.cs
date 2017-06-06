using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager
{
    private static TestGameManager m_Ins;
    public StageManager m_StageManager;

    private TestGameManager() { }
    public static TestGameManager Instance
    {
        get
        {
            if (m_Ins == null)
            {
                //Debug.Log("Create StageManager");
                m_Ins = new TestGameManager();
            }
            return m_Ins;
        }
    }
}
