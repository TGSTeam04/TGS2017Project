using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPaformTwin : ClearParformance
{
    public GameObject m_Twins;

    public override bool CheckNecessary(GameManager gm)
    {
        var lRobot = gm.m_LRobot;
        return !lRobot.activeSelf;
    }

    protected void Awake()
    {
        if (!Redy()) return;
        GameManager gm = GameManager.Instance;

        var lRobot = gm.m_LRobot;
        var rRobot = gm.m_RRobot;       

        lRobot.transform.parent = m_Twins.transform;
        rRobot.transform.parent = m_Twins.transform;
    }

    private void Start()
    {
        StartCoroutine(PerformManagement());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Camera.transform.LookAt(m_Twins.transform.position);
    }

    protected override IEnumerator PlayerParform()
    {       
        yield return new WaitForSeconds(1.0f);
        m_PerformAnim.Play();

        yield return new WaitForSeconds(m_PerformAnim.clip.length);  
    }
}
