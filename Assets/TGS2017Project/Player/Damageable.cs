using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    //残りHPを引数にしたダメージイベント
    public UnityAction<float, MonoBehaviour> Del_ReciveDamage;

    /// <summary>
    /// 対象にダメージを与える
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="src">与えてきた相手（基本呼ぶ側はthisを入れれば良い）</param>    
    public virtual void ApplyDamage(float damage, MonoBehaviour src)
    {
        Debug.Log(src + "ApplyDamage" + this);
        if (Del_ReciveDamage != null)
            Del_ReciveDamage.Invoke(damage, src);
    }
}
