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

    protected override void Awake()
    {
        base.Awake();
        GameManager gm = GameManager.Instance;

        var lRobot = gm.m_LRobot;
        var rRobot = gm.m_RRobot;       

        lRobot.transform.parent = m_Twins.transform;
        rRobot.transform.parent = m_Twins.transform;
    }

    private void Start()
    {
        StartCoroutine(Parform());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Camera.transform.LookAt(m_Twins.transform.position);
    }

    IEnumerator Parform()
    {
        GameManager gm = GameManager.Instance;
        gm.m_PlayCamera.SetActive(false);
        m_Camera.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        m_PerformAnim.Play();

        yield return new WaitForSeconds(m_PerformAnim.clip.length);        
        var async = gm.m_GameStarter.AddScene("Result");
        //while (async.isDone)
        //{
        //    yield return null;
        //}        
    }

    private void OnDestroy()
    {
        GameManager.Instance.m_PlayCamera.SetActive(true);
        //Pauser.Resume(PauseTag.Enemy);
    }
}
