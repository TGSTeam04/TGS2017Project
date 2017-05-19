using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// MonoBehaviorの拡張クラス
/// </summary>
public static class MonoBehaviorExtentsion
{
    //Delay
    public static IEnumerator Delay<T1, T2, T3>(this MonoBehaviour mono, YieldInstruction yieldInstructtion, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
    {
        yield return yieldInstructtion;
        action(t1, t2, t3);
    }

    //Delay
    public static IEnumerator Delay<T1, T2>(this MonoBehaviour mono, YieldInstruction yieldInstructtion, Action<T1, T2> action, T1 t1, T2 t2)
    {
        yield return yieldInstructtion;
        action(t1, t2);
    }

    //Delay
    public static IEnumerator Delay<T>(this MonoBehaviour mono, YieldInstruction yieldInstructtion, Action<T> action, T t)
    {
        yield return yieldInstructtion;
        action(t);
    }

    //Delay
    public static IEnumerator Delay(this MonoBehaviour mono, YieldInstruction yieldInstructtion, Action action)
    {
        yield return yieldInstructtion;
        action();
    }
}