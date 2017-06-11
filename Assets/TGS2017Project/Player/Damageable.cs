using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    //残りHPを引数にしたダメージイベント
    public UnityAction<float, MonoBehaviour> Event_Damaged;
    public virtual void ApplyDamage(float damage, MonoBehaviour src)
    {
        if (Event_Damaged != null)            
            Event_Damaged.Invoke(damage, src);
    }
}
