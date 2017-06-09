using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss3_Controller : MonoBehaviour
{
    public GameObject m_TwinRobot;
    public GameObject m_HRobot;

    public AnimationClip m_CombineAnim;
    public AnimationClip m_ReleaseAnim;
    //合体、分裂　エフェクト
    public GameObject m_ElectricGuid;   //？？
    public GameObject m_Electric;
    public GameObject m_CombineEffect;

    public Animator m_HAnimator;

    //合体完了時イベント        
    //public UnityAction m_CombineEnd;
    public UnityEvent m_CombineEnd;

    //プレイヤーと共有するとき使う
    //各イベント
    //public UnityEvent m_ReleaseStart;
    //public UnityEvent m_ReleaseEnd;
    //public UnityEvent m_CombineStart;
    //public UnityEvent m_CombineEnd;
    //public UnityEvent<List<Collider>> m_CombineCollide;
    //操作等の行動を無効化するためのスクリプトの参照
    //public MonoBehaviour m_Controller;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            StartCoroutine(Combine());
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Release());
    }
    public IEnumerator Combine()
    {
        Vector3 pos = m_TwinRobot.transform.position;
        transform.position = pos;
        m_HRobot.transform.position = pos;
        m_CombineEffect.transform.position = pos + new Vector3(0, 1, 0);
        m_ElectricGuid.SetActive(true);
        float timer = 0.0f;
        while (timer < m_CombineAnim.length)
        {
            timer += Time.deltaTime;
            m_CombineAnim.SampleAnimation(gameObject, timer);
            yield return null;
        }
        StartCoroutine(CombineEffect());
        m_HAnimator.SetTrigger("Combined");
        m_CombineEnd.Invoke();
    }

    public IEnumerator Release()
    {
        transform.position = m_HAnimator.transform.position;
        StartCoroutine(CombineEffect());
        float timer = 0.0f;
        while (timer < m_ReleaseAnim.length)
        {
            timer += Time.deltaTime;
            m_ReleaseAnim.SampleAnimation(gameObject, timer);
            yield return null;
        }
        m_ElectricGuid.SetActive(true);
    }

    private IEnumerator CombineEffect()
    {
        m_CombineEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        m_CombineEffect.SetActive(false);
    }
}
