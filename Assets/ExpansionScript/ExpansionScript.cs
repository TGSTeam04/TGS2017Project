using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// MonoBehaviorの拡張クラス
/// </summary>


public delegate bool BAction();//<T>(params T[] pram);

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

    //BActionがtureを返す限りparYield経つごとに繰り返しBActionを実行。
    //参照されているGameObjectが消えても参照している側が生きている限り呼ばれ続けます。
    public static IEnumerator UpdateWhileMethodBool(this MonoBehaviour mono, BAction action, YieldInstruction parYield = null)
    {
        while (action())
        {
            yield return parYield;
        }
    }
    //BActionがture 且つ ターゲットが存在する限りparYield経つごとに繰り返しBActionを実行。
    public static IEnumerator SafeUpdateWhileMethodBool(this MonoBehaviour mono, MonoBehaviour target, BAction action, YieldInstruction parYield = null)
    {
        while (target != null && action())
        {
            yield return parYield;
        }
    }

    /*安全なUnityEventの呼び出し*/
    public static void SafeEvent(this MonoBehaviour mono, UnityEvent e)
    {
        if (e != null)
            e.Invoke();
    }
    /*安全なUnityEventの呼び出し*/
    public static void SafeEvent<T>(this MonoBehaviour mono, UnityEvent<T> e, T t1)
    {
        if (e != null)
            e.Invoke(t1);
    }
}