using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アニメーション再生終了まで待つ
/// <summary>
/// 
/// </summary>
public class WaitForAnimation : CustomYieldInstruction
{
    Animator m_Animator;
    int m_LastStateHash;
    int m_LayerNo;
    float m_ExitAnimRate;

    public WaitForAnimation(Animator animator, float exitAnimRate = 1.0f, int layerNo = 0)
    {
        //補間中は前のアニメーション判定なのでとりあえずそれを保存
        Init(animator, exitAnimRate, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).shortNameHash);
    }

    void Init(Animator animator, float exitAnimRate, int layerNo, int hash)
    {
        m_LayerNo = layerNo;
        m_Animator = animator;
        m_LastStateHash = hash;
        m_ExitAnimRate = exitAnimRate;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorState = m_Animator.GetCurrentAnimatorStateInfo(m_LayerNo);
            //補間が終了していて（補間時のアニメーションと現在のアニメーションが違う。）
            //且つ、現在のアニメーションが指定Rateより進んでいる。
            return (currentAnimatorState.normalizedTime < m_ExitAnimRate) || currentAnimatorState.fullPathHash == m_LastStateHash;
        }
    }
}